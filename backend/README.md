# Backend - Guía técnica de arranque desde cero

Este documento describe, paso a paso, cómo preparar SQL Server, aplicar migraciones, poblar datos iniciales y probar los endpoints de la API. Es reproducible en un entorno limpio.

## 0) Requisitos
- .NET SDK instalado (`dotnet --version`).
- SQL Server instalado y accesible (por ejemplo, `localhost\\SQLEXPRESS`).
- dotnet-ef CLI:
```powershell
dotnet tool install --global dotnet-ef
dotnet ef --version
```

## 1) Crear base y usuario en SQL Server (SSMS o sqlcmd)
Conéctate a tu instancia con un login con permisos (Windows Auth admin o `sa`). Ejecuta:

```sql
-- Crear base
CREATE DATABASE gestion_usuarios;
GO

-- Crear login para la app (si no existe)
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'app_user')
    CREATE LOGIN app_user WITH PASSWORD = 'C0ntraSegura!2025', CHECK_POLICY = ON, CHECK_EXPIRATION = ON;
GO

-- Crear user en la base y otorgar db_owner para desarrollo
USE gestion_usuarios;
GO
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'app_user')
    CREATE USER app_user FOR LOGIN app_user;
EXEC sp_addrolemember N'db_owner', N'app_user';
GO
```

> Nota: En producción, otorga permisos mínimos en lugar de `db_owner`.

## 2) Configurar la cadena de conexión
Archivo: `backend/appsettings.json`

Ejemplo para SQLEXPRESS con usuario SQL:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=gestion_usuarios;User Id=app_user;Password=C0ntraSegura!2025;Encrypt=True;TrustServerCertificate=True;"
}
```

Alternativas:
- Puerto fijo: `Server=localhost,1433;...`
- Windows Auth: `Trusted_Connection=True;`

## 3) (Opcional) Limpiar y recrear migraciones
En la carpeta del proyecto `backend/`:

```powershell
# Eliminar migraciones locales
Remove-Item -Recurse -Force ".\Migrations" -ErrorAction SilentlyContinue

# Crear migración del dominio (AppDbContext)
dotnet ef migrations add InitialApp -c AppDbContext -o Migrations

# Crear migración de Identity (AuthDbContext)
dotnet ef migrations add InitialAuth -c AuthDbContext -o Migrations/AuthDb
```

## 4) Aplicar migraciones
```powershell
# Dominio
dotnet ef database update -c AppDbContext

# Identity
dotnet ef database update -c AuthDbContext
```

> Si aparece `CREATE DATABASE permission denied`, crea la base con un login con permisos (paso 1) o usa temporalmente un login con permisos altos para migrar.

## 5) Ejecutar la API (migraciones + seed en runtime)
```powershell
dotnet build
dotnet run
```
En el arranque, `Program.cs` ejecuta `Migrate()` para ambos contextos y `DbSeeder.Seed(db)` que SOLO puebla si `Users` está vacía. El seeder crea:
- Roles: `Admin`, `Editor`, `Viewer`
- Permisos de ejemplo
- Usuarios de dominio demo (`Usuario1..Usuario50`)
- Usuario de dominio `admin@example.com` con rol `Admin`

## 6) Verificación en SQL (SSMS)
```sql
USE gestion_usuarios;
-- Conteos por tabla
SELECT 'Users' AS Tabla, COUNT(*) Cantidad FROM dbo.Users
UNION ALL SELECT 'Roles', COUNT(*) FROM dbo.Roles
UNION ALL SELECT 'Permissions', COUNT(*) FROM dbo.Permissions
UNION ALL SELECT 'UserRoles', COUNT(*) FROM dbo.UserRoles
UNION ALL SELECT 'RolePermissions', COUNT(*) FROM dbo.RolePermissions;

-- admin@example.com existe y tiene Admin
SELECT * FROM dbo.Users WHERE Email = 'admin@example.com';
SELECT u.Email, r.Name AS Rol
FROM dbo.Users u
JOIN dbo.UserRoles ur ON ur.UserId = u.Id
JOIN dbo.Roles r ON r.Id = ur.RoleId
WHERE u.Email = 'admin@example.com';

-- Usuarios Identity
SELECT Id, Email, UserName FROM dbo.AspNetUsers WHERE Email = 'admin@example.com';
```

## 7) Registrar, loguear y obtener token con rol Admin
Swagger: `https://localhost:<puerto>/swagger`

- Registrar (si no existe en Identity):
```json
POST api/auth/register
{
  "email": "admin@example.com",
  "password": "P4ssw0rd!"
}
```
- Login:
```json
POST api/auth/login
{
  "email": "admin@example.com",
  "password": "P4ssw0rd!"
}
```
- Authorize → pega `Bearer {token}`.
- Verificar claims:
```text
GET api/auth/me  => Debe contener role: Admin
```

> El `JwtProvider` agrega roles del dominio al token si el email de Identity coincide con un `User` del dominio con roles asignados.

## 8) Pruebas mínimas de endpoints
- Roles
  - `GET api/role`
  - `POST api/role` [Admin]
    ```json
    { "id": 0, "name": "Manager", "userRoles": [], "rolePermission": [] }
    ```
- Permisos
  - `POST api/permission` [Admin]
    ```json
    { "id": 0, "name": "Users.Read", "rolePermissions": [] }
    ```
- Asignar permiso a rol
  - `POST api/rolepermission` [Admin]
    ```json
    { "roleId": 1, "permissionId": 2 }
    ```
- Usuarios (dominio)
  - `GET api/user`
  - `POST api/user` [Admin]  (password = HASH)
    ```json
    { "id": 0, "name": "Juan Pérez", "email": "juan.perez@example.com", "password": "$2a$11$hash-o-un-placeholder" }
    ```
- Asignar rol a usuario
  - `POST api/userrole/assign` [Admin]
    ```json
    { "userId": 60, "roleId": 1 }
    ```

## 9) Reset total (si necesitas recomenzar)
Borrar la base y levantar de nuevo:
```sql
USE master;
IF DB_ID('gestion_usuarios') IS NOT NULL
BEGIN
    ALTER DATABASE gestion_usuarios SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE gestion_usuarios;
END
GO
```
Luego repite desde el paso 1 o 4 (según permisos y necesidades).

---

## Notas técnicas
- El token JWT incluye claims de rol del dominio al momento del login. Si cambias roles en el dominio, vuelve a loguear para regenerar el token.
- `DbSeeder.Seed` es idempotente (solo corre si `Users` está vacío). No duplica datos.
- Si recibes 403 en endpoints Admin, verifica `GET /api/auth/me` para confirmar que el token tiene `role: Admin`.

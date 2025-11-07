using Microsoft.AspNetCore.Identity; // Librería que provee clases para manejar usuarios, roles y autenticación
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Permite integrar Identity con Entity Framework Core
using Microsoft.EntityFrameworkCore; // ORM para trabajar con bases de datos relacionales

namespace backend.Data
{
    // DbContext especializado para la capa de autenticación usando ASP.NET Core Identity
    // Administra tablas como AspNetUsers, AspNetRoles, AspNetUserRoles, AspNetUserClaims, etc.
    public class AuthDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        // Constructor: inyecta opciones de configuración (connection string, proveedor, etc.)
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        // Permite personalizar el modelo y tablas de Identity
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Aplica la configuración estándar de Identity
            // Aquí se podrían renombrar tablas, agregar restricciones o relaciones adicionales si se desea
        }

        /*
         Observaciones de mejora:
         1. Considerar crear entidades derivadas de IdentityUser/IdentityRole
            para agregar propiedades adicionales (p. ej. FirstName, LastName, EmailConfirmed).
         2. Se puede implementar soft delete para usuarios en lugar de borrado físico.
         3. Revisar indices y constraints para mejorar rendimiento de consultas de login y roles.
         4. Es recomendable auditar cambios de usuarios y roles por seguridad.
        */
    }
}

using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Runtime.ConstrainedExecution;

namespace backend.Domain.Entities
{
    // Clase que representa un permiso dentro del sistema
    public class Permission
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del permiso es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
        public string Name { get; set; } = string.Empty;

        // Relación many-to-many con roles
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>(); //Se inicializa para evitar null y facilitar uso.
    }
}

//Consideraciones y relación con el resto del proyecto (por qué esto importa):
//Validaciones: los atributos[Required] y[StringLength] se usan por el binder de ASP.NET Core para validar entrada de APIs automáticamente antes de ejecutar controladores.
//Mapeo EF Core: EF usa estas anotaciones y convenciones para generar esquema (tipo/longitud/nullable). Si necesitas reglas avanzadas, se configuran en OnModelCreating de DbContext.
//Nullability y advertencias (relacionado con la advertencia CS8619 que viste): en el RolePermission que viste en el contexto, las propiedades de navegación están declaradas como Role? y Permission?. Eso hace que una proyección LINQ como .Select(rp => rp.Permission) sea tratada por el compilador como Permission?, lo que puede chocar con firmas que devuelven List<Permission> no-nullable. Soluciones:
//Hacer las navegaciones no-nullable (p. ej. public Permission Permission { get; set; } = null!;) si la relación es siempre requerida.
//Mantenerlas nullable y ajustar la API/repo para devolver List<Permission?> o filtrar nulos antes de proyectar.
//Configurar la relación como requerida en AppDbContext.OnModelCreating para que EF infiera no-null.
//Sugerencias prácticas:
//Si en tu dominio un RolePermission siempre tiene Permission, considera declarar Permission como no-nullable y/o configurar la relación como requerida para evitar advertencias y hacer la intención explícita.
//Si no necesitas campos adicionales en la tabla intermedia, podrías usar las skip navigations de EF Core (directamente ICollection<Role> en Permission), pero la entidad intermedia es mejor si necesitas datos extra o control explícito.
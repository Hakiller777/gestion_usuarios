using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace backend.Domain.Entities
{
    // Tabla intermedia para relación many-to-many entre Role y Permission
    public class RolePermission
    {
        public int RoleId { get; set; }
        public Role? Role { get; set; }

        public int PermissionId { get; set; }
        public Permission? Permission { get; set; }
    }
}


//Conceptos y decisiones importantes relacionadas con este archivo
//Entidad intermedia vs skip-navigation:
//Aquí se modela explícitamente la tabla intermedia (RolePermission). Esto es útil si quieres datos adicionales en la relación (por ejemplo, AssignedAt) o control explícito en repositorios.
//Clave compuesta:
//No hay[Key] ni propiedad Id en esta clase. Para garantizar unicidad (evitar duplicados RoleId+PermissionId) debes configurar la clave compuesta en el DbContext.OnModelCreating:
//Ejemplo en OnModelCreating: modelBuilder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionId });
//Si no configuras esto, EF podría crear una PK distinta (por convención creará una PK si pones Id) o dejar la entidad sin PK válida para algunas operaciones; es recomendable declarar la PK compuesta explícitamente.
//Nullable navigation properties y la advertencia CS8619:
//Las propiedades Role? y Permission? son nullable reference types. Cuando haces consultas LINQ que proyectan las navegaciones, el compilador trata el resultado como nullable (Permission?), lo que causó la advertencia que viste en la build al intentar asignar a List<Permission> no-nullable.
//Soluciones prácticas:
//1.Si la relación es siempre requerida: marcar navegaciones como no-nullable y/o configurar la relación como requerida en el modelo (recomendado si es verdad del dominio).
//2.Si pueden ser nulas: ajustar firmas a List<Permission?> o filtrar nulos antes de proyectar en repositorios.
//3.Filtrar nulos al proyectar: .Where(rp => rp.Permission != null).Select(rp => rp.Permission!) — elimina la advertencia y excluye nulos en tiempo de ejecución.
//•	Performance / conveniencia:
//•	Mantener la entidad intermedia permite operaciones directas sobre la relación (añadir/quitar) sin necesidad de cargar las colecciones completas del rol o del permiso.
//•	Buenas prácticas:
//•	Inicializar colecciones en las entidades principales (ya hecho en Role y Permission) para evitar NRE.
//•	Definir la PK compuesta en AppDbContext para reforzar la integridad y evitar duplicados a nivel BD.
//•	Decidir y documentar si las navegaciones son requeridas o no y reflejarlo en tipos (nullable/no-nullable) y en la configuración EF.
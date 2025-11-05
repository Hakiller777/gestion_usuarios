using System.ComponentModel.DataAnnotations;

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
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}

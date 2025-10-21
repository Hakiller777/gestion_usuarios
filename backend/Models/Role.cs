using System.ComponentModel.DataAnnotations;
using backend.Models;
using Microsoft.AspNetCore.Identity;

namespace backend.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del rol es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
        public string Name { get; set; } = string.Empty;

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>(); //Coleccion que representa a todos los usuarios que tienen este rol

        public ICollection<RolePermission> RolePermission { get; set; } = new List<RolePermission>(); //Coleccion que representa todos los permisos asociados a este rol
        
    }
}
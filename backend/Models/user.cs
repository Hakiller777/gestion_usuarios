// Importa las anotaciones para validaciones y claves primarias
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    // Clase que representa un usuario dentro del sistema
    public class User
    {
        [Key] // Define esta propiedad como la clave primaria de la tabla
        public int Id { get; set; } // Identificador único del usuario

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        public string Name { get; set; } // Nombre completo del usuario

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email es inválido")]
        [StringLength(150, ErrorMessage = "El email no puede exceder 150 caracteres")]
        public string Email { get; set; } // Email del usuario

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
        public string Password { get; set; } // Contraseña del usuario (recomendado hashear)

        // Ejemplo opcional: confirmación de contraseña (no se guarda en DB)
        [Compare("Password", ErrorMessage = "La confirmación de contraseña no coincide")]
        public string ConfirmPassword { get; set; } 
    }
}

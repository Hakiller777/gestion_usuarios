// Importa las anotaciones para validaciones y claves primarias 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Domain.ValueObjects;

namespace backend.Models //Referenciamos a la carpeta donde estamos 
{
    // Clase que representa un usuario dentro del sistema
    public class User
    {
        [Key] // Define esta propiedad como la clave primaria de la tabla
        public int Id { get; set; } // Identificador único del usuario

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        public string Name { get; set; } = string.Empty;// Nombre completo del usuario

        [Required(ErrorMessage = "El email es obligatorio")]
        public Email Email { get; set; } = Email.Create("placeholder@example.com"); // Se mapea a string vía converter

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public PasswordHash Password { get; set; } = PasswordHash.FromHashed("placeholder"); // Se mapea a string vía converter

        // Ejemplo opcional: confirmación de contraseña (no se guarda en DB)
        [NotMapped]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

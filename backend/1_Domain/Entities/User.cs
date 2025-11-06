// Importa las anotaciones para validaciones y claves primarias 
using backend.Domain.ValueObjects;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; //Atributos de mapeo como [NotMapped] para Shema de EF Core

namespace backend.Domain.Entities //Referenciamos a la carpeta donde estamos 
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
        public Email Email { get; set; } = 1; // Se mapea a string vía converter

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public PasswordHash Password { get; set; } = PasswordHash.FromHashed("placeholder"); // Se mapea a string vía converter

        // Ejemplo opcional: confirmación de contraseña (no se guarda en DB)
        [NotMapped] //EFcore no lo mira 
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

//Observaciones, buenas prácticas y riesgos:
//Value objects(Email, PasswordHash) son buenos: centralizan validación y formato. Asegúrate de:
//Implementar ValueConverter en AppDbContext para persistirlos como string.
//Usar EmailJsonConverter / PasswordHashJsonConverter para serializar adecuadamente en JSON (ya registrado en Program.cs).
//Inicializaciones con placeholders son prácticas para cumplir nullability, pero preferible:
//Proveer constructores/fábricas que obliguen a crear instancias válidas, o
//Hacer properties init y usar patrones de creación desde servicios para evitar valores por defecto inválidos en runtime.
//Seguridad:
//Password debe contener solo hashes; nunca exponer el valor en respuestas JSON. Asegura que PasswordHashJsonConverter no devuelva el hash en endpoints públicos.
//Considera no incluir Password en DTOs de salida; utiliza DTOs separados para entrada/salida.
//Validación: los atributos[Required] y [StringLength] funcionan para model binding, pero para reglas de dominio más complejas (p. ej. unicidad de email) implementa validaciones en servicios o en la capa de dominio.
//Nullability: evitar initializers inseguros; si la intención es que Email siempre sea válido, valida e infórmalo en la fábrica del value object.
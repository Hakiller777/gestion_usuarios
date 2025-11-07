// =======================================================
// Archivo: User.cs
// Capa: Dominio (Domain)
// Descripción: Define la entidad principal del sistema — User
// =======================================================

using backend.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Domain.Entities
{
    /// <summary>
    /// Representa un usuario dentro del dominio del sistema.
    /// Esta clase pertenece a la capa de Dominio y modela los datos
    /// fundamentales que identifican y describen a un usuario.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Clave primaria — identificador único del usuario.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nombre completo del usuario.
        /// </summary>
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Dirección de correo electrónico del usuario.
        /// Se almacena como un Value Object (Email), que garantiza su formato válido.
        /// </summary>
        [Required(ErrorMessage = "El email es obligatorio.")]
        public Email Email { get; set; } = null!; // Mapeado a string mediante un ValueConverter en la capa de Infraestructura.

        /// <summary>
        /// Contraseña encriptada (hash).
        /// Se encapsula en el Value Object PasswordHash para manejar la lógica de hash y verificación.
        /// </summary>
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public PasswordHash Password { get; set; } = PasswordHash.FromHashed("placeholder");

        /// <summary>
        /// Campo opcional solo usado en validaciones de formularios.
        /// No se persiste en la base de datos.
        /// </summary>
        [NotMapped]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    // =======================================================
    // OBSERVACIONES Y POSIBLES MEJORAS
    // =======================================================
    /*
     1️⃣  El Value Object Email debería validar formato automáticamente (por ejemplo usando Regex).
         - Recomendación: Asegurarse de que el ValueConverter esté correctamente configurado en el DbContext.
     
     2️⃣  El Value Object PasswordHash debería manejar internamente:
         - Hashing al crear usuario (con BCrypt o similar)
         - Verificación de contraseña en métodos como Verify()
     
     3️⃣  Agregar relaciones:
         - Si en el futuro el usuario tiene Roles o Permisos, se podrían agregar propiedades de navegación:
           Ej: public ICollection<Role> Roles { get; set; } = new List<Role>();
     
     4️⃣  Validaciones de dominio:
         - Actualmente las validaciones son por DataAnnotations (nivel aplicación/presentación).
         - Se podría agregar validación de dominio en constructores o métodos estáticos para reforzar reglas.
     
     5️⃣  Se recomienda crear un constructor protegido o privado + método estático Create() para imponer reglas de creación de usuario:
         Ejemplo:
           public static User Create(string name, Email email, PasswordHash password) { ... }
    */
}

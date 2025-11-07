using backend.Data;
using System;
using System.Diagnostics.Metrics;

namespace backend.Domain.ValueObjects
{
    public sealed record PasswordHash // Value Object inmutable que representa un hash de contraseña
    {
        public string Value { get; } // Propiedad pública de solo lectura que almacena el valor del hash de la contraseña

        private PasswordHash(string value) // Constructor privado para forzar el uso del método de fábrica
        {
            Value = value;
        }

        public static PasswordHash FromHashed(string hash) // Método de fábrica estático para crear instancias de PasswordHash
        {
            if (string.IsNullOrWhiteSpace(hash)) // Valida que el hash no sea nulo, vacío o solo espacios en blanco
                throw new ArgumentException("Password hash is required.", nameof(hash)); // Lanza una excepción si la validación falla
            return new PasswordHash(hash); // Crea y devuelve una nueva instancia de PasswordHash con el valor proporcionado
        }

        public override string ToString() => Value; // Sobrescribe el método ToString para devolver el valor del hash como cadena
    }
}


//Comentarios prácticos y recomendaciones:
//Responsabilidad: este value object representa el hash; la generación del hash (bcrypt/Argon2/HMAC, etc.) debe ocurrir en un servicio (p. ej. AuthService). Mantener separación evita mezclar hashing con persistencia.
//Persistencia: AppDbContext usa HasConversion(v => v.Value, v => PasswordHash.FromHashed(v)), por lo que EF persistirá el Value y reconstruirá el value object al leer.
//Seguridad:
//Nunca exponer PasswordHash.Value en respuestas públicas ni en logs. Usa DTOs de salida sin contraseña y asegura que cualquier JsonConverter omita o oculte el hash.
//Evita ToString() en logs de producción; mejor no confiar en que ToString no será usado por terceros.
//Robustez: FromHashed lanza si recibe valores inválidos. Si existe riesgo de datos corruptos en BD, podrías implementar una conversión más tolerante (p. ej. retornar null o un objeto marcado como inválido), pero eso debilitaría los invariantes del dominio.
//Extensiones posibles:
//Añadir un método Verify(plain, hasher) que delegue al servicio de hashing para verificar passwords (aunque preferible mantener verificación en la capa de servicios).
//Crear FromPlain(string plain, IPasswordHasher) si quieres centralizar hashing en el value object (menos común; suele hacerse en servicios).
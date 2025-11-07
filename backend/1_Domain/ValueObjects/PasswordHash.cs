using System;

namespace backend.Domain.ValueObjects
{
    /// <summary>
    /// Value Object inmutable que representa un hash de contraseña.
    /// Se usa para garantizar que el valor del hash esté siempre validado y protegido.
    /// </summary>
    public sealed record PasswordHash
    {
        /// <summary>
        /// Valor interno del hash de la contraseña.
        /// Es de solo lectura para mantener la inmutabilidad.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Constructor privado: evita que se creen instancias sin pasar por la validación del método de fábrica.
        /// </summary>
        /// <param name="value">Valor del hash ya procesado.</param>
        private PasswordHash(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Método de fábrica estático para crear instancias de PasswordHash a partir de un hash existente.
        /// Garantiza que el valor no sea nulo ni vacío.
        /// </summary>
        /// <param name="hash">Hash de la contraseña.</param>
        /// <returns>Instancia válida de <see cref="PasswordHash"/>.</returns>
        /// <exception cref="ArgumentException">Si el hash está vacío o solo contiene espacios.</exception>
        public static PasswordHash FromHashed(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
                throw new ArgumentException("Password hash is required.", nameof(hash));

            return new PasswordHash(hash);
        }

        /// <summary>
        /// Convierte el objeto a su representación en cadena (el valor del hash).
        /// </summary>
        public override string ToString() => Value;
    }

    /*
     * 🧠 Observaciones:
     * - Si luego vas a manejar el proceso de hasheo dentro del dominio (no solo recibir un hash ya hecho),
     *   convendría agregar un método estático "FromPlainText(string password)" que use BCrypt o similar
     *   para generar el hash de forma segura.
     * - También podrías implementar una validación para verificar si un texto plano coincide con el hash almacenado.
     * - No es necesario importar "backend.Data" ni "System.Diagnostics.Metrics" → se eliminaron porque no se usaban.
     */
}

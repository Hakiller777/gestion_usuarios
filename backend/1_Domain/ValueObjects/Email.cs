using Microsoft.AspNetCore.Http.HttpResults;
using System; // Importa el espacio de nombres System para usar tipos básicos como ArgumentException

namespace backend.Domain.ValueObjects // Define el espacio de nombres para los Value Objects dentro del dominio
{
    public sealed record Email // Define un Value Object inmutable llamado Email (No se puede heredar de él por ser sealed)(Igualar por valor por ser record)
    {
        public string Value { get; } // Propiedad pública de solo lectura que almacena el valor del email •	Propiedad solo-lectura que contiene la representación interna del email (string normalizado). Es la pieza que se persistirá/serializará mediante conversiones.

        private Email(string value) // Constructor privado para forzar el uso del método de fábrica
        {
            Value = value; // Asigna el valor proporcionado a la propiedad Value
        }

        public static Email Create(string value) // Método de fábrica estático para crear instancias de Email con validación
        {
            if (string.IsNullOrWhiteSpace(value))// Valida que el valor no sea nulo, vacío o solo espacios en blanco
                throw new ArgumentException("Email is required.", nameof(value));// Lanza una excepción si la validación falla

            var normalized = value.Trim().ToLowerInvariant();// Normaliza el email eliminando espacios y convirtiéndolo a minúsculas
            return new Email(normalized); // Crea y devuelve una nueva instancia de Email con el valor normalizado
        }

        public override string ToString() => Value; // Sobrescribe el método ToString para devolver el valor del email como cadena (override sobrescribe el método base)
    }
}

//Comentarios y consideraciones prácticas:
//•	Inmutabilidad y equidad por valor: al ser record y exponer solo Value lectura, Email actúa como verdadero value object (fácil de comparar y testear).
//•	Normalización centralizada: la fábrica Create garantiza uniformidad (muy importante para búsquedas/índices/unique constraints).
//•	Interacción con EF Core:
//•	En AppDbContext usas HasConversion(v => v.Value, v => Email.Create(v)). Esto persiste el Value y reconstrute Email al leer. Si la BD tiene valores inválidos (null/empty), Email.Create lanzará — asegúrate de que la columna sea NOT NULL y el seed/ingreso siempre den valores válidos.
//•	Exposición en JSON/API:
//•	En Program.cs registraste EmailJsonConverter para JSON; confirma que ese converter serialice/deserialice consistentemente con Create.
//•	Errores posibles:
//•	Create lanza excepciones al leer datos corruptos. Si esperas datos inconsistentes, considera una conversión tolerante en lectura que retorne null o un Email de fallback, pero eso diluye invariantes del dominio.
//•	Seguridad/privacidad: el email no es sensible como contraseña, pero considera protección frente a inyección al mostrarlo en logs.
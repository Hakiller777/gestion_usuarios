using backend.Domain.Entities;

namespace backend.Application.Abstractions.Repositories
{
    /// <summary>
    /// Interfaz que define las operaciones del repositorio para la entidad User.
    /// Forma parte de la capa de Aplicación, dentro del principio de abstracción del dominio.
    /// Su función es declarar los métodos que las implementaciones concretas (en Infraestructura)
    /// deberán cumplir para interactuar con la base de datos u otra fuente de datos.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Obtiene todos los usuarios del sistema de manera asíncrona.
        /// </summary>
        Task<List<User>> GetAllAsync();

        /// <summary>
        /// Busca y devuelve un usuario por su identificador único.
        /// Retorna null si no existe.
        /// </summary>
        Task<User?> GetByIdAsync(int id);

        /// <summary>
        /// Agrega un nuevo usuario a la base de datos.
        /// </summary>
        Task<User> AddAsync(User user);

        /// <summary>
        /// Actualiza los datos de un usuario existente.
        /// Retorna el usuario actualizado o null si no se encontró.
        /// </summary>
        Task<User?> UpdateAsync(User user);

        /// <summary>
        /// Elimina un usuario según su ID.
        /// Retorna true si la operación fue exitosa, false si no se encontró o falló.
        /// </summary>
        Task<bool> DeleteAsync(int id);
    }
}

/*
────────────────────────────────────────────
🟢 OBSERVACIONES / POSIBLES MEJORAS:
────────────────────────────────────────────
1️⃣ Podría usarse una interfaz genérica (por ejemplo IBaseRepository<T>)
     para reducir repetición si otros repositorios (Role, Permission, etc.) 
     tienen los mismos métodos CRUD.

2️⃣ Faltan métodos especializados si el dominio lo requiere, 
     por ejemplo: GetByEmailAsync(string email) o SearchAsync(string name).

3️⃣ En la implementación concreta se recomienda usar patrones como:
     - Unit of Work
     - Repository Pattern con EF Core o Dapper
     - Manejo de excepciones y logging.
────────────────────────────────────────────
*/

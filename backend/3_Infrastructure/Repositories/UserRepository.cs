// Importa modelos y contexto
using backend.Domain.Entities;      // Entidad User desde la capa de dominio
using backend.Data;                  // DbContext de la aplicación
using Microsoft.EntityFrameworkCore; // Funcionalidades de EF Core (ToListAsync, FindAsync, etc.)
using backend.Application.Abstractions.Repositories; // Interfaz IUserRepository

namespace backend.Repositories
{
    // Clase que maneja la lógica de acceso a datos de los usuarios
    // Implementa la interfaz IUserRepository para desacoplar la capa de acceso a datos de la capa de servicios
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context; // Contexto de la DB, inyectado desde Program.cs 

        // Constructor: inyectamos el DbContext
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        // Obtener todos los usuarios
        public async Task<List<User>> GetAllAsync()
        {
            // ToListAsync() ejecuta la consulta y convierte el resultado a lista de manera asincrónica
            return await _context.Users.ToListAsync();
        }

        // Obtener un usuario por Id
        public async Task<User?> GetByIdAsync(int id)
        {
            // FindAsync busca la entidad por clave primaria, retorna null si no existe
            return await _context.Users.FindAsync(id);
        }

        // Agregar un nuevo usuario
        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);          // Agrega el usuario al DbSet (pendiente de guardar)
            await _context.SaveChangesAsync(); // Persiste los cambios en la base de datos
            return user;                       // Devuelve el usuario creado
        }

        // Actualizar un usuario existente
        public async Task<User?> UpdateAsync(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id); // Busca el usuario por Id
            if (existingUser == null) return null;                      // Retorna null si no existe

            // Actualiza campos permitidos
            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.Password = user.Password;

            await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos
            return existingUser;               // Devuelve el usuario actualizado
        }

        // Eliminar un usuario por Id
        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id); // Busca el usuario por Id
            if (user == null) return false;                // Retorna false si no existe

            _context.Users.Remove(user);      // Marca el usuario para eliminación
            await _context.SaveChangesAsync(); // Persiste la eliminación
            return true;                       // Retorna true indicando que se eliminó
        }
    }
}

/*
OBSERVACIONES Y POSIBLES MEJORAS:
1. Manejo de errores: Actualmente no se manejan excepciones por problemas de conexión o concurrencia.
   Podría envolverse SaveChangesAsync() en try/catch y registrar errores.
2. Validaciones: Antes de agregar o actualizar, se podrían validar reglas de negocio, por ejemplo, que el email no esté duplicado.
3. Optimización: Para UpdateAsync, si se manejan muchos campos, se puede usar _context.Entry(existingUser).CurrentValues.SetValues(user)
   para mapear automáticamente.
4. Transacciones: Si en un futuro se agregan operaciones que involucren varias tablas, considerar uso de transacciones.
5. Logging: Podría agregarse logging de operaciones para auditoría y debugging.
*/

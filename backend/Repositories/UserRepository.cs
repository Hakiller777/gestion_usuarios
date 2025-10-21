    // Importa modelos y contexto
    using backend.Models;
    using backend.Data;
    using Microsoft.EntityFrameworkCore;

    namespace backend.Repositories
    {
        // Clase que maneja la lógica de acceso a datos de los usuarios
        public class UserRepository
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
                // ToListAsync() convierte la consulta en lista asincrónica
                return await _context.Users.ToListAsync();
            }

            // Obtener un usuario por Id
            public async Task<User?> GetByIdAsync(int id)
            {
                // FindAsync busca la entidad por clave primaria
                return await _context.Users.FindAsync(id);
            }

            // Agregar un nuevo usuario
            public async Task<User> AddAsync(User user)
            {
                _context.Users.Add(user);        // Agrega a la DB (pendiente de guardar)
                await _context.SaveChangesAsync(); // Guarda los cambios en la DB
                return user;                     // Devuelve el usuario creado
            }

            // Actualizar un usuario existente
            public async Task<User?> UpdateAsync(User user)
            {
                var existingUser = await _context.Users.FindAsync(user.Id);
                if (existingUser == null) return null;

                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
                existingUser.Password = user.Password;

                await _context.SaveChangesAsync(); // Guarda los cambios
                return existingUser;
            }

            // Eliminar un usuario por Id
            public async Task<bool> DeleteAsync(int id)
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null) return false;

                _context.Users.Remove(user);      // Marca para eliminación
                await _context.SaveChangesAsync(); // Guarda los cambios
                return true;
            }
        }
    }

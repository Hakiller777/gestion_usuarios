using backend.Domain.Entities; // Importa la entidad User desde la capa de dominio
using backend.Application.Abstractions.Repositories; // Importa la interfaz IUserRepository para desacoplar la lógica de acceso a datos

namespace backend.Services
{
    // Servicio que maneja la lógica de negocio relacionada con los usuarios
    public class UserService
    {
        // Repositorio de usuarios inyectado mediante constructor
        private readonly IUserRepository _userRepository;

        // Constructor que recibe la dependencia del repositorio de usuarios
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository; // Asigna el repositorio a la propiedad privada
        }

        // Obtiene todos los usuarios del sistema
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync(); // Llama al método del repositorio para traer todos los usuarios
        }

        // Obtiene un usuario por su Id
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id); // Llama al repositorio para obtener un usuario específico
        }

        // Crea un nuevo usuario en el sistema
        public async Task<User> CreateUserAsync(User user)
        {
            return await _userRepository.AddAsync(user); // Llama al repositorio para agregar un nuevo usuario
        }

        // Actualiza la información de un usuario existente
        public async Task<User?> UpdateUserAsync(User user)
        {
            return await _userRepository.UpdateAsync(user); // Llama al repositorio para actualizar el usuario
        }

        // Elimina un usuario por su Id
        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteAsync(id); // Llama al repositorio para eliminar el usuario
        }
    }
}

/*
OBSERVACIONES Y POSIBLES MEJORAS:
1. Manejo de errores: Actualmente no se manejan excepciones ni validaciones antes de llamar al repositorio.
   Se podría envolver cada llamada en try/catch y retornar errores claros o logs.
2. Validaciones adicionales: Por ejemplo, al crear o actualizar un usuario, validar Email y Password antes de pasar al repositorio.
3. Transacciones: Si en el futuro se agregan operaciones que involucren varias entidades, considerar uso de Unit of Work.
4. DTOs: Si la capa de presentación no debe recibir directamente la entidad User, se podrían usar DTOs para desacoplar.
5. Inyección de dependencias: Está bien, pero asegurarse de registrar IUserRepository en el contenedor de DI.
*/

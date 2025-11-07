using backend.Domain.Entities;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController] // Marca la clase como controlador de API, habilita validación automática de modelos y binding.
    [Route("api/[controller]")] // Define la ruta base del controlador, [controller] se reemplaza por "user".
    [Authorize] // Requiere que el usuario esté autenticado para acceder a todos los endpoints.
    public class UserController : ControllerBase
    {
        private readonly UserService _userService; // Servicio de aplicación que contiene la lógica de User.

        public UserController(UserService userService)
        {
            _userService = userService; // Se inyecta el servicio vía constructor (Dependency Injection)
        }

        // GET: api/user
        [HttpGet] // Define un endpoint GET
        public async Task<ActionResult<List<User>>> GetAll()
        {
            var users = await _userService.GetAllUsersAsync(); // Llama al servicio para obtener todos los usuarios
            return Ok(users); // Devuelve HTTP 200 con la lista de usuarios
        }

        // GET: api/user/5
        [HttpGet("{id}")] // Endpoint GET con parámetro id
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id); // Obtiene un usuario por Id
            if (user == null)
                return NotFound(); // Retorna 404 si no existe
            return Ok(user); // Retorna 200 con el usuario
        }

        // POST: api/user
        [HttpPost]
        [Authorize(Roles = "Admin")] // Solo Admin puede crear usuarios
        public async Task<ActionResult<User>> Create(User user)
        {
            var createdUser = await _userService.CreateUserAsync(user); // Crea el usuario
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
            // Retorna 201 con la ruta del recurso creado
        }

        // PUT: api/user/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Solo Admin puede actualizar
        public async Task<ActionResult<User>> Update(int id, User user)
        {
            if (id != user.Id)
                return BadRequest(); // Retorna 400 si los IDs no coinciden

            var updatedUser = await _userService.UpdateUserAsync(user); // Actualiza usuario
            if (updatedUser == null)
                return NotFound(); // Retorna 404 si no existe

            return Ok(updatedUser); // Retorna 200 con usuario actualizado
        }

        // DELETE: api/user/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Solo Admin puede eliminar
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _userService.DeleteUserAsync(id); // Elimina usuario
            if (!result)
                return NotFound(); // Retorna 404 si no existe

            return NoContent(); // Retorna 204 si eliminado correctamente
        }
    }
}

/* Observaciones / Mejoras posibles:
1. Validación de entrada: Sería recomendable usar DTOs (UserCreateDto, UserUpdateDto) en lugar de exponer la entidad directamente.
2. Manejo de errores: Considerar un middleware global de excepciones para capturar errores y retornar respuestas consistentes.
3. Logging: Agregar logs en operaciones críticas como creación, actualización y eliminación.
4. Seguridad: No devolver la contraseña ni datos sensibles en los endpoints GET/POST/PUT.
5. Paginación: Para GetAll(), implementar paginación si la tabla Users puede ser grande.
6. Mapping: Usar AutoMapper para mapear entre DTOs y entidades.
*/

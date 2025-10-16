// Importa modelos, repositorios y utilidades de ASP.NET Core
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    // Anotación [ApiController] indica que es un controlador Web API
    // [Route("api/[controller]")] genera rutas automáticas, ej: /api/user
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _repository;

        // Constructor: inyecta el repositorio
        public UserController(UserRepository repository)
        {
            _repository = repository;
        }

        // GET: api/user
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAll()
        {
            var users = await _repository.GetAllAsync();
            return Ok(users); // Retorna 200 OK con la lista de usuarios
        }

        // GET: api/user/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
                return NotFound(); // Retorna 404 si no existe
            return Ok(user);
        }

        // POST: api/user
        [HttpPost]
        public async Task<ActionResult<User>> Create(User user)
        {
            var createdUser = await _repository.AddAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
        }

        // PUT: api/user/5
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> Update(int id, User user)
        {
            if (id != user.Id)
                return BadRequest(); // 400 si el id de la URL no coincide con el del cuerpo

            var updatedUser = await _repository.UpdateAsync(user);
            if (updatedUser == null)
                return NotFound(); // 404 si no existe el usuario

            return Ok(updatedUser);
        }

        // DELETE: api/user/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _repository.DeleteAsync(id);
            if (!result)
                return NotFound(); // 404 si no existe
            return NoContent(); // 204 si se eliminó correctamente
        }
    }
}

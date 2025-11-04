using backend.Domain.Entities;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // GET: api/user
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/user/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        // POST: api/user
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> Create(User user)
        {
            var createdUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
        }

        // PUT: api/user/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> Update(int id, User user)
        {
            if (id != user.Id)
                return BadRequest();

            var updatedUser = await _userService.UpdateUserAsync(user);
            if (updatedUser == null)
                return NotFound();

            return Ok(updatedUser);
        }

        // DELETE: api/user/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
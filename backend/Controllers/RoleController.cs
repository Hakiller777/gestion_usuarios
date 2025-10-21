using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly RoleService _roleService;

        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }

        // GET: api/role
        [HttpGet]
        public async Task<ActionResult<List<Role>>> GetAll()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        // GET: api/role/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetById(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
                return NotFound();
            return Ok(role);
        }

        // POST: api/role
        [HttpPost]
        public async Task<ActionResult<Role>> Create(Role role)
        {
            var createdRole = await _roleService.CreateRoleAsync(role);
            return CreatedAtAction(nameof(GetById), new { id = createdRole.Id }, createdRole);
        }

        // PUT: api/role/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Role>> Update(int id, Role role)
        {
            if (id != role.Id)
                return BadRequest();

            var updatedRole = await _roleService.UpdateRoleAsync(role);
            if (updatedRole == null)
                return NotFound();

            return Ok(updatedRole);
        }

        // DELETE: api/role/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _roleService.DeleteRoleAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
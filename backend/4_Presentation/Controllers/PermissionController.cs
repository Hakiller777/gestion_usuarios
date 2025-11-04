using backend.Domain.Entities;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PermissionController : ControllerBase
    {
        private readonly PermissionService _permissionService;

        public PermissionController(PermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        // GET: api/permission
        [HttpGet]
        public async Task<ActionResult<List<Permission>>> GetAll()
        {
            var permissions = await _permissionService.GetAllPermissionsAsync();
            return Ok(permissions);
        }

        // GET: api/permission/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Permission>> GetById(int id)
        {
            var permission = await _permissionService.GetPermissionByIdAsync(id);
            if (permission == null)
                return NotFound();
            return Ok(permission);
        }

        // POST: api/permission
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Permission>> Create(Permission permission)
        {
            var createdPermission = await _permissionService.CreatePermissionAsync(permission);
            return CreatedAtAction(nameof(GetById), new { id = createdPermission.Id }, createdPermission);
        }

        // PUT: api/permission/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Permission>> Update(int id, Permission permission)
        {
            if (id != permission.Id)
                return BadRequest();

            var updatedPermission = await _permissionService.UpdatePermissionAsync(permission);
            if (updatedPermission == null)
                return NotFound();

            return Ok(updatedPermission);
        }

        // DELETE: api/permission/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _permissionService.DeletePermissionAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolePermissionController : ControllerBase
    {
        private readonly RolePermissionService _rolePermissionService;

        public RolePermissionController(RolePermissionService rolePermissionService)
        {
            _rolePermissionService = rolePermissionService;
        }

        // Asigna un permiso a un rol
        [HttpPost]
        public async Task<ActionResult<RolePermission>> AssignPermission([FromBody] RolePermission rolePermission)
        {
            var assignedPermission = await _rolePermissionService.AssignPermissionToRoleAsync(rolePermission.RoleId, rolePermission.PermissionId);
            return CreatedAtAction(nameof(GetPermissionsByRole), new { roleId = rolePermission.RoleId }, assignedPermission);
        }

        // Elimina un permiso de un rol
        [HttpDelete("{roleId}/{permissionId}")]
        public async Task<ActionResult> RemovePermission(int roleId, int permissionId)
        {
            var result = await _rolePermissionService.RemovePermissionFromRoleAsync(roleId, permissionId);
            if (!result) return NotFound();
            return NoContent();
        }

        // Obtiene todos los permisos de un rol
        [HttpGet("{roleId}")]
        public async Task<ActionResult<List<Permission>>> GetPermissionsByRole(int roleId)
        {
            var permissions = await _rolePermissionService.GetPermissionsByRoleAsync(roleId);
            return Ok(permissions);
        }
    }
}
using backend.Domain.Entities;
using backend.Services;
using backend.Application.Contracts.UserRole;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserRoleController : ControllerBase
    {
        private readonly UserRoleService _userRoleService;

        public UserRoleController(UserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        // POST: api/userrole/assign
        [HttpPost("assign")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserRole>> AssignRole([FromBody] backend.Application.Contracts.UserRole.AssignUserRoleRequest req)
        {
            try
            {
                var assignedRole = await _userRoleService.AssignRoleToUserAsync(req.UserId, req.RoleId);
                return CreatedAtAction(nameof(GetRolesByUser), new { userId = req.UserId }, assignedRole);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE: api/userrole/remove
        [HttpDelete("user/{userId}/role/{roleId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RemoveRole([FromRoute] int userId, [FromRoute] int roleId)
        {
            var result = await _userRoleService.RemoveRoleFromUserAsync(userId, roleId);
            if (!result)
                return NotFound();
            return NoContent();
        }

        // GET: api/userrole/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<UserRole>>> GetRolesByUser(int userId)
        {
            var roles = await _userRoleService.GetRolesByUserAsync(userId);
            return Ok(roles);
        }
    }
}
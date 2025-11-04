using backend.Domain.Entities;

namespace backend.Application.Abstractions.Repositories
{
    public interface IRolePermissionRepository
    {
        Task<RolePermission> AssignPermissionAsync(int roleId, int permissionId);
        Task<bool> RemovePermissionAsync(int roleId, int permissionId);
        Task<List<Permission>> GetPermissionsByRoleAsync(int roleId);
    }
}

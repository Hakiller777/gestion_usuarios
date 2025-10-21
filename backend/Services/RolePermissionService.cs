using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class RolePermissionService
    {
        private readonly RolePermissionRepository _rolePermissionRepository;

        public RolePermissionService(RolePermissionRepository rolePermissionRepository)
        {
            _rolePermissionRepository = rolePermissionRepository;
        }

        // Asigna un permiso a un rol
        public async Task<RolePermission> AssignPermissionToRoleAsync(int roleId, int permissionId)
        {
            return await _rolePermissionRepository.AssignPermissionAsync(roleId, permissionId);
        }

        // Elimina un permiso de un rol
        public async Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId)
        {
            return await _rolePermissionRepository.RemovePermissionAsync(roleId, permissionId);
        }

        // Obtiene todos los permisos de un rol
        public async Task<List<Permission>> GetPermissionsByRoleAsync(int roleId)
        {
            return await _rolePermissionRepository.GetPermissionsByRoleAsync(roleId);
        }
    }
}
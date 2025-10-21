using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class PermissionService
    {
        private readonly PermissionRepository _permissionRepository;

        public PermissionService(PermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<List<Permission>> GetAllPermissionsAsync()
        {
            return await _permissionRepository.GetAllAsync();
        }

        public async Task<Permission?> GetPermissionByIdAsync(int id)
        {
            return await _permissionRepository.GetByIdAsync(id);
        }

        public async Task<Permission> CreatePermissionAsync(Permission permission)
        {
            return await _permissionRepository.AddAsync(permission);
        }

        public async Task<Permission?> UpdatePermissionAsync(Permission permission)
        {
            return await _permissionRepository.UpdateAsync(permission);
        }

        public async Task<bool> DeletePermissionAsync(int id)
        {
            return await _permissionRepository.DeleteAsync(id);
        }
    }
}
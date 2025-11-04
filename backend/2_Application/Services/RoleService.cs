using backend.Domain.Entities;
using backend.Application.Abstractions.Repositories;

namespace backend.Services
{
    public class RoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _roleRepository.GetAllAsync();
        }

        public async Task<Role?> GetRoleByIdAsync(int id)
        {
            return await _roleRepository.GetByIdAsync(id);
        }

        public async Task<Role> CreateRoleAsync(Role role)
        {
            return await _roleRepository.AddAsync(role);
        }

        public async Task<Role?> UpdateRoleAsync(Role role)
        {
            return await _roleRepository.UpdateAsync(role);
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            return await _roleRepository.DeleteAsync(id);
        }
    }
}
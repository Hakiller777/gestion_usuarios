using backend.Models;
using backend.Application.Abstractions.Repositories;

namespace backend.Services
{
    public class UserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;

        public UserRoleService(IUserRoleRepository userRoleRepository)
        {
            _userRoleRepository = userRoleRepository;
        }

        // Asigna un rol a un usuario
        public async Task<UserRole> AssignRoleToUserAsync(int userId, int roleId)
        {
            return await _userRoleRepository.AssignRoleAsync(userId, roleId);
        }

        // Elimina un rol de un usuario
        public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            return await _userRoleRepository.RemoveRoleAsync(userId, roleId);
        }

        // Obtiene todos los roles de un usuario
        public async Task<List<Role>> GetRolesByUserAsync(int userId)
        {
            return await _userRoleRepository.GetRolesByUserAsync(userId);
        }
    }
}
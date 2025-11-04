using backend.Domain.Entities;

namespace backend.Application.Abstractions.Repositories
{
    public interface IUserRoleRepository
    {
        Task<UserRole> AssignRoleAsync(int userId, int roleId);
        Task<bool> RemoveRoleAsync(int userId, int roleId);
        Task<List<Role>> GetRolesByUserAsync(int userId);
    }
}

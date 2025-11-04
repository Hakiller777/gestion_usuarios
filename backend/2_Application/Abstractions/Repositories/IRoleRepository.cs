using backend.Domain.Entities;

namespace backend.Application.Abstractions.Repositories
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(int id);
        Task<Role> AddAsync(Role role);
        Task<Role?> UpdateAsync(Role role);
        Task<bool> DeleteAsync(int id);
    }
}

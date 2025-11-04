using backend.Domain.Entities;

namespace backend.Application.Abstractions.Repositories
{
    public interface IPermissionRepository
    {
        Task<List<Permission>> GetAllAsync();
        Task<Permission?> GetByIdAsync(int id);
        Task<Permission> AddAsync(Permission permission);
        Task<Permission?> UpdateAsync(Permission permission);
        Task<bool> DeleteAsync(int id);
    }
}

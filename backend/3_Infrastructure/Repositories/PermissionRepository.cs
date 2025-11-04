using backend.Data;
using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using backend.Application.Abstractions.Repositories;

namespace backend.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly AppDbContext _context;

        public PermissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Permission>> GetAllAsync()
        {
            return await _context.Permissions
                .Include(p => p.RolePermissions)
                .ToListAsync();
        }

        public async Task<Permission?> GetByIdAsync(int id)
        {
            return await _context.Permissions
                .Include(p => p.RolePermissions)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Permission> AddAsync(Permission permission)
        {
            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();
            return permission;
        }

        public async Task<Permission?> UpdateAsync(Permission permission)
        {
            var existing = await _context.Permissions.FindAsync(permission.Id);
            if (existing == null) return null;

            existing.Name = permission.Name;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Permissions.FindAsync(id);
            if (existing == null) return false;

            _context.Permissions.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
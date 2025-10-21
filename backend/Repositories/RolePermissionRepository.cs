using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class RolePermissionRepository
    {
        private readonly AppDbContext _context;

        public RolePermissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RolePermission> AssignPermissionAsync(int roleId, int permissionId)
        {
            var rp = new RolePermission { RoleId = roleId, PermissionId = permissionId };
            _context.RolePermissions.Add(rp);
            await _context.SaveChangesAsync();  
            return rp;
        }

        public async Task<bool> RemovePermissionAsync(int roleId, int permissionId)
        {
            var rp = await _context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
            if (rp == null) return false;

            _context.RolePermissions.Remove(rp);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Permission>> GetPermissionsByRoleAsync(int roleId)
        {
            return await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.Permission)
                .ToListAsync();
        }
    }
}
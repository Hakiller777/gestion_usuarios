using backend.Data;
using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using backend.Application.Abstractions.Repositories;

namespace backend.Repositories
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly AppDbContext _context;

        public RolePermissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RolePermission> AssignPermissionAsync(int roleId, int permissionId)
        {
            // Validar existencia de Role y Permission
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId);
            var permExists = await _context.Permissions.AnyAsync(p => p.Id == permissionId);
            if (!roleExists || !permExists)
            {
                throw new KeyNotFoundException("Role or Permission not found");
            }

            // Evitar duplicados (clave compuesta)
            var alreadyExists = await _context.RolePermissions.AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
            if (alreadyExists)
            {
                return new RolePermission { RoleId = roleId, PermissionId = permissionId };
            }

            // Adjuntar entidades principales como Unchanged
            _context.Attach(new Role { Id = roleId }).State = EntityState.Unchanged;
            _context.Attach(new Permission { Id = permissionId }).State = EntityState.Unchanged;

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
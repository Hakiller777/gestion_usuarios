using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class RoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        // Traer todos los roles
        public async Task<List<Role>> GetAllAsync()
        {
            return await _context.Roles
                .Include(r => r.UserRoles)
                .Include(r => r.RolePermission)
                .ToListAsync();
        }

        // Traer un rol por Id
        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _context.Roles
                .Include(r => r.UserRoles)
                .Include(r => r.RolePermission)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        // Crear un rol
        public async Task<Role> AddAsync(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return role;
        }

        // Actualizar un rol
        public async Task<Role?> UpdateAsync(Role role)
        {
            var existing = await _context.Roles.FindAsync(role.Id);
            if (existing == null) return null;

            existing.Name = role.Name;
            await _context.SaveChangesAsync();
            return existing;
        }

        // Eliminar un rol
        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Roles.FindAsync(id);
            if (existing == null) return false;

            _context.Roles.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
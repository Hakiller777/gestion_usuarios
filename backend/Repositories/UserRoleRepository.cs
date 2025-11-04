using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using backend.Application.Abstractions.Repositories;

namespace backend.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly AppDbContext _context;

        public UserRoleRepository(AppDbContext context)
        {
            _context = context;
        }

        // Asignar un rol a un usuario
        public async Task<UserRole> AssignRoleAsync(int userId, int roleId)
        {
            // Validar existencia de User y Role
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId);
            if (!userExists || !roleExists)
            {
                throw new KeyNotFoundException("User or Role not found");
            }

            // Evitar duplicados (clave compuesta)
            var alreadyExists = await _context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
            if (alreadyExists)
            {
                return new UserRole { UserId = userId, RoleId = roleId };
            }

            // Adjuntar entidades principales como Unchanged para que EF conozca los FKs
            _context.Attach(new User { Id = userId }).State = EntityState.Unchanged;
            _context.Attach(new Role { Id = roleId }).State = EntityState.Unchanged;

            var userRole = new UserRole { UserId = userId, RoleId = roleId };
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
            return userRole;
        }

        // Eliminar un rol de un usuario
        public async Task<bool> RemoveRoleAsync(int userId, int roleId)
        {
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
            if (userRole == null) return false;

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
            return true;
        }

        // Traer roles de un usuario
        public async Task<List<Role>> GetRolesByUserAsync(int userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role)
                .ToListAsync();
        }
    }
}
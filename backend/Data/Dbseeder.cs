using backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace backend.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            // Si ya hay usuarios, no hace nada
            if (context.Users.Any())
                return;

            // ---------------------------
            // CREAR USUARIOS
            // ---------------------------
            var users = new List<User>();
            for (int i = 1; i <= 50; i++)
            {
                users.Add(new User
                {
                    Name = $"Usuario{i}",
                    Email = $"usuario{i}@example.com",
                    Password = "1234" // Para desarrollo solo
                });
            }
            context.Users.AddRange(users);
            context.SaveChanges();

            // ---------------------------
            // CREAR ROLES
            // ---------------------------
            var adminRole = new Role { Name = "Administrador" };
            var editorRole = new Role { Name = "Editor" };
            var viewerRole = new Role { Name = "Viewer" };

            context.Roles.AddRange(adminRole, editorRole, viewerRole);
            context.SaveChanges();

            // ---------------------------
            // CREAR PERMISOS
            // ---------------------------
            var permisos = new List<Permission>
            {
                new Permission { Name = "VerUsuarios" },
                new Permission { Name = "CrearUsuarios" },
                new Permission { Name = "EditarUsuarios" },
                new Permission { Name = "EliminarUsuarios" }
            };
            context.Permissions.AddRange(permisos);
            context.SaveChanges();

            // ---------------------------
            // ASIGNAR PERMISOS A ROLES
            // ---------------------------
            // Admin tiene todos los permisos
            foreach (var permiso in permisos)
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = adminRole.Id,
                    PermissionId = permiso.Id
                });
            }

            // Editor puede ver y editar usuarios
            context.RolePermissions.Add(new RolePermission { RoleId = editorRole.Id, PermissionId = permisos.First(p => p.Name == "VerUsuarios").Id });
            context.RolePermissions.Add(new RolePermission { RoleId = editorRole.Id, PermissionId = permisos.First(p => p.Name == "EditarUsuarios").Id });

            // Viewer solo puede ver
            context.RolePermissions.Add(new RolePermission { RoleId = viewerRole.Id, PermissionId = permisos.First(p => p.Name == "VerUsuarios").Id });

            context.SaveChanges();

            // ---------------------------
            // ASIGNAR ROLES A USUARIOS
            // ---------------------------
            var adminUser = users.First(); // Usuario1 es Admin
            context.UserRoles.Add(new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id });

            var editorUser = users[1]; // Usuario2 es Editor
            context.UserRoles.Add(new UserRole { UserId = editorUser.Id, RoleId = editorRole.Id });

            var viewerUser = users[2]; // Usuario3 es Viewer
            context.UserRoles.Add(new UserRole { UserId = viewerUser.Id, RoleId = viewerRole.Id });

            context.SaveChanges();
        }
    }
}
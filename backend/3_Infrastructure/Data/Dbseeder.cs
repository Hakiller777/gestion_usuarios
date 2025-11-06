using backend.Domain.Entities;
using backend.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;

namespace backend.Data
{
    public static class DbSeeder // Clase estática para inicializar datos en la base de datos
    {
        public static void Seed(AppDbContext context) // Método estático que recibe el contexto de la base de datos
        {
            // Si ya hay usuarios, no hace nada
            if (context.Users.Any()) //Si ya existen usuarios en la BD, no hacemos nada
                return;// Salir del método si ya hay datos

            // ---------------------------
            // CREAR USUARIOS
            // ---------------------------
            var users = new List<User>(); //Lista para almacenar los usuarios creados
            for (int i = 1; i <= 50; i++) //Crear 50 usuarios de ejemplo
            { 
                users.Add(new User //Crear un nuevo usuario
                {
                    Name = $"Usuario{i}", // Nombre del usuario
                    Email = Email.Create($"usuario{i}@example.com"), // Email del usuario
                    Password = PasswordHash.FromHashed("1234") // Contraseña hasheada (placeholder)
                });
            }
            context.Users.AddRange(users); //Agregar los usuarios al contexto
            context.SaveChanges(); //Guardar los cambios en la base de datos

            // ---------------------------
            // CREAR ROLES
            // ---------------------------
            var adminRole = new Role { Name = "Admin" }; // Crear rol Admin
            var editorRole = new Role { Name = "Editor" }; // Crear rol Editor
            var viewerRole = new Role { Name = "Viewer" }; // Crear rol Viewer

            context.Roles.AddRange(adminRole, editorRole, viewerRole); // Agregar roles al contexto
            context.SaveChanges(); // Guardar los cambios en la base de datos

            // ---------------------------
            // CREAR PERMISOS
            // ---------------------------
            var permisos = new List<Permission> // Lista para almacenar los permisos creados
            {
                new Permission { Name = "VerUsuarios" }, // Permiso para ver usuarios
                new Permission { Name = "CrearUsuarios" }, // Permiso para crear usuarios
                new Permission { Name = "EditarUsuarios" }, // Permiso para editar usuarios
                new Permission { Name = "EliminarUsuarios" } // Permiso para eliminar usuarios
            };
            context.Permissions.AddRange(permisos); // Agregar permisos al contexto
            context.SaveChanges(); // Guardar los cambios en la base de datos

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

            // ---------------------------
            // USUARIO DE DOMINIO: admin@example.com como Admin
            // ---------------------------
            if (!context.Users.Any(u => EF.Property<string>(u, "Email") == "admin@example.com"))
            {
                var domainAdmin = new User
                {
                    Name = "Admin",
                    Email = Email.Create("admin@example.com"),
                    Password = PasswordHash.FromHashed("$2a$11$admin-seeded-hash") // placeholder en dev
                };
                context.Users.Add(domainAdmin);
                context.SaveChanges();
                context.UserRoles.Add(new UserRole { UserId = domainAdmin.Id, RoleId = adminRole.Id });
                context.SaveChanges();
            }
        }
    }
}
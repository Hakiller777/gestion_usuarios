using backend.Models; // Ajusta a tu namespace
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
        }
    }
}
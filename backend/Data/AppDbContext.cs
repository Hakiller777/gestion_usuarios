// Importa los modelos que usaremos en la base de datos
using backend.Models;

// Importa Entity Framework Core para DbContext
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    // Clase que representa el contexto de la base de datos
    // Hereda de DbContext, que es el componente principal de EF Core
    public class AppDbContext : DbContext
    {
        // Constructor que recibe opciones de configuración
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSet representa una tabla en la base de datos
        // Cada propiedad DbSet<T> será creada como tabla por EF Core
        //EF Core se encarga de que la tabla en la Db se llame Users mapeada con la clase User
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        // Método opcional para configuración avanzada del modelo
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Definir clave compuesta para UserRole
             modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });
            // Definir clave compuesta para RolePermission
            modelBuilder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionId });
        }
    }
}

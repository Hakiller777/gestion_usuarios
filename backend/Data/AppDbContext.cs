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
            modelBuilder.Entity<UserRole>().Property(ur => ur.UserId).ValueGeneratedNever();
            modelBuilder.Entity<UserRole>().Property(ur => ur.RoleId).ValueGeneratedNever();
            // Relaciones explícitas para UserRole
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            // Definir clave compuesta para RolePermission
            modelBuilder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionId });
            modelBuilder.Entity<RolePermission>().Property(rp => rp.RoleId).ValueGeneratedNever();
            modelBuilder.Entity<RolePermission>().Property(rp => rp.PermissionId).ValueGeneratedNever();
            // Relaciones explícitas para RolePermission
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermission)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

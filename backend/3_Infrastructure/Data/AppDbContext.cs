// Importa los modelos que usaremos en la base de datos
using backend.Domain.Entities;
using backend.Domain.ValueObjects;

// Importa Entity Framework Core para DbContext
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    // Clase que representa el contexto de la base de datos
    // Hereda de DbContext, que es el componente principal de EF Core
    public class AppDbContext : DbContext
    {
        // Constructor que recibe opciones de configuración
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) //Pasa las opciones al constructor base de DbContext
        {
        }

        // DbSet representa una tabla en la base de datos
        public DbSet<User> Users { get; set; } // Tabla de usuarios
        public DbSet<Role> Roles { get; set; } // Tabla de roles
        public DbSet<Permission> Permissions { get; set; } // Tabla de permisos
        public DbSet<UserRole> UserRoles { get; set; } // Tabla intermedia para relación many-to-many entre usuarios y roles
        public DbSet<RolePermission> RolePermissions { get; set; } // Tabla intermedia para relación many-to-many entre roles y permisos

        // Método opcional para configuración avanzada del modelo
        protected override void OnModelCreating(ModelBuilder modelBuilder) //Aqui implementamos fluent API
        {
            base.OnModelCreating(modelBuilder);

            //Value Objects: Configurar conversiones para Email
            modelBuilder.Entity<User>() //Accedemos a la entidad User
                .Property(u => u.Email) //Accedemos al campo Email de la entidad User
                .HasConversion( //mapea el value object Email a la columna (string) en la BD.
                    v => v.Value, //Convierte Email a string para almacenar en BD
                    v => Email.Create(v)); //Convierte string de BD a Email al leer

            //Value Objects: Configurar conversiones para PasswordHash
            modelBuilder.Entity<User>() //Accedemos a la entidad User
                .Property(u => u.Password) //Accedemos al campo Password de la entidad User
                .HasConversion( //mapea el value object PasswordHash a la columna (string) en la BD.
                    v => v.Value, //Convierte Password a string para almacenar en BD
                    v => PasswordHash.FromHashed(v)); //Convierte string de BD a PasswordHash al leer

            // Definir clave compuesta para UserRole
            modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId }); //Clave compuesta
            modelBuilder.Entity<UserRole>().Property(ur => ur.UserId).ValueGeneratedNever(); //Indica que UserId no es autogenerado, Ef core no debe esperar que la BD lo genere
            modelBuilder.Entity<UserRole>().Property(ur => ur.RoleId).ValueGeneratedNever(); //Indica que RoleId no es autogenerado, Ef core no debe esperar que la BD lo genere

            // Relaciones explícitas para UserRole con User
            modelBuilder.Entity<UserRole>() //Accede a la entidad UserRole
                .HasOne(ur => ur.User) //Relaciona UserRole con User
                .WithMany() //Un User puede tener muchos UserRoles sin lambdas porque user no tiene colección de UserRoles
                .HasForeignKey(ur => ur.UserId) //Clave foránea hacia User
                .OnDelete(DeleteBehavior.Cascade); //Elimina UserRoles relacionados si se elimina el User

            // Relaciones explícitas para UserRole con Role
            modelBuilder.Entity<UserRole>() //Accede a la entidad UserRole
                .HasOne(ur => ur.Role) //Relaciona UserRole con Role
                .WithMany(r => r.UserRoles) //Un Role puede tener muchos UserRoles
                .HasForeignKey(ur => ur.RoleId) //Clave foránea hacia Role
                .OnDelete(DeleteBehavior.Cascade); //Elimina UserRoles relacionados si se elimina el Role

            // Definir clave compuesta para RolePermission
            modelBuilder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionId }); //Clave compuesta
            modelBuilder.Entity<RolePermission>().Property(rp => rp.RoleId).ValueGeneratedNever(); //Indica que RoleId no es autogenerado, Ef core no debe esperar que la BD lo genere
            modelBuilder.Entity<RolePermission>().Property(rp => rp.PermissionId).ValueGeneratedNever(); //Indica que PermissionId no es autogenerado, Ef core no debe esperar que la BD lo genere

            // Relaciones explícitas para RolePermission con Role
            modelBuilder.Entity<RolePermission>() //Accede a la entidad RolePermission
                .HasOne(rp => rp.Role) //Relaciona RolePermission con Role
                .WithMany(r => r.RolePermissions) //Un Role puede tener muchos RolePermissions
                .HasForeignKey(rp => rp.RoleId) //Clave foránea hacia Role
                .OnDelete(DeleteBehavior.Cascade); //Elimina RolePermissions relacionados si se elimina el Role

            // Relaciones explícitas para RolePermission con Permission
            modelBuilder.Entity<RolePermission>() //Accede a la entidad RolePermission
                .HasOne(rp => rp.Permission)//Relaciona RolePermission con Permission
                .WithMany(p => p.RolePermissions)//Un Permission puede tener muchos RolePermissions
                .HasForeignKey(rp => rp.PermissionId)//Clave foránea hacia Permission
                .OnDelete(DeleteBehavior.Cascade);//Elimina RolePermissions relacionados si se elimina el Permission
        }
    }
}

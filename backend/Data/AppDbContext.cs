// Importa los modelos que usaremos en la base de datos
using Backend.Models;

// Importa Entity Framework Core para DbContext
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
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
        public DbSet<User> Users { get; set; }

        // Método opcional para configuración avanzada del modelo
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aquí podemos personalizar la tabla Users si queremos
            // Ejemplo: cambiar nombre de tabla
            // modelBuilder.Entity<User>().ToTable("Usuarios");

            // Podemos agregar relaciones, restricciones y seed data
        }
    }
}

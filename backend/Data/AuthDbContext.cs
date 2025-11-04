using Microsoft.AspNetCore.Identity; //Libreria utilizada para usar metodos de Identity
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; //Libreria para usar Identity con Entity Framework Core
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    // DbContext específico de ASP.NET Core Identity (almacena AspNetUsers, AspNetRoles, etc.)
    public class AuthDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) // Usa la misma cadena de conexión configurada en Program.cs
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Aplica la configuración por defecto de Identity (nombres de tablas/relaciones/keys)
        }
    }
}

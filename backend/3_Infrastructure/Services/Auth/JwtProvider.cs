using System.IdentityModel.Tokens.Jwt; // Para manejar tokens JWT
using System.Security.Claims; // Para Claims (reclamos de identidad)
using System.Text; // Para Encoding.UTF8
using System.Linq; // Para operaciones LINQ en memoria
using backend.Application.Abstractions; // Interfaz IJwtProvider
using backend.Data; // Contexto de la base de datos de dominio
using Microsoft.AspNetCore.Identity; // IdentityUser
using Microsoft.EntityFrameworkCore; // Para AsNoTracking
using Microsoft.Extensions.Configuration; // Para leer appsettings
using Microsoft.IdentityModel.Tokens; // Para SigningCredentials y SecurityKey

namespace backend.Infrastructure.Services.Auth
{
    // Implementación de IJwtProvider: genera JWT incluyendo roles desde dominio
    public class JwtProvider : IJwtProvider
    {
        private readonly IConfiguration _config; // Para leer clave, issuer, audience y expiración
        private readonly AppDbContext _db; // Contexto de dominio para obtener roles y usuarios

        public JwtProvider(IConfiguration config, AppDbContext db)
        {
            _config = config;
            _db = db;
        }

        // Genera un JWT para un usuario IdentityUser
        public string GenerateToken(IdentityUser user)
        {
            // Leer configuración JWT
            var jwt = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)); // Clave secreta
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // Algoritmo HMAC-SHA256
            var expire = DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpireMinutes"]!)); // Expiración

            // Crear claims base
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id), // Subject
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Token ID único
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? user.Email ?? string.Empty)
            };

            // Agregar roles desde la base de datos de dominio si existe un User con el mismo email
            var email = user.Email ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(email))
            {
                // Buscar usuario de dominio correspondiente
                var domainUser = _db.Users
                    .AsNoTracking() // Evita tracking innecesario
                    .AsEnumerable() // Traer a memoria para usar ValueObject Email
                    .FirstOrDefault(u => u.Email.Value == email);

                if (domainUser != null)
                {
                    // Obtener los roles asociados
                    var roleIds = _db.UserRoles
                        .Where(ur => ur.UserId == domainUser.Id)
                        .Select(ur => ur.RoleId)
                        .ToList();

                    var roleNames = _db.Roles
                        .Where(r => roleIds.Contains(r.Id))
                        .Select(r => r.Name)
                        .ToList();

                    foreach (var r in roleNames)
                    {
                        if (!string.IsNullOrWhiteSpace(r))
                        {
                            claims.Add(new Claim(ClaimTypes.Role, r)); // Agregar cada rol como claim
                        }
                    }
                }
            }

            // Crear el token JWT
            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: expire,
                signingCredentials: creds);

            // Retornar token serializado
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /*
         Observaciones de mejora:
         1. Evitar traer toda la tabla a memoria con AsEnumerable(); usar EF Core puro para comparar emails
            (actualmente se hace para comparar ValueObject, pero se puede crear un converter o expresión para EF).
         2. Considerar usar Claims personalizados o namespaces para evitar colisiones con roles existentes.
         3. Revisar performance si hay muchos usuarios y roles (consultas pueden ser optimizadas con Include y Join).
         4. Validar expiración mínima y máxima para seguridad.
         5. Posible refactor para desacoplar IdentityUser del dominio (actualmente se mezclan claims de dominio con Identity).
        */
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using backend.Application.Abstractions;
using backend.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace backend.Infrastructure.Services.Auth
{
    public class JwtProvider : IJwtProvider
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _db;

        public JwtProvider(IConfiguration config, AppDbContext db)
        {
            _config = config;
            _db = db;
        }

        public string GenerateToken(IdentityUser user)
        {
            var jwt = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expire = DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpireMinutes"]!));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? user.Email ?? string.Empty)
            };

            // Cargar roles del dominio asociados al mismo email del usuario Identity
            // Si existe un User de dominio con el mismo email, agregamos sus roles como claims
            var email = user.Email ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(email))
            {
                // Evaluar comparación del ValueObject Email en memoria para evitar errores de conversión
                var domainUser = _db.Users
                    .AsNoTracking()
                    .AsEnumerable()
                    .FirstOrDefault(u => u.Email.Value == email);
                if (domainUser != null)
                {
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
                            claims.Add(new Claim(ClaimTypes.Role, r));
                        }
                    }
                }
            }

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: expire,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

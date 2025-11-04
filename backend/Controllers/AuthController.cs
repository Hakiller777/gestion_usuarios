using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // Inyectamos componentes de ASP.NET Core Identity para crear/validar usuarios
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _config; // Fuente de la configuración JWT (Issuer, Audience, Key, Expire)

        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration config)
        {
            _userManager = userManager;   // Maneja operaciones de usuario (crear, buscar, validar password, etc.)
            _signInManager = signInManager; // Disponible si quisiéramos flujos de sign-in más avanzados
            _config = config;              // Usado para leer la sección Jwt del appsettings
        }

        // Modelos de request para registrar e iniciar sesión (Identity trabaja con email/password)
        public record RegisterRequest(string Email, string Password);
        public record LoginRequest(string Email, string Password);

        // Registro no requiere token: [AllowAnonymous]
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            var existing = await _userManager.FindByEmailAsync(req.Email); // Verifica si el email ya existe en AspNetUsers
            if (existing != null) return Conflict(new { message = "Email already registered" });

            var user = new IdentityUser { UserName = req.Email, Email = req.Email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, req.Password); // Crea el usuario y hashea el password
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors); // Devuelve errores de validación de Identity (password policy, etc.)
            }
            return Ok(new { message = "Registered" });
        }

        // Login no requiere token: [AllowAnonymous]
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var user = await _userManager.FindByEmailAsync(req.Email); // Busca el usuario por email en AspNetUsers
            if (user == null) return Unauthorized(new { message = "Invalid credentials" });

            var passOk = await _userManager.CheckPasswordAsync(user, req.Password); // Valida el password hasheado
            if (!passOk) return Unauthorized(new { message = "Invalid credentials" });

            var token = GenerateToken(user); // Emite JWT firmado con Key/Issuer/Audience del appsettings
            return Ok(new { token });
        }

        // Este endpoint requiere un JWT válido: [Authorize]
        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            // Retornamos identidad y claims leídos del JWT validado por JwtBearer
            return Ok(new
            {
                name = User.Identity?.Name,
                claims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }

        // Construye un JWT con claims básicos de Identity y expiración configurable
        private string GenerateToken(IdentityUser user)
        {
            var jwt = _config.GetSection("Jwt"); // Lee Issuer, Audience, Key, ExpireMinutes
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)); // Key simétrica para HS256
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);   // Algoritmo de firma
            var expire = DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpireMinutes"]!)); // Expiración estricta (ClockSkew = 0 en Program.cs)

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),               // Identificador del sujeto (usuario)
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Id único del token
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? user.Email ?? string.Empty)
            };

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],        // Debe coincidir con ValidIssuer en la validación
                audience: jwt["Audience"],    // Debe coincidir con ValidAudience en la validación
                claims: claims,
                expires: expire,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token); // Serializa el JWT (header.payload.signature)
        }
    }
}

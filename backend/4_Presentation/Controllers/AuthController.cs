using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Application.Abstractions;
using backend.Application.Contracts.Auth;

namespace backend.Controllers
{
    [ApiController] // Marca la clase como controlador de API, habilita binding automático de modelos y validación.
    [Route("api/[controller]")] // Define la ruta base: /api/auth
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService; // Servicio de autenticación (Register/Login)

        public AuthController(IAuthService authService)
        {
            _authService = authService; // Inyección de dependencia de AuthService
        }

        // Registro de usuario: no requiere JWT
        [AllowAnonymous] // Permite acceso sin token
        [HttpPost("register")] // POST /api/auth/register
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            try
            {
                await _authService.RegisterAsync(req); // Llama al servicio para crear usuario
                return Ok(new { message = "Registered" }); // Devuelve 200 si todo salió bien
            }
            catch (InvalidOperationException ex) // Email ya registrado
            {
                return Conflict(new { message = ex.Message }); // 409 Conflict
            }
            catch (ApplicationException ex) // Otros errores de validación de Identity
            {
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
        }

        // Login de usuario: no requiere JWT
        [AllowAnonymous]
        [HttpPost("login")] // POST /api/auth/login
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            try
            {
                var token = await _authService.LoginAsync(req); // Llama a AuthService que valida y genera JWT
                return Ok(new { token }); // Devuelve JWT
            }
            catch (UnauthorizedAccessException ex) // Credenciales inválidas
            {
                return Unauthorized(new { message = ex.Message }); // 401 Unauthorized
            }
        }

        // Endpoint protegido: requiere JWT válido
        [Authorize] // Middleware JwtBearer valida token antes de entrar
        [HttpGet("me")] // GET /api/auth/me
        public IActionResult Me()
        {
            // Devuelve información del usuario extraída del JWT
            return Ok(new
            {
                name = User.Identity?.Name, // Nombre del usuario (claim Name)
                claims = User.Claims.Select(c => new { c.Type, c.Value }) // Lista de claims (roles, email, etc.)
            });
        }
    }

    /*
     Observaciones / posibles mejoras:
     - Actualmente no se aplican políticas de roles en endpoints sensibles (solo Login/Register están públicos).
     - No hay validaciones adicionales de contraseña (fuerza, longitud) aquí; depende de Identity.
     - La propagación de errores de Identity al cliente es directa, podría filtrarse más para no exponer detalles.
     - El endpoint /me devuelve todos los claims tal cual; en producción conviene filtrar información sensible.
     - Podría implementarse un refresh token flow para mayor seguridad y expiración de JWT.
    */
}

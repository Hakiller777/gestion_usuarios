using backend.Application.Abstractions; // Interfaces de servicios de la capa de aplicación
using backend.Application.Contracts.Auth; // DTOs para Login y Register
using Microsoft.AspNetCore.Identity; // Identity para manejo de usuarios, roles y passwords

namespace backend.Infrastructure.Services.Auth
{
    // Servicio de autenticación que implementa IAuthService
    // Encargado de registrar usuarios y generar tokens JWT
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager; // Maneja operaciones de usuarios (CRUD, passwords, validaciones)
        private readonly IJwtProvider _jwtProvider; // Genera tokens JWT para autenticación

        public AuthService(UserManager<IdentityUser> userManager, IJwtProvider jwtProvider)
        {
            _userManager = userManager;
            _jwtProvider = jwtProvider;
        }

        // Registra un nuevo usuario
        public async Task RegisterAsync(RegisterRequest req)
        {
            // Verifica si ya existe un usuario con el mismo email
            var existing = await _userManager.FindByEmailAsync(req.Email);
            if (existing != null)
            {
                throw new InvalidOperationException("Email already registered"); // Evita duplicados
            }

            // Crea un nuevo usuario de Identity
            var user = new IdentityUser
            {
                UserName = req.Email,
                Email = req.Email,
                EmailConfirmed = true // Se puede cambiar a false si se implementa verificación por email
            };

            // Intenta crear el usuario con la contraseña provista
            var result = await _userManager.CreateAsync(user, req.Password);
            if (!result.Succeeded)
            {
                // Concatenar todos los errores de validación para lanzarlos
                var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new ApplicationException(errorMessage);
            }
        }

        // Login de usuario y generación de JWT
        public async Task<string> LoginAsync(LoginRequest req)
        {
            // Buscar usuario por email
            var user = await _userManager.FindByEmailAsync(req.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials"); // No revela si el email existe
            }

            // Verifica la contraseña
            var passOk = await _userManager.CheckPasswordAsync(user, req.Password);
            if (!passOk)
            {
                throw new UnauthorizedAccessException("Invalid credentials"); // Mensaje genérico para seguridad
            }

            // Genera y devuelve un JWT
            return _jwtProvider.GenerateToken(user);
        }

        /*
         Observaciones de mejora:
         1. Considerar usar DTOs de salida para no exponer IdentityUser directamente.
         2. Implementar bloqueo de cuenta tras varios intentos fallidos (seguridad contra brute force).
         3. Registrar eventos de login y registro para auditoría.
         4. Validar fuerza de password según políticas de seguridad.
         5. Para escalabilidad, separar la lógica de email confirmado y roles en servicios independientes.
        */
    }
}

using backend.Application.Contracts.Auth;

namespace backend.Application.Abstractions
{
    // Interface que define los métodos de autenticación
    // Esta interfaz permite abstraer la lógica de registro y login
    // y facilita la inyección de dependencias, pruebas unitarias y desacoplamiento
    public interface IAuthService
    {
        // Método para registrar un usuario
        // Recibe un DTO RegisterRequest que contiene la información necesaria (email, password)
        // Devuelve un Task porque es asincrónico y puede implicar acceso a DB u otros servicios
        Task RegisterAsync(RegisterRequest req);

        // Método para autenticar un usuario
        // Recibe un DTO LoginRequest que contiene credenciales (email y password)
        // Devuelve un Task<string> que usualmente será un token JWT si la autenticación es exitosa
        Task<string> LoginAsync(LoginRequest req);
    }

    /*
     Observaciones de mejora:
     1. Podría agregarse un método para RefreshToken si se planea usar tokens de larga duración.
     2. Validar que los DTOs contengan las restricciones necesarias (email válido, password segura).
     3. Manejar excepciones explícitamente para diferenciar errores de login (usuario no existe vs password incorrecta).
     4. Considerar devolver un DTO en vez de solo string en LoginAsync para incluir info adicional como roles o expiración del token.
    */
}

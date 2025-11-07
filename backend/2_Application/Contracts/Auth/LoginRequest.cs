namespace backend.Application.Contracts.Auth
{
    // DTO que representa los datos requeridos para iniciar sesión en el sistema
    // Se recibe desde el controller o endpoint de login
    public record LoginRequest(
        string Email,    // Email del usuario. Debe existir en la base de datos.
        string Password  // Contraseña en texto plano proporcionada por el usuario.
    );

    /*
     Observaciones de mejora:
     1. Validar email y password antes de enviarlo al servicio de autenticación.
     2. Nunca registrar la contraseña en logs ni exponerla de ninguna manera.
     3. Se puede agregar un campo opcional como "RememberMe" para control de sesión persistente.
     4. Considerar limitar intentos de login para prevenir ataques de fuerza bruta.
    */
}

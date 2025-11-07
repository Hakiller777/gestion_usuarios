using Microsoft.AspNetCore.Identity;

namespace backend.Application.Abstractions
{
    // Interface que define un proveedor de tokens JWT
    // Permite generar tokens de forma desacoplada de la implementación concreta
    // Esto facilita pruebas unitarias y cambios futuros en la forma de generar tokens
    public interface IJwtProvider
    {
        // Genera un token JWT para un usuario dado
        // Recibe un IdentityUser (clase de ASP.NET Identity que representa al usuario)
        // Devuelve un string que representa el token JWT
        string GenerateToken(IdentityUser user);
    }

    /*
     Observaciones de mejora:
     1. Podría recibir un DTO más completo si se quieren incluir roles, claims personalizados, o expiración.
     2. Podría devolverse un objeto en vez de solo string para incluir token + expiración + tipo de token.
     3. Considerar manejo de excepciones al generar tokens inválidos.
     4. Si se usan varios tipos de usuario (Admin, Cliente, etc.), el método podría generalizar claims según el rol.
    */
}

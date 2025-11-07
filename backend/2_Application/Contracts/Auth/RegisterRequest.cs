namespace backend.Application.Contracts.Auth
{
    // DTO (Data Transfer Object) que representa los datos requeridos para registrar un usuario
    // Se usa para recibir la información desde un controller o servicio de autenticación
    public record RegisterRequest(
        string Email,    // Email del usuario que se registrará. Se espera que sea único.
        string Password  // Contraseña en texto plano que luego será hasheada antes de almacenarse.
    );

    /*
     Observaciones de mejora:
     1. Validar email y password en el DTO usando DataAnnotations o FluentValidation antes de enviar al servicio.
     2. Podría incluir ConfirmPassword si se quiere validar que el usuario escriba correctamente la contraseña.
     3. Considerar encriptar la contraseña en tránsito (HTTPS obligatorio) y no exponerla en logs.
     4. Se podría extender con campos opcionales como Nombre, Rol inicial, etc.
    */
}

using Frontend1.Shared.Shared.Models.Auth;

namespace frontend1.Services;

public interface IAuthService
{
    Task<string?> LoginAsync(LoginRequest request);
}

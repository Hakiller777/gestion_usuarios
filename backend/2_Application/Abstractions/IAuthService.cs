using backend.Application.Contracts.Auth;

namespace backend.Application.Abstractions
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest req);
        Task<string> LoginAsync(LoginRequest req);
    }
}

using backend.Application.Abstractions;
using backend.Application.Contracts.Auth;
using Microsoft.AspNetCore.Identity;

namespace backend.Infrastructure.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IJwtProvider _jwtProvider;

        public AuthService(UserManager<IdentityUser> userManager, IJwtProvider jwtProvider)
        {
            _userManager = userManager;
            _jwtProvider = jwtProvider;
        }

        public async Task RegisterAsync(RegisterRequest req)
        {
            var existing = await _userManager.FindByEmailAsync(req.Email);
            if (existing != null)
            {
                throw new InvalidOperationException("Email already registered");
            }

            var user = new IdentityUser { UserName = req.Email, Email = req.Email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, req.Password);
            if (!result.Succeeded)
            {
                var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new ApplicationException(errorMessage);
            }
        }

        public async Task<string> LoginAsync(LoginRequest req)
        {
            var user = await _userManager.FindByEmailAsync(req.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var passOk = await _userManager.CheckPasswordAsync(user, req.Password);
            if (!passOk)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            return _jwtProvider.GenerateToken(user);
        }
    }
}

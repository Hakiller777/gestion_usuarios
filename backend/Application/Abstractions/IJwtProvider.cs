using Microsoft.AspNetCore.Identity;

namespace backend.Application.Abstractions
{
    public interface IJwtProvider
    {
        string GenerateToken(IdentityUser user);
    }
}

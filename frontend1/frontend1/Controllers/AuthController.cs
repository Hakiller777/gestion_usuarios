using Frontend1.Shared.Shared.Models.Auth;
using Microsoft.AspNetCore.Mvc;
using frontend1.Services;

namespace frontend1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var token = await _authService.LoginAsync(request);
        if (string.IsNullOrEmpty(token))
            return Unauthorized();

        return Ok(new LoginResponse { Token = token });
    }
}

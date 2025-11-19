using frontend1.Services;
using Frontend1.Shared.Shared.Models.Auth;
using System.Net.Http.Json;

namespace frontend1.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;

    public AuthService(HttpClient http)
    {
        _http = http;
    }

    public async Task<string?> LoginAsync(LoginRequest request)
    {
        // Aquí llamamos al backend real
        var response = await _http.PostAsJsonAsync("http://localhost:5212/api/Auth/login", request);

        if (!response.IsSuccessStatusCode)
            return null;

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return result?.Token;
    }
}

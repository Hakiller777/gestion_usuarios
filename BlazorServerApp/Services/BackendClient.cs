using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BlazorServerApp.Services
{
    public class BackendClient
    {
        private readonly HttpClient _http;
        private readonly TokenService _tokenService;

        public BackendClient(HttpClient http, TokenService tokenService)
        {
            _http = http;
            _tokenService = tokenService;
        }

        private void ApplyToken()
        {
            var token = _tokenService.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // EJEMPLO de consulta al backend
        public async Task<T?> GetAsync<T>(string url)
        {
            ApplyToken();
            return await _http.GetFromJsonAsync<T>(url);
        }

        // EJEMPLO de Login
        public async Task<bool> LoginAsync(string email, string password)
        {
            var response = await _http.PostAsJsonAsync("/auth/login", new
            {
                Email = email,
                Password = password
            });

            if (!response.IsSuccessStatusCode)
                return false;

            var data = await response.Content.ReadFromJsonAsync<LoginResult>();

            if (data?.Token is null)
                return false;

            _tokenService.SetToken(data.Token);
            return true;
        }
    }

    public class LoginResult
    {
        public string Token { get; set; } = string.Empty;
    }
}

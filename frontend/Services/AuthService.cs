using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;
using System.Security.Claims;

namespace frontend.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        // Inyectamos IHttpClientFactory para usar el HttpClient nombrado "API"
        public AuthService(IHttpClientFactory httpFactory, ILocalStorageService localStorage)
        {
            _http = httpFactory.CreateClient("API");
            _localStorage = localStorage;
        }

        // ---- LOGIN ----
        public async Task<bool> LoginAsync(string email, string password)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", new { email, password });
            if (!response.IsSuccessStatusCode) return false;

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions);
            if (result?.Token is null) return false;

            await _localStorage.SetItemAsync("authToken", result.Token);
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);

            return true;
        }

        // ---- REGISTER ----
        public async Task<bool> RegisterAsync(string email, string password)
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", new { email, password });
            return response.IsSuccessStatusCode;
        }

        // ---- LOGOUT ----
        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            _http.DefaultRequestHeaders.Authorization = null;
        }

        // ---- GET CURRENT USER ----
        public async Task<UserInfo?> GetCurrentUserAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrEmpty(token)) return null;

            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return await _http.GetFromJsonAsync<UserInfo>("api/auth/me", _jsonOptions);
        }

        // ---- GET TOKEN ----
        // Esto permite que Login.razor llame a AuthService.GetTokenAsync()
        public async Task<string?> GetTokenAsync()
        {
            return await _localStorage.GetItemAsync<string>("authToken");
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; } = "";
    }

    public class UserInfo
    {
        public string Email { get; set; } = "";
        public List<ClaimInfo> Claims { get; set; } = new();
    }

    public class ClaimInfo
    {
        public string Type { get; set; } = "";
        public string Value { get; set; } = "";
    }
}

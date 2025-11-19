using Blazored.LocalStorage;
using Frontend1.Shared.Shared.Models.Auth;
using Frontend1.Shared.Shared.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace frontend1.Client.Services
{
    public class AuthClientService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;

        public AuthClientService(HttpClient http, ILocalStorageService localStorage)
        {
            _http = http;
            _localStorage = localStorage;
        }

        // Llama al intermediario para login
        public async Task<string?> LoginAsync(string email, string password)
        {
            var request = new LoginRequest
            {
                Email = email,
                Password = password
            };

            var response = await _http.PostAsJsonAsync("api/auth/login", request);

            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return result?.Token;
        }

        // Guarda el token en LocalStorage
        public async Task SaveTokenAsync(string token)
        {
            await _localStorage.SetItemAsync("authToken", token);
        }

        // Recupera el token
        public async Task<string?> GetTokenAsync()
        {
            return await _localStorage.GetItemAsync<string>("authToken");
        }
    }
}

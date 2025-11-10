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
        public AuthService(IHttpClientFactory httpFactory, ILocalStorageService localStorage) //Inyecta IHttpClientFactory y ILocalStorageService en el constructor de AuthService
        {
            _http = httpFactory.CreateClient("API"); //Crea un HttpClient nombrado "API" para comunicarse con el backend
            _localStorage = localStorage;
        }

        // ---- LOGIN ----
        public async Task<bool> LoginAsync(string email, string password) //Aqui es donde se llama al endpoint de login en el backend
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", new { email, password }); //Aqui se comunica con el bakend para el login 
            if (!response.IsSuccessStatusCode) return false; //Si la respuesta al bakend no es exitosa, retorna false

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions); //Aqui se recibe el token del backend y lo convierte en un objeto LoginResponse
            if (result?.Token is null) return false; //Si el token es nulo, retorna false

            await _localStorage.SetItemAsync("authToken", result.Token); //Guarda el token en el almacenamiento local del navegador
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token); //Configura el encabezado de autorizacion para futuras solicitudes HTTP

            return true; //Retorna true si el login fue exitoso
        }

        // ---- REGISTER ---- aqui es donde se llama al endpoint de registro en el backend
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
        public async Task<string?> GetTokenAsync() //Metodo para obtener el token almacenado en el almacenamiento local del navegador
        {
            return await _localStorage.GetItemAsync<string>("authToken"); //Retorna el token almacenado en el almacenamiento local del navegador
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

using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace frontend.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage; // Servicio para acceder al almacenamiento local del navegador

        public CustomAuthStateProvider(ILocalStorageService localStorage) // Inyecta ILocalStorageService en el constructor
        {
            _localStorage = localStorage; // Asigna el servicio inyectado a la variable privada
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken"); // Obtiene el token JWT del almacenamiento local

            if (string.IsNullOrWhiteSpace(token)) // Si no hay token
            {
                // Usuario no autenticado
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())); // Retorna un estado de autenticación con un usuario anónimo
            }

            var claims = ParseClaimsFromJwt(token); // Parsea los claims del token JWT
            var identity = new ClaimsIdentity(claims, "jwt"); // Crea una identidad con los claims y el esquema "jwt"

            var user = new ClaimsPrincipal(identity); // Crea un usuario con la identidad
            return new AuthenticationState(user); // Retorna el estado de autenticación con el usuario autenticado
        }

        public void NotifyUserAuthentication(string token) // Notifica que el usuario ha iniciado sesión
        {
            var claims = ParseClaimsFromJwt(token); // Parsea los claims del token JWT
            var identity = new ClaimsIdentity(claims, "jwt"); // Crea una identidad con los claims y el esquema "jwt"
            var user = new ClaimsPrincipal(identity); // Crea un usuario con la identidad

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user))); // Notifica el cambio en el estado de autenticación
        }

        public void NotifyUserLogout() // Notifica que el usuario ha cerrado sesión
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity()); // Crea un usuario anónimo
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser))); // Notifica el cambio en el estado de autenticación
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt) // Parsea los claims de un token JWT
        {
            var claims = new List<Claim>(); // Lista para almacenar los claims

            // JWT tiene 3 partes separadas por '.'
            var payload = jwt.Split('.')[1]; // Obtiene la segunda parte (payload) del token JWT
            var jsonBytes = ParseBase64WithoutPadding(payload); // Decodifica el payload de Base64
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes); // Deserializa el payload JSON a un diccionario

            if (keyValuePairs == null) return claims; // Si no hay claims, retorna la lista vacía

            foreach (var kvp in keyValuePairs)
            {
                // Convertir todo a string para evitar error con números
                var value = kvp.Value?.ToString() ?? string.Empty;

                // Normalizamos el claim "unique_name" a Name
                if (kvp.Key == "unique_name")
                    claims.Add(new Claim(ClaimTypes.Name, value));
                else if (kvp.Key == "role")
                {
                    // Role puede ser string o array
                    if (kvp.Value is JsonElement element)
                    {
                        if (element.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var role in element.EnumerateArray())
                                claims.Add(new Claim(ClaimTypes.Role, role.GetString() ?? ""));
                        }
                        else
                        {
                            claims.Add(new Claim(ClaimTypes.Role, element.GetString() ?? ""));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, value));
                    }
                }
                else
                {
                    claims.Add(new Claim(kvp.Key, value));
                }
            }

            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}

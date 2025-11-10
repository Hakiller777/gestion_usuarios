using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using frontend.Services;
using frontend;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args); // Crea el host de la aplicación Blazor WebAssembly con la configuración predeterminada

builder.RootComponents.Add<App>("#app"); // Agrega el componente raíz "App" al elemento con id "app" en el HTML
builder.RootComponents.Add<HeadOutlet>("head::after"); // Agrega el componente "HeadOutlet" para manejar el contenido del head del HTML

// Servicios
builder.Services.AddBlazoredLocalStorage(); // Registro del servicio de almacenamiento local del navegador que usaremos para guardar el token JWT

// Auth
builder.Services.AddScoped<CustomAuthStateProvider>(); // Registro del proveedor de estado de autenticación personalizado
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthStateProvider>()); // Registro del proveedor de estado de autenticación para que use el personalizado
builder.Services.AddAuthorizationCore(); // Habilita la autorización en Blazor WebAssembly
builder.Services.AddScoped<AuthService>(); // Registro del servicio de autenticación (sirve para login, registro, logout, etc.)

// HttpClient nombrado "API" apuntando a tu backend
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri("http://localhost:5212/"); // Aqui podemos ver la conexion directa con el backend ( El enlace al mismo )
});

// HttpClient genérico (opcional)
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("API")); // Registro de un HttpClient genérico que usa el HttpClient nombrado "API"

await builder.Build().RunAsync();

using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using frontend.Services;
using frontend;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Componentes raíz
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient para llamadas al backend
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5212/") // URL de tu API
});

// Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Autenticación
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddAuthorizationCore();

// Servicios de autenticación
builder.Services.AddScoped<AuthService>();

await builder.Build().RunAsync();

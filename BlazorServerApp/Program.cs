using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BlazorServerApp;

var builder = WebApplication.CreateBuilder(args);

// 1) Habilitamos CORS para permitir que el front (WASM) en :5097 haga requests al intermediario
//    Ajustá el origen si tu WASM corre en otra URL.
var frontendOrigin = "http://localhost:5097";
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(frontendOrigin)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // si vas a usar cookies o auth basada en cookies
    });
});

// 2) Servicios Blazor Server (los dejamos por si los necesitamos)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 3) Habilitamos Controllers (API endpoints) en otras palabras permite que este proyecto actúe como un API REST
builder.Services.AddControllers();

// 4) Registramos HttpClient para comunicarnos con el backend real.
//    Usaremos un cliente nombrado "BackendApi" con BaseAddress = http://localhost:5212
builder.Services.AddHttpClient("BackendApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5212");
});

// Opcional: si querés un servicio tipado que encapsule llamadas al backend,
// lo podemos agregar luego con AddScoped<BackendService>() y AddHttpClient<BackendService>(...)
//
// builder.Services.AddHttpClient<BackendService>(client => {
//     client.BaseAddress = new Uri("http://localhost:5212");
// });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Proxy API V1");
    });
}


app.UseStaticFiles();

// 5) Usamos CORS middleware (debe ir antes de MapControllers)
app.UseCors();

// 6) El middleware antiforgery requerido por los endpoints interactivos de Razor Components
app.UseAntiforgery();

// 7) Mapear controllers API Todo lo que pongas en Controllers/*.cs con [ApiController] y [Route("api/[controller]")] se expone aquí.
app.MapControllers();

// 8) Mantenemos la posibilidad de renderizar componentes Blazor Server (opcional)
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();

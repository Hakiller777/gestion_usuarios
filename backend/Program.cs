using backend.Data;          // Para AppDbContext
using backend.Repositories;  // Para UserRepository
using Microsoft.EntityFrameworkCore; // Para UseNpgsql

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --------------------
// INYECCIÓN DEL DBCONTEXT
// --------------------
// Modificado: Se agregó AppDbContext con conexión a PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --------------------
// INYECCIÓN DEL REPOSITORIO
// --------------------
// Modificado: Se agregó UserRepository como servicio Scoped
builder.Services.AddScoped<UserRepository>();

var app = builder.Build();

// --------------------
// SWAGGER
// --------------------
// Ya estaba, solo confirmamos que se usa en Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --------------------
// POBLADO INICIAL DE LA BASE DE DATOS
// --------------------
// Modificado: Se agregó DbSeeder para crear 50 usuarios solo si la tabla está vacía
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.Seed(db); // Esto solo correrá si la tabla Users está vacía
}

// --------------------
// ENDPOINT DE EJEMPLO
// --------------------
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

// --------------------
// RECORD WEATHERFORECAST
// --------------------
// No modificado, solo se deja como estaba
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
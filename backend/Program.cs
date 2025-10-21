using backend.Data;          // Para AppDbContext
using backend.Models;
using backend.Repositories;  // Para los Repositorios
using backend.Services;      // Para los Servicios
using Microsoft.EntityFrameworkCore; // Para UseNpgsql

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// --------------------
// INYECCIÓN DEL DBCONTEXT
// --------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --------------------
// INYECCIÓN DE REPOSITORIOS
// --------------------
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<RoleRepository>();
builder.Services.AddScoped<PermissionRepository>();
builder.Services.AddScoped<UserRoleRepository>();
builder.Services.AddScoped<RolePermissionRepository>();

// --------------------
// INYECCIÓN DE SERVICIOS
// --------------------
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<PermissionService>();
builder.Services.AddScoped<UserRoleService>();
builder.Services.AddScoped<RolePermissionService>();

var app = builder.Build();

// --------------------
// SWAGGER
// --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

// --------------------
// POBLADO INICIAL DE LA BASE DE DATOS
// --------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.Seed(db); // Esto solo correrá si la tabla Users está vacía
}

app.Run();

// --------------------
// RECORD WEATHERFORECAST (no modificado)
// --------------------
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
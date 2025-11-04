using backend.Data;          // Para AppDbContext
using backend.Models;
using backend.Repositories;  // Para los Repositorios
using backend.Services;      // Para los Servicios
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; // Identity: UserManager/RoleManager/SignInManager + stores EF
using Microsoft.AspNetCore.Authentication.JwtBearer; // Middleware para validar JWT en cada request
using Microsoft.IdentityModel.Tokens; // Tipos para validación/firma de tokens
using System.Text; // Encoding de la clave simétrica HS256
using Microsoft.OpenApi.Models; // Swagger: definición de esquema Bearer
using backend.Domain.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => // Swagger: agrega esquema Bearer para pegar el JWT desde la UI
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Gestion Usuarios API", Version = "v1" });

    // Definición de seguridad Bearer
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autenticación JWT. Usa: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // Requisito global: aplicar Bearer por defecto
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new EmailJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new PasswordHashJsonConverter());
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true; // opcional, para que el JSON se vea bonito
    });

// --------------------
// CORS
// --------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", p =>
        p.WithOrigins(
            "http://localhost:5000",
            "https://localhost:5001"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

// --------------------
// INYECCIÓN DEL DBCONTEXT
// --------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// DbContext para ASP.NET Core Identity (tablas AspNetUsers/AspNetRoles/...)
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ASP.NET Core Identity: registra UserManager/SignInManager y almacena en AuthDbContext
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

// AUTENTICACIÓN JWT: configura validación del token emitido por AuthController
var jwt = builder.Configuration.GetSection("Jwt"); // Lee Issuer/Audience/Key/Expiración
var key = Encoding.UTF8.GetBytes(jwt["Key"] ?? ""); // Clave simétrica (HS256)

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => // Middleware que valida el JWT en cada request entrante
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
    // Eventos útiles para diagnóstico de 401 (invalid_token, expiración, etc.)
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            // Logea detalle del fallo de autenticación (Development)
            Console.WriteLine($"JWT auth failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine("JWT challenge: token faltante o inválido");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(); // Habilita [Authorize]/[AllowAnonymous] en controladores

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
app.UseCors("FrontendPolicy"); // Permite al frontend (Blazor) invocar con JWT
app.UseAuthentication(); // Valida JWT y crea User principal por request
app.UseAuthorization();  // Aplica políticas/roles sobre endpoints
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
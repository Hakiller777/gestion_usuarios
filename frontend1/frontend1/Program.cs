//using frontend1.Client.Pages;
using frontend1.Components;
using frontend1.Services;
using Microsoft.FluentUI.AspNetCore.Components;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpClient(); // necesario para HttpClient en AuthService
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddFluentUIComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Intermediario API V1");
    });
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseAntiforgery();

app.MapStaticAssets();
//app.MapRazorComponents<App>()
    //.AddInteractiveServerRenderMode()
    //.AddInteractiveWebAssemblyRenderMode()
    //.AddAdditionalAssemblies(typeof(frontend1.Client._Imports).Assembly);

app.Run();

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Back.Infrastructure.Data;
using Back.Presentacion.Custom;

var builder = WebApplication.CreateBuilder(args);

// Configurar el logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Agregar servicios al contenedor
var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

// Configurar la cadena de conexión y el contexto de la base de datos
var connectionString = configuration.GetConnectionString("Connection");
services.AddDbContext<SocialNetworkDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Configurar CORS
services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:4173", "http://localhost:3000", "http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Permitir cookies en CORS
    });
});

services.AddSingleton<Utilidades>();
services.AddSingleton<IConfiguration>(configuration);

// Configurar autenticación JWT
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = false,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:key"]!))
            };
            options.SaveToken = true;
        });

// Configurar política de cookies globalmente
services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.Secure = CookieSecurePolicy.Always;
});

var app = builder.Build();

// Configurar la canalización de solicitud HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowAll"); // Configuración de CORS

app.UseAuthentication();
app.UseAuthorization();

app.UseCookiePolicy();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// Middleware para asignar el token JWT a la cookie después de la autenticación
app.Use(async (context, next) =>
{
    var authResult = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
    if (authResult.Succeeded && authResult.Principal.Identity is ClaimsIdentity identity)
    {
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = context.Request.IsHttps,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddHours(1) // Duración de la cookie
        };

        // Guardar el token JWT en la cookie
        context.Response.Cookies.Append("jwt", token, cookieOptions);
    }

    await next();
});

app.Run();

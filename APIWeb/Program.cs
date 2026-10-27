using Microsoft.EntityFrameworkCore;
using APIWeb.Data;
using APIWeb.Models;
using APIWeb.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Conexión a la base de datos MySQL (Northwind)
builder.Services.AddDbContext<ContextoBD>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("ConexionMySQL")));

// Servicios para crear tokens y encriptar contraseñas de los usuarios
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

// Configuración de JWT: aquí le decimos a la API cómo verificar que el token enviado sea legítimo
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false; // Mantiene los nombres estándar de los datos del token (sub, email, name)
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwt["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero, // Hace que la sesión termine exactamente al cumplirse el tiempo de expiración
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
            NameClaimType = "name"
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Habilita el botón "Authorize" en Swagger para poder probar endpoints protegidos pegando el token
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Pega aquí tu token generado en /api/Auth/login"
    });
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

builder.Services.AddOpenApi();

// Permite que el frontend consuma la API sin problemas de permisos de origen
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseStaticFiles();

// Revisa quién es el usuario y si tiene permiso antes de llegar a los controladores
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

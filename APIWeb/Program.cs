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

// Configuración de la Base de Datos
builder.Services.AddDbContext<ContextoBD>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("ConexionMySQL")));

// Servicios de autenticación propios
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

// Configura la API para validar los JWT que ELLA MISMA emite en /api/Auth/login
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false; // Conserva los nombres cortos: sub, email, name
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwt["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero, // El token expira exactamente a la hora indicada
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
            NameClaimType = "name"
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Botón "Authorize" en Swagger para probar los endpoints protegidos
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Pega aquí el token obtenido en /api/Auth/login"
    });
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()   // Peticiones desde cualquier origen
                  .AllowAnyHeader()   // Cabeceras HTTP permitidas
                  .AllowAnyMethod();  // GET, POST, PUT, DELETE
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseStaticFiles();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


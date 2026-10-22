using Microsoft.EntityFrameworkCore;
using APIWeb.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de la Base de Datos MySQL
builder.Services.AddDbContext<ContextoBD>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("ConexionMySQL")!));

// 2. Obtener el ClientId desde secrets.json / appsettings.json o usar el tuyo como respaldo en IIS
var googleClientId = builder.Configuration["Authentication:Google:ClientId"]
                     ?? "184597401086-5c0amm2gmnid263jr8t2fjiv2ker9eer.apps.googleusercontent.com";

// 3. Configurar validación de tokens JWT de Google
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://accounts.google.com";
        options.Audience = googleClientId;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            // Acepta ambas variantes que Google suele enviar en el claim 'iss'
            ValidIssuers = new[] { "https://accounts.google.com", "accounts.google.com" },
            ValidateAudience = true,
            ValidAudience = googleClientId,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

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

// Habilitar Swagger también si quieres probar en el servidor, o dejarlo solo en Development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseCors("AllowAll");

// Sirve automáticamente index.html al entrar a la raíz del dominio en IIS
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication(); // Valida el token de Google
app.UseAuthorization();  // Verifica el atributo [Authorize]

app.MapControllers();

app.Run();
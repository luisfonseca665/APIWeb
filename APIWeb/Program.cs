using Microsoft.EntityFrameworkCore;
using APIWeb.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
var builder = WebApplication.CreateBuilder(args);

// Configuración de la Base de Datos
builder.Services.AddDbContext<ContextoBD>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("ConexionMySQL")));

// Configura la API para validar los tokens que envíe el cliente (Frontend/Postman) desde Google
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://accounts.google.com";
        options.Audience = builder.Configuration["Authentication:Google:ClientId"]; // Lee automáticamente del secrets.json
    });

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseStaticFiles();


app.UseAuthentication(); // <- Agregado: Valida quién es el usuario antes de verificar sus permisos
app.UseAuthorization();  // <- Verifica a qué tiene derecho a acceder

app.MapControllers();

app.Run();


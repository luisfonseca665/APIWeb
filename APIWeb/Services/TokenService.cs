using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using APIWeb.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace APIWeb.Services
{
    // Clase encargada de armar y firmar el token cuando un usuario inicia sesión
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public (string token, DateTime expira) GenerarToken(Usuario usuario)
        {
            // Tomamos la clave secreta configurada en appsettings para firmar digitalmente el token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var minutos = int.TryParse(_config["Jwt:ExpiraMinutos"], out var m) ? m : 60;
            var expira = DateTime.UtcNow.AddMinutes(minutos);

            // Guardamos la información básica del usuario dentro del token para reconocerlo en cada petición
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, usuario.Email),
                new(JwtRegisteredClaimNames.Name, usuario.Nombre),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                Expires = expira,
                SigningCredentials = credenciales
            };

            var token = new JsonWebTokenHandler().CreateToken(descriptor);
            return (token, expira);
        }
    }
}

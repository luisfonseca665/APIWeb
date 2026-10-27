using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using APIWeb.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace APIWeb.Services
{
    /// <summary>
    /// Genera los JWT firmados por NUESTRA API (antes los firmaba Google).
    /// </summary>
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public (string token, DateTime expira) GenerarToken(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var minutos = int.TryParse(_config["Jwt:ExpiraMinutos"], out var m) ? m : 60;
            var expira = DateTime.UtcNow.AddMinutes(minutos);

            // Claims = datos del usuario que viajan dentro del token
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

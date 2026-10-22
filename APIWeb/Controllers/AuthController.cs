using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using APIWeb.Data;
using APIWeb.Models;

namespace APIWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly ContextoBD _context;

        public AuthController(ContextoBD context)
        {
            _context = context;
        }

        [HttpPost("sincronizar")]
        public async Task<IActionResult> SincronizarUsuario()
        {
            // Extraer datos del token validado
            var googleId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var nombre = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(googleId))
            {
                return BadRequest("Token inválido o sin ID");
            }

            // Buscar si el usuario ya existe en la base de datos
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.GoogleId == googleId);

            if (usuario == null)
            {
                usuario = new Usuario
                {
                    GoogleId = googleId,
                    Email = email ?? "Sin correo",
                    Nombre = nombre ?? "Usuario",
                    FechaRegistro = DateTime.UtcNow,
                    UltimoAcceso = DateTime.UtcNow
                };
                _context.Usuarios.Add(usuario);
            }
            else
            {
                // Si existe, actualizamos su nombre y su último acceso
                usuario.Nombre = nombre ?? usuario.Nombre;
                usuario.UltimoAcceso = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Acceso registrado", usuario.Id, usuario.Nombre });
        }
    }
}
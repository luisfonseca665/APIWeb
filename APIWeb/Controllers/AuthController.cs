using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIWeb.Data;
using APIWeb.Models;
using APIWeb.Services;

namespace APIWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ContextoBD _context;
        private readonly TokenService _tokenService;
        private readonly IPasswordHasher<Usuario> _hasher;

        public AuthController(ContextoBD context, TokenService tokenService, IPasswordHasher<Usuario> hasher)
        {
            _context = context;
            _tokenService = tokenService;
            _hasher = hasher;
        }

        // Registro de nuevos usuarios: encripta la contraseña antes de guardarla en MySQL
        [HttpPost("registro")]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(RegistroDto dto)
        {
            var email = dto.Email.Trim().ToLower();

            // Evitamos duplicar correos
            if (await _context.Usuarios.AnyAsync(u => u.Email == email))
            {
                return Conflict(new { mensaje = "Ya existe una cuenta con ese correo" });
            }

            var usuario = new Usuario
            {
                Nombre = dto.Nombre.Trim(),
                Email = email,
                FechaRegistro = DateTime.UtcNow,
                UltimoAcceso = DateTime.UtcNow
            };

            usuario.PasswordHash = _hasher.HashPassword(usuario, dto.Password);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created,
                new { mensaje = "Usuario registrado correctamente", usuario.Id, usuario.Nombre, usuario.Email });
        }

        // Inicio de sesión: valida correo y contraseña, y si todo coincide entrega el token JWT
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenRespuestaDto>> Login(LoginDto dto)
        {
            var email = dto.Email.Trim().ToLower();
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

            if (usuario == null ||
                _hasher.VerifyHashedPassword(usuario, usuario.PasswordHash, dto.Password) == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { mensaje = "Correo o contraseña incorrectos" });
            }

            usuario.UltimoAcceso = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var (token, expira) = _tokenService.GenerarToken(usuario);

            return Ok(new TokenRespuestaDto
            {
                Token = token,
                Expira = expira,
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email
            });
        }

        // Endpoint para verificar que el token recibido es válido y extraer los datos del usuario conectado
        [HttpGet("perfil")]
        [Authorize]
        public IActionResult Perfil()
        {
            return Ok(new
            {
                Id = User.FindFirst("sub")?.Value,
                Email = User.FindFirst("email")?.Value,
                Nombre = User.FindFirst("name")?.Value
            });
        }
    }
}
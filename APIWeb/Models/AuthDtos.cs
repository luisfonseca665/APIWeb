using System;
using System.ComponentModel.DataAnnotations;

namespace APIWeb.Models
{
    // Datos que envía el cliente para crear una cuenta
    public class RegistroDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo no válido")]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; } = string.Empty;
    }

    // Datos que envía el cliente para iniciar sesión
    public class LoginDto
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo no válido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; } = string.Empty;
    }

    // Respuesta que devuelve la API cuando el login es correcto
    public class TokenRespuestaDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expira { get; set; }
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}

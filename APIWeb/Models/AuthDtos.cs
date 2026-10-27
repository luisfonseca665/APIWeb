using System;
using System.ComponentModel.DataAnnotations;

namespace APIWeb.Models
{
    // Modelo para recibir los datos de registro desde el formulario o Postman
    public class RegistroDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato de correo no es válido")]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; } = string.Empty;
    }

    // Modelo para recibir las credenciales de inicio de sesión
    public class LoginDto
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato de correo no es válido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; } = string.Empty;
    }

    // Estructura de la respuesta enviada al cliente tras un inicio de sesión exitoso
    public class TokenRespuestaDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expira { get; set; }
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}

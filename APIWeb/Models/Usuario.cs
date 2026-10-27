using System;
using System.ComponentModel.DataAnnotations;

namespace APIWeb.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        // El correo es el "usuario" con el que se inicia sesión (debe ser único)
        [Required]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        // Nunca se guarda la contraseña en texto plano, solo su hash
        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime UltimoAcceso { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
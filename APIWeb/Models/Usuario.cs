using System;
using System.ComponentModel.DataAnnotations;

namespace APIWeb.Models
{
    // Representa la tabla Usuarios en MySQL para almacenar cuentas del sistema
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        // Correo electrónico utilizado para iniciar sesión
        [Required]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        // Contraseña cifrada con algoritmo seguro (nunca en texto plano)
        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime UltimoAcceso { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
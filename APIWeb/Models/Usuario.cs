using System;
using System.ComponentModel.DataAnnotations;

namespace APIWeb.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string GoogleId { get; set; } = string.Empty;

        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        public DateTime UltimoAcceso { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
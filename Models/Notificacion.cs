using System;
using System.ComponentModel.DataAnnotations;

namespace tutorias.Models
{
    public class Notificacion
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public string? EmisorId { get; set; }
        public ApplicationUser? Emisor { get; set; }

        [Required]
        [StringLength(150)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Mensaje { get; set; } = string.Empty;

        public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;

        public bool Leida { get; set; } = false;

        public DateTime? FechaLectura { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace tutorias.Models.ViewModels
{
    public class NotificacionItemViewModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public DateTime FechaEnvio { get; set; }
        public string FechaEnvioTexto => FechaEnvio.ToString("dd/MM/yyyy HH:mm");
        public string TiempoRelativo { get; set; } = string.Empty;
        public bool Leida { get; set; }
        public string EmisorNombre { get; set; } = string.Empty;
        public string EmisorRol { get; set; } = string.Empty;
    }

    public class CampanaNotificacionesViewModel
    {
        public int TotalNoLeidas { get; set; }
        public List<NotificacionItemViewModel> Recientes { get; set; } = new();
    }

    public class CrearNotificacionViewModel
    {
        [Required(ErrorMessage = "El título de la notificación es obligatorio.")]
        [StringLength(150, ErrorMessage = "El título no puede exceder 150 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El mensaje de la notificación es obligatorio.")]
        [StringLength(1000, ErrorMessage = "El mensaje no puede exceder 1000 caracteres.")]
        public string Mensaje { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debes seleccionar al menos un destinatario.")]
        public List<string> DestinatariosIds { get; set; } = new();

        public List<SelectListItem> DestinatariosDisponibles { get; set; } = new();

        public bool EsCoordinador { get; set; }

        public string? FiltroRol { get; set; }
        public int? FiltroCarreraId { get; set; }
        public List<SelectListItem> CarrerasDisponibles { get; set; } = new();
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using tutorias.Models.ViewModels;

namespace tutorias.Services
{
    public interface INotificacionService
    {
        Task<CampanaNotificacionesViewModel> ObtenerNotificacionesCampanaAsync(string userId);
        Task<(bool Success, string? ErrorMessage)> MarcarComoLeidaAsync(int notificacionId, string userId);
        Task<(bool Success, string? ErrorMessage)> MarcarTodasComoLeidasAsync(string userId);
        Task<CrearNotificacionViewModel> PrepararFormularioCrearAsync(string emisorId, bool esCoordinador, string? filtroRol, int? filtroCarreraId);
        Task<(bool Success, string? ErrorMessage)> EnviarNotificacionAsync(string emisorId, bool esCoordinador, CrearNotificacionViewModel model);
    }
}

using tutorias.Models.ViewModels;

namespace tutorias.Services
{
    public interface ITutorSolicitudService
    {
        Task<SolicitudTutorViewModel> GetSolicitudViewModelAsync(string userId);
        Task<(bool Success, string? ErrorMessage)> RegistrarSolicitudAsync(string userId);
    }
}

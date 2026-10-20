using tutorias.Models.ViewModels;

namespace tutorias.Services
{
    public interface ITutorPerfilService
    {
        Task<TutorPerfilViewModel> GetPerfilAsync(string userId);
        Task<(bool Success, string? ErrorMessage)> ActualizarPerfilAsync(string userId, GuardarTutorPerfilInputModel model);
        Task<(bool Success, string? ErrorMessage)> GuardarHorarioAsync(string userId, GuardarHorarioInputModel model);
        Task<(bool Success, string? ErrorMessage)> EliminarHorarioAsync(string userId, int horarioId);
        Task<(bool Success, string? ErrorMessage)> CambiarVisibilidadAsync(string userId, bool esPublico);
        Task<(bool Success, string? ErrorMessage)> EliminarPerfilAcademicoAsync(string userId);
    }
}

using tutorias.Models.ViewModels;

namespace tutorias.Services
{
    public interface ICoordinacionService
    {
        Task<GestionUsuariosViewModel> GetGestionUsuariosAsync(string currentUserId, string? busqueda = null, string? rolFiltro = null, int pagina = 1, int tamanoPagina = 10);
        Task<(bool Success, string? ErrorMessage)> PromoverACoordinadorAsync(string targetUserId, string currentUserId);
        Task<(bool Success, string? ErrorMessage)> RevocarCoordinadorAsync(string targetUserId, string currentUserId);
    }
}

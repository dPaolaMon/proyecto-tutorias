using tutorias.Models.ViewModels;

namespace tutorias.Services
{
    public interface ICatalogoService
    {
        Task<CatalogoFiltrosViewModel> GetCatalogoAsync(string? busqueda = null, int? materiaId = null, int? carreraId = null, string? modalidad = null, int pagina = 1, int tamanoPagina = 12);
        Task<TutorDetalleViewModel?> GetTutorDetalleAsync(int perfilId);
    }
}

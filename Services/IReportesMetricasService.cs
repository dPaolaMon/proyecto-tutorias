using System.Threading.Tasks;
using tutorias.Models.ViewModels;

namespace tutorias.Services
{
    public interface IReportesMetricasService
    {
        Task<ReporteMetricasViewModel> ObtenerReporteMetricasAsync(
            string usuarioId,
            bool esCoordinador,
            ReporteMetricasFiltroViewModel filtro);
    }
}

using System;
using System.Threading.Tasks;
using tutorias.Models.ViewModels;

namespace tutorias.Services
{
    public interface IReportesService
    {
        Task<ReporteDiarioViewModel> ObtenerReporteDiarioAsync(string usuarioId, bool esCoordinador, DateTime? fecha, string? tutorFiltroId, int? carreraFiltroId);
        Task<ReporteMensualViewModel> ObtenerReporteMensualAsync(string usuarioId, bool esCoordinador, int? anio, int? mes, string? tutorFiltroId, int? carreraFiltroId);
    }
}

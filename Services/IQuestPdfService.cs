using tutorias.Models.ViewModels;

namespace tutorias.Services
{
    public interface IQuestPdfService
    {
        byte[] GenerarReporteMetricasPdf(ReporteMetricasViewModel datos);
    }
}

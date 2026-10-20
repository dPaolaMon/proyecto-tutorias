using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tutorias.Helpers;
using tutorias.Models.ViewModels;
using tutorias.Services;

namespace tutorias.Controllers
{
    [Authorize(Roles = "Coordinador,Tutor")]
    public class ReportesController : Controller
    {
        private readonly IReportesService _reportesService;
        private readonly IReportesMetricasService _metricasService;
        private readonly IQuestPdfService _pdfService;

        public ReportesController(
            IReportesService reportesService,
            IReportesMetricasService metricasService,
            IQuestPdfService pdfService)
        {
            _reportesService = reportesService;
            _metricasService = metricasService;
            _pdfService = pdfService;
        }

        [HttpGet]
        public IActionResult Index(DateTime? fecha, string? tutorId, int? carreraId)
        {
            return RedirectToAction(nameof(Diario), new { fecha, tutorId, carreraId });
        }

        [HttpGet]
        public async Task<IActionResult> Diario(DateTime? fecha, string? tutorId, int? carreraId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var esCoordinador = User.IsInRole("Coordinador");

            var vm = await _reportesService.ObtenerReporteDiarioAsync(userId, esCoordinador, fecha, tutorId, carreraId);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Mensual(string? mesInput, int? anio, int? mes, string? tutorId, int? carreraId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var esCoordinador = User.IsInRole("Coordinador");

            if (!string.IsNullOrWhiteSpace(mesInput))
            {
                var partes = mesInput.Split('-');
                if (partes.Length == 2 && int.TryParse(partes[0], out var a) && int.TryParse(partes[1], out var m))
                {
                    anio = a;
                    mes = m;
                }
            }

            var vm = await _reportesService.ObtenerReporteMensualAsync(userId, esCoordinador, anio, mes, tutorId, carreraId);
            return View(vm);
        }

        [HttpGet]
        [Authorize(Roles = "Coordinador")]
        public async Task<IActionResult> Metricas(ReporteMetricasFiltroViewModel filtro)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var vm = await _metricasService.ObtenerReporteMetricasAsync(userId, true, filtro);
            return View(vm);
        }

        [HttpGet]
        [Authorize(Roles = "Coordinador")]
        public async Task<IActionResult> ExportarMetricasPdf(ReporteMetricasFiltroViewModel filtro)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var vm = await _metricasService.ObtenerReporteMetricasAsync(userId, true, filtro);
            var pdfBytes = _pdfService.GenerarReporteMetricasPdf(vm);

            var fechaStr = TimeHelper.NowMexico.ToString("yyyyMMdd_HHmm");
            var nombreArchivo = $"Reporte_Metricas_Tutorias_UPIICSA_{fechaStr}.pdf";

            return File(pdfBytes, "application/pdf", nombreArchivo);
        }
    }
}

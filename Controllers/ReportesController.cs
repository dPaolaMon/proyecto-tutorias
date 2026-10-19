using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tutorias.Services;

namespace tutorias.Controllers
{
    [Authorize(Roles = "Coordinador,Tutor")]
    public class ReportesController : Controller
    {
        private readonly IReportesService _reportesService;

        public ReportesController(IReportesService reportesService)
        {
            _reportesService = reportesService;
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
    }
}

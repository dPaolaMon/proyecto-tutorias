using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tutorias.Models.ViewModels;
using tutorias.Services;

namespace tutorias.Controllers
{
    [Authorize]
    public class AsistenciaController : Controller
    {
        private readonly IAsistenciaService _asistenciaService;

        public AsistenciaController(IAsistenciaService asistenciaService) => _asistenciaService = asistenciaService;

        private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        private string GetBaseUrl() => $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

        [Authorize(Roles = "Tutor")]
        [HttpGet]
        public async Task<IActionResult> Index() => View(await _asistenciaService.GetPanelTutorAsync(GetCurrentUserId()));

        [Authorize(Roles = "Tutor")]
        [HttpGet]
        public async Task<IActionResult> BuscarAlumnos(string? q) =>
            Json(string.IsNullOrWhiteSpace(q) ? new System.Collections.Generic.List<BuscarAlumnoResultViewModel>() : await _asistenciaService.BuscarAlumnosAsync(q));

        [Authorize(Roles = "Tutor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarManual(RegistrarAsistenciaInputModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Debe seleccionar un alumno para solicitar el registro de asistencia.";
                return RedirectToAction(nameof(Index));
            }

            var (success, error) = await _asistenciaService.RegistrarAsistenciaAutomaticaAsync(GetCurrentUserId(), model.AlumnoId, model.MateriaId);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] = success
                ? "Solicitud de asistencia enviada al alumno. Aparecerá en su perfil para su validación."
                : (error ?? "No fue posible registrar la asistencia.");

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Tutor")]
        [HttpGet]
        public async Task<IActionResult> GenerarQr(int? materiaId = null)
        {
            var (success, detalle, error) = await _asistenciaService.ObtenerOGenerarTokenQrAsync(GetCurrentUserId(), materiaId, GetBaseUrl());
            if (!success || detalle == null)
            {
                TempData["ErrorMessage"] = error ?? "No fue posible generar el código QR para esta sesión.";
                return RedirectToAction(nameof(Index));
            }
            return View(detalle);
        }

        [Authorize(Roles = "Tutor")]
        [HttpGet]
        public async Task<IActionResult> ObtenerEstadoQr(int? materiaId = null)
        {
            var (success, detalle, error) = await _asistenciaService.ObtenerOGenerarTokenQrAsync(GetCurrentUserId(), materiaId, GetBaseUrl());
            if (!success || detalle == null) return Json(new { success = false, message = error });

            return Json(new
            {
                success = true,
                token = detalle.Token,
                qrBase64 = detalle.QrBase64,
                segundosRestantes = detalle.SegundosRestantes,
                totalAsistencias = detalle.Asistencias.Count,
                asistencias = detalle.Asistencias
            });
        }

        [Authorize(Roles = "Tutor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegenerarQr(int? materiaId = null)
        {
            var (success, _, error) = await _asistenciaService.RegenerarTokenQrAsync(GetCurrentUserId(), materiaId, GetBaseUrl());
            TempData[success ? "SuccessMessage" : "ErrorMessage"] = success
                ? "Código QR renovado exitosamente con 20 minutos de vigencia."
                : (error ?? "No fue posible regenerar el código QR.");

            return RedirectToAction(nameof(GenerarQr), new { materiaId });
        }

        [HttpGet]
        public async Task<IActionResult> ValidarQr(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["ErrorMessage"] = "El código QR o enlace proporcionado no es válido.";
                return RedirectToAction(nameof(MisAsistencias));
            }

            var (success, resultado, error) = await _asistenciaService.RegistrarAsistenciaPorQrAsync(GetCurrentUserId(), token);
            if (!success || resultado == null)
            {
                TempData["ErrorMessage"] = error ?? "No fue posible validar tu asistencia.";
                return RedirectToAction(nameof(MisAsistencias));
            }

            return View("RegistroQrExitoso", resultado);
        }

        [HttpGet]
        public IActionResult Escanear() => View();

        [HttpGet]
        public async Task<IActionResult> MisAsistencias() => View(await _asistenciaService.GetMisAsistenciasAsync(GetCurrentUserId()));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar(int asistenciaId)
        {
            if (asistenciaId <= 0) return BadRequest();
            var (success, error) = await _asistenciaService.ConfirmarAsistenciaAsync(asistenciaId, GetCurrentUserId());
            TempData[success ? "SuccessMessage" : "ErrorMessage"] = success
                ? "Asistencia confirmada exitosamente. Tu registro ha sido validado."
                : (error ?? "Error al confirmar la asistencia.");

            return RedirectToAction(nameof(MisAsistencias));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rechazar(int asistenciaId)
        {
            if (asistenciaId <= 0) return BadRequest();
            var (success, error) = await _asistenciaService.RechazarAsistenciaAsync(asistenciaId, GetCurrentUserId());
            TempData[success ? "WarningMessage" : "ErrorMessage"] = success
                ? "Has indicado que no asististe a esta sesión. El registro pendiente fue descartado."
                : (error ?? "Error al procesar el rechazo de asistencia.");

            return RedirectToAction(nameof(MisAsistencias));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Calificar(int asistenciaId, int calificacion)
        {
            if (asistenciaId <= 0 || calificacion < 1 || calificacion > 5)
            {
                return BadRequest("Parámetros de calificación inválidos.");
            }

            var (success, error) = await _asistenciaService.CalificarAsistenciaAsync(asistenciaId, GetCurrentUserId(), calificacion);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success, error });
            }

            TempData[success ? "SuccessMessage" : "ErrorMessage"] = success
                ? "Calificación registrada exitosamente. Gracias por tu evaluación."
                : (error ?? "Error al registrar la calificación.");

            return RedirectToAction(nameof(MisAsistencias));
        }
    }
}

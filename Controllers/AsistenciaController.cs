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

        public AsistenciaController(IAsistenciaService asistenciaService)
        {
            _asistenciaService = asistenciaService;
        }

        private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        [Authorize(Roles = "Tutor")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tutorId = GetCurrentUserId();
            var vm = await _asistenciaService.GetPanelTutorAsync(tutorId);
            return View(vm);
        }

        [Authorize(Roles = "Tutor")]
        [HttpGet]
        public async Task<IActionResult> BuscarAlumnos(string? q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return Json(new System.Collections.Generic.List<BuscarAlumnoResultViewModel>());
            }

            var resultados = await _asistenciaService.BuscarAlumnosAsync(q);
            return Json(resultados);
        }

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

            var tutorId = GetCurrentUserId();
            var (success, error) = await _asistenciaService.RegistrarAsistenciaAutomaticaAsync(tutorId, model.AlumnoId, model.MateriaId);

            if (!success)
            {
                TempData["ErrorMessage"] = error ?? "No fue posible registrar la asistencia.";
            }
            else
            {
                TempData["SuccessMessage"] = "Solicitud de asistencia enviada al alumno. Aparecerá en su perfil para su validación.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> MisAsistencias()
        {
            var userId = GetCurrentUserId();
            var vm = await _asistenciaService.GetMisAsistenciasAsync(userId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar(int asistenciaId)
        {
            if (asistenciaId <= 0)
            {
                TempData["ErrorMessage"] = "Identificador de asistencia no válido.";
                return RedirectToAction(nameof(MisAsistencias));
            }

            var userId = GetCurrentUserId();
            var (success, error) = await _asistenciaService.ConfirmarAsistenciaAsync(asistenciaId, userId);

            if (!success)
            {
                TempData["ErrorMessage"] = error ?? "Error al confirmar la asistencia.";
            }
            else
            {
                TempData["SuccessMessage"] = "Asistencia confirmada exitosamente. Tu registro ha sido validado.";
            }

            return RedirectToAction(nameof(MisAsistencias));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rechazar(int asistenciaId)
        {
            if (asistenciaId <= 0)
            {
                TempData["ErrorMessage"] = "Identificador de asistencia no válido.";
                return RedirectToAction(nameof(MisAsistencias));
            }

            var userId = GetCurrentUserId();
            var (success, error) = await _asistenciaService.RechazarAsistenciaAsync(asistenciaId, userId);

            if (!success)
            {
                TempData["ErrorMessage"] = error ?? "Error al procesar el rechazo de asistencia.";
            }
            else
            {
                TempData["WarningMessage"] = "Has indicado que no asististe a esta sesión. El registro pendiente fue descartado.";
            }

            return RedirectToAction(nameof(MisAsistencias));
        }
    }
}

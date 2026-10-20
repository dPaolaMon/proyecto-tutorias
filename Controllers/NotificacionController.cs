using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tutorias.Models.ViewModels;
using tutorias.Services;

namespace tutorias.Controllers
{
    [Authorize]
    public class NotificacionController : Controller
    {
        private readonly INotificacionService _notificacionService;

        public NotificacionController(INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        [HttpGet]
        public async Task<IActionResult> Campana()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var vm = await _notificacionService.ObtenerNotificacionesCampanaAsync(userId);
            return Json(new { totalNoLeidas = vm.TotalNoLeidas, notificaciones = vm.Recientes });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarLeida(int id)
        {
            if (id <= 0) return BadRequest();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var (success, error) = await _notificacionService.MarcarComoLeidaAsync(id, userId);
            return Json(new { success, error });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarTodasLeidas()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var (success, error) = await _notificacionService.MarcarTodasComoLeidasAsync(userId);
            return Json(new { success, error });
        }

        [Authorize(Roles = "Coordinador,Tutor")]
        [HttpGet]
        public async Task<IActionResult> Enviar(string? filtroRol, int? carreraId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var esCoordinador = User.IsInRole("Coordinador");

            var vm = await _notificacionService.PrepararFormularioCrearAsync(userId, esCoordinador, filtroRol, carreraId);
            return View(vm);
        }

        [Authorize(Roles = "Coordinador,Tutor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enviar(CrearNotificacionViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var esCoordinador = User.IsInRole("Coordinador");

            if (!ModelState.IsValid)
            {
                var prep = await _notificacionService.PrepararFormularioCrearAsync(userId, esCoordinador, model.FiltroRol, model.FiltroCarreraId);
                model.DestinatariosDisponibles = prep.DestinatariosDisponibles;
                model.CarrerasDisponibles = prep.CarrerasDisponibles;
                model.EsCoordinador = esCoordinador;
                return View(model);
            }

            var (success, error) = await _notificacionService.EnviarNotificacionAsync(userId, esCoordinador, model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Error al enviar la notificación.");
                var prep = await _notificacionService.PrepararFormularioCrearAsync(userId, esCoordinador, model.FiltroRol, model.FiltroCarreraId);
                model.DestinatariosDisponibles = prep.DestinatariosDisponibles;
                model.CarrerasDisponibles = prep.CarrerasDisponibles;
                model.EsCoordinador = esCoordinador;
                return View(model);
            }

            TempData["SuccessMessage"] = $"Notificación enviada exitosamente a {model.DestinatariosIds.Count} destinatario(s).";
            return RedirectToAction(nameof(Enviar));
        }
    }
}

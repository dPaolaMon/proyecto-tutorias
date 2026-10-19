using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using tutorias.Models;
using tutorias.Services;

namespace tutorias.Controllers
{
    [Authorize(Roles = "Coordinador")]
    public class CoordinacionController : Controller
    {
        private readonly ICoordinacionService _coordinacionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CoordinacionController(
            ICoordinacionService coordinacionService,
            UserManager<ApplicationUser> userManager)
        {
            _coordinacionService = coordinacionService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? busqueda, string? rolFiltro, int pagina = 1)
        {
            var currentUserId = _userManager.GetUserId(User) ?? string.Empty;
            var vm = await _coordinacionService.GetGestionUsuariosAsync(currentUserId, busqueda, rolFiltro, pagina, 10);
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Promover(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["ErrorMessage"] = "Identificador de usuario inválido.";
                return RedirectToAction(nameof(Index));
            }

            var currentUserId = _userManager.GetUserId(User) ?? string.Empty;
            var (success, error) = await _coordinacionService.PromoverACoordinadorAsync(id, currentUserId);

            if (success)
            {
                TempData["SuccessMessage"] = "Usuario promovido exitosamente al rol de Coordinador.";
            }
            else
            {
                TempData["ErrorMessage"] = error ?? "Error al promover al usuario.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Revocar(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["ErrorMessage"] = "Identificador de usuario inválido.";
                return RedirectToAction(nameof(Index));
            }

            var currentUserId = _userManager.GetUserId(User) ?? string.Empty;
            var (success, error) = await _coordinacionService.RevocarCoordinadorAsync(id, currentUserId);

            if (success)
            {
                TempData["SuccessMessage"] = "Se ha revocado el rol de Coordinador al usuario.";
            }
            else
            {
                TempData["ErrorMessage"] = error ?? "Error al revocar el rol.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> SolicitudesTutores(string? busqueda, string? rolFiltro, EstadoSolicitud? estadoFiltro, int pagina = 1)
        {
            var vm = await _coordinacionService.GetBandejaSolicitudesAsync(busqueda, rolFiltro, estadoFiltro, pagina, 10);
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AprobarSolicitud(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Identificador de solicitud inválido.";
                return RedirectToAction(nameof(SolicitudesTutores));
            }

            var currentUserId = _userManager.GetUserId(User) ?? string.Empty;
            var (success, error) = await _coordinacionService.AprobarSolicitudTutorAsync(id, currentUserId);

            if (success)
            {
                TempData["SuccessMessage"] = "Solicitud aprobada correctamente. El rol de Tutor y perfil han sido habilitados.";
            }
            else
            {
                TempData["ErrorMessage"] = error ?? "Error al aprobar la solicitud.";
            }

            return RedirectToAction(nameof(SolicitudesTutores));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RechazarSolicitud(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Identificador de solicitud inválido.";
                return RedirectToAction(nameof(SolicitudesTutores));
            }

            var currentUserId = _userManager.GetUserId(User) ?? string.Empty;
            var (success, error) = await _coordinacionService.RechazarSolicitudTutorAsync(id, currentUserId);

            if (success)
            {
                TempData["SuccessMessage"] = "Solicitud rechazada. Se ha emitido la notificación oficial al solicitante.";
            }
            else
            {
                TempData["ErrorMessage"] = error ?? "Error al rechazar la solicitud.";
            }

            return RedirectToAction(nameof(SolicitudesTutores));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SuspenderTutor(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["ErrorMessage"] = "Identificador de tutor inválido.";
                return RedirectToAction(nameof(SolicitudesTutores));
            }

            var currentUserId = _userManager.GetUserId(User) ?? string.Empty;
            var (success, error) = await _coordinacionService.SuspenderTutorAsync(id, currentUserId);

            if (success)
            {
                TempData["SuccessMessage"] = "Estatus de tutor suspendido y perfil despublicado exitosamente.";
            }
            else
            {
                TempData["ErrorMessage"] = error ?? "Error al suspender el estatus de tutor.";
            }

            return RedirectToAction(nameof(SolicitudesTutores));
        }

        [HttpGet]
        public async Task<IActionResult> Encuestas([FromServices] IEncuestasService encuestasService)
        {
            var vm = await encuestasService.ObtenerPanelEncuestasAsync();
            return View(vm);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using tutorias.Models;
using tutorias.Services;

namespace tutorias.Controllers
{
    [Authorize]
    public class TutoriasController : Controller
    {
        private readonly ITutorSolicitudService _tutorSolicitudService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TutoriasController(
            ITutorSolicitudService tutorSolicitudService,
            UserManager<ApplicationUser> userManager)
        {
            _tutorSolicitudService = tutorSolicitudService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Solicitud()
        {
            if (User.IsInRole("Coordinador"))
            {
                TempData["ErrorMessage"] = "El perfil de Coordinación no realiza solicitudes de tutor; administra las tutorías desde su panel.";
                return RedirectToAction("Index", "Coordinacion");
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var vm = await _tutorSolicitudService.GetSolicitudViewModelAsync(userId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Solicitar()
        {
            if (User.IsInRole("Coordinador"))
            {
                TempData["ErrorMessage"] = "El perfil de Coordinación no puede postularse como tutor.";
                return RedirectToAction("Index", "Coordinacion");
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var (success, error) = await _tutorSolicitudService.RegistrarSolicitudAsync(userId);
            if (success)
            {
                TempData["SuccessMessage"] = "Tu solicitud de alta como Tutor ha sido enviada a Coordinación.";
            }
            else
            {
                TempData["ErrorMessage"] = error ?? "Ocurrió un error al procesar tu solicitud.";
            }

            return RedirectToAction(nameof(Solicitud));
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using tutorias.Models;
using tutorias.Models.ViewModels;
using tutorias.Services;

namespace tutorias.Controllers
{
    [Authorize(Roles = "Tutor")]
    public class TutorPerfilController : Controller
    {
        private readonly ITutorPerfilService _tutorPerfilService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TutorPerfilController(
            ITutorPerfilService tutorPerfilService,
            UserManager<ApplicationUser> userManager)
        {
            _tutorPerfilService = tutorPerfilService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            var vm = await _tutorPerfilService.GetPerfilAsync(userId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarPerfil(GuardarTutorPerfilInputModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Por favor verifica los campos ingresados.";
                return RedirectToAction(nameof(Index));
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            var (success, error) = await _tutorPerfilService.ActualizarPerfilAsync(userId, model);
            if (success)
            {
                TempData["SuccessMessage"] = "Tu perfil académico y materias se han guardado exitosamente.";
            }
            else
            {
                TempData["ErrorMessage"] = error ?? "Error al actualizar el perfil.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarHorario(GuardarHorarioInputModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Por favor verifica los datos del horario y salón asignado.";
                return RedirectToAction(nameof(Index));
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            var (success, error) = await _tutorPerfilService.GuardarHorarioAsync(userId, model);
            if (success)
            {
                TempData["SuccessMessage"] = "Horario y aula asignada guardados correctamente.";
            }
            else
            {
                TempData["ErrorMessage"] = error ?? "Error al guardar el horario.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarHorario(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Identificador de horario inválido.";
                return RedirectToAction(nameof(Index));
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            var (success, error) = await _tutorPerfilService.EliminarHorarioAsync(userId, id);
            if (success)
            {
                TempData["SuccessMessage"] = "Bloque de horario eliminado exitosamente.";
            }
            else
            {
                TempData["ErrorMessage"] = error ?? "Error al eliminar el horario.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleVisibilidad(bool esPublico)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            var (success, error) = await _tutorPerfilService.CambiarVisibilidadAsync(userId, esPublico);
            if (success)
            {
                TempData["SuccessMessage"] = esPublico
                    ? "Tu perfil ahora es público y visible en el Catálogo de Tutorías."
                    : "Tu perfil ha sido despublicado y se encuentra oculto en el Catálogo.";
            }
            else
            {
                TempData["ErrorMessage"] = error ?? "Error al cambiar la visibilidad del perfil.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> VistaPrevia()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            var vm = await _tutorPerfilService.GetPerfilAsync(userId);
            return View(vm);
        }
    }
}

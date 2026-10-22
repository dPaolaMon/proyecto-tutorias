using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tutorias.Services;

namespace tutorias.Controllers
{
    [Authorize]
    public class CatalogoController : Controller
    {
        private readonly ICatalogoService _catalogoService;

        public CatalogoController(ICatalogoService catalogoService)
        {
            _catalogoService = catalogoService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(
            string? busqueda,
            int? materiaId,
            int? carreraId,
            string? modalidad,
            int pagina = 1)
        {
            const int pageSize = 12;
            var vm = await _catalogoService.GetCatalogoAsync(busqueda, materiaId, carreraId, modalidad, pagina, pageSize);
            return View(vm);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Detalle(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var vm = await _catalogoService.GetTutorDetalleAsync(id);
            if (vm == null)
            {
                TempData["ErrorMessage"] = "El perfil del tutor no se encuentra disponible o no está publicado actualmente.";
                return RedirectToAction(nameof(Index));
            }

            return View(vm);
        }
    }
}

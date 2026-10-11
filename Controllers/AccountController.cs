using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using tutorias.Models;
using tutorias.Models.ViewModels;
using tutorias.Services;

namespace tutorias.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserProfileService _profileService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IUserProfileService profileService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _profileService = profileService;
        }

        [HttpGet]
        public IActionResult Register() => User.Identity?.IsAuthenticated == true ? RedirectToAction("Index", "Home") : View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var emailLower = model.Email.Trim().ToLowerInvariant();
            if (!emailLower.EndsWith("@alumno.ipn.mx") && !emailLower.EndsWith("@ipn.mx"))
            {
                ModelState.AddModelError("Email", "El correo debe pertenecer al dominio institucional (@alumno.ipn.mx o @ipn.mx).");
                return View(model);
            }

            var rolInicial = emailLower.EndsWith("@alumno.ipn.mx") ? "Alumno" : "Docente";
            var user = new ApplicationUser { UserName = emailLower, Email = emailLower, NombreCompleto = model.NombreCompleto.Trim(), Boleta = model.Boleta.Trim(), PerfilCompleto = false, Activo = true };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors) ModelState.AddModelError(string.Empty, err.Description);
                return View(model);
            }

            await _userManager.AddToRoleAsync(user, rolInicial);
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction(nameof(CompleteProfile));
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null) { ViewData["ReturnUrl"] = returnUrl; return View(); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email.Trim());
            if (user != null && !user.Activo)
            {
                ModelState.AddModelError(string.Empty, "Esta cuenta ha sido dada de baja. Si consideras que es un error, contacta a la Coordinación.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (!result.Succeeded) { ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido."); return View(model); }

            if (user != null && !user.PerfilCompleto) return RedirectToAction(nameof(CompleteProfile));
            return (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) ? Redirect(returnUrl) : RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CompleteProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));
            ViewBag.Carreras = await _profileService.GetCarrerasSelectListAsync();
            return View(new CompleteProfileViewModel { EsDocente = await _userManager.IsInRoleAsync(user, "Docente") });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteProfile(CompleteProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));

            model.EsDocente = await _userManager.IsInRoleAsync(user, "Docente");
            if (!model.EsDocente && !model.Semestre.HasValue) ModelState.AddModelError("Semestre", "Debe seleccionar su semestre actual.");

            if (!ModelState.IsValid)
            {
                ViewBag.Carreras = await _profileService.GetCarrerasSelectListAsync(model.CarreraId);
                return View(model);
            }

            var (ok, error) = await _profileService.CompleteProfileAsync(user, model);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "Error al completar el perfil.");
                ViewBag.Carreras = await _profileService.GetCarrerasSelectListAsync(model.CarreraId);
                return View(model);
            }
            TempData["SuccessMessage"] = "¡Perfil completado exitosamente!";
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));
            var vm = await _profileService.GetProfileAsync(user);
            return vm == null ? RedirectToAction(nameof(Login)) : View(vm);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));
            var vm = await _profileService.GetEditProfileAsync(user);
            if (vm == null) return RedirectToAction(nameof(Login));
            ViewBag.Carreras = await _profileService.GetCarrerasSelectListAsync(vm.CarreraId);
            return View(vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));

            model.EsDocente = await _userManager.IsInRoleAsync(user, "Docente");
            model.Email = user.Email ?? string.Empty; // Inmutable

            if (!ModelState.IsValid)
            {
                ViewBag.Carreras = await _profileService.GetCarrerasSelectListAsync(model.CarreraId);
                return View(model);
            }

            var (ok, error) = await _profileService.UpdateProfileAsync(user, model);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "Error al actualizar perfil.");
                ViewBag.Carreras = await _profileService.GetCarrerasSelectListAsync(model.CarreraId);
                return View(model);
            }
            TempData["SuccessMessage"] = "Tu perfil ha sido actualizado correctamente.";
            return RedirectToAction(nameof(Profile));
        }

        [Authorize]
        [HttpGet]
        public IActionResult DeleteProfile() => View(new DeleteProfileViewModel());

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProfile(DeleteProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));

            var (ok, error) = await _profileService.LogicalDeleteUserAsync(user, model.MotivoBaja);
            if (!ok) { ModelState.AddModelError(string.Empty, error ?? "Error al procesar la baja."); return View(model); }

            await _signInManager.SignOutAsync();
            TempData["SuccessMessage"] = "Tu cuenta ha sido dada de baja exitosamente del Sistema de Tutorías.";
            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() { await _signInManager.SignOutAsync(); return RedirectToAction("Index", "Home"); }

        [HttpGet]
        public IActionResult AccessDenied() => View();
    }
}

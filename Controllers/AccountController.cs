using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IEmailSenderService _emailService;
        private readonly IWebHostEnvironment _environment;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUserProfileService profileService,
            IEmailSenderService emailService,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _profileService = profileService;
            _emailService = emailService;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Register() => User.Identity?.IsAuthenticated == true ? RedirectToAction("Index", "Home") : View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var emailLower = model.Email.Trim().ToLowerInvariant();
            if (!emailLower.EndsWith("@alumno.ipn.mx") && !emailLower.EndsWith("@ipn.mx"))
            {
                ModelState.AddModelError("Email", "El correo debe pertenecer al dominio institucional (@alumno.ipn.mx o @ipn.mx).");
                return View(model);
            }

            var boletaTrimmed = model.Boleta.Trim();
            if (!boletaTrimmed.All(char.IsDigit))
            {
                ModelState.AddModelError("Boleta", "La boleta o número de empleado solo debe contener dígitos numéricos.");
                return View(model);
            }

            var boletaExiste = await _userManager.Users.AnyAsync(u => u.Boleta == boletaTrimmed);
            if (boletaExiste)
            {
                ModelState.AddModelError("Boleta", "La boleta o número de empleado ingresado ya se encuentra registrado en el sistema.");
                return View(model);
            }

            var rolInicial = emailLower.EndsWith("@alumno.ipn.mx") ? "Alumno" : "Docente";
            var user = new ApplicationUser
            {
                UserName = emailLower,
                Email = emailLower,
                Nombre = model.Nombre.Trim(),
                ApellidoPaterno = model.ApellidoPaterno.Trim(),
                ApellidoMaterno = model.ApellidoMaterno.Trim(),
                Boleta = model.Boleta.Trim(),
                PerfilCompleto = false,
                Activo = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors) ModelState.AddModelError(string.Empty, err.Description);
                return View(model);
            }

            // Asignar rol inicial
            await _userManager.AddToRoleAsync(user, rolInicial);

            // Generar token de confirmación de correo institucional
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, token }, protocol: Request.Scheme);

            await _emailService.SendConfirmationEmailAsync(user.Email, confirmationLink!, user.NombreCompleto);

            return RedirectToAction(nameof(RegisterConfirmation), new { email = user.Email, devLink = _environment.IsDevelopment() ? confirmationLink : null });
        }

        [HttpGet]
        public IActionResult RegisterConfirmation(string email, string? devLink = null)
        {
            ViewBag.Email = email;
            ViewBag.DevLink = devLink;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                ViewBag.ErrorMessage = "El enlace de confirmación es inválido o está incompleto.";
                return View();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = "No se encontró el usuario solicitado.";
                return View();
            }

            if (user.EmailConfirmed)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                TempData["SuccessMessage"] = "Tu correo institucional ya ha sido verificado.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                TempData["SuccessMessage"] = "¡Tu correo institucional ha sido verificado con éxito!";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "El enlace de confirmación ha expirado o no es válido.";
            return View();
        }

        [HttpGet]
        public IActionResult ResendConfirmationEmail(string? email = null)
        {
            return View(new ResendConfirmationEmailViewModel { Email = email ?? string.Empty });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendConfirmationEmail(ResendConfirmationEmailViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email.Trim().ToLowerInvariant());
            if (user != null && !user.EmailConfirmed)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, token }, protocol: Request.Scheme);
                await _emailService.SendConfirmationEmailAsync(user.Email!, confirmationLink!, user.NombreCompleto);

                return RedirectToAction(nameof(RegisterConfirmation), new { email = user.Email, devLink = _environment.IsDevelopment() ? confirmationLink : null });
            }

            TempData["SuccessMessage"] = "Si el correo corresponde a una cuenta registrada no confirmada, se ha enviado un nuevo enlace de activación.";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null) { ViewData["ReturnUrl"] = returnUrl; return View(); }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email.Trim());
            if (user != null)
            {
                if (!await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    ModelState.AddModelError(string.Empty, "Contraseña incorrecta.");
                    return View(model);
                }

                if (!user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Debes confirmar tu correo institucional antes de iniciar sesión.");
                    ViewBag.RequireEmailConfirmation = true;
                    ViewBag.UnconfirmedEmail = user.Email;
                    return View(model);
                }

                if (!user.Activo)
                {
                    await _signInManager.SignInAsync(user, model.RememberMe);
                    TempData["WarningMessage"] = "Tu cuenta se encuentra dada de baja. Puedes reactivarla a continuación.";
                    return RedirectToAction(nameof(Profile));
                }
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (!result.Succeeded) { ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido."); return View(model); }

            if (user != null && !user.PerfilCompleto) return RedirectToAction(nameof(CompleteProfile));
            return (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) ? Redirect(returnUrl) : RedirectToAction("Index", "Home");
        }

        [Authorize, HttpGet]
        public async Task<IActionResult> CompleteProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));
            ViewBag.Carreras = await _profileService.GetCarrerasSelectListAsync();
            return View(new CompleteProfileViewModel { EsDocente = await _userManager.IsInRoleAsync(user, "Docente") });
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteProfile(CompleteProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));

            model.EsDocente = await _userManager.IsInRoleAsync(user, "Docente");
            if (model.EsDocente)
            {
                if (string.IsNullOrWhiteSpace(model.Academia))
                {
                    ModelState.AddModelError("Academia", "Debe ingresar el nombre de su academia.");
                }
                else if (!System.Text.RegularExpressions.Regex.IsMatch(model.Academia.Trim(), @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$"))
                {
                    ModelState.AddModelError("Academia", "La academia solo debe contener letras y espacios.");
                }
            }
            else
            {
                if (!model.CarreraId.HasValue || model.CarreraId.Value <= 0)
                {
                    ModelState.AddModelError("CarreraId", "Debe seleccionar su carrera.");
                }
                if (!model.Semestre.HasValue)
                {
                    ModelState.AddModelError("Semestre", "Debe seleccionar su semestre actual.");
                }
            }

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

        [Authorize, HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));
            var vm = await _profileService.GetProfileAsync(user);
            return vm == null ? RedirectToAction(nameof(Login)) : View(vm);
        }

        [Authorize, HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));
            var vm = await _profileService.GetEditProfileAsync(user);
            if (vm == null) return RedirectToAction(nameof(Login));
            ViewBag.Carreras = await _profileService.GetCarrerasSelectListAsync(vm.CarreraId);
            return View(vm);
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));

            model.EsDocente = await _userManager.IsInRoleAsync(user, "Docente");
            model.Email = user.Email ?? string.Empty; // Inmutable

            if (model.EsDocente)
            {
                if (string.IsNullOrWhiteSpace(model.Academia))
                {
                    ModelState.AddModelError("Academia", "Debe ingresar el nombre de su academia.");
                }
                else if (!System.Text.RegularExpressions.Regex.IsMatch(model.Academia.Trim(), @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$"))
                {
                    ModelState.AddModelError("Academia", "La academia solo debe contener letras y espacios.");
                }
            }
            else
            {
                if (!model.CarreraId.HasValue || model.CarreraId.Value <= 0)
                {
                    ModelState.AddModelError("CarreraId", "Debe seleccionar su carrera.");
                }
                if (!model.Semestre.HasValue)
                {
                    ModelState.AddModelError("Semestre", "Debe seleccionar su semestre actual.");
                }
            }

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

        [Authorize, HttpGet]
        public IActionResult DeleteProfile() => View(new DeleteProfileViewModel());

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProfile(DeleteProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));
            if (!model.ConfirmarBaja) { ModelState.AddModelError("ConfirmarBaja", "Debes confirmar que deseas dar de baja tu cuenta."); return View(model); }

            var (ok, error) = await _profileService.LogicalDeleteUserAsync(user, model.MotivoBaja);
            if (!ok) { ModelState.AddModelError(string.Empty, error ?? "Error al procesar la baja."); return View(model); }

            TempData["WarningMessage"] = "Tu cuenta ha sido dada de baja exitosamente. Puedes reactivarla en cualquier momento.";
            return RedirectToAction(nameof(Profile));
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ReactivateProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));

            var (ok, error) = await _profileService.ReactivateUserAsync(user);
            if (!ok) { TempData["ErrorMessage"] = error ?? "Error al reactivar cuenta."; return RedirectToAction(nameof(Profile)); }

            TempData["SuccessMessage"] = "¡Tu cuenta ha sido reactivada exitosamente! Todas las funciones del sistema han sido restauradas.";
            return RedirectToAction(nameof(Profile));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() { await _signInManager.SignOutAsync(); return RedirectToAction("Index", "Home"); }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var emailLower = model.Email.Trim().ToLowerInvariant();
            if (!emailLower.EndsWith("@alumno.ipn.mx") && !emailLower.EndsWith("@ipn.mx"))
            {
                ModelState.AddModelError("Email", "El correo debe pertenecer al dominio institucional (@alumno.ipn.mx o @ipn.mx).");
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(emailLower);
            string? devLink = null;

            if (user != null && user.EmailConfirmed)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action(nameof(ResetPassword), "Account", new { token, email = user.Email }, protocol: Request.Scheme);
                await _emailService.SendPasswordResetEmailAsync(user.Email!, resetLink!, user.NombreCompleto);

                if (_environment.IsDevelopment())
                {
                    devLink = resetLink;
                }
            }

            return RedirectToAction(nameof(ForgotPasswordConfirmation), new { email = emailLower, devLink });
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation(string? email = null, string? devLink = null)
        {
            ViewBag.Email = email;
            ViewBag.DevLink = devLink;
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string? token = null, string? email = null)
        {
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.ErrorMessage = "El enlace de restablecimiento es inválido o no contiene un token.";
                return View("Error");
            }

            var model = new ResetPasswordViewModel
            {
                Token = token,
                Email = email ?? string.Empty
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email.Trim().ToLowerInvariant());
            if (user == null)
            {
                // No revelar que el usuario no existe
                TempData["SuccessMessage"] = "Tu contraseña ha sido restablecida exitosamente. Ya puedes iniciar sesión.";
                return RedirectToAction(nameof(Login));
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Tu contraseña ha sido restablecida exitosamente. Ya puedes iniciar sesión.";
                return RedirectToAction(nameof(Login));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();
    }
}

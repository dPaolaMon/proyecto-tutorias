using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using tutorias.Data;
using tutorias.Models;
using tutorias.Models.ViewModels;

namespace tutorias.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxImageSizeBytes = 2 * 1024 * 1024; // 2MB

        public UserProfileService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        public async Task<List<Carrera>> GetCarrerasAsync()
        {
            return await _context.Carreras
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<Microsoft.AspNetCore.Mvc.Rendering.SelectList> GetCarrerasSelectListAsync(int? selectedId = null)
        {
            var carreras = await GetCarrerasAsync();
            return new Microsoft.AspNetCore.Mvc.Rendering.SelectList(carreras, "Id", "Nombre", selectedId);
        }

        public async Task<ProfileViewModel?> GetProfileAsync(ApplicationUser user)
        {
            if (user == null) return null;

            await _context.Entry(user).Reference(u => u.Carrera).LoadAsync();
            var roles = await _userManager.GetRolesAsync(user);

            return new ProfileViewModel
            {
                Id             = user.Id,
                Nombre         = user.Nombre,
                ApellidoPaterno = user.ApellidoPaterno,
                ApellidoMaterno = user.ApellidoMaterno,
                NombreCompleto = user.NombreCompleto,
                Boleta         = user.Boleta,
                Email          = user.Email ?? string.Empty,
                CarreraNombre  = user.Carrera?.Nombre,
                Academia       = user.Academia,
                Semestre       = user.Semestre,
                FotoUrl        = user.FotoUrl,
                TipoTutor      = user.TipoTutor,
                Roles          = roles,
                PerfilCompleto = user.PerfilCompleto,
                Activo         = user.Activo,
                FechaBaja      = user.FechaBaja,
                MotivoBaja     = user.MotivoBaja
            };
        }

        public async Task<EditProfileViewModel?> GetEditProfileAsync(ApplicationUser user)
        {
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new EditProfileViewModel
            {
                Nombre         = user.Nombre,
                ApellidoPaterno = user.ApellidoPaterno,
                ApellidoMaterno = user.ApellidoMaterno,
                Boleta         = user.Boleta,
                Email          = user.Email ?? string.Empty, // Solo lectura en vista
                CarreraId      = user.CarreraId,
                Academia       = user.Academia,
                Semestre       = user.Semestre,
                FotoUrlActual  = user.FotoUrl,
                EsDocente      = roles.Contains("Docente")
            };
        }

        public async Task<(bool Succeeded, string? ErrorMessage)> UpdateProfileAsync(
            ApplicationUser user,
            EditProfileViewModel model)
        {
            if (user == null)
            {
                return (false, "El usuario no fue encontrado.");
            }

            if (!model.Boleta.Trim().All(char.IsDigit))
            {
                return (false, "La boleta solo debe contener dígitos numéricos.");
            }

            // REGLA: El correo es estrictamente inmutable; no se altera Email ni UserName
            user.Nombre = model.Nombre.Trim();
            user.ApellidoPaterno = model.ApellidoPaterno.Trim();
            user.ApellidoMaterno = model.ApellidoMaterno.Trim();
            user.Boleta = model.Boleta.Trim();

            if (model.EsDocente)
            {
                if (!string.IsNullOrWhiteSpace(model.Academia) && !System.Text.RegularExpressions.Regex.IsMatch(model.Academia.Trim(), @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$"))
                {
                    return (false, "La academia solo debe contener letras y espacios.");
                }
                user.Academia = model.Academia?.Trim();
                user.CarreraId = null;
                user.Semestre = null;
            }
            else
            {
                user.CarreraId = model.CarreraId;
                user.Semestre = model.Semestre;
                user.Academia = null;
            }

            if (model.Foto != null && model.Foto.Length > 0)
            {
                var (saved, fotoUrl, error) = await SaveProfileImageAsync(model.Foto);
                if (!saved)
                {
                    return (false, error);
                }
                user.FotoUrl = fotoUrl;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errorMsg = string.Join("; ", result.Errors.Select(e => e.Description));
                return (false, errorMsg);
            }

            return (true, null);
        }

        public async Task<(bool Succeeded, string? ErrorMessage)> CompleteProfileAsync(
            ApplicationUser user,
            CompleteProfileViewModel model)
        {
            if (user == null)
            {
                return (false, "El usuario no fue encontrado.");
            }

            if (model.EsDocente)
            {
                if (!string.IsNullOrWhiteSpace(model.Academia) && !System.Text.RegularExpressions.Regex.IsMatch(model.Academia.Trim(), @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$"))
                {
                    return (false, "La academia solo debe contener letras y espacios.");
                }
                user.Academia = model.Academia?.Trim();
                user.CarreraId = null;
                user.Semestre = null;
            }
            else
            {
                user.CarreraId = model.CarreraId;
                user.Semestre = model.Semestre;
                user.Academia = null;
            }
            user.PerfilCompleto = true;

            if (model.Foto != null && model.Foto.Length > 0)
            {
                var (saved, fotoUrl, error) = await SaveProfileImageAsync(model.Foto);
                if (!saved)
                {
                    return (false, error);
                }
                user.FotoUrl = fotoUrl;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errorMsg = string.Join("; ", result.Errors.Select(e => e.Description));
                return (false, errorMsg);
            }

            return (true, null);
        }

        public async Task<(bool Succeeded, string? ErrorMessage)> LogicalDeleteUserAsync(
            ApplicationUser user,
            string? motivoBaja)
        {
            if (user == null)
            {
                return (false, "El usuario no fue encontrado.");
            }

            user.Activo = false;
            user.FechaBaja = DateTime.UtcNow;
            user.MotivoBaja = string.IsNullOrWhiteSpace(motivoBaja)
                ? "Baja de cuenta solicitada por el usuario"
                : motivoBaja.Trim();

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errorMsg = string.Join("; ", result.Errors.Select(e => e.Description));
                return (false, errorMsg);
            }

            return (true, null);
        }

        public async Task<(bool Succeeded, string? ErrorMessage)> ReactivateUserAsync(ApplicationUser user)
        {
            if (user == null)
            {
                return (false, "El usuario no fue encontrado.");
            }

            user.Activo = true;
            user.FechaBaja = null;
            user.MotivoBaja = null;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errorMsg = string.Join("; ", result.Errors.Select(e => e.Description));
                return (false, errorMsg);
            }

            return (true, null);
        }

        private async Task<(bool Success, string? Url, string? Error)> SaveProfileImageAsync(IFormFile file)
        {
            if (file.Length > MaxImageSizeBytes)
            {
                return (false, null, "La imagen no debe superar los 2 MB.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedImageExtensions.Contains(extension))
            {
                return (false, null, "Formato de imagen inválido. Solo se admiten archivos JPG, PNG o WEBP.");
            }

            var uploadDir = Path.Combine(_environment.WebRootPath, "uploads", "perfiles");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            var uniqueFileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadDir, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return (true, $"/uploads/perfiles/{uniqueFileName}", null);
        }
    }
}

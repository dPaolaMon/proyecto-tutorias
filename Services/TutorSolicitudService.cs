using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using tutorias.Data;
using tutorias.Models;
using tutorias.Models.ViewModels;

namespace tutorias.Services
{
    public class TutorSolicitudService : ITutorSolicitudService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TutorSolicitudService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<SolicitudTutorViewModel> GetSolicitudViewModelAsync(string userId)
        {
            var user = await _context.Users
                .Include(u => u.Carrera)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new InvalidOperationException("Usuario no encontrado.");
            }

            if (await _userManager.IsInRoleAsync(user, "Coordinador"))
            {
                throw new InvalidOperationException("El perfil de Coordinación no tiene permitido acceder a la solicitud de tutor.");
            }

            var esDocente = user.Email != null && user.Email.EndsWith("@ipn.mx") && !user.Email.EndsWith("@alumno.ipn.mx");
            var esTutor = await _userManager.IsInRoleAsync(user, "Tutor") || user.TipoTutor != TipoTutor.Ninguno;

            var tipoTutorSolicitado = esDocente ? TipoTutor.DocenteTutor : TipoTutor.AlumnoTutor;

            var ultimaSolicitud = await _context.SolicitudesTutor
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.FechaSolicitud)
                .FirstOrDefaultAsync();

            var programaOAcademia = esDocente
                ? (!string.IsNullOrWhiteSpace(user.Academia) ? user.Academia : "Sin academia asignada")
                : (user.Carrera?.Nombre ?? "Sin carrera asignada");

            return new SolicitudTutorViewModel
            {
                UserId = user.Id,
                NombreCompleto = user.NombreCompleto,
                Boleta = user.Boleta,
                Email = user.Email ?? string.Empty,
                EsDocente = esDocente,
                ProgramaOAcademia = programaOAcademia,
                PerfilCompleto = user.PerfilCompleto,
                TipoTutorSolicitado = tipoTutorSolicitado,
                YaEsTutor = esTutor,
                UltimoEstado = ultimaSolicitud?.Estado,
                FechaUltimaSolicitud = ultimaSolicitud?.FechaSolicitud,
                RespuestaCoordinacion = ultimaSolicitud?.RespuestaCoordinacion,
                FechaRespuesta = ultimaSolicitud?.FechaRespuesta
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> RegistrarSolicitudAsync(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return (false, "El usuario no fue encontrado.");
            }

            if (await _userManager.IsInRoleAsync(user, "Coordinador"))
            {
                return (false, "Los usuarios con rol de Coordinador no pueden postularse como tutores.");
            }

            if (!user.Activo)
            {
                return (false, "Tu cuenta se encuentra en baja lógica. Debes reactivarla antes de realizar una solicitud.");
            }

            if (!user.PerfilCompleto)
            {
                return (false, "Debes completar tu perfil académico antes de solicitar el estatus de Tutor.");
            }

            var esTutor = await _userManager.IsInRoleAsync(user, "Tutor") || user.TipoTutor != TipoTutor.Ninguno;
            if (esTutor)
            {
                return (false, "Ya cuentas con el estatus de Tutor activo en el sistema.");
            }

            var tienePendiente = await _context.SolicitudesTutor
                .AnyAsync(s => s.UserId == userId && s.Estado == EstadoSolicitud.Pendiente);

            if (tienePendiente)
            {
                return (false, "Ya tienes una solicitud pendiente de revisión por parte de la Coordinación.");
            }

            var esDocente = user.Email != null && user.Email.EndsWith("@ipn.mx") && !user.Email.EndsWith("@alumno.ipn.mx");
            var tipoTutorSolicitado = esDocente ? TipoTutor.DocenteTutor : TipoTutor.AlumnoTutor;

            var nuevaSolicitud = new SolicitudTutor
            {
                UserId = userId,
                TipoTutorSolicitado = tipoTutorSolicitado,
                FechaSolicitud = DateTime.UtcNow,
                Estado = EstadoSolicitud.Pendiente,
                RespuestaCoordinacion = null,
                FechaRespuesta = null
            };

            _context.SolicitudesTutor.Add(nuevaSolicitud);
            await _context.SaveChangesAsync();

            return (true, null);
        }
    }
}

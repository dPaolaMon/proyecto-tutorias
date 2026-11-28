using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using tutorias.Data;
using tutorias.Helpers;
using tutorias.Models;
using Aviso = System.ValueTuple<System.Collections.Generic.IEnumerable<string>, string, string>;

namespace tutorias.Services
{
    public class NotificacionAutomaticaService : INotificacionAutomaticaService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<NotificacionAutomaticaService> _logger;

        public NotificacionAutomaticaService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<NotificacionAutomaticaService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public Task SolicitudNuevaAsync(string solicitanteId) => EnviarSeguroAsync(async () =>
        {
            var nombre = await NombreAsync(solicitanteId);
            return (await CoordinadoresAsync(), "Nueva solicitud de tutor",
                $"{nombre} solicitó darse de alta como tutor. Revísala en la bandeja de solicitudes.");
        });

        public Task SolicitudResueltaAsync(string solicitanteId, bool aprobada) => EnviarSeguroAsync(() =>
            Task.FromResult<Aviso>((new[] { solicitanteId },
                aprobada ? "Solicitud de tutor aprobada" : "Solicitud de tutor rechazada",
                aprobada
                    ? "Tu solicitud fue aprobada. Ya puedes configurar tu perfil de tutor y tus horarios."
                    : "Tu solicitud no fue aprobada. Pasa a Coordinación para validar tu situación.")));

        public Task TutorRevocadoAsync(string tutorId) => EnviarSeguroAsync(() =>
            Task.FromResult<Aviso>((new[] { tutorId }, "Se revocó tu estatus de tutor",
                "Coordinación revocó tu estatus de tutor y tu perfil dejó de publicarse en el catálogo.")));

        public Task CoordinadorCambiadoAsync(string usuarioId, bool asignado) => EnviarSeguroAsync(() =>
            Task.FromResult<Aviso>((new[] { usuarioId },
                asignado ? "Ahora eres Coordinación" : "Se retiró tu rol de Coordinación",
                asignado
                    ? "Se te asignó el rol de Coordinación. Si no ves sus opciones, cierra sesión y vuelve a entrar."
                    : "Ya no cuentas con el rol de Coordinación.")));

        public Task AsistenciaPorConfirmarAsync(int asistenciaId) => EnviarSeguroAsync(async () =>
        {
            var datos = await _context.Asistencias
                .Where(a => a.Id == asistenciaId)
                .Select(a => new { a.AlumnoId, a.SesionId })
                .FirstAsync();
            var (tutorId, materia) = await SesionAsync(datos.SesionId);
            return (new[] { datos.AlumnoId }, "Tienes una asistencia por confirmar",
                $"{await NombreAsync(tutorId)} registró tu asistencia a {materia}. Confírmala o recházala en «Mis asistencias».");
        });

        public Task AsistenciaRespondidaAsync(int sesionId, string alumnoId, bool confirmada) => EnviarSeguroAsync(async () =>
        {
            var (tutorId, materia) = await SesionAsync(sesionId);
            var nombre = await NombreAsync(alumnoId);
            return (new[] { tutorId },
                confirmada ? "Asistencia confirmada" : "Asistencia rechazada",
                confirmada
                    ? $"{nombre} confirmó su asistencia a {materia}."
                    : $"{nombre} indicó que no asistió a {materia}. El registro se descartó.");
        });

        public Task InvitacionACalificarAsync(int sesionId, string alumnoId) => EnviarSeguroAsync(async () =>
        {
            var (_, materia) = await SesionAsync(sesionId);
            return (new[] { alumnoId }, "Califica tu tutoría",
                $"Tu asistencia a {materia} quedó registrada. Puedes calificar la tutoría en «Mis asistencias».");
        });

        public Task CalificacionBajaAsync(int sesionId, int calificacion) => EnviarSeguroAsync(async () =>
        {
            var (tutorId, materia) = await SesionAsync(sesionId);
            // No se identifica al alumno: la evaluación no debe exponerlo
            return (await CoordinadoresAsync(), "Calificación baja en una tutoría",
                $"Una tutoría de {materia} con {await NombreAsync(tutorId)} recibió {calificacion} de 5 estrellas.");
        });

        public Task HorarioModificadoAsync(string tutorId, TutorHorario anterior, TutorHorario nuevo) => EnviarSeguroAsync(async () =>
            (await AlumnosAtendidosAsync(tutorId), "Cambio de horario de tutoría",
                $"{await NombreAsync(tutorId)} cambió un horario: antes {Describir(anterior)}; ahora {Describir(nuevo)}."));

        public Task HorariosEliminadosAsync(string tutorId, params TutorHorario[] horarios) =>
            horarios.Length == 0
                ? Task.CompletedTask
                : EnviarSeguroAsync(async () =>
                    (await AlumnosAtendidosAsync(tutorId), "Horario de tutoría eliminado",
                        $"{await NombreAsync(tutorId)} ya no atiende en: {string.Join("; ", horarios.Select(Describir))}."));

        private static string Describir(TutorHorario h)
        {
            var dia = h.DiaSemana == DiaSemana.Miercoles ? "Miércoles" : h.DiaSemana.ToString();
            return $"{dia} de {h.HoraInicio:hh\\:mm} a {h.HoraFin:hh\\:mm} en {h.Lugar}";
        }

        private async Task<string> NombreAsync(string usuarioId) =>
            (await _context.Users.FirstOrDefaultAsync(u => u.Id == usuarioId))?.NombreCompleto ?? "Un usuario";

        private async Task<IEnumerable<string>> CoordinadoresAsync() =>
            (await _userManager.GetUsersInRoleAsync("Coordinador")).Select(u => u.Id);

        private async Task<(string TutorId, string Materia)> SesionAsync(int sesionId)
        {
            var sesion = await _context.SesionesTutoria
                .Where(s => s.Id == sesionId)
                .Select(s => new { s.TutorId, Materia = s.Materia!.Nombre })
                .FirstAsync();
            return (sesion.TutorId ?? string.Empty, sesion.Materia);
        }

        // Alumnos con al menos una asistencia confirmada con este tutor
        private async Task<IEnumerable<string>> AlumnosAtendidosAsync(string tutorId) =>
            await _context.Asistencias
                .Where(a => a.Confirmada && a.Sesion!.TutorId == tutorId && a.AlumnoId != tutorId)
                .Select(a => a.AlumnoId)
                .Distinct()
                .ToListAsync();

        private async Task EnviarSeguroAsync(Func<Task<Aviso>> preparar)
        {
            var nuevas = new List<Notificacion>();
            try
            {
                var (destinatarios, titulo, mensaje) = await preparar();
                var ids = destinatarios.Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().ToList();
                var activos = await _context.Users
                    .Where(u => ids.Contains(u.Id) && u.Activo)
                    .Select(u => u.Id)
                    .ToListAsync();

                var ahora = TimeHelper.UtcNow;
                // Sin emisor: la campana las muestra como enviadas por el sistema
                nuevas.AddRange(activos.Select(id => new Notificacion
                {
                    UserId = id,
                    Titulo = titulo,
                    Mensaje = mensaje.Length > 1000 ? mensaje[..1000] : mensaje,
                    FechaEnvio = ahora
                }));

                _context.Notificaciones.AddRange(nuevas);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Se descartan para que no se reintenten en un guardado posterior
                foreach (var n in nuevas) _context.Entry(n).State = EntityState.Detached;
                _logger.LogError(ex, "No se pudo generar una notificación automática.");
            }
        }
    }
}

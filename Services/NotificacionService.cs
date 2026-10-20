using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tutorias.Data;
using tutorias.Helpers;
using tutorias.Models;
using tutorias.Models.ViewModels;

namespace tutorias.Services
{
    public class NotificacionService : INotificacionService
    {
        private readonly ApplicationDbContext _context;

        public NotificacionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CampanaNotificacionesViewModel> ObtenerNotificacionesCampanaAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new CampanaNotificacionesViewModel();
            }

            var totalNoLeidas = await _context.Notificaciones
                .CountAsync(n => n.UserId == userId && !n.Leida);

            var notificaciones = await _context.Notificaciones
                .Include(n => n.Emisor)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.FechaEnvio)
                .Take(10)
                .ToListAsync();

            var ahoraMexico = TimeHelper.NowMexico;

            var items = notificaciones.Select(n =>
            {
                var fechaLocal = TimeHelper.ConvertToMexico(n.FechaEnvio);
                var diff = ahoraMexico - fechaLocal;

                string tiempoRelativo;
                if (diff.TotalMinutes < 1)
                {
                    tiempoRelativo = "Hace un momento";
                }
                else if (diff.TotalMinutes < 60)
                {
                    tiempoRelativo = $"Hace {(int)diff.TotalMinutes} min";
                }
                else if (diff.TotalHours < 24)
                {
                    tiempoRelativo = $"Hace {(int)diff.TotalHours} h";
                }
                else
                {
                    tiempoRelativo = fechaLocal.ToString("dd/MM/yyyy HH:mm");
                }

                string emisorRol = "Coordinacion";
                if (n.Emisor != null)
                {
                    if (n.Emisor.TipoTutor == TipoTutor.DocenteTutor)
                    {
                        emisorRol = "Docente Tutor";
                    }
                    else if (n.Emisor.TipoTutor == TipoTutor.AlumnoTutor)
                    {
                        emisorRol = "Alumno Tutor";
                    }
                }

                return new NotificacionItemViewModel
                {
                    Id = n.Id,
                    Titulo = n.Titulo,
                    Mensaje = n.Mensaje,
                    FechaEnvio = fechaLocal,
                    TiempoRelativo = tiempoRelativo,
                    Leida = n.Leida,
                    EmisorNombre = n.Emisor?.NombreCompleto ?? "Coordinación Institucional",
                    EmisorRol = emisorRol
                };
            }).ToList();

            return new CampanaNotificacionesViewModel
            {
                TotalNoLeidas = totalNoLeidas,
                Recientes = items
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> MarcarComoLeidaAsync(int notificacionId, string userId)
        {
            var notif = await _context.Notificaciones
                .FirstOrDefaultAsync(n => n.Id == notificacionId && n.UserId == userId);

            if (notif == null)
            {
                return (false, "Notificación no encontrada.");
            }

            if (!notif.Leida)
            {
                notif.Leida = true;
                notif.FechaLectura = TimeHelper.UtcNow;
                await _context.SaveChangesAsync();
            }

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> MarcarTodasComoLeidasAsync(string userId)
        {
            var noLeidas = await _context.Notificaciones
                .Where(n => n.UserId == userId && !n.Leida)
                .ToListAsync();

            if (noLeidas.Any())
            {
                var ahoraUtc = TimeHelper.UtcNow;
                foreach (var n in noLeidas)
                {
                    n.Leida = true;
                    n.FechaLectura = ahoraUtc;
                }
                await _context.SaveChangesAsync();
            }

            return (true, null);
        }

        public async Task<CrearNotificacionViewModel> PrepararFormularioCrearAsync(
            string emisorId,
            bool esCoordinador,
            string? filtroRol,
            int? filtroCarreraId)
        {
            var vm = new CrearNotificacionViewModel
            {
                EsCoordinador = esCoordinador,
                FiltroRol = filtroRol,
                FiltroCarreraId = filtroCarreraId
            };

            if (!esCoordinador)
            {
                // Regla estricta para tutores: únicamente alumnos que tengan asistencias en sesiones impartidas por este tutor
                var alumnosIds = await _context.Asistencias
                    .Where(a => a.Sesion != null && a.Sesion.TutorId == emisorId)
                    .Select(a => a.AlumnoId)
                    .Distinct()
                    .ToListAsync();

                vm.DestinatariosDisponibles = await _context.Users
                    .Include(u => u.Carrera)
                    .Where(u => alumnosIds.Contains(u.Id) && u.Activo)
                    .OrderBy(u => u.Nombre)
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id,
                        Text = $"{u.Nombre} {u.ApellidoPaterno} {u.ApellidoMaterno}".Trim() + $" ({u.Boleta}) - " + (u.Carrera != null ? u.Carrera.Nombre : (u.Academia ?? "UPIICSA"))
                    })
                    .ToListAsync();
            }
            else
            {
                // Coordinación puede seleccionar cualquier usuario con filtros opcionales
                var query = _context.Users
                    .Include(u => u.Carrera)
                    .Where(u => u.Activo && u.Id != emisorId);

                if (filtroRol == "Alumnos")
                {
                    query = query.Where(u => u.TipoTutor == TipoTutor.Ninguno);
                }
                else if (filtroRol == "Tutores")
                {
                    query = query.Where(u => u.TipoTutor == TipoTutor.DocenteTutor || u.TipoTutor == TipoTutor.AlumnoTutor);
                }
                else if (filtroRol == "Docentes")
                {
                    query = query.Where(u => u.TipoTutor == TipoTutor.DocenteTutor);
                }

                if (filtroCarreraId.HasValue && filtroCarreraId.Value > 0)
                {
                    query = query.Where(u => u.CarreraId == filtroCarreraId.Value);
                }

                vm.DestinatariosDisponibles = await query
                    .OrderBy(u => u.Nombre)
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id,
                        Text = $"{u.Nombre} {u.ApellidoPaterno} {u.ApellidoMaterno}".Trim() + $" ({u.Boleta}) - " + (u.Carrera != null ? u.Carrera.Nombre : (u.Academia ?? "UPIICSA"))
                    })
                    .ToListAsync();

                vm.CarrerasDisponibles = await _context.Carreras
                    .OrderBy(c => c.Nombre)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Nombre
                    })
                    .ToListAsync();

                vm.CarrerasDisponibles.Insert(0, new SelectListItem { Value = "", Text = "-- Todas las carreras --" });
            }

            return vm;
        }

        public async Task<(bool Success, string? ErrorMessage)> EnviarNotificacionAsync(
            string emisorId,
            bool esCoordinador,
            CrearNotificacionViewModel model)
        {
            if (model.DestinatariosIds == null || !model.DestinatariosIds.Any())
            {
                return (false, "Debes seleccionar al menos un destinatario para enviar la notificación.");
            }

            if (string.IsNullOrWhiteSpace(model.Titulo))
            {
                return (false, "El título de la notificación no puede estar vacío.");
            }

            if (string.IsNullOrWhiteSpace(model.Mensaje))
            {
                return (false, "El mensaje de la notificación no puede estar vacío.");
            }

            var idsLimpios = model.DestinatariosIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct()
                .ToList();

            if (!idsLimpios.Any())
            {
                return (false, "No se seleccionaron destinatarios válidos.");
            }

            if (!esCoordinador)
            {
                // Validación estricta en servidor: asegurar que todos los destinatarios son alumnos asistentes de este tutor
                var permitidos = await _context.Asistencias
                    .Where(a => a.Sesion != null && a.Sesion.TutorId == emisorId)
                    .Select(a => a.AlumnoId)
                    .Distinct()
                    .ToListAsync();

                var invalidos = idsLimpios.Except(permitidos).ToList();
                if (invalidos.Any())
                {
                    return (false, "No tienes autorización para enviar notificaciones a alumnos que no han asistido a tus tutorías.");
                }
            }

            var ahoraUtc = TimeHelper.UtcNow;
            foreach (var destId in idsLimpios)
            {
                var notif = new Notificacion
                {
                    UserId = destId,
                    EmisorId = emisorId,
                    Titulo = model.Titulo.Trim(),
                    Mensaje = model.Mensaje.Trim(),
                    FechaEnvio = ahoraUtc,
                    Leida = false
                };
                _context.Notificaciones.Add(notif);
            }

            await _context.SaveChangesAsync();
            return (true, null);
        }
    }
}

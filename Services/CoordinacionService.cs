using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using tutorias.Data;
using tutorias.Models;
using tutorias.Models.ViewModels;

namespace tutorias.Services
{
    public class CoordinacionService : ICoordinacionService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CoordinacionService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<GestionUsuariosViewModel> GetGestionUsuariosAsync(string currentUserId, string? busqueda = null, string? rolFiltro = null, int pagina = 1, int tamanoPagina = 10)
        {
            if (pagina < 1) pagina = 1;
            if (tamanoPagina < 1) tamanoPagina = 10;

            var query = _context.Users
                .Include(u => u.Carrera)
                .Include(u => u.PerfilTutor)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var term = busqueda.Trim().ToLower();
                query = query.Where(u =>
                    u.Nombre.ToLower().Contains(term) ||
                    u.ApellidoPaterno.ToLower().Contains(term) ||
                    u.ApellidoMaterno.ToLower().Contains(term) ||
                    (u.Email != null && u.Email.ToLower().Contains(term)) ||
                    u.Boleta.ToLower().Contains(term));
            }

            var usuariosDb = await query
                .OrderBy(u => u.ApellidoPaterno)
                .ThenBy(u => u.ApellidoMaterno)
                .ThenBy(u => u.Nombre)
                .ToListAsync();

            var userItems = new List<UsuarioCoordinacionItemViewModel>();

            foreach (var user in usuariosDb)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var esCoord = roles.Contains("Coordinador");

                userItems.Add(new UsuarioCoordinacionItemViewModel
                {
                    Id = user.Id,
                    Nombre = user.Nombre,
                    ApellidoPaterno = user.ApellidoPaterno,
                    ApellidoMaterno = user.ApellidoMaterno,
                    NombreCompleto = user.NombreCompleto,
                    Email = user.Email ?? string.Empty,
                    Boleta = user.Boleta,
                    CarreraNombre = user.Carrera?.Nombre,
                    Academia = user.Academia,
                    Semestre = user.Semestre,
                    FotoUrl = user.FotoUrl,
                    TipoTutor = user.TipoTutor,
                    Roles = roles,
                    EsCoordinador = esCoord,
                    EsUsuarioActual = user.Id == currentUserId,
                    EmailConfirmed = user.EmailConfirmed,
                    Activo = user.Activo,
                    FechaBaja = user.FechaBaja,
                    MotivoBaja = user.MotivoBaja
                });
            }

            var totalCoordinadores = (await _userManager.GetUsersInRoleAsync("Coordinador")).Count;
            var totalUsuarios = await _context.Users.CountAsync();

            if (!string.IsNullOrWhiteSpace(rolFiltro) && rolFiltro != "Todos")
            {
                userItems = userItems.Where(u => u.Roles.Contains(rolFiltro, StringComparer.OrdinalIgnoreCase)).ToList();
            }

            var totalFiltrados = userItems.Count;
            var itemsPaginados = userItems
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToList();

            return new GestionUsuariosViewModel
            {
                Busqueda = busqueda,
                RolFiltro = rolFiltro,
                TotalCoordinadores = totalCoordinadores,
                TotalUsuarios = totalUsuarios,
                PaginaActual = pagina,
                TamanoPagina = tamanoPagina,
                TotalRegistros = totalFiltrados,
                Usuarios = itemsPaginados
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> PromoverACoordinadorAsync(string targetUserId, string currentUserId)
        {
            var targetUser = await _userManager.FindByIdAsync(targetUserId);
            if (targetUser == null)
            {
                return (false, "El usuario seleccionado no existe.");
            }

            if (!targetUser.Activo)
            {
                return (false, "No se puede promover a un usuario que se encuentra dado de baja.");
            }

            if (!targetUser.EmailConfirmed)
            {
                return (false, "El usuario debe verificar su correo institucional antes de poder recibir permisos de Coordinador.");
            }

            if (await _userManager.IsInRoleAsync(targetUser, "Coordinador"))
            {
                return (false, "El usuario ya cuenta con el rol de Coordinador.");
            }

            var result = await _userManager.AddToRoleAsync(targetUser, "Coordinador");
            if (!result.Succeeded)
            {
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> RevocarCoordinadorAsync(string targetUserId, string currentUserId)
        {
            if (string.Equals(targetUserId, currentUserId, StringComparison.OrdinalIgnoreCase))
            {
                return (false, "Por seguridad, no puedes revocar tus propios privilegios de Coordinador.");
            }

            var targetUser = await _userManager.FindByIdAsync(targetUserId);
            if (targetUser == null)
            {
                return (false, "El usuario seleccionado no existe.");
            }

            if (!await _userManager.IsInRoleAsync(targetUser, "Coordinador"))
            {
                return (false, "El usuario no cuenta con el rol de Coordinador.");
            }

            var coordinadoresActuales = await _userManager.GetUsersInRoleAsync("Coordinador");
            if (coordinadoresActuales.Count <= 1)
            {
                return (false, "No se puede revocar el rol porque debe permanecer al menos un Coordinador activo en el sistema.");
            }

            var result = await _userManager.RemoveFromRoleAsync(targetUser, "Coordinador");
            if (!result.Succeeded)
            {
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return (true, null);
        }

        public async Task<BandejaSolicitudesViewModel> GetBandejaSolicitudesAsync(string? busqueda = null, string? rolFiltro = null, EstadoSolicitud? estadoFiltro = null, int pagina = 1, int tamanoPagina = 10)
        {
            if (pagina < 1) pagina = 1;
            if (tamanoPagina < 1) tamanoPagina = 10;

            var totalPendientes = await _context.SolicitudesTutor.CountAsync(s => s.Estado == EstadoSolicitud.Pendiente);
            var totalAprobadas = await _context.SolicitudesTutor.CountAsync(s => s.Estado == EstadoSolicitud.Aprobada);
            var totalRechazadas = await _context.SolicitudesTutor.CountAsync(s => s.Estado == EstadoSolicitud.Rechazada);

            var query = _context.SolicitudesTutor
                .Include(s => s.User)
                    .ThenInclude(u => u!.Carrera)
                .AsQueryable();

            if (estadoFiltro.HasValue)
            {
                query = query.Where(s => s.Estado == estadoFiltro.Value);
            }

            if (!string.IsNullOrWhiteSpace(rolFiltro))
            {
                if (rolFiltro.Equals("Alumno", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(s => s.TipoTutorSolicitado == TipoTutor.AlumnoTutor);
                }
                else if (rolFiltro.Equals("Docente", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(s => s.TipoTutorSolicitado == TipoTutor.DocenteTutor);
                }
            }

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var term = busqueda.Trim().ToLower();
                query = query.Where(s => s.User != null && (
                    s.User.Nombre.ToLower().Contains(term) ||
                    s.User.ApellidoPaterno.ToLower().Contains(term) ||
                    s.User.ApellidoMaterno.ToLower().Contains(term) ||
                    (s.User.Email != null && s.User.Email.ToLower().Contains(term)) ||
                    s.User.Boleta.ToLower().Contains(term)));
            }

            var totalRegistros = await query.CountAsync();

            var solicitudesDb = await query
                .OrderByDescending(s => s.FechaSolicitud)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            var items = new List<SolicitudTutorItemViewModel>();
            foreach (var s in solicitudesDb)
            {
                var u = s.User;
                var esDocente = s.TipoTutorSolicitado == TipoTutor.DocenteTutor;
                var programaOAcademia = esDocente
                    ? (!string.IsNullOrWhiteSpace(u?.Academia) ? u.Academia : "Sin academia asignada")
                    : (u?.Carrera?.Nombre ?? "Sin carrera asignada");

                var esTutorActivo = u != null && (await _userManager.IsInRoleAsync(u, "Tutor") || u.TipoTutor != TipoTutor.Ninguno);

                items.Add(new SolicitudTutorItemViewModel
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    NombreCompleto = u != null ? u.NombreCompleto : "Usuario Desconocido",
                    Email = u?.Email ?? string.Empty,
                    Boleta = u?.Boleta ?? string.Empty,
                    ProgramaOAcademia = programaOAcademia,
                    EsDocente = esDocente,
                    TipoTutorSolicitado = s.TipoTutorSolicitado,
                    FechaSolicitud = s.FechaSolicitud,
                    Estado = s.Estado,
                    RespuestaCoordinacion = s.RespuestaCoordinacion,
                    FechaRespuesta = s.FechaRespuesta,
                    EsTutorActivo = esTutorActivo
                });
            }

            return new BandejaSolicitudesViewModel
            {
                Busqueda = busqueda,
                RolFiltro = rolFiltro,
                EstadoFiltro = estadoFiltro,
                TotalPendientes = totalPendientes,
                TotalAprobadas = totalAprobadas,
                TotalRechazadas = totalRechazadas,
                PaginaActual = pagina,
                TamanoPagina = tamanoPagina,
                TotalRegistros = totalRegistros,
                Solicitudes = items
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> AprobarSolicitudTutorAsync(int solicitudId, string currentUserId)
        {
            var solicitud = await _context.SolicitudesTutor
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == solicitudId);

            if (solicitud == null)
            {
                return (false, "La solicitud no fue encontrada.");
            }

            if (solicitud.Estado != EstadoSolicitud.Pendiente)
            {
                return (false, "Solo se pueden aprobar solicitudes que se encuentren en estado Pendiente.");
            }

            var user = solicitud.User;
            if (user == null)
            {
                return (false, "El usuario asociado a la solicitud no existe.");
            }

            if (!await _userManager.IsInRoleAsync(user, "Tutor"))
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "Tutor");
                if (!roleResult.Succeeded)
                {
                    return (false, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }

            user.TipoTutor = solicitud.TipoTutorSolicitado;

            var perfilExistente = await _context.TutoresPerfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (perfilExistente == null)
            {
                var nuevoPerfil = new TutorPerfil
                {
                    UserId = user.Id,
                    Biografia = null,
                    SalonAsignado = null,
                    EsPublico = false,
                    ReputacionPromedio = 0.0
                };
                _context.TutoresPerfiles.Add(nuevoPerfil);
            }

            solicitud.Estado = EstadoSolicitud.Aprobada;
            solicitud.FechaRespuesta = DateTime.UtcNow;
            solicitud.RespuestaCoordinacion = "Solicitud aprobada por la Coordinación de Tutorías.";

            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> RechazarSolicitudTutorAsync(int solicitudId, string currentUserId)
        {
            var solicitud = await _context.SolicitudesTutor
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == solicitudId);

            if (solicitud == null)
            {
                return (false, "La solicitud no fue encontrada.");
            }

            if (solicitud.Estado != EstadoSolicitud.Pendiente)
            {
                return (false, "Solo se pueden rechazar solicitudes que se encuentren en estado Pendiente.");
            }

            solicitud.Estado = EstadoSolicitud.Rechazada;
            solicitud.FechaRespuesta = DateTime.UtcNow;
            solicitud.RespuestaCoordinacion = "Pasa a Coordinación para validar tu situación";

            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> SuspenderTutorAsync(string tutorUserId, string currentUserId)
        {
            var user = await _context.Users
                .Include(u => u.PerfilTutor)
                .FirstOrDefaultAsync(u => u.Id == tutorUserId);

            if (user == null)
            {
                return (false, "El tutor seleccionado no existe.");
            }

            if (await _userManager.IsInRoleAsync(user, "Tutor"))
            {
                var roleResult = await _userManager.RemoveFromRoleAsync(user, "Tutor");
                if (!roleResult.Succeeded)
                {
                    return (false, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }

            user.TipoTutor = TipoTutor.Ninguno;

            if (user.PerfilTutor != null)
            {
                user.PerfilTutor.EsPublico = false;
            }

            await _context.SaveChangesAsync();
            return (true, null);
        }
    }
}

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
    }
}

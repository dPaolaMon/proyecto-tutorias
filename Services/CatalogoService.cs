using Microsoft.EntityFrameworkCore;
using tutorias.Data;
using tutorias.Models;
using tutorias.Models.ViewModels;

namespace tutorias.Services
{
    public class CatalogoService : ICatalogoService
    {
        private readonly ApplicationDbContext _context;

        public CatalogoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CatalogoFiltrosViewModel> GetCatalogoAsync(
            string? busqueda = null,
            int? materiaId = null,
            int? carreraId = null,
            string? modalidad = null,
            int pagina = 1,
            int tamanoPagina = 12)
        {
            if (pagina < 1) pagina = 1;
            if (tamanoPagina < 1) tamanoPagina = 12;

            // 1. Obtener carreras y materias para los filtros
            var carreras = await _context.Carreras
                .OrderBy(c => c.Nombre)
                .Select(c => new CarreraFiltroItemViewModel
                {
                    Id = c.Id,
                    Clave = c.Clave,
                    Nombre = c.Nombre
                })
                .ToListAsync();

            var materias = await _context.Materias
                .OrderBy(m => m.Nombre)
                .Select(m => new MateriaItemViewModel
                {
                    Id = m.Id,
                    Clave = m.Clave,
                    Nombre = m.Nombre,
                    Semestre = m.Semestre,
                    CarreraNombre = m.Carrera != null ? m.Carrera.Nombre : string.Empty
                })
                .ToListAsync();

            var tutoresList = new List<TutorCardViewModel>();

            var queryTutores = _context.TutoresPerfiles
                .Include(p => p.User)
                    .ThenInclude(u => u!.Carrera)
                .Include(p => p.MateriasImpartidas)
                    .ThenInclude(tm => tm.Materia)
                .Include(p => p.Horarios)
                .Where(p => p.EsPublico && p.User != null && p.User.Activo)
                .AsQueryable();

            // Filtro por modalidad
            if (!string.IsNullOrWhiteSpace(modalidad))
            {
                if (modalidad.Equals("Pares", StringComparison.OrdinalIgnoreCase))
                {
                    queryTutores = queryTutores.Where(p => p.User!.TipoTutor == TipoTutor.AlumnoTutor);
                }
                else if (modalidad.Equals("Docente", StringComparison.OrdinalIgnoreCase))
                {
                    queryTutores = queryTutores.Where(p => p.User!.TipoTutor == TipoTutor.DocenteTutor);
                }
            }

            // Filtro por carrera
            if (carreraId.HasValue && carreraId.Value > 0)
            {
                queryTutores = queryTutores.Where(p =>
                    (p.User!.CarreraId == carreraId.Value) ||
                    p.MateriasImpartidas.Any(tm => tm.Materia != null && tm.Materia.CarreraId == carreraId.Value));
            }

            // Filtro por materia específica
            if (materiaId.HasValue && materiaId.Value > 0)
            {
                queryTutores = queryTutores.Where(p =>
                    p.MateriasImpartidas.Any(tm => tm.MateriaId == materiaId.Value));
            }

            // Filtro por búsqueda de texto (nombre, materia o salón)
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var term = busqueda.Trim().ToLower();
                queryTutores = queryTutores.Where(p =>
                    p.User!.Nombre.ToLower().Contains(term) ||
                    p.User.ApellidoPaterno.ToLower().Contains(term) ||
                    p.User.ApellidoMaterno.ToLower().Contains(term) ||
                    p.MateriasImpartidas.Any(tm => tm.Materia != null && (
                        tm.Materia.Nombre.ToLower().Contains(term) ||
                        tm.Materia.Clave.ToLower().Contains(term))) ||
                    p.Horarios.Any(h => h.Activo && h.Lugar.ToLower().Contains(term)));
            }

            var tutoresDb = await queryTutores
                .OrderByDescending(p => p.ReputacionPromedio)
                .ThenBy(p => p.User!.Nombre)
                .ToListAsync();

            foreach (var p in tutoresDb)
            {
                var u = p.User!;
                var esDocente = u.TipoTutor == TipoTutor.DocenteTutor;
                var carreraOAcademia = esDocente
                    ? (!string.IsNullOrWhiteSpace(u.Academia) ? u.Academia : "Academia UPIICSA")
                    : (u.Carrera?.Nombre ?? "UPIICSA");

                var modTitulo = esDocente
                    ? "Tutoría Individual (Docente)"
                    : "Tutoría entre Pares (Alumno)";

                tutoresList.Add(new TutorCardViewModel
                {
                    PerfilId = p.Id,
                    UserId = u.Id,
                    NombreCompleto = u.NombreCompleto,
                    FotoUrl = u.FotoUrl,
                    CarreraOAcademia = carreraOAcademia,
                    ModalidadTitulo = modTitulo,
                    TipoTutor = u.TipoTutor,
                    ReputacionPromedio = p.ReputacionPromedio,
                    MateriasNombres = p.MateriasImpartidas
                        .Where(tm => tm.Materia != null)
                        .Select(tm => tm.Materia!.Nombre)
                        .ToList(),
                    TotalHorarios = p.Horarios.Count(h => h.Activo)
                });
            }

            int totalResultados = tutoresList.Count;

            // Paginación sobre tutores
            var tutoresPaginados = tutoresList
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToList();

            return new CatalogoFiltrosViewModel
            {
                Busqueda = busqueda,
                MateriaId = materiaId,
                CarreraId = carreraId,
                Modalidad = modalidad,
                CarrerasDisponibles = carreras,
                MateriasDisponibles = materias,
                Tutores = tutoresPaginados,
                TotalResultados = totalResultados,
                PaginaActual = pagina,
                TamanoPagina = tamanoPagina
            };
        }

        public async Task<TutorDetalleViewModel?> GetTutorDetalleAsync(int perfilId)
        {
            var perfil = await _context.TutoresPerfiles
                .Include(p => p.User)
                    .ThenInclude(u => u!.Carrera)
                .Include(p => p.MateriasImpartidas)
                    .ThenInclude(tm => tm.Materia)
                        .ThenInclude(m => m!.Carrera)
                .Include(p => p.Horarios)
                    .ThenInclude(h => h.Materia)
                .FirstOrDefaultAsync(p => p.Id == perfilId && p.EsPublico && p.User != null && p.User.Activo);

            if (perfil == null) return null;

            var u = perfil.User!;
            var esDocente = u.TipoTutor == TipoTutor.DocenteTutor;
            var carreraOAcademia = esDocente
                ? (!string.IsNullOrWhiteSpace(u.Academia) ? u.Academia : "Academia UPIICSA")
                : (u.Carrera?.Nombre ?? "UPIICSA");

            var modTitulo = esDocente
                ? "Tutoría Individual (Docente)"
                : "Tutoría entre Pares (Alumno)";

            var materias = perfil.MateriasImpartidas
                .Where(tm => tm.Materia != null)
                .OrderBy(tm => tm.Materia!.Semestre)
                .ThenBy(tm => tm.Materia!.Nombre)
                .Select(tm => new MateriaItemViewModel
                {
                    Id = tm.Materia!.Id,
                    Clave = tm.Materia!.Clave,
                    Nombre = tm.Materia!.Nombre,
                    Semestre = tm.Materia!.Semestre,
                    CarreraNombre = tm.Materia!.Carrera != null ? tm.Materia!.Carrera.Nombre : string.Empty
                })
                .ToList();

            var horarios = perfil.Horarios
                .Where(h => h.Activo)
                .OrderBy(h => h.DiaSemana)
                .ThenBy(h => h.HoraInicio)
                .Select(h => new TutorHorarioItemViewModel
                {
                    Id = h.Id,
                    DiaSemana = h.DiaSemana,
                    HoraInicio = h.HoraInicio,
                    HoraFin = h.HoraFin,
                    Lugar = h.Lugar,
                    MateriaId = h.MateriaId,
                    MateriaNombre = h.Materia != null ? h.Materia.Nombre : "Tutoría General",
                    Activo = h.Activo
                })
                .ToList();

            return new TutorDetalleViewModel
            {
                PerfilId = perfil.Id,
                UserId = u.Id,
                NombreCompleto = u.NombreCompleto,
                FotoUrl = u.FotoUrl,
                CarreraOAcademia = carreraOAcademia,
                ModalidadTitulo = modTitulo,
                TipoTutor = u.TipoTutor,
                Biografia = perfil.Biografia,
                ReputacionPromedio = perfil.ReputacionPromedio,
                Materias = materias,
                Horarios = horarios
            };
        }
    }
}

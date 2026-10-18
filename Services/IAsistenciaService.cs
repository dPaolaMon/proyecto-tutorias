using System.Collections.Generic;
using System.Threading.Tasks;
using tutorias.Models.ViewModels;

namespace tutorias.Services
{
    public interface IAsistenciaService
    {
        Task<List<BuscarAlumnoResultViewModel>> BuscarAlumnosAsync(string termino);
        Task<PanelTutorAsistenciaViewModel> GetPanelTutorAsync(string tutorId);
        Task<(bool Success, string? ErrorMessage)> RegistrarAsistenciaAutomaticaAsync(string tutorId, string alumnoId, int? materiaId);
        Task<MisAsistenciasViewModel> GetMisAsistenciasAsync(string alumnoId);
        Task<(bool Success, string? ErrorMessage)> ConfirmarAsistenciaAsync(int asistenciaId, string alumnoId);
        Task<(bool Success, string? ErrorMessage)> RechazarAsistenciaAsync(int asistenciaId, string alumnoId);
    }
}

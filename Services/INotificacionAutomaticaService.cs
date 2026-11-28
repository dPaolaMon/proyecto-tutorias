using System.Threading.Tasks;
using tutorias.Models;

namespace tutorias.Services
{
    // Avisos que el sistema genera solo. Se llaman después de guardar la acción principal
    // y nunca propagan errores: un aviso fallido no debe deshacer ni romper la operación.
    public interface INotificacionAutomaticaService
    {
        Task SolicitudNuevaAsync(string solicitanteId);
        Task SolicitudResueltaAsync(string solicitanteId, bool aprobada);
        Task TutorRevocadoAsync(string tutorId);
        Task CoordinadorCambiadoAsync(string usuarioId, bool asignado);
        Task AsistenciaPorConfirmarAsync(int asistenciaId);
        Task AsistenciaRespondidaAsync(int sesionId, string alumnoId, bool confirmada);
        Task InvitacionACalificarAsync(int sesionId, string alumnoId);
        Task CalificacionBajaAsync(int sesionId, int calificacion);
        Task HorarioModificadoAsync(string tutorId, TutorHorario anterior, TutorHorario nuevo);
        Task HorariosEliminadosAsync(string tutorId, params TutorHorario[] horarios);
    }
}

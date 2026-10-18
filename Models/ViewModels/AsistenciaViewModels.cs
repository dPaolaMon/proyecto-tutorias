using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using tutorias.Models;

namespace tutorias.Models.ViewModels
{
    public class BuscarAlumnoResultViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Boleta { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CarreraNombre { get; set; } = string.Empty;
        public string? FotoUrl { get; set; }
    }

    public class RegistrarAsistenciaInputModel
    {
        [Required(ErrorMessage = "Debe seleccionar un alumno para registrar asistencia.")]
        public string AlumnoId { get; set; } = string.Empty;

        public int? MateriaId { get; set; }
    }

    public class HorarioEnCursoViewModel
    {
        public int HorarioId { get; set; }
        public DiaSemana DiaSemana { get; set; }
        public string DiaSemanaNombre { get; set; } = string.Empty;
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string HorarioTexto => $"{HoraInicio:hh\\:mm} a {HoraFin:hh\\:mm} hrs";
        public string Lugar { get; set; } = string.Empty;
        public int? MateriaId { get; set; }
        public string? MateriaNombre { get; set; }
        public bool RequiereSeleccionMateria => !MateriaId.HasValue;
    }

    public class PanelTutorAsistenciaViewModel
    {
        public bool HayHorarioEnCurso { get; set; }
        public HorarioEnCursoViewModel? HorarioEnCurso { get; set; }
        public SesionTutoriaItemViewModel? SesionActual { get; set; }
        public List<AsistenciaItemViewModel> AsistenciasDeHoy { get; set; } = new();
        public List<MateriaItemViewModel> MateriasImpartidas { get; set; } = new();
        public List<TutorHorarioItemViewModel> HorariosRegistrados { get; set; } = new();
        public string DiaActualNombre { get; set; } = string.Empty;
        public string HoraActualTexto { get; set; } = string.Empty;
    }

    public class AsistenciaItemViewModel
    {
        public int Id { get; set; }
        public int SesionId { get; set; }
        public string TituloSesion { get; set; } = string.Empty;
        public string MateriaNombre { get; set; } = string.Empty;
        public string TutorNombre { get; set; } = string.Empty;
        public string Lugar { get; set; } = string.Empty;
        public string AlumnoId { get; set; } = string.Empty;
        public string AlumnoNombre { get; set; } = string.Empty;
        public string AlumnoBoleta { get; set; } = string.Empty;
        public string AlumnoCarrera { get; set; } = string.Empty;
        public DateTime FechaHoraRegistro { get; set; }
        public bool Confirmada { get; set; }
        public DateTime? FechaHoraConfirmacion { get; set; }
        public MetodoAsistencia Metodo { get; set; }
        public int? CalificacionEncuesta { get; set; }
        public string? ComentarioEncuesta { get; set; }

        public string FechaHoraRegistroTexto => FechaHoraRegistro.ToString("dd/MM/yyyy HH:mm");
        public string HoraRegistroTexto => FechaHoraRegistro.ToString("HH:mm");
        public string? FechaHoraConfirmacionTexto => FechaHoraConfirmacion?.ToString("dd/MM/yyyy HH:mm");
        public string? HoraConfirmacionTexto => FechaHoraConfirmacion?.ToString("HH:mm");
    }

    public class SesionTutoriaItemViewModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public int MateriaId { get; set; }
        public string MateriaNombre { get; set; } = string.Empty;
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }
        public string Lugar { get; set; } = string.Empty;
        public bool Activa { get; set; }
        public int CapacidadMaxima { get; set; }
        public int TotalAsistencias { get; set; }
        public int TotalConfirmadas { get; set; }
        public int TotalPendientes => Math.Max(0, TotalAsistencias - TotalConfirmadas);
        public string HorarioFormato => $"{FechaHoraInicio:dd/MM/yyyy HH:mm} - {FechaHoraFin:HH:mm} hrs";
    }

    public class AsistenciaPendienteAlumnoViewModel
    {
        public int AsistenciaId { get; set; }
        public int SesionId { get; set; }
        public string TituloSesion { get; set; } = string.Empty;
        public string MateriaNombre { get; set; } = string.Empty;
        public string TutorNombre { get; set; } = string.Empty;
        public string Lugar { get; set; } = string.Empty;
        public DateTime FechaHoraRegistro { get; set; }
        public MetodoAsistencia Metodo { get; set; }

        public string FechaHoraRegistroTexto => FechaHoraRegistro.ToString("dd/MM/yyyy HH:mm");
        public string HoraRegistroTexto => FechaHoraRegistro.ToString("HH:mm");
    }

    public class MisAsistenciasViewModel
    {
        public List<AsistenciaPendienteAlumnoViewModel> Pendientes { get; set; } = new();
        public List<AsistenciaItemViewModel> Confirmadas { get; set; } = new();
    }
}

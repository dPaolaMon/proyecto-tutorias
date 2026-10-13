using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using tutorias.Models;

namespace tutorias.Models.ViewModels
{
    public class TutorPerfilViewModel
    {
        public int PerfilId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Boleta { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FotoUrl { get; set; }
        public bool EsDocente { get; set; }
        public string ProgramaOAcademia { get; set; } = string.Empty;
        public string ModalidadTitulo { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La biografía no puede exceder los 500 caracteres.")]
        public string? Biografia { get; set; }

        public bool EsPublico { get; set; }
        public double ReputacionPromedio { get; set; }

        // Materias
        public List<int> MateriasSeleccionadasIds { get; set; } = new();
        public List<MateriaItemViewModel> MateriasDisponibles { get; set; } = new();
        public List<MateriaItemViewModel> MateriasAsignadas { get; set; } = new();

        // Horarios
        public List<TutorHorarioItemViewModel> Horarios { get; set; } = new();
    }

    public class MateriaItemViewModel
    {
        public int Id { get; set; }
        public string Clave { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int? Semestre { get; set; }
        public string CarreraNombre { get; set; } = string.Empty;
    }

    public class TutorHorarioItemViewModel
    {
        public int Id { get; set; }
        public DiaSemana DiaSemana { get; set; }
        public string DiaSemanaNombre => DiaSemana switch
        {
            DiaSemana.Lunes => "Lunes",
            DiaSemana.Martes => "Martes",
            DiaSemana.Miercoles => "Miércoles",
            DiaSemana.Jueves => "Jueves",
            DiaSemana.Viernes => "Viernes",
            _ => "Desconocido"
        };
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string HorarioFormato => $"{HoraInicio:hh\\:mm} - {HoraFin:hh\\:mm}";
        public string Lugar { get; set; } = string.Empty;
        public int? MateriaId { get; set; }
        public string? MateriaNombre { get; set; }
        public bool Activo { get; set; }
    }

    public class GuardarTutorPerfilInputModel
    {
        [StringLength(500, ErrorMessage = "La biografía no puede exceder los 500 caracteres.")]
        public string? Biografia { get; set; }

        public bool EsPublico { get; set; }

        public List<int> MateriasSeleccionadasIds { get; set; } = new();
    }

    public class GuardarHorarioInputModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "El día de la semana es obligatorio.")]
        public DiaSemana DiaSemana { get; set; }

        [Required(ErrorMessage = "La hora de inicio es obligatoria.")]
        public TimeSpan HoraInicio { get; set; }

        [Required(ErrorMessage = "La hora de fin es obligatoria.")]
        public TimeSpan HoraFin { get; set; }

        [Required(ErrorMessage = "El salón o lugar es obligatorio.")]
        [StringLength(100, ErrorMessage = "El salón o lugar no puede exceder los 100 caracteres.")]
        public string Lugar { get; set; } = string.Empty;

        public int? MateriaId { get; set; }
    }
}

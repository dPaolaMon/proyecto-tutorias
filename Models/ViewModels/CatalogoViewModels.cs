using System;
using System.Collections.Generic;
using tutorias.Models;

namespace tutorias.Models.ViewModels
{
    public class CatalogoFiltrosViewModel
    {
        public string? Busqueda { get; set; }
        public int? MateriaId { get; set; }
        public int? CarreraId { get; set; }
        public string? Modalidad { get; set; } // "Pares", "Docente" o null

        public List<CarreraFiltroItemViewModel> CarrerasDisponibles { get; set; } = new();
        public List<MateriaItemViewModel> MateriasDisponibles { get; set; } = new();

        public List<TutorCardViewModel> Tutores { get; set; } = new();

        public int TotalResultados { get; set; }
        public int PaginaActual { get; set; } = 1;
        public int TamanoPagina { get; set; } = 12;
        public int TotalPaginas => (int)Math.Ceiling((double)TotalResultados / (TamanoPagina > 0 ? TamanoPagina : 12));
        public bool TienePaginaAnterior => PaginaActual > 1;
        public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;
    }

    public class CarreraFiltroItemViewModel
    {
        public int Id { get; set; }
        public string Clave { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
    }

    public class TutorCardViewModel
    {
        public int PerfilId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string? FotoUrl { get; set; }
        public string CarreraOAcademia { get; set; } = string.Empty;
        public string ModalidadTitulo { get; set; } = string.Empty;
        public TipoTutor TipoTutor { get; set; }
        public double ReputacionPromedio { get; set; }
        public List<string> MateriasNombres { get; set; } = new();
        public int TotalHorarios { get; set; }
    }

    public class TutorDetalleViewModel
    {
        public int PerfilId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string? FotoUrl { get; set; }
        public string CarreraOAcademia { get; set; } = string.Empty;
        public string ModalidadTitulo { get; set; } = string.Empty;
        public TipoTutor TipoTutor { get; set; }
        public string? Biografia { get; set; }
        public double ReputacionPromedio { get; set; }
        public List<MateriaItemViewModel> Materias { get; set; } = new();
        public List<TutorHorarioItemViewModel> Horarios { get; set; } = new();
    }
}

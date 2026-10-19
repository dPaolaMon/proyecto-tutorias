using System.Threading.Tasks;
using tutorias.Models.ViewModels;

namespace tutorias.Services
{
    public interface IEncuestasService
    {
        Task<PanelEncuestasViewModel> ObtenerPanelEncuestasAsync();
    }
}

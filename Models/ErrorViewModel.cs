namespace tutorias.Models;

public class ErrorViewModel
{
    public string Titulo { get; set; } = "Algo salió mal";

    public string Mensaje { get; set; } = "Ocurrió un error al procesar tu solicitud. Intenta de nuevo en unos minutos.";

    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}

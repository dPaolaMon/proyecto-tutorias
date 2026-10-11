namespace tutorias.Models
{
    public enum TipoTutor
    {
        Ninguno = 0,
        AlumnoTutor = 1,
        DocenteTutor = 2
    }

    public enum EstadoSolicitud
    {
        Pendiente = 0,
        Aprobada = 1,
        Rechazada = 2
    }

    public enum TipoTutoria
    {
        EntrePares = 1,
        IndividualDocente = 2,
        EspecialCoordinacion = 3
    }

    public enum MetodoAsistencia
    {
        CodigoQR = 1,
        ManualSignalR = 2
    }
}

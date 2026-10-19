namespace tutorias.Services
{
    public interface IQrCodeService
    {
        string GenerarQrPngBase64(string payload);
    }
}

using OpenQA.Selenium;

namespace tutorias.Tests.Selenium.PageObjects
{
    public class AsistenciaPage : BasePage
    {
        private static readonly By QrImage = By.CssSelector("img[alt*='QR'], img.qr-image, #qrImage, .qr-container img");
        private static readonly By Temporizador = By.CssSelector("#temporizador, #timer, .timer-badge");
        private static readonly By TokenInput = By.Name("token");
        private static readonly By ValidarButton = By.CssSelector("button[type='submit']");

        public AsistenciaPage(IWebDriver driver, string baseUrl) : base(driver, baseUrl) { }

        public AsistenciaPage IrAGenerarQr()
        {
            Driver.Navigate().GoToUrl($"{BaseUrl}/Asistencia/GenerarQr");
            return this;
        }

        public AsistenciaPage IrAEscanear()
        {
            Driver.Navigate().GoToUrl($"{BaseUrl}/Asistencia/Escanear");
            return this;
        }

        public bool EsQrVisible()
        {
            try
            {
                var img = FindElementWithWait(QrImage);
                return img != null && img.Displayed;
            }
            catch
            {
                return false;
            }
        }

        public void ValidarTokenManual(string token)
        {
            SendKeysWithWait(TokenInput, token);
            ClickWithWait(ValidarButton);
        }
    }
}

using OpenQA.Selenium;

namespace tutorias.Tests.Selenium.PageObjects
{
    public class CoordinacionPage : BasePage
    {
        private static readonly By TablaUsuarios = By.CssSelector("table");
        private static readonly By BusquedaInput = By.Name("busqueda");
        private static readonly By SubmitFilterButton = By.CssSelector("form button[type='submit']");
        private static readonly By BotonAprobar = By.CssSelector("button[title='Aprobar solicitud']");

        public CoordinacionPage(IWebDriver driver, string baseUrl) : base(driver, baseUrl) { }

        public CoordinacionPage IrAGestionUsuarios()
        {
            Driver.Navigate().GoToUrl($"{BaseUrl}/Coordinacion/Index");
            return this;
        }

        public CoordinacionPage IrASolicitudesTutores()
        {
            Driver.Navigate().GoToUrl($"{BaseUrl}/Coordinacion/SolicitudesTutores");
            return this;
        }

        public bool EsTablaVisible()
        {
            try
            {
                var tabla = FindElementWithWait(TablaUsuarios);
                return tabla != null && tabla.Displayed;
            }
            catch
            {
                return false;
            }
        }

        public void BuscarSolicitud(string texto)
        {
            SendKeysWithWait(BusquedaInput, texto);
            ClickWithWait(SubmitFilterButton);
        }

        public bool ExisteSolicitudPendienteParaAprobar()
        {
            try
            {
                return Driver.FindElements(BotonAprobar).Count > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}

using tutorias.Tests.Selenium.Drivers;
using tutorias.Tests.Selenium.PageObjects;
using Xunit;

namespace tutorias.Tests.Selenium.Specs
{
    public class SEL04_ControlAccesoRbacTests : IDisposable
    {
        private readonly OpenQA.Selenium.IWebDriver _driver;
        private readonly string _baseUrl;

        public SEL04_ControlAccesoRbacTests()
        {
            _driver = WebDriverFactory.CreateDriver();
            _baseUrl = WebDriverFactory.GetBaseUrl();
        }

        [Fact(DisplayName = "SEL-04: Usuario anonimo intentando acceder a Coordinacion debe ser redirigido a Login")]
        public void AccesoAnonimo_ACoordinacion_DebeRedirigirALogin()
        {
            var coordinacionPage = new CoordinacionPage(_driver, _baseUrl);
            coordinacionPage.IrAGestionUsuarios();

            // Debe interceptar y redirigir a Login
            string urlActual = coordinacionPage.GetCurrentUrl();
            Assert.Contains("Account/Login", urlActual);
        }

        public void Dispose()
        {
            _driver.Quit();
        }
    }
}

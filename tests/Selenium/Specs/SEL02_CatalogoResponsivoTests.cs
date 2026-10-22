using System.Drawing;
using tutorias.Tests.Selenium.Drivers;
using tutorias.Tests.Selenium.PageObjects;
using Xunit;

namespace tutorias.Tests.Selenium.Specs
{
    public class SEL02_CatalogoResponsivoTests : IDisposable
    {
        private readonly OpenQA.Selenium.IWebDriver _driver;
        private readonly string _baseUrl;

        public SEL02_CatalogoResponsivoTests()
        {
            _driver = WebDriverFactory.CreateDriver();
            _baseUrl = WebDriverFactory.GetBaseUrl();
        }

        [Fact(DisplayName = "SEL-02: Validar adaptabilidad responsiva del catalogo y menu en resolucion movil (375x667)")]
        public void Catalogo_EnResolucionMovil_DebeColapsarMenuYMostrarFiltros()
        {
            // 1. Redimensionar navegador a dimensiones de telefono movil (Mobile-First)
            _driver.Manage().Window.Size = new Size(375, 667);

            var catalogoPage = new CatalogoPage(_driver, _baseUrl);
            catalogoPage.IrAlCatalogo();

            // 2. Verificar que el menu de navegacion se encuentre colapsado en el boton hamburguesa
            Assert.True(catalogoPage.EsMenuHamburguesaVisible(), "El boton de navegacion tipo hamburguesa debe ser visible en pantallas moviles.");

            // 3. Desplegar el menu interactivo
            catalogoPage.AbrirMenuHamburguesa();
            Assert.True(catalogoPage.EsMenuNavegacionDesplegado(), "El menu de navegacion debe desplegarse al hacer clic en el boton hamburguesa.");

            // 4. Verificar que las tarjetas del catalogo se rendericen
            int tarjetas = catalogoPage.ObtenerCantidadTarjetas();
            Assert.True(tarjetas > 0, "El catalogo debe desplegar las tarjetas de tutorias ofertadas.");
        }

        public void Dispose()
        {
            _driver.Quit();
        }
    }
}

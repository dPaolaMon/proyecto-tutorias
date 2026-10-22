using tutorias.Tests.Selenium.Drivers;
using tutorias.Tests.Selenium.PageObjects;
using Xunit;

namespace tutorias.Tests.Selenium.Specs
{
    public class SEL01_E2E_CicloTutoriaTests : IDisposable
    {
        private readonly OpenQA.Selenium.IWebDriver _driver;
        private readonly string _baseUrl;

        public SEL01_E2E_CicloTutoriaTests()
        {
            _driver = WebDriverFactory.CreateDriver();
            _baseUrl = WebDriverFactory.GetBaseUrl();
        }

        [Fact(DisplayName = "SEL-01 (E2E): Recorrido de extremo a extremo de autenticacion, coordinacion y catalogo")]
        public void Flujo_E2E_CoordinadorYCatalogo_DebeOperarSinExcepciones()
        {
            // 1. Fase de Autenticacion: Inicio de sesion como Coordinador institucional
            var loginPage = new LoginPage(_driver, _baseUrl);
            loginPage.IrALogin();
            loginPage.IniciarSesion("coordinacion@ipn.mx", "CoordTutorias#2026");

            // 2. Fase de Coordinacion: Validar acceso a la bandeja de gestion
            var coordinacionPage = new CoordinacionPage(_driver, _baseUrl);
            coordinacionPage.IrAGestionUsuarios();
            Assert.True(coordinacionPage.EsTablaVisible(), "El panel de Coordinacion debe mostrar la tabla de gestion de usuarios.");

            // 3. Fase de Bandeja de Solicitudes: Navegar a solicitudes de tutores
            coordinacionPage.IrASolicitudesTutores();
            Assert.True(coordinacionPage.EsTablaVisible(), "La bandeja de solicitudes de tutores debe ser visible.");

            // 4. Fase de Catálogo: Navegación al catálogo público y aplicación de filtro de búsqueda
            var catalogoPage = new CatalogoPage(_driver, _baseUrl);
            catalogoPage.IrAlCatalogo();
            catalogoPage.BuscarPorTexto("Docente");

            int cantidadTarjetas = catalogoPage.ObtenerCantidadTarjetas();
            Assert.True(cantidadTarjetas > 0, "El catalogo debe devolver al menos un resultado para la busqueda.");
        }

        public void Dispose()
        {
            _driver.Quit();
        }
    }
}

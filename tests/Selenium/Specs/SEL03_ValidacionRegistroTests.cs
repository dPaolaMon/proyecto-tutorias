using tutorias.Tests.Selenium.Drivers;
using tutorias.Tests.Selenium.PageObjects;
using Xunit;

namespace tutorias.Tests.Selenium.Specs
{
    public class SEL03_ValidacionRegistroTests : IDisposable
    {
        private readonly OpenQA.Selenium.IWebDriver _driver;
        private readonly string _baseUrl;

        public SEL03_ValidacionRegistroTests()
        {
            _driver = WebDriverFactory.CreateDriver();
            _baseUrl = WebDriverFactory.GetBaseUrl();
        }

        [Fact(DisplayName = "SEL-03: Validar rechazo de correo no institucional en formulario de registro")]
        public void Registro_ConCorreoExterno_DebeMostrarMensajeDeValidacion()
        {
            var registerPage = new RegisterPage(_driver, _baseUrl);
            registerPage.IrARegistro();

            // Intento de registro con dominio comercial (@gmail.com)
            registerPage.CompletarRegistro(
                "Alumno",
                "Prueba",
                "Selenium",
                "2026112233",
                "alumno.prueba@gmail.com",
                "Password#2026");

            // Verificar que el sistema muestre la advertencia institucional
            bool contieneError = registerPage.ContieneMensajeError("El correo debe pertenecer al dominio institucional")
                                || registerPage.ContieneMensajeError("institucional");

            Assert.True(contieneError, "El sistema debe rechazar visualmente correos que no pertenecen a @ipn.mx o @alumno.ipn.mx.");
        }

        public void Dispose()
        {
            _driver.Quit();
        }
    }
}

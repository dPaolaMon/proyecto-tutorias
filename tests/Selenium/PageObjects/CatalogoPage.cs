using OpenQA.Selenium;

namespace tutorias.Tests.Selenium.PageObjects
{
    public class CatalogoPage : BasePage
    {
        private static readonly By BusquedaInput = By.Name("busqueda");
        private static readonly By CarreraSelect = By.Name("carreraId");
        private static readonly By SubmitButton = By.CssSelector("form[method='get'] button[type='submit']");
        private static readonly By TutorCards = By.CssSelector(".card");
        private static readonly By NavbarToggler = By.CssSelector(".navbar-toggler");
        private static readonly By NavbarCollapse = By.CssSelector("#navbarMain, .navbar-collapse");

        public CatalogoPage(IWebDriver driver, string baseUrl) : base(driver, baseUrl) { }

        public CatalogoPage IrAlCatalogo()
        {
            Driver.Navigate().GoToUrl($"{BaseUrl}/Catalogo/Index");
            return this;
        }

        public void BuscarPorTexto(string texto)
        {
            SendKeysWithWait(BusquedaInput, texto);
            ClickWithWait(SubmitButton);
        }

        public void FiltrarPorCarrera(string carreraId)
        {
            var selectElement = FindElementWithWait(CarreraSelect);
            var option = selectElement.FindElement(By.CssSelector($"option[value='{carreraId}']"));
            option.Click();
            ClickWithWait(SubmitButton);
        }

        public int ObtenerCantidadTarjetas()
        {
            return Driver.FindElements(TutorCards).Count;
        }

        public bool EsMenuHamburguesaVisible()
        {
            try
            {
                var toggler = Driver.FindElement(NavbarToggler);
                return toggler.Displayed;
            }
            catch
            {
                return false;
            }
        }

        public void AbrirMenuHamburguesa()
        {
            ClickWithWait(NavbarToggler);
        }

        public bool EsMenuNavegacionDesplegado()
        {
            try
            {
                return Wait.Until(d =>
                {
                    var collapse = d.FindElement(NavbarCollapse);
                    string? classes = collapse.GetAttribute("class");
                    return collapse.Displayed && classes != null && classes.Contains("show");
                });
            }
            catch
            {
                return false;
            }
        }
    }
}

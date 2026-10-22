using OpenQA.Selenium;

namespace tutorias.Tests.Selenium.PageObjects
{
    public class RegisterPage : BasePage
    {
        private static readonly By NombreInput = By.Id("Nombre");
        private static readonly By ApellidoPaternoInput = By.Id("ApellidoPaterno");
        private static readonly By ApellidoMaternoInput = By.Id("ApellidoMaterno");
        private static readonly By BoletaInput = By.Id("Boleta");
        private static readonly By EmailInput = By.Id("Email");
        private static readonly By PasswordInput = By.Id("Password");
        private static readonly By ConfirmPasswordInput = By.Id("ConfirmPassword");
        private static readonly By SubmitButton = By.CssSelector("button[type='submit']");
        private static readonly By ValidationErrors = By.CssSelector(".field-validation-error, .text-danger");

        public RegisterPage(IWebDriver driver, string baseUrl) : base(driver, baseUrl) { }

        public RegisterPage IrARegistro()
        {
            Driver.Navigate().GoToUrl($"{BaseUrl}/Account/Register");
            return this;
        }

        public void CompletarRegistro(string nombre, string apPaterno, string apMaterno, string boleta, string email, string password)
        {
            SendKeysWithWait(NombreInput, nombre);
            SendKeysWithWait(ApellidoPaternoInput, apPaterno);
            SendKeysWithWait(ApellidoMaternoInput, apMaterno);
            SendKeysWithWait(BoletaInput, boleta);
            SendKeysWithWait(EmailInput, email);
            SendKeysWithWait(PasswordInput, password);
            SendKeysWithWait(ConfirmPasswordInput, password);
            ClickWithWait(SubmitButton);
        }

        public bool ContieneMensajeError(string textoEsperado)
        {
            try
            {
                var elementos = Driver.FindElements(ValidationErrors);
                foreach (var el in elementos)
                {
                    if (el.Text.Contains(textoEsperado, System.StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}

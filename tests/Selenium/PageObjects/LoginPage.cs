using OpenQA.Selenium;

namespace tutorias.Tests.Selenium.PageObjects
{
    public class LoginPage : BasePage
    {
        private static readonly By EmailInput = By.Id("Email");
        private static readonly By PasswordInput = By.Id("Password");
        private static readonly By SubmitButton = By.CssSelector("button[type='submit']");
        private static readonly By ValidationErrors = By.CssSelector(".text-danger, .validation-summary-errors");

        public LoginPage(IWebDriver driver, string baseUrl) : base(driver, baseUrl) { }

        public LoginPage IrALogin()
        {
            Driver.Navigate().GoToUrl($"{BaseUrl}/Account/Login");
            return this;
        }

        public void IniciarSesion(string email, string password)
        {
            SendKeysWithWait(EmailInput, email);
            SendKeysWithWait(PasswordInput, password);
            ClickWithWait(SubmitButton);
        }

        public bool TieneErrorValidacion()
        {
            try
            {
                var el = FindElementWithWait(ValidationErrors);
                return el != null && !string.IsNullOrWhiteSpace(el.Text);
            }
            catch
            {
                return false;
            }
        }
    }
}

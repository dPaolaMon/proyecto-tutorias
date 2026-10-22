using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace tutorias.Tests.Selenium.PageObjects
{
    public abstract class BasePage
    {
        protected readonly IWebDriver Driver;
        protected readonly WebDriverWait Wait;
        protected readonly string BaseUrl;

        protected BasePage(IWebDriver driver, string baseUrl, int timeoutSeconds = 10)
        {
            Driver = driver;
            BaseUrl = baseUrl.TrimEnd('/');
            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
        }

        protected IWebElement FindElementWithWait(By by)
        {
            return Wait.Until(d =>
            {
                var elements = d.FindElements(by);
                foreach (var el in elements)
                {
                    if (el.Displayed && el.Enabled)
                    {
                        return el;
                    }
                }
                return null!;
            });
        }

        protected void ClickWithWait(By by)
        {
            var element = FindElementWithWait(by);
            element.Click();
        }

        protected void SendKeysWithWait(By by, string text)
        {
            var element = FindElementWithWait(by);
            element.Clear();
            element.SendKeys(text);
        }

        public string GetCurrentUrl() => Driver.Url;

        public bool HasSuccessAlert()
        {
            try
            {
                var alert = Wait.Until(d => d.FindElement(By.CssSelector(".alert-success")));
                return alert != null && alert.Displayed;
            }
            catch
            {
                return false;
            }
        }

        public bool HasErrorAlert()
        {
            try
            {
                var alert = Wait.Until(d => d.FindElement(By.CssSelector(".alert-danger")));
                return alert != null && alert.Displayed;
            }
            catch
            {
                return false;
            }
        }

        public string GetAlertText()
        {
            try
            {
                var alert = Driver.FindElement(By.CssSelector(".alert"));
                return alert.Text;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}

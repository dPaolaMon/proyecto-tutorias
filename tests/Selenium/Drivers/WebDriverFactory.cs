using System;
using System.IO;
using System.Text.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace tutorias.Tests.Selenium.Drivers
{
    public class TestSettingsModel
    {
        public string BaseUrl { get; set; } = "http://localhost:5178";
        public string AzureUrl { get; set; } = "https://tu-app-tutorias.azurewebsites.net";
        public bool UseAzure { get; set; } = false;
        public string Browser { get; set; } = "Firefox";
        public bool Headless { get; set; } = false;
        public int DefaultTimeoutSeconds { get; set; } = 10;
    }

    public static class WebDriverFactory
    {
        private static readonly TestSettingsModel Settings;

        static WebDriverFactory()
        {
            Settings = new TestSettingsModel();

            string configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.test.json");
            if (File.Exists(configPath))
            {
                try
                {
                    string json = File.ReadAllText(configPath);
                    using var doc = JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("TestSettings", out var section))
                    {
                        if (section.TryGetProperty("BaseUrl", out var prop)) Settings.BaseUrl = prop.GetString() ?? Settings.BaseUrl;
                        if (section.TryGetProperty("AzureUrl", out var propAzure)) Settings.AzureUrl = propAzure.GetString() ?? Settings.AzureUrl;
                        if (section.TryGetProperty("UseAzure", out var propUse)) Settings.UseAzure = propUse.GetBoolean();
                        if (section.TryGetProperty("Browser", out var propBrowser)) Settings.Browser = propBrowser.GetString() ?? Settings.Browser;
                        if (section.TryGetProperty("Headless", out var propHeadless)) Settings.Headless = propHeadless.GetBoolean();
                        if (section.TryGetProperty("DefaultTimeoutSeconds", out var propTimeout)) Settings.DefaultTimeoutSeconds = propTimeout.GetInt32();
                    }
                }
                catch
                {
                    // Utiliza valores por defecto si el archivo JSON no está disponible
                }
            }

            // Sobrescritura opcional por variables de entorno
            string? envBaseUrl = Environment.GetEnvironmentVariable("SELENIUM_BASE_URL");
            if (!string.IsNullOrWhiteSpace(envBaseUrl)) Settings.BaseUrl = envBaseUrl;

            string? envBrowser = Environment.GetEnvironmentVariable("SELENIUM_BROWSER");
            if (!string.IsNullOrWhiteSpace(envBrowser)) Settings.Browser = envBrowser;
        }

        public static string GetBaseUrl()
        {
            return Settings.UseAzure ? Settings.AzureUrl : Settings.BaseUrl;
        }

        public static int GetDefaultTimeoutSeconds()
        {
            return Settings.DefaultTimeoutSeconds;
        }

        public static IWebDriver CreateDriver()
        {
            IWebDriver driver;
            if (Settings.Browser.Equals("Chrome", StringComparison.OrdinalIgnoreCase))
            {
                var options = new ChromeOptions();
                options.PageLoadStrategy = PageLoadStrategy.Eager;
                if (Settings.Headless)
                {
                    options.AddArgument("--headless=new");
                }
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--window-size=1920,1080");

                driver = new ChromeDriver(options);
            }
            else
            {
                // Por defecto Firefox
                var options = new FirefoxOptions();
                options.PageLoadStrategy = PageLoadStrategy.Eager;
                if (Settings.Headless)
                {
                    options.AddArgument("-headless");
                }
                options.AddArgument("--width=1920");
                options.AddArgument("--height=1080");

                driver = new FirefoxDriver(options);
            }

            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
            return driver;
        }
    }
}

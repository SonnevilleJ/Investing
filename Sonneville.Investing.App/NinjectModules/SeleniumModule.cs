using System;
using System.IO;
using System.Reflection;
using Ninject.Modules;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Sonneville.Investing.Fidelity.WebDriver.Logging;

namespace Sonneville.Investing.App.NinjectModules
{
    public class SeleniumModule : NinjectModule
    {
        public override void Load()
        {
            try
            {
                Unbind<IWebDriver>();

                Bind<IWebDriver>()
                    .ToMethod(context => CreateWebDriver())
                    .WhenInjectedInto<PatientWebDriver>();

                Bind<IWebDriver>()
                    .To<PatientWebDriver>()
                    .WhenInjectedInto<LoggingWebDriver>();
                
                Bind<IWebDriver>()
                    .To<LoggingWebDriver>()
                    .InSingletonScope();
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: Failed to initialize WebDriver: {e}");
            }
        }

        private static IWebDriver CreateWebDriver()
        {
            var chromeOptions = new ChromeOptions();
#if !DEBUG
            chromeOptions.AddArgument("--headless");
#endif
            var chromeDriverDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return new ChromeDriver(chromeDriverDirectory, chromeOptions);
        }
    }
}

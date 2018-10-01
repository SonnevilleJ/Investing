using System;
using System.IO;
using System.Reflection;
using Ninject.Modules;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Sonneville.Investing.Fidelity.WebDriver.Logging;

namespace Sonneville.Investing.App.Ninject
{
    public class SeleniumModule : NinjectModule
    {
        public override void Load()
        {
            try
            {
                Unbind<IWebDriver>();

                ChromeDriver instance = null;
                ChromeDriver CreateWebDriver()
                {
                    return instance ?? (instance = SeleniumModule.CreateWebDriver());
                }

                Bind<ITakesScreenshot>()
                    .ToMethod(context => CreateWebDriver())
                    .InSingletonScope();

                Bind<IWebDriver>()
                    .ToMethod(context => CreateWebDriver())
                    .WhenInjectedInto<ExceptionReportGenerator>()
                    .InSingletonScope();

                Bind<IWebDriver>()
                    .ToMethod(context => CreateWebDriver())
                    .WhenInjectedInto<PatientWebDriver>()
                    .InSingletonScope();

                Bind<IWebDriver>()
                    .To<PatientWebDriver>()
                    .WhenInjectedInto<LoggingWebDriver>()
                    .InSingletonScope();

                Bind<IWebDriver>()
                    .To<LoggingWebDriver>()
                    .WhenInjectedInto<ExceptionReportingWebDriver>()
                    .InSingletonScope();

                Bind<IWebDriver>()
                    .To<ExceptionReportingWebDriver>()
                    .InSingletonScope();
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: Failed to initialize WebDriver: {e}");
            }
        }

        private static ChromeDriver CreateWebDriver()
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
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Ninject.Modules;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Sonneville.Selenium.log4net;

namespace Sonneville.Selenium.Ninject
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
            var workingDirectory = GetWorkingDirectory();
            ConfigureExceptionReports(workingDirectory);
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

        private static string GetWorkingDirectory()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                GetAssemblyAttribute<AssemblyCompanyAttribute>().Company,
                GetAssemblyAttribute<AssemblyTitleAttribute>().Title
            );
        }

        private void ConfigureExceptionReports(string workingDirectory)
        {
            Bind<string>()
                .ToConstant(Path.Combine(workingDirectory, "ErrorReports"))
                .WhenInjectedInto<ExceptionReportGenerator>()
                .InSingletonScope();
        }

        private static T GetAssemblyAttribute<T>()
        {
            return Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(T), false)
                .Cast<T>()
                .Single();
        }
    }
}

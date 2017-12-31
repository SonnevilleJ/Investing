using System;
using Ninject.Modules;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Sonneville.Fidelity.Shell.log4net;

namespace Sonneville.Fidelity.Shell.AppStartup.NinjectModules
{
    public class SeleniumModule : NinjectModule
    {
        public override void Load()
        {
            try
            {
                Kernel.Unbind<IWebDriver>();

                Kernel.Bind<IWebDriver>()
                    .To<ChromeDriver>()
                    .WhenInjectedInto<LoggingWebDriver>()
                    .InSingletonScope();

                Kernel.Bind<IWebDriver>()
                    .To<LoggingWebDriver>()
                    .InSingletonScope();
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: Failed to initialize WebDriver: {e}");
            }
        }
    }
}

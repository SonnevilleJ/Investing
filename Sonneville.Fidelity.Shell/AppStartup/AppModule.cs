using System;
using System.IO;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Sonneville.Utilities.Configuration;

namespace Sonneville.Fidelity.Shell.AppStartup
{
    public class AppModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(syntax => syntax.FromAssembliesMatching("Sonneville.*")
                .SelectAllClasses()
                .BindDefaultInterface()
                .Configure(configurationAction => configurationAction.InSingletonScope()));

            Bind<IWebDriver>().To<ChromeDriver>().InSingletonScope();

            Bind<TextReader>().ToConstant(Console.In).WhenInjectedInto<ICommandRouter>();
            Bind<TextWriter>().ToConstant(Console.Out).WhenInjectedInto<ICommandRouter>();

            BindConfig();
        }

        private void BindConfig()
        {
            var configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FidelityWebDriver.Demo.ini");
            
            Kernel.Rebind<INiniConfigStore>().ToConstant(new NiniConfigStore(configPath));
        }
    }
}
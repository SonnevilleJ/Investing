using System;
using System.IO;
using System.IO.IsolatedStorage;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Sonneville.FidelityWebDriver.Configuration;
using Sonneville.Fidelity.Shell.Configuration;
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

            BindConfig(IsolatedStorageFile.GetUserStoreForAssembly());
        }

        private void BindConfig(IsolatedStorageFile isolatedStorageFile)
        {
            var configStore = new ConfigStore(isolatedStorageFile);
            Kernel.Rebind<FidelityConfiguration>().ToConstant(configStore.Get<FidelityConfiguration>());
            Kernel.Rebind<PortfolioManagerConfiguration>().ToConstant(configStore.Get<PortfolioManagerConfiguration>());
        }
    }
}
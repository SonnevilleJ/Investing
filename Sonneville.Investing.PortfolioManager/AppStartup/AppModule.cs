using System.IO.IsolatedStorage;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Sonneville.FidelityWebDriver.Configuration;
using Sonneville.Investing.PortfolioManager.Configuration;

namespace Sonneville.Investing.PortfolioManager.AppStartup
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

            BindConfig(IsolatedStorageFile.GetUserStoreForAssembly());
        }

        private void BindConfig(IsolatedStorageFile isolatedStore)
        {
            var fidelityConfiguration = FidelityConfiguration.Initialize(isolatedStore);
            Kernel.Rebind<FidelityConfiguration>().ToConstant(fidelityConfiguration);
            var portfolioManagerConfiguration = PortfolioManagerConfiguration.Initialize(isolatedStore);
            Kernel.Rebind<PortfolioManagerConfiguration>().ToConstant(portfolioManagerConfiguration);
        }
    }
}
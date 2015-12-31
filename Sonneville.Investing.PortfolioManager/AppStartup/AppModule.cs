using Ninject.Extensions.Conventions;
using Ninject.Modules;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Sonneville.FidelityWebDriver.Configuration;

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

            BindFidelityConfiguration();
        }

        private void BindFidelityConfiguration()
        {
            var fidelityConfiguration = new FidelityConfiguration();
            fidelityConfiguration.Initialize();
            Kernel.Rebind<FidelityConfiguration>()
                .ToConstant(fidelityConfiguration);
        }
    }
}
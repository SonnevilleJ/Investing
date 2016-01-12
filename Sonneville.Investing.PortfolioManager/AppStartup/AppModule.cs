using Ninject.Extensions.Conventions;
using Ninject.Modules;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Sonneville.FidelityWebDriver.Configuration;
using Sonneville.Investing.PortfolioManager.Configuration;
using Westwind.Utilities.Configuration;

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

            BindSingletonConfiguration<FidelityConfiguration>();
            BindSingletonConfiguration<PortfolioManagerConfiguration>();
        }

        private void BindSingletonConfiguration<T>() where T : AppConfiguration, new()
        {
            var configuration = new T();
            configuration.Initialize();
            Kernel.Rebind<T>()
                .ToConstant(configuration);
        }
    }
}
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using Sonneville.FidelityWebDriver.Configuration;

namespace Sonneville.FidelityWebDriver.Demo.Ninject
{
    public class AppModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(syntax => syntax.FromAssembliesMatching("Sonneville.*")
                .SelectAllClasses()
                .BindAllInterfaces()
                .Configure(configurationAction => configurationAction.InSingletonScope()));

            Bind<FidelityConfiguration>().ToProvider<FidelityConfigurationProvider>();
        }
    }
}
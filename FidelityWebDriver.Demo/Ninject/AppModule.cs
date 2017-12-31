using System;
using System.IO;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using Sonneville.Utilities.Configuration;

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

            BindConfig();
        }

        private void BindConfig()
        {
            var configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FidelityWebDriver.Demo.ini");

            Rebind<INiniConfigStore>().ToConstant(new NiniConfigStore(configPath));
        }
    }
}

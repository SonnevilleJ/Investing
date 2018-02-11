using System;
using System.IO;
using Ninject;
using Ninject.Modules;
using Sonneville.Fidelity.Shell.Configuration;
using Sonneville.Fidelity.WebDriver.Configuration;
using Sonneville.Utilities.Persistence.v2;

namespace Sonneville.Fidelity.Shell.AppStartup.NinjectModules
{
    public class ConfigModule : NinjectModule
    {
        private readonly string _configPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "FidelityWebDriver.Demo.json"
        );

        public override void Load()
        {
            Bind<string>().ToConstant(_configPath)
                .WhenInjectedExactlyInto<JsonDataStore>();

            Rebind<FidelityConfiguration>().ToMethod(context => DataStore.Get<FidelityConfiguration>());
            Rebind<SeleniumConfiguration>().ToMethod(context => DataStore.Get<SeleniumConfiguration>());
        }

        private IDataStore DataStore => KernelInstance.Get<JsonDataStore>();
    }
}
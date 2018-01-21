using System;
using System.IO;
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
            var configStore = new JsonDataStore(_configPath);
            Rebind<IDataStore>().ToConstant(configStore);

            Rebind<FidelityConfiguration>().ToConstant(configStore.Load<FidelityConfiguration>());
            Rebind<SeleniumConfiguration>().ToConstant(configStore.Load<SeleniumConfiguration>());
        }
    }
}
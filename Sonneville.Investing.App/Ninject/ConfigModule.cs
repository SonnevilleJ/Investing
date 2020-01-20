using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Ninject;
using Ninject.Modules;
using Sonneville.Investing.Fidelity.WebDriver.Configuration;
using Sonneville.Selenium.Config;
using Sonneville.Selenium.log4net;
using Sonneville.Utilities.Persistence.v2;

namespace Sonneville.Investing.App.Ninject
{
    public class ConfigModule : NinjectModule
    {
        public override void Load()
        {
            var workingDirectory = GetWorkingDirectory();
            ConfigureLocalDataStore(workingDirectory);
        }

        private static string GetWorkingDirectory()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                GetAssemblyAttribute<AssemblyCompanyAttribute>().Company,
                GetAssemblyAttribute<AssemblyTitleAttribute>().Title
            );
        }

        private void ConfigureLocalDataStore(string workingDirectory)
        {
            Rebind<IDataStore>().To<JsonDataStore>().InSingletonScope();
            var configPath = Path.Combine(
                workingDirectory,
                "FidelityWebDriver.Demo.json"
            );
            Bind<string>().ToConstant(configPath)
                .WhenInjectedExactlyInto<JsonDataStore>();

            var dataStore = KernelInstance.Get<IDataStore>();
            Rebind<FidelityConfiguration>().ToMethod(context => dataStore.Get<FidelityConfiguration>());
            Rebind<SeleniumConfiguration>().ToMethod(context => dataStore.Get<SeleniumConfiguration>());
        }

        private static T GetAssemblyAttribute<T>()
        {
            return Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(T), false)
                .Cast<T>()
                .Single();
        }
    }
}

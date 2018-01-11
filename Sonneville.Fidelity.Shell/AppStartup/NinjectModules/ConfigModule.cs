﻿using System;
using System.IO;
using Ninject.Modules;
using Sonneville.FidelityWebDriver.Configuration;
using Sonneville.Utilities.Configuration;

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
            var configStore = new JsonConfigStore(_configPath);
            Rebind<IConfigStore>().ToConstant(configStore);

            Rebind<FidelityConfiguration>().ToConstant(configStore.Load<FidelityConfiguration>());
        }
    }
}

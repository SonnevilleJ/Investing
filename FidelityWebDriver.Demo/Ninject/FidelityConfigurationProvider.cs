using System;
using System.IO;
using Nini.Config;
using Ninject.Activation;
using Sonneville.FidelityWebDriver.Configuration;

namespace Sonneville.FidelityWebDriver.Demo.Ninject
{
    public class FidelityConfigurationProvider : IProvider<FidelityConfiguration>
    {
        public static readonly string ConfigLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FidelityWebDriver.Demo.ini");

        private static readonly FidelityConfiguration Instance = new FidelityConfiguration();

        public object Create(IContext context)
        {
            if (File.Exists(ConfigLocation))
            {
                var config = new IniConfigSource(ConfigLocation).Configs["Fidelity"];
                Instance.Username = config.Get("Username");
                Instance.Password = config.Get("Password");
            }
            return Instance;
        }

        public Type Type => typeof(FidelityConfiguration);
    }
}

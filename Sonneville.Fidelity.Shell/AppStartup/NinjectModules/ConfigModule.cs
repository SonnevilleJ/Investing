using System;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Ninject;
using Ninject.Modules;
using Sonneville.Fidelity.Shell.Configuration;
using Sonneville.Fidelity.WebDriver.Configuration;
using Sonneville.Utilities.Persistence.v2;

namespace Sonneville.Fidelity.Shell.AppStartup.NinjectModules
{
    public class ConfigModule : NinjectModule
    {
        public override void Load()
        {
            var workingDirectory = GetWorkingDirectory();
            
            ConfigureLog4Net(workingDirectory);
            Bind<ILog>().ToProvider<LogProvider>();

            Rebind<IDataStore>().To<JsonDataStore>().InSingletonScope();
            var configPath = Path.Combine(
                workingDirectory,
                "FidelityWebDriver.Demo.json"
            );
            Bind<string>().ToConstant(configPath)
                .WhenInjectedExactlyInto<JsonDataStore>();

            Rebind<FidelityConfiguration>().ToMethod(context => DataStore.Get<FidelityConfiguration>());
            Rebind<SeleniumConfiguration>().ToMethod(context => DataStore.Get<SeleniumConfiguration>());
        }

        private IDataStore DataStore => KernelInstance.Get<JsonDataStore>();

        private static string GetWorkingDirectory()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                GetAssemblyAttribute<AssemblyCompanyAttribute>().Company,
                GetAssemblyAttribute<AssemblyTitleAttribute>().Title
            );
        }

        private static void ConfigureLog4Net(string workingDirectory)
        {
            var localDataPath = workingDirectory;
            var layout = ConfigurePatternLayout();
            var rollingFileAppender = ConfigureRollingFileAppender(layout, localDataPath);
            var consoleAppender = ConfigureConsoleAppender(layout);

            var hierarchy = (Hierarchy) LogManager.GetRepository();
            hierarchy.Root.AddAppender(rollingFileAppender);
            hierarchy.Root.AddAppender(consoleAppender);
            hierarchy.Root.Level = Level.All;
            hierarchy.Configured = true;
        }

        private static T GetAssemblyAttribute<T>()
        {
            return Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(T), false)
                .Cast<T>()
                .Single();
        }

        private static PatternLayout ConfigurePatternLayout()
        {
            var patternLayout = new PatternLayout
            {
                ConversionPattern = "%date [%thread] %-5level %logger - %message%newline"
            };
            patternLayout.ActivateOptions();
            return patternLayout;
        }

        private static RollingFileAppender ConfigureRollingFileAppender(ILayout layout, string loggingDirectory)
        {
            var appender = new RollingFileAppender
            {
                Name = "Default logger",
                AppendToFile = true,
                File = Path.Combine(loggingDirectory, $"Demo-{DateTime.Today:yyyyMMdd}.log"),
                Layout = layout,
                MaxSizeRollBackups = 5,
                MaximumFileSize = "10MB",
                RollingStyle = RollingFileAppender.RollingMode.Size,
                StaticLogFileName = true,
                ImmediateFlush = true,
                Threshold = Level.All,
            };
            appender.ActivateOptions();
            return appender;
        }

        private static IAppender ConfigureConsoleAppender(ILayout layout)
        {
            var appender = new ConsoleAppender
            {
                Name = "Console appender",
                Layout = layout,
                Target = Console.Error.ToString(),
                Threshold = Level.Warn,
            };
            appender.ActivateOptions();
            return appender;
        }
    }
}
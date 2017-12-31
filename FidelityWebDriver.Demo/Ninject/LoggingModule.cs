using System;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Ninject.Modules;

namespace Sonneville.FidelityWebDriver.Demo.Ninject
{
    public class LoggingModule : NinjectModule
    {
        public override void Load()
        {
            ConfigureLog4Net();

            Bind<ILog>().ToProvider<LogProvider>();
        }

        private static void ConfigureLog4Net()
        {
            var localDataPath = GetLocalDataPath();
            var patternLayout = ConfigurePatternLayout();
            var rollingFileAppender = ConfigureRollingFileAppender(patternLayout, localDataPath);

            var hierarchy = (Hierarchy) LogManager.GetRepository();
            hierarchy.Root.AddAppender(rollingFileAppender);
            hierarchy.Root.Level = Level.All;
            hierarchy.Configured = true;
        }

        private static string GetLocalDataPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                GetAssemblyAttribute<AssemblyCompanyAttribute>().Company,
                GetAssemblyAttribute<AssemblyTitleAttribute>().Title
            );
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

        private static RollingFileAppender ConfigureRollingFileAppender(PatternLayout patternLayout, string loggingDirectory)
        {
            var appender = new RollingFileAppender
            {
                Name = "Default logger",
                AppendToFile = true,
                File = Path.Combine(loggingDirectory, $"Demo-{DateTime.Today:yyyyMMdd}.log"),
                Layout = patternLayout,
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
    }
}

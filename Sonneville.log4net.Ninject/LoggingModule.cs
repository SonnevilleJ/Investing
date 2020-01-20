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
using Sonneville.Ninject;

namespace Sonneville.log4net.Ninject
{
    public class LoggingModule : NinjectModule
    {
        public override void Load()
        {
            var workingDirectory = GetWorkingDirectory();
            ConfigureLog4Net(workingDirectory);
        }

        private static string GetWorkingDirectory()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                GetAssemblyAttribute<AssemblyCompanyAttribute>().Company,
                GetAssemblyAttribute<AssemblyTitleAttribute>().Title
            );
        }

        private void ConfigureLog4Net(string workingDirectory)
        {
            var layout = ConfigurePatternLayout();
            var rollingFileAppender = ConfigureRollingFileAppender(layout, workingDirectory);
            var consoleAppender = ConfigureConsoleAppender(layout);

            var hierarchy = (Hierarchy) LogManager.GetRepository(Assembly.GetEntryAssembly());
            hierarchy.Root.AddAppender(rollingFileAppender);
            hierarchy.Root.AddAppender(consoleAppender);
            hierarchy.Root.Level = Level.All;
            hierarchy.Configured = true;

            Bind<ILog>().ToProvider<LogProvider>();
        }

        private static ILayout ConfigurePatternLayout()
        {
            return ActivateLayout(new PatternLayout
            {
                ConversionPattern = "%date [%thread] %-5level %logger - %message%newline"
            });
        }

        private static IAppender ConfigureRollingFileAppender(ILayout layout, string loggingDirectory)
        {
            return ActivateAppender(new RollingFileAppender
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
                Threshold = Level.All
            });
        }

        private static IAppender ConfigureConsoleAppender(ILayout layout)
        {
            return ActivateAppender(new ConsoleAppender
            {
                Name = "Console appender",
                Layout = layout,
                Target = Console.Error.ToString(),
                Threshold = Level.Warn
            });
        }

        private static IAppender ActivateAppender(AppenderSkeleton appender)
        {
            appender.ActivateOptions();
            return appender;
        }

        private static ILayout ActivateLayout(LayoutSkeleton layout)
        {
            layout.ActivateOptions();
            return layout;
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
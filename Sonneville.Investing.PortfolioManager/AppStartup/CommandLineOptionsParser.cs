using System.Collections.Generic;
using System.IO;
using NDesk.Options;
using Sonneville.FidelityWebDriver.Configuration;

namespace Sonneville.Investing.PortfolioManager.AppStartup
{
    public interface ICommandLineOptionsParser
    {
        bool ShouldExecute(IEnumerable<string> args, FidelityConfiguration fidelityConfiguration, TextWriter textWriter);
    }

    public class CommandLineOptionsParser : ICommandLineOptionsParser
    {
        public bool ShouldExecute(IEnumerable<string> args, FidelityConfiguration fidelityConfiguration,
            TextWriter textWriter)
        {
            var shouldPersistOptions = false;
            var shouldShowHelp = false;
            var optionSet = new OptionSet
            {
                {
                    "u|username=", "the username to use when logging into Fidelity.",
                    username => { fidelityConfiguration.Username = username; }
                },
                {
                    "p|password=", "the password to use when logging into Fidelity.",
                    password => { fidelityConfiguration.Password = password; }
                },
                {
                    "s|save", "indicates options should be persisted.",
                    save => { shouldPersistOptions = true; }
                },
                {
                    "h|help", "shows this message and exits.",
                    help => { shouldShowHelp = true; }
                },
            };
            optionSet.Parse(args);

            if (shouldShowHelp)
            {
                optionSet.WriteOptionDescriptions(textWriter);
                return false;
            }
            if (shouldPersistOptions)
            {
                fidelityConfiguration.Write();
            }
            return true;
        }
    }
}
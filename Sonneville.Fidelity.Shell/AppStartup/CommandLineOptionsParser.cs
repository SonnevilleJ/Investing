using System;
using System.Collections.Generic;
using System.IO;
using NDesk.Options;
using Sonneville.FidelityWebDriver.Configuration;
using Sonneville.Fidelity.Shell.Configuration;
using Sonneville.Investing.Trading;

namespace Sonneville.Fidelity.Shell.AppStartup
{
    public interface ICommandLineOptionsParser
    {
        bool ShouldExecute(IEnumerable<string> args, TextWriter textWriter);
    }

    public class CommandLineOptionsParser : ICommandLineOptionsParser
    {
        private readonly FidelityConfiguration _fidelityConfiguration;
        private readonly PortfolioManagerConfiguration _portfolioManagerConfiguration;

        private readonly IDictionary<string, AccountType> _accountTypes = new Dictionary<string, AccountType>
        {
            {"IA", AccountType.InvestmentAccount},
            {"HS", AccountType.HealthSavingsAccount},
            {"RA", AccountType.RetirementAccount},
            {"CC", AccountType.CreditCard},
            {"OA", AccountType.Other},
        };

        public CommandLineOptionsParser(FidelityConfiguration fidelityConfiguration,
            PortfolioManagerConfiguration portfolioManagerConfiguration)
        {
            _fidelityConfiguration = fidelityConfiguration;
            _portfolioManagerConfiguration = portfolioManagerConfiguration;
        }

        public bool ShouldExecute(IEnumerable<string> args, TextWriter textWriter)
        {
            var shouldPersistOptions = false;
            var shouldShowHelp = false;
            var optionSet = new OptionSet
            {
                {
                    "u|username=", "the username to use when logging into Fidelity.",
                    username => { _fidelityConfiguration.Username = username; }
                },
                {
                    "p|password=", "the password to use when logging into Fidelity.",
                    password => { _fidelityConfiguration.Password = password; }
                },
                {
                    "at|accountType=", "an account type to consider in scope.",
                    accountTypeCode =>
                    {
                        var upperCode = accountTypeCode.ToUpper();
                        if (!_accountTypes.ContainsKey(upperCode))
                            throw new ArgumentException($"Unknown account type code: {accountTypeCode}");
                        var accountType = _accountTypes[upperCode];
                        _portfolioManagerConfiguration.InScopeAccountTypes.Add(accountType);
                    }
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
                _fidelityConfiguration.Write();
                _portfolioManagerConfiguration.Write();
            }
            return true;
        }
    }
}
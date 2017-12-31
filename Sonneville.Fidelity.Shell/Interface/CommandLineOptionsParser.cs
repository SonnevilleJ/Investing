using System;
using System.Collections.Generic;
using System.IO;
using NDesk.Options;
using Sonneville.Fidelity.Shell.Configuration;
using Sonneville.FidelityWebDriver.Configuration;
using Sonneville.Investing.Trading;
using Sonneville.Utilities.Configuration;

namespace Sonneville.Fidelity.Shell.Interface
{
    public interface ICommandLineOptionsParser
    {
        bool ShouldExecute(IEnumerable<string> args, TextWriter textWriter);
    }

    public class CommandLineOptionsParser : ICommandLineOptionsParser
    {
        private readonly INiniConfigStore _configStore;
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

        public CommandLineOptionsParser(INiniConfigStore configStore)
        {
            _configStore = configStore;
            _fidelityConfiguration = _configStore.Read<FidelityConfiguration>();
            _portfolioManagerConfiguration = _configStore.Read<PortfolioManagerConfiguration>();
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
                _configStore.Save(_fidelityConfiguration);
                _configStore.Save(_portfolioManagerConfiguration);
            }
            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using log4net;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Data;

namespace Sonneville.FidelityWebDriver.Positions
{
    public interface IAccountDetailsAggregator
    {
        IAccountDetails ParseAccountDetails(IReadOnlyDictionary<string, AccountType> accountTypesByAccountNumber, IEnumerator<IWebElement> e);
    }

    public class AccountDetailsAggregator : IAccountDetailsAggregator
    {
        private readonly ILog _log;
        private readonly IPositionDetailsExtractor _positionDetailsExtractor;
        private readonly IPositionRowsAccumulator _positionRowsAccumulator;
        private readonly IPendingActivityExtractor _pendingActivityExtractor;
        private readonly ITotalGainLossExtractor _totalGainLossExtractor;
        private readonly IAccountIdentifierExtractor _accountIdentifierExtractor;

        public AccountDetailsAggregator(ILog log, IPositionDetailsExtractor positionDetailsExtractor, IPositionRowsAccumulator positionRowsAccumulator)
        {
            _log = log;
            _positionDetailsExtractor = positionDetailsExtractor;
            _positionRowsAccumulator = positionRowsAccumulator;
            _pendingActivityExtractor = new PendingActivityExtractor();
            _totalGainLossExtractor = new TotalGainLossExtractor();
            _accountIdentifierExtractor = new AccountIdentifierExtractor(_log);
        }

        public IAccountDetails ParseAccountDetails(IReadOnlyDictionary<string, AccountType> accountTypesByAccountNumber, IEnumerator<IWebElement> e)
        {
            var partialAccountDetails = CreatePartialAccountDetails(e.Current, accountTypesByAccountNumber);
            return CompleteAccountDetails(partialAccountDetails, e);
        }

        private AccountDetails CreatePartialAccountDetails(IWebElement tableRow, IReadOnlyDictionary<string, AccountType> accountTypesByAccountNumber)
        {
            return new AccountDetails
            {
                Name = _accountIdentifierExtractor.ExtractAccountName(tableRow),
                AccountNumber = _accountIdentifierExtractor.ExctractAccountNumber(tableRow),
                AccountType = accountTypesByAccountNumber[_accountIdentifierExtractor.ExctractAccountNumber(tableRow)],
            };
        }

        private IAccountDetails CompleteAccountDetails(AccountDetails accountDetails, IEnumerator<IWebElement> e)
        {
            var positionRows = _positionRowsAccumulator.AccumulateRowsForPositions(e);
            var tableRow = e.Current;

            if (tableRow != null)
            {
                _log.Debug($"Completing extraction of details for account {accountDetails.AccountNumber}...");
                accountDetails.PendingActivity = _pendingActivityExtractor.ReadPendingActivity(tableRow);
                var totalGainSpans = tableRow.FindElements(By.ClassName("magicgrid--stacked-data-value"));
                var trimmedGainText = totalGainSpans[0].Text.Trim();
                accountDetails.TotalGainDollar = _totalGainLossExtractor.ReadTotalDollarGain(trimmedGainText);
                accountDetails.TotalGainPercent = _totalGainLossExtractor.ReadTotalPercentGain(totalGainSpans, trimmedGainText);
                accountDetails.Positions = _positionDetailsExtractor.ExtractPositionDetails(positionRows);
                return accountDetails;
            }

            throw new NotImplementedException();
        }
    }
}

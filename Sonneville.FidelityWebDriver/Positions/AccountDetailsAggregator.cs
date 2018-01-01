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
        private readonly IPositionDetailsExtractor _positionDetailsExtractor;
        private readonly IPositionRowsAccumulator _positionRowsAccumulator;
        private readonly IPendingActivityExtractor _pendingActivityExtractor;
        private readonly ITotalGainLossExtractor _totalGainLossExtractor;
        private readonly IAccountIdentifierExtractor _accountIdentifierExtractor;

        public AccountDetailsAggregator(ILog log, IPositionDetailsExtractor positionDetailsExtractor, IPositionRowsAccumulator positionRowsAccumulator)
        {
            _positionDetailsExtractor = positionDetailsExtractor;
            _positionRowsAccumulator = positionRowsAccumulator;
            _pendingActivityExtractor = new PendingActivityExtractor();
            _totalGainLossExtractor = new TotalGainLossExtractor();
            _accountIdentifierExtractor = new AccountIdentifierExtractor(log);
        }

        public IAccountDetails ParseAccountDetails(IReadOnlyDictionary<string, AccountType> accountTypesByAccountNumber, IEnumerator<IWebElement> e)
        {
            var accountDefinitionRow = e.Current;
            var positionRows = _positionRowsAccumulator.AccumulateRowsForPositions(e);
            var accountSummaryRow = e.Current;

            var accountNumber = _accountIdentifierExtractor.ExctractAccountNumber(accountDefinitionRow);
            return new AccountDetails
            {
                Name = _accountIdentifierExtractor.ExtractAccountName(accountDefinitionRow),
                AccountNumber = accountNumber,
                AccountType = accountTypesByAccountNumber[accountNumber],
                PendingActivity = _pendingActivityExtractor.ReadPendingActivity(accountSummaryRow),
                TotalGainDollar = _totalGainLossExtractor.ReadTotalDollarGain(accountSummaryRow),
                TotalGainPercent = _totalGainLossExtractor.ReadTotalPercentGain(accountSummaryRow),
                Positions = _positionDetailsExtractor.ExtractPositionDetails(positionRows),
            };
        }
    }
}

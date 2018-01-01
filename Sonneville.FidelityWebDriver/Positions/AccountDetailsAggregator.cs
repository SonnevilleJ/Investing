using System.Collections.Generic;
using log4net;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Data;

namespace Sonneville.FidelityWebDriver.Positions
{
    public interface IAccountDetailsAggregator
    {
        IAccountDetails ParseAccountDetails(IEnumerator<IWebElement> e, IWebDriver webDriver);
    }

    public class AccountDetailsAggregator : IAccountDetailsAggregator
    {
        private readonly IPositionDetailsExtractor _positionDetailsExtractor;
        private readonly IPositionRowsAccumulator _positionRowsAccumulator;
        private readonly IPendingActivityExtractor _pendingActivityExtractor;
        private readonly ITotalGainLossExtractor _totalGainLossExtractor;
        private readonly IAccountIdentifierExtractor _accountIdentifierExtractor;

        public AccountDetailsAggregator(ILog log, IPositionDetailsExtractor positionDetailsExtractor, IPositionRowsAccumulator positionRowsAccumulator, IAccountTypesMapper accountTypesMapper)
        {
            _positionDetailsExtractor = positionDetailsExtractor;
            _positionRowsAccumulator = positionRowsAccumulator;
            _pendingActivityExtractor = new PendingActivityExtractor();
            _totalGainLossExtractor = new TotalGainLossExtractor();
            _accountIdentifierExtractor = new AccountIdentifierExtractor(log, accountTypesMapper);
        }

        public IAccountDetails ParseAccountDetails(IEnumerator<IWebElement> e, IWebDriver webDriver)
        {
            var accountDefinitionRow = e.Current;
            var positionRows = _positionRowsAccumulator.AccumulateRowsForPositions(e);
            var accountSummaryRow = e.Current;

            return new AccountDetails
            {
                Name = _accountIdentifierExtractor.ExtractAccountName(accountDefinitionRow),
                AccountNumber = _accountIdentifierExtractor.ExtractAccountNumber(accountDefinitionRow),
                AccountType = _accountIdentifierExtractor.ExtractAccountType(accountDefinitionRow, webDriver),
                PendingActivity = _pendingActivityExtractor.ReadPendingActivity(accountSummaryRow),
                TotalGainDollar = _totalGainLossExtractor.ReadTotalDollarGain(accountSummaryRow),
                TotalGainPercent = _totalGainLossExtractor.ReadTotalPercentGain(accountSummaryRow),
                Positions = _positionDetailsExtractor.ExtractPositionDetails(positionRows),
            };
        }
    }
}

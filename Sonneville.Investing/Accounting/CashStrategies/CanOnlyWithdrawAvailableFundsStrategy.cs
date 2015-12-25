using System;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Accounting.CashStrategies
{
    public class CanOnlyWithdrawAvailableFundsStrategy : ICashTransactionStrategy<IWithdrawal>
    {
        private readonly ICashAccountBalanceCalculator _calculator;

        private readonly decimal _accountMinimum;

        public CanOnlyWithdrawAvailableFundsStrategy(ICashAccountBalanceCalculator calculator, decimal accountMinimum)
        {
            _calculator = calculator;
            _accountMinimum = accountMinimum;
        }

        public void ThrowIfInvalid(IWithdrawal withdrawal, ICashAccount cashAccount)
        {
            var amountAfterWithdrawal = _calculator.CalculateBalance(withdrawal.SettlementDate, cashAccount.CashTransactions) - withdrawal.Amount;
            if (amountAfterWithdrawal < _accountMinimum)
            {
                throw new InvalidOperationException(
                    $"Cannot withdraw funds if it would cause the account balance to go below the account minimum: {_accountMinimum}");
            }
        }
    }
}
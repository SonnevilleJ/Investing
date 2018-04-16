using System.Collections.Generic;
using System.Linq;
using Sonneville.Investing.Domain;
using Sonneville.Investing.Trading;

namespace Sonneville.Fidelity.Shell.FidelityWebDriver
{
    public interface IAccountMapper
    {
        TradingAccount Map(IAccountDetails accountDetails);

        IEnumerable<TradingAccount> Map(IEnumerable<IAccountDetails> accountDetails);
    }

    public class AccountMapper : IAccountMapper
    {
        private readonly IPositionMapper _positionMapper;
        private readonly AccountTypeMapper _accountTypeMapper;

        public AccountMapper(IPositionMapper positionMapper)
        {
            _positionMapper = positionMapper;
            _accountTypeMapper = new AccountTypeMapper();
        }

        public TradingAccount Map(IAccountDetails accountDetails)
        {
            return new TradingAccount
            {
                AccountId = accountDetails.AccountNumber,
                PendingFunds = accountDetails.PendingActivity,
                Positions = _positionMapper.Map(accountDetails.Positions).ToList(),
                AccountType = _accountTypeMapper.MapToInvesting(accountDetails.AccountType),
            };
        }

        public IEnumerable<TradingAccount> Map(IEnumerable<IAccountDetails> accountDetails)
        {
            return accountDetails.Select(Map);
        }
    }
}
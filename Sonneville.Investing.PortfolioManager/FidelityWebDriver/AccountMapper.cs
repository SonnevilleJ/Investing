using System.Collections.Generic;
using System.Linq;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.Investing.Trading;

namespace Sonneville.Investing.PortfolioManager.FidelityWebDriver
{
    public interface IAccountMapper
    {
        TradingAccount Map(IAccountDetails accountDetails);

        List<TradingAccount> Map(IEnumerable<IAccountDetails> accountDetails);
    }

    public class AccountMapper : IAccountMapper
    {
        private readonly IPositionMapper _positionMapper;

        public AccountMapper(IPositionMapper positionMapper)
        {
            _positionMapper = positionMapper;
        }

        public TradingAccount Map(IAccountDetails accountDetails)
        {
            return new TradingAccount
            {
                AccountId = accountDetails.AccountNumber,
                PendingFunds = accountDetails.PendingActivity,
                Positions = _positionMapper.Map(accountDetails.Positions),
            };
        }

        public List<TradingAccount> Map(IEnumerable<IAccountDetails> accountDetails)
        {
            return accountDetails.Select(Map).ToList();
        }
    }
}
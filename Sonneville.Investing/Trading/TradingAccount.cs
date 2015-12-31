using System.Collections.Generic;

namespace Sonneville.Investing.Trading
{
    public class TradingAccount
    {
        public string AccountId { get; set; }

        public decimal PendingFunds { get; set; }

        public IEnumerable<Position> Positions { get; set; }
    }
}
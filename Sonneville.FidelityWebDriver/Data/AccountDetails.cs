using System.Collections.Generic;

namespace Sonneville.FidelityWebDriver.Data
{
    public interface IAccountDetails
    {
        AccountType AccountType { get; }

        string Name { get; }

        string AccountNumber { get; }

        decimal PendingActivity { get; }

        decimal TotalGainDollar { get; }

        decimal TotalGainPercent { get; }

        IEnumerable<IPosition> Positions { get; }
    }

    public class AccountDetails : IAccountDetails
    {
        public AccountType AccountType { get; set; }

        public string Name { get; set; }

        public string AccountNumber { get; set; }

        public decimal PendingActivity { get; set; }

        public decimal TotalGainDollar { get; set; }

        public decimal TotalGainPercent { get; set; }

        public IEnumerable<IPosition> Positions { get; set; }
    }
}
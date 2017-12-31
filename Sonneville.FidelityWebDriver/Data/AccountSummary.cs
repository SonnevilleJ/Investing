namespace Sonneville.FidelityWebDriver.Data
{
    public interface IAccountSummary
    {
        AccountType AccountType { get; }
        double MostRecentValue { get; }
        string Name { get; }
        string AccountNumber { get; }
    }

    public class AccountSummary : IAccountSummary
    {
        public AccountType AccountType { get; set; }

        public double MostRecentValue { get; set; }

        public string Name { get; set; }

        public string AccountNumber { get; set; }
    }
}
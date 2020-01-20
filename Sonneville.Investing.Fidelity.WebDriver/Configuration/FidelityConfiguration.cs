using System.Collections.Generic;
using Sonneville.Investing.Domain;

namespace Sonneville.Investing.Fidelity.WebDriver.Configuration
{
    public class FidelityConfiguration
    {
        public HashSet<AccountType> InScopeAccountTypes { get; set; } = new HashSet<AccountType>();
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}

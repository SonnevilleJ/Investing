using System.Collections.Generic;
using Sonneville.Investing.Trading;

namespace Sonneville.Fidelity.Shell.Configuration
{
    public class PortfolioManagerConfiguration
    {
        public HashSet<AccountType> InScopeAccountTypes { get; set; }
    }
}

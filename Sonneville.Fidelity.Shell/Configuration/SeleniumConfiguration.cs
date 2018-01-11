using System;
using System.Collections.Generic;
using Sonneville.Investing.Trading;

namespace Sonneville.Fidelity.Shell.Configuration
{
    public class SeleniumConfiguration
    {
        public HashSet<AccountType> InScopeAccountTypes { get; set; } = new HashSet<AccountType>();

        public TimeSpan WebElementDisplayTimeout { get; set; } = TimeSpan.FromMinutes(1);
    }
}
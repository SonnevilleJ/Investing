using System;
using System.Collections.Generic;
using Sonneville.Investing.Domain;

namespace Sonneville.Investing.Fidelity.WebDriver.Configuration
{
    public class SeleniumConfiguration
    {
        public HashSet<AccountType> InScopeAccountTypes { get; set; } = new HashSet<AccountType>();

        public TimeSpan WebElementDisplayTimeout { get; set; } = TimeSpan.FromMinutes(1);
    }
}
using System;
using System.Collections.Generic;
using Sonneville.Fidelity.WebDriver.Data;

namespace Sonneville.Fidelity.WebDriver.Configuration
{
    public class SeleniumConfiguration
    {
        public HashSet<AccountType> InScopeAccountTypes { get; set; } = new HashSet<AccountType>();

        public TimeSpan WebElementDisplayTimeout { get; set; } = TimeSpan.FromMinutes(1);
    }
}
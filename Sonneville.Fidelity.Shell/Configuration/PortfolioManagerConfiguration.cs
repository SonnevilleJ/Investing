using System.Collections.Generic;
using Sonneville.Investing.Trading;
using Westwind.Utilities.Configuration;

namespace Sonneville.Fidelity.Shell.Configuration
{
    public class PortfolioManagerConfiguration : AppConfiguration
    {
        protected override void OnInitialize(IConfigurationProvider provider, string sectionName, object configData)
        {
            base.OnInitialize(provider, sectionName, configData);
            if (InScopeAccountTypes == null) InScopeAccountTypes = new HashSet<AccountType>();
        }

        public HashSet<AccountType> InScopeAccountTypes { get; set; }
    }
}
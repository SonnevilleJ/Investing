using System.Collections.Generic;
using Sonneville.Investing.Trading;
using Sonneville.Utilities;
using Westwind.Utilities.Configuration;

namespace Sonneville.Investing.PortfolioManager.Configuration
{
    public class PortfolioManagerConfiguration : AppConfiguration
    {
        protected override IConfigurationProvider OnCreateDefaultProvider(string sectionName, object configData)
        {
            return new IsolatedStorageConfigurationProvider<PortfolioManagerConfiguration>();
        }

        public HashSet<AccountType> InScopeAccountTypes { get; set; }
    }
}
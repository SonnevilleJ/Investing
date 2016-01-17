using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Sonneville.Investing.Trading;
using Sonneville.Utilities;
using Westwind.Utilities.Configuration;

namespace Sonneville.Investing.PortfolioManager.Configuration
{
    public class PortfolioManagerConfiguration : AppConfiguration
    {
        public static PortfolioManagerConfiguration Initialize(IsolatedStorageFile isolatedStore)
        {
            var configuration = new PortfolioManagerConfiguration {Store = isolatedStore};
            configuration.Initialize();
            return configuration;
        }

        protected override IConfigurationProvider OnCreateDefaultProvider(string sectionName, object configData)
        {
            return new IsolatedStorageConfigurationProvider<PortfolioManagerConfiguration>(Store);
        }

        private IsolatedStorageFile Store { get; set; }

        protected override void OnInitialize(IConfigurationProvider provider, string sectionName, object configData)
        {
            base.OnInitialize(provider, sectionName, configData);
            if (InScopeAccountTypes == null) InScopeAccountTypes = new HashSet<AccountType>();
        }

        public HashSet<AccountType> InScopeAccountTypes { get; set; }
    }
}
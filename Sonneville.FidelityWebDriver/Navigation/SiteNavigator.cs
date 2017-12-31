using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Login;
using Sonneville.FidelityWebDriver.Positions;
using Sonneville.FidelityWebDriver.Summary;
using Sonneville.FidelityWebDriver.Transactions;

namespace Sonneville.FidelityWebDriver.Navigation
{
    public interface ISiteNavigator : IDisposable
    {
        TPage GoTo<TPage>() where TPage : IPage;
    }

    public class SiteNavigator : ISiteNavigator
    {
        private readonly ILog _log;
        private IWebDriver _webDriver;
        private readonly IPage[] _allPages;

        private readonly Dictionary<Type, Action<ISiteNavigator, IWebDriver>> _pageNavigationActions = new Dictionary
            <Type, Action<ISiteNavigator, IWebDriver>>
        {
            {typeof (IHomePage), (siteNavigator, webDriver) => webDriver.Navigate().GoToUrl("https://www.fidelity.com")},
            {typeof (ILoginPage), (siteNavigator, webDriver) => siteNavigator.GoTo<IHomePage>().GoToLoginPage()},
            {typeof (ISummaryPage), (siteNavigator, webDriver) => webDriver.Navigate().GoToUrl("https://oltx.fidelity.com/ftgw/fbc/oftop/portfolio")},
            {typeof (IPositionsPage), (siteNavigator, webDriver) => siteNavigator.GoTo<ISummaryPage>().GoToPositionsPage()},
            {typeof (IActivityPage), (siteNavigator, webDriver) => siteNavigator.GoTo<ISummaryPage>().GoToActivityPage()},
        };

        public SiteNavigator(ILog log, IWebDriver webDriver, IPage[] allPages)
        {
            _log = log;
            _webDriver = webDriver;
            _allPages = allPages;
        }

        public TPage GoTo<TPage>() where TPage : IPage
        {
            _log.Info($"Navigating to {typeof(TPage).Name}");

            _pageNavigationActions[typeof (TPage)](this, _webDriver);
            return (TPage) _allPages.Single(page => page is TPage);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _webDriver?.Dispose();
                _webDriver = null;
            }
        }
    }
}
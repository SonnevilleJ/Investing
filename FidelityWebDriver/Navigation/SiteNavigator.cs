﻿using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Login;
using Sonneville.FidelityWebDriver.Transactions;

namespace Sonneville.FidelityWebDriver.Navigation
{
    public class SiteNavigator : ISiteNavigator
    {
        private IWebDriver _webDriver;
        private readonly IPageFactory _pageFactory;

        private readonly Dictionary<Type, Action<ISiteNavigator, IWebDriver>> _pageNavigationActions = new Dictionary
            <Type, Action<ISiteNavigator, IWebDriver>>
        {
            {typeof (IHomePage), (siteNavigator, webDriver) => webDriver.Navigate().GoToUrl("https://www.fidelity.com")},
            {typeof (ILoginPage), (siteNavigator, webDriver) => siteNavigator.GoTo<IHomePage>().GoToLoginPage()},
            {typeof (ISummaryPage), (siteNavigator, webDriver) => webDriver.Navigate().GoToUrl("https://oltx.fidelity.com/ftgw/fbc/oftop/portfolio")},
            {typeof (IActivityPage), (siteNavigator, webDriver) => siteNavigator.GoTo<ISummaryPage>().GoToActivityPage()},
        };

        public SiteNavigator(IWebDriver webDriver, IPageFactory pageFactory)
        {
            _webDriver = webDriver;
            _pageFactory = pageFactory;
        }

        public TPage GoTo<TPage>() where TPage : IPage
        {
            _pageNavigationActions[typeof (TPage)](this, _webDriver);
            return _pageFactory.GetPage<TPage>();
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
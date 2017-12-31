using System;
using System.Collections.Generic;
using log4net;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Login;
using Sonneville.FidelityWebDriver.Navigation;
using Sonneville.FidelityWebDriver.Positions;
using Sonneville.FidelityWebDriver.Summary;
using Sonneville.FidelityWebDriver.Transactions;

namespace Sonneville.FidelityWebDriver.Tests.Navigation
{
    [TestFixture]
    public class SiteNavigatorTests : IDisposable
    {
        private Mock<ILog> _logMock;

        private Mock<INavigation> _navigationMock;
        private Mock<IWebDriver> _webDriverMock;

        private Mock<IHomePage> _homePageMock;
        private Mock<ILoginPage> _loginPageMock;
        private Mock<ISummaryPage> _summaryPageMock;
        private Mock<IPositionsPage> _positionsPageMock;
        private Mock<IActivityPage> _activityPageMock;

        private SiteNavigator _siteNavigator;

        [SetUp]
        public void Setup()
        {
            _logMock = new Mock<ILog>();

            _navigationMock = new Mock<INavigation>();

            _webDriverMock = new Mock<IWebDriver>();
            _webDriverMock.Setup(webDriver => webDriver.Navigate()).Returns(_navigationMock.Object);

            _homePageMock = new Mock<IHomePage>();
            _loginPageMock = new Mock<ILoginPage>();
            _summaryPageMock = new Mock<ISummaryPage>();
            _positionsPageMock = new Mock<IPositionsPage>();
            _activityPageMock = new Mock<IActivityPage>();
            var allPages = new IPage[]
            {
                _homePageMock.Object,
                _loginPageMock.Object,
                _summaryPageMock.Object,
                _positionsPageMock.Object,
                _activityPageMock.Object,
            };

            _siteNavigator = new SiteNavigator(_logMock.Object, _webDriverMock.Object, allPages);
        }

        [Test]
        public void ShouldDisposeWebDriver()
        {
            _siteNavigator.Dispose();
            _siteNavigator.Dispose();

            _webDriverMock.Verify(driver => driver.Dispose(), Times.Once());
        }

        [Test]
        public void ShouldThrowIfInspecificPageRequested()
        {
            Assert.Throws<KeyNotFoundException>(() => _siteNavigator.GoTo<IPage>());
        }

        [Test]
        public void ShouldReturnSamePageForEachRequest()
        {
            var page1 = _siteNavigator.GoTo<ILoginPage>();
            var page2 = _siteNavigator.GoTo<ILoginPage>();

            Assert.AreSame(page1, page2);
        }

        [Test]
        public void ShouldGoToHomePage()
        {
            var homePage = _siteNavigator.GoTo<IHomePage>();

            _navigationMock.Verify(navigation => navigation.GoToUrl("https://www.fidelity.com"));
            Assert.AreSame(_homePageMock.Object, homePage);
        }

        [Test]
        public void ShouldGoToLoginPage()
        {
            var loginPage = _siteNavigator.GoTo<ILoginPage>();

            _homePageMock.Verify(page => page.GoToLoginPage());
            Assert.AreSame(_loginPageMock.Object, loginPage);
        }

        [Test]
        public void ShouldGoToSummaryPage()
        {
            var summaryPage = _siteNavigator.GoTo<ISummaryPage>();

            _navigationMock.Verify(navigation => navigation.GoToUrl("https://oltx.fidelity.com/ftgw/fbc/oftop/portfolio"));
            Assert.AreSame(_summaryPageMock.Object, summaryPage);
        }

        [Test]
        public void ShouldGoToPositionsPage()
        {
            var positionsPage = _siteNavigator.GoTo<IPositionsPage>();

            _summaryPageMock.Verify(summaryPage => summaryPage.GoToPositionsPage());
            ;
            Assert.AreSame(_positionsPageMock.Object, positionsPage);
        }

        [Test]
        public void ShouldGoToActivityPage()
        {
            var activityPage = _siteNavigator.GoTo<IActivityPage>();

            _summaryPageMock.Verify(summaryPage => summaryPage.GoToActivityPage());
            Assert.AreSame(_activityPageMock.Object, activityPage);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var siteNavigator = _siteNavigator;
                if (siteNavigator != null)
                {
                    siteNavigator.Dispose();
                    _siteNavigator = null;
                }
            }
        }
    }
}

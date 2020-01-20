using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Sonneville.Selenium.Utilities.Test.Logging
{
    public abstract class WebDriverTestsBase<T> where T : class, IWebDriver
    {
        protected Mock<IWebDriver> WebDriverMock;

        protected ISearchContext WebDriverWrapper;

        [SetUp]
        public virtual void Setup()
        {
            WebDriverMock = new Mock<IWebDriver>();

            WebDriverWrapper = InstantiateWebDriverWrapper(WebDriverMock.Object);
        }

        protected abstract T InstantiateWebDriverWrapper(IWebDriver webDriver);

        [Test]
        public void FindElementShouldCreateWrappedWebElement()
        {
            AssertFindElementReturnsWrappedWebElements(WebDriverWrapper, WebDriverMock);
        }

        [Test]
        public void FindElementsShouldCreateWrappedWebElements()
        {
            AssertFindElementsReturnsWrappedWebElements(WebDriverWrapper, WebDriverMock);
        }

        private void AssertFindElementReturnsWrappedWebElements(
            ISearchContext wrapperSearchContext,
            Mock<IWebDriver> webDriverMock
        )
        {
            var innerMock = new Mock<IWebElement>();

            var locator = By.Id("some ID");
            webDriverMock.Setup(webDriver => webDriver.FindElement(locator))
                .Returns(innerMock.Object);

            var outer = wrapperSearchContext.FindElement(locator);

            AssertSubjectInvokesDependencyCorrectly(outer, innerMock,
                wrapper => wrapper.Click(),
                inner => inner.Click()
            );
        }

        private void AssertFindElementsReturnsWrappedWebElements(
            ISearchContext wrapperSearchContext,
            Mock<IWebDriver> innerSearchContextMock
        )
        {
            var innerMock = new Mock<IWebElement>();

            var locator = By.Id("some ID");
            innerSearchContextMock.Setup(webElement => webElement.FindElements(locator))
                .Returns(new List<IWebElement> {innerMock.Object}.AsReadOnly());

            var outer = wrapperSearchContext.FindElements(locator).Single();

            AssertSubjectInvokesDependencyCorrectly(outer, innerMock,
                wrapper => wrapper.Click(),
                inner => inner.Click()
            );
        }

        protected abstract void AssertSubjectInvokesDependencyCorrectly(
            IWebElement wrapper,
            Mock<IWebElement> innerMock,
            Action<IWebElement> wrapperAction,
            Expression<Action<IWebElement>> expectedInnerInvocation);
    }
}
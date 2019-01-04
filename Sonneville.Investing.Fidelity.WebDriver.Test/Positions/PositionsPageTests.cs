﻿using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.Investing.Domain;
using Sonneville.Investing.Fidelity.WebDriver.Positions;

namespace Sonneville.Investing.Fidelity.WebDriver.Test.Positions
{
    [TestFixture]
    public class PositionsPageTests
    {
        [SetUp]
        public void Setup()
        {
            var classes = "account-selector--tab-all account-selector--target-tab"; // actual div has both
            _allAccountSelectorDiv = new Mock<IWebElement>();
            _allAccountSelectorDiv.Setup(div => div.GetAttribute("class"))
                .Returns(classes);

            _webDriverMock = new Mock<IWebDriver>();
            foreach (var c in classes.Split(" "))
                _webDriverMock.Setup(webDriver => webDriver.FindElement(By.ClassName(c)))
                    .Returns(_allAccountSelectorDiv.Object);

            _accountSummariesExtractorMock = new Mock<IAccountSummariesExtractor>();

            _accountDetailsExtractorMock = new Mock<IAccountDetailsExtractor>();

            _positionsPage = new PositionsPage(_webDriverMock.Object,
                _accountSummariesExtractorMock.Object, _accountDetailsExtractorMock.Object);
        }

        private PositionsPage _positionsPage;
        private Mock<IWebDriver> _webDriverMock;
        private Mock<IAccountSummariesExtractor> _accountSummariesExtractorMock;
        private Mock<IAccountDetailsExtractor> _accountDetailsExtractorMock;
        private Mock<IWebElement> _allAccountSelectorDiv;

        [Test]
        public void ShouldClickAllAccountsBeforeParsingDetails()
        {
            _accountDetailsExtractorMock.Setup(extractor => extractor.ExtractAccountDetails(_webDriverMock.Object))
                .Callback(() => { _allAccountSelectorDiv.Verify(div => div.Click()); });

            _positionsPage.GetAccountDetails();

            _accountDetailsExtractorMock.Verify(extractor => extractor.ExtractAccountDetails(_webDriverMock.Object));
        }

        [Test]
        public void ShouldReturnExtractedDetails()
        {
            var expectedDetails = new List<IAccountDetails>();
            _accountDetailsExtractorMock
                .Setup(extractor => extractor.ExtractAccountDetails(_webDriverMock.Object))
                .Returns(expectedDetails);

            var actualDetails = _positionsPage.GetAccountDetails();

            Assert.AreSame(expectedDetails, actualDetails);
        }

        [Test]
        public void ShouldReturnExtractedSummaries()
        {
            var expectedSummaries = new List<IAccountSummary>();
            _accountSummariesExtractorMock
                .Setup(extractor => extractor.ExtractAccountSummaries(_webDriverMock.Object))
                .Returns(expectedSummaries);

            var actualSummaries = _positionsPage.GetAccountSummaries();

            Assert.AreSame(expectedSummaries, actualSummaries);
        }
    }
}
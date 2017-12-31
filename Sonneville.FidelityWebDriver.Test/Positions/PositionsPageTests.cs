﻿using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.FidelityWebDriver.Positions;

namespace Sonneville.FidelityWebDriver.Test.Positions
{
    [TestFixture]
    public class PositionsPageTests
    {
        private PositionsPage _positionsPage;
        private Mock<IWebDriver> _webDriverMock;
        private Mock<IAccountSummariesExtractor> _accountSummariesExtractorMock;
        private Mock<IAccountDetailsExtractor> _accountDetailsExtractorMock;

        [SetUp]
        public void Setup()
        {
            _webDriverMock = new Mock<IWebDriver>();

            _accountSummariesExtractorMock = new Mock<IAccountSummariesExtractor>();

            _accountDetailsExtractorMock = new Mock<IAccountDetailsExtractor>();

            _positionsPage = new PositionsPage(_webDriverMock.Object,
                _accountSummariesExtractorMock.Object, _accountDetailsExtractorMock.Object);
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
    }
}
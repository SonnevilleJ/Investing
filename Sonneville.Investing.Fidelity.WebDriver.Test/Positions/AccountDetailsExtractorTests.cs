using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.Investing.Domain;
using Sonneville.Investing.Fidelity.WebDriver.Logging;
using Sonneville.Investing.Fidelity.WebDriver.Positions;

namespace Sonneville.Investing.Fidelity.WebDriver.Test.Positions
{
    [TestFixture]
    public class AccountDetailsExtractorTests
    {
        private AccountDetailsExtractor _extractor;
        private Mock<IWebDriver> _webDriverMock;
        private List<IAccountDetails> _expectedAccountDetails;
        private Dictionary<string, List<IPosition>> _positionsByAccount;
        private Mock<IPositionDetailsExtractor> _positionDetailsExtractorMock;
        private Mock<ILog> _logMock;
        private Mock<IWebElement> _tableBodyMock;
        private Mock<ISeleniumWaiter> _seleniumWaiterMock;

        [SetUp]
        public void Setup()
        {
            _tableBodyMock = new Mock<IWebElement>();

            _webDriverMock = new Mock<IWebDriver>();
            StubFindElements(new List<IWebElement> {new Mock<IWebElement>().Object});

            _logMock = new Mock<ILog>();

            _positionDetailsExtractorMock = new Mock<IPositionDetailsExtractor>();

            _expectedAccountDetails = new List<IAccountDetails>
            {
                new AccountDetails
                {
                    AccountType = AccountType.InvestmentAccount,
                    Name = "INDIVIDUAL",
                    AccountNumber = "abcd1234",
                    PendingActivity = 12.34m,
                    TotalGainDollar = .56m,
                    TotalGainPercent = .7890m,
                    Positions = new List<IPosition> {new Mock<IPosition>().Object}
                },
                new AccountDetails
                {
                    AccountType = AccountType.RetirementAccount,
                    Name = "BrokerageLink",
                    AccountNumber = "xyz",
                    PendingActivity = 0,
                    TotalGainDollar = 987.65m,
                    TotalGainPercent = 0.4321m,
                    Positions = new List<IPosition> {new Mock<IPosition>().Object}
                },
                new AccountDetails
                {
                    AccountType = AccountType.Other,
                    Name = "OtherAccount",
                    AccountNumber = "qwerty57†",
                    PendingActivity = 0,
                    Positions = new List<IPosition> {new Mock<IPosition>().Object}
                },
            };
            foreach (var accountType in AccountTypesMapper.CodesForKnownAccountTypes.Keys)
            {
                var accountGroupDivMock = new Mock<IWebElement>();
                var accountNumberSpans = _expectedAccountDetails
                    .Where(accountDetails => accountDetails.AccountType == accountType)
                    .Select(accountDetails => FilterIllegalCharacters(accountDetails.AccountNumber))
                    .Select(accountNumber =>
                    {
                        var accountNumberSpanMock = new Mock<IWebElement>();
                        accountNumberSpanMock.Setup(span => span.Text).Returns(accountNumber);
                        return accountNumberSpanMock.Object;
                    })
                    .ToList()
                    .AsReadOnly();

                accountGroupDivMock.Setup(div => div.FindElements(By.ClassName("account-selector--account-number")))
                    .Returns(accountNumberSpans);

                _webDriverMock.Setup(webDriver => webDriver.FindElement(By.ClassName(AccountTypesMapper.CodesForKnownAccountTypes[accountType])))
                    .Returns(accountGroupDivMock.Object);
            }

            _seleniumWaiterMock = new Mock<ISeleniumWaiter>();
            _seleniumWaiterMock.Setup(waiter =>
                    waiter.WaitUntil(It.IsAny<Func<IWebDriver, bool>>(), It.IsAny<TimeSpan>(), It.IsAny<IWebDriver>()))
                .Callback((Func<IWebDriver, bool> func, TimeSpan timeSpan, IWebDriver driver) =>
                {
                    Assert.AreEqual(TimeSpan.FromMinutes(1), timeSpan);
                    Assert.AreEqual(_webDriverMock.Object, driver);
                    Assert.IsFalse(func(driver));
                    StubFindElements(new List<IWebElement> {new Mock<IWebElement>().Object, _tableBodyMock.Object});
                    Assert.IsTrue(func(driver));
                });

            var accountDetailsAggregator = new AccountDetailsAggregator(
                _logMock.Object,
                _positionDetailsExtractorMock.Object,
                new PositionRowsAccumulator(),
                new AccountTypesMapper()
            );
            _extractor = new AccountDetailsExtractor(accountDetailsAggregator, _seleniumWaiterMock.Object);
        }

        private void StubFindElements(List<IWebElement> tBodyElements)
        {
            _webDriverMock.Setup(webDriver => webDriver.FindElements(By.ClassName("p-positions-tbody")))
                .Returns(tBodyElements.AsReadOnly());
        }

        [Test]
        public void ShouldLogWhenNewAccountTypeFound()
        {
            const string newAccountNumber = "new9999";
            _expectedAccountDetails.Add(new AccountDetails
                {
                    AccountType = AccountType.Unknown,
                    Name = "NEW ACCOUNT",
                    AccountNumber = newAccountNumber,
                    PendingActivity = 12.34m,
                    TotalGainDollar = .56m,
                    TotalGainPercent = .7890m,
                    Positions = new List<IPosition> {new Mock<IPosition>().Object}
                });
            SetupPageContent(_expectedAccountDetails);

            var result = _extractor.ExtractAccountDetails(_webDriverMock.Object).ToList();

            _logMock.Verify(log => log.Warn(It.Is<object>(message => message.ToString().Contains(newAccountNumber))));
            Assert.AreEqual(_expectedAccountDetails.Count, result.Count);
        }

        [Test]
        public void ShouldExtractAccountDetails()
        {
            SetupPageContent(_expectedAccountDetails);

            var actuals = _extractor.ExtractAccountDetails(_webDriverMock.Object).ToList();

            Assert.AreEqual(_expectedAccountDetails.Count, actuals.Count);
            foreach (var actual in actuals)
            {
                var matchingExpected = _expectedAccountDetails.Single(
                    expected => FilterIllegalCharacters(expected.AccountNumber) == actual.AccountNumber);

                Assert.AreEqual(matchingExpected.Name, actual.Name);
                Assert.AreEqual(matchingExpected.AccountType, actual.AccountType);
                Assert.AreSame(_positionsByAccount[actual.Name], actual.Positions);
                Assert.AreEqual(matchingExpected.PendingActivity, actual.PendingActivity);
                Assert.AreEqual(matchingExpected.TotalGainDollar, actual.TotalGainDollar);
                Assert.AreEqual(matchingExpected.TotalGainPercent, actual.TotalGainPercent);
            }
        }

        private string FilterIllegalCharacters(string accountNumber)
        {
            return accountNumber.Replace("†", "");
        }

        private void SetupPageContent(IEnumerable<IAccountDetails> accountDetails)
        {
            _positionsByAccount = new Dictionary<string, List<IPosition>>();
            var tableRows = accountDetails.SelectMany(SetupAccountRows).ToList();
            _tableBodyMock.Setup(tableBody => tableBody.FindElements(By.TagName("tr")))
                .Returns(tableRows.AsReadOnly);
        }

        private List<IWebElement> SetupAccountRows(IAccountDetails account)
        {
            return new List<IWebElement>
                {
                    SetupIgnoredRow(),
                    SetupAccountTitleRow(account.Name, account.AccountNumber),
                    SetupIgnoredRow(),
                }
                .Concat(SetupAccountDetailsRows(account))
                .ToList();
        }

        private IEnumerable<IWebElement> SetupAccountDetailsRows(IAccountDetails account)
        {
            return account.Positions.SelectMany(position => SetupPositionRowsForAccount(account));
        }

        private IEnumerable<IWebElement> SetupPositionRowsForAccount(IAccountDetails account)
        {
            var positionRows = new List<IWebElement>
            {
                SetupPositionRowNormal(),
                SetupPositionRowContent(),
                SetupIgnoredRow(),
                SetupIgnoredRow(),
                SetupAccountTotalRow(account),
                SetupFakeTotalRow()
            };
            var positions = new List<IPosition>();
            _positionsByAccount.Add(account.Name, positions);
            _positionDetailsExtractorMock.Setup(extractor => extractor.ExtractPositionDetails(
                    It.Is<IEnumerable<IWebElement>>(arg => positionRows
                        .Where(row => row.GetAttribute("class") != "this row should be ignored")
                        .Where(row => row.GetAttribute("class") != "magicgrid--total-row")
                        .All(arg.Contains))))
                .Returns(positions);
            return positionRows;
        }

        private static IWebElement SetupAccountTotalRow(IAccountDetails account)
        {
            var totalRowMock = new Mock<IWebElement>();
            totalRowMock.Setup(row => row.GetAttribute("class"))
                .Returns("magicgrid--total-row");

            SetupTotalGain(account, totalRowMock);

            SetupPendingActivity(account, totalRowMock);

            return totalRowMock.Object;
        }

        private static void SetupTotalGain(IAccountDetails account, Mock<IWebElement> totalRowMock)
        {
            var totalGainDollarSpanMock = new Mock<IWebElement>();
            totalGainDollarSpanMock.Setup(span => span.GetAttribute("class")).Returns("magicgrid--stacked-data-value");
            if (account.TotalGainDollar != 0)
            {
                totalGainDollarSpanMock.Setup(span => span.Text)
                    .Returns($@"
                                            {account.TotalGainDollar:C}");
                totalGainDollarSpanMock.Setup(row => row.ToString()).Returns("Total Gain - Dollar != 0");
            }
            else
            {
                totalGainDollarSpanMock.Setup(span => span.Text)
                    .Returns("");
                totalGainDollarSpanMock.Setup(row => row.ToString()).Returns("Total Gain - Dollar == 0");
            }

            var totalGainPercentSpanMock = new Mock<IWebElement>();
            totalGainPercentSpanMock.Setup(span => span.GetAttribute("class")).Returns("magicgrid--stacked-data-value");
            if (account.TotalGainPercent != 0)
            {
                totalGainPercentSpanMock.Setup(span => span.Text)
                    .Returns($@"
                                            {account.TotalGainPercent:P}");
                totalGainPercentSpanMock.Setup(row => row.ToString()).Returns("Total Gain - Percent == 0");
            }
            else
            {
                totalGainPercentSpanMock.Setup(span => span.Text)
                    .Returns("");
                totalGainPercentSpanMock.Setup(row => row.ToString()).Returns("Total Gain - Percent != 0");
            }

            var totalGainSpans = new List<IWebElement> {totalGainDollarSpanMock.Object, totalGainPercentSpanMock.Object};
            totalRowMock.Setup(row => row.FindElements(By.ClassName("magicgrid--stacked-data-value")))
                .Returns(totalGainSpans.AsReadOnly());
        }

        private static void SetupPendingActivity(IAccountDetails account, Mock<IWebElement> totalRowMock)
        {
            if (account.PendingActivity != 0)
            {
                var valueSpanMock = new Mock<IWebElement>();
                valueSpanMock.Setup(span => span.Text)
                    .Returns(account.PendingActivity.ToString("C"));

                var pendingActivityAnchorMock = new Mock<IWebElement>();
                pendingActivityAnchorMock.Setup(anchor => anchor.FindElement(By.ClassName("value")))
                    .Returns(valueSpanMock.Object);

                var pendingActivityTdMock = new Mock<IWebElement>();
                pendingActivityTdMock.Setup(td => td.Text).Returns("you have xyz in pending activity");
                pendingActivityTdMock.Setup(td => td.FindElement(By.ClassName("magicgrid--total-pending-activity-link")))
                    .Returns(pendingActivityAnchorMock.Object);

                totalRowMock.Setup(row => row.FindElement(By.ClassName("magicgrid--total-pending-activity-link-cell")))
                    .Returns(pendingActivityTdMock.Object);
                totalRowMock.Setup(row => row.ToString()).Returns("Total Row - Pending Activity != 0");
            }
            else
            {
                var pendingActivityTdMock = new Mock<IWebElement>();
                pendingActivityTdMock.Setup(td => td.Text).Returns("");

                totalRowMock.Setup(row => row.FindElement(It.IsAny<By>()))
                    .Throws(new NoSuchElementException("should not attempt reading pending activity"));
                totalRowMock.Setup(row => row.FindElement(By.ClassName("magicgrid--total-pending-activity-link-cell")))
                    .Returns(pendingActivityTdMock.Object);
                totalRowMock.Setup(row => row.ToString()).Returns("Total Row - Pending Activity == 0");
            }
        }

        private static IWebElement SetupFakeTotalRow()
        {
            var totalRowMock = new Mock<IWebElement>();
            totalRowMock.Setup(row => row.GetAttribute("class"))
                .Returns("magicgrid--total-row");
            totalRowMock.Setup(row => row.FindElement(It.IsAny<By>()))
                .Throws(new NoSuchElementException("should ignore this table row"));
            totalRowMock.Setup(row => row.ToString()).Returns("FAKE Total Row");
            return totalRowMock.Object;
        }

        private static IWebElement SetupPositionRowContent()
        {
            const string contentRowClasses = "content-row ";
            var rowMock = new Mock<IWebElement>();
            rowMock.Setup(row => row.GetAttribute("class")).Returns(contentRowClasses);
            rowMock.Setup(row => row.ToString()).Returns("Position Row - Content");
            return rowMock.Object;
        }

        private static IWebElement SetupPositionRowNormal()
        {
            const string normalRowClasses =
                "normal-row  NG--NotUsedClass  NG--NotUsedSeparateClass  NG--NotUsedSeparateClass magicgrid--defer-display position-preload-row";
            var rowMock = new Mock<IWebElement>();
            rowMock.Setup(row => row.GetAttribute("class")).Returns(normalRowClasses);
            rowMock.Setup(row => row.ToString()).Returns("Position Row - Normal");
            return rowMock.Object;
        }

        private static IWebElement SetupIgnoredRow()
        {
            var tableRowMock = new Mock<IWebElement>();
            tableRowMock.Setup(row => row.GetAttribute("class")).Returns("this row should be ignored");
            tableRowMock.Setup(span => span.ToString()).Returns("Ignored Row");
            return tableRowMock.Object;
        }

        private static IWebElement SetupAccountTitleRow(string accountName, string accountId)
        {
            var accountTitleRowMock = new Mock<IWebElement>();
            accountTitleRowMock.Setup(row => row.GetAttribute("class")).Returns("magicgrid--account-title-row ");
            var accountNameSpanMock = SetupAccountTitleSpan(accountName);
            accountTitleRowMock.Setup(row => row.FindElement(By.ClassName("magicgrid--account-title-text")))
                .Returns(accountNameSpanMock);
            var accountDescriptionSpanMock = SetupAccountDescriptionSpan(accountId);
            accountTitleRowMock.Setup(row => row.FindElement(By.ClassName("magicgrid--account-title-description")))
                .Returns(accountDescriptionSpanMock);
            
            accountTitleRowMock.Setup(row => row.ToString()).Returns("Account Title Row");
            
            return accountTitleRowMock.Object;
        }

        private static IWebElement SetupAccountTitleSpan(string accountName)
        {
            var accountTitleTextSpanMock = new Mock<IWebElement>();
            accountTitleTextSpanMock.Setup(span => span.Text).Returns($"{accountName}  - ");
            accountTitleTextSpanMock.Setup(span => span.ToString()).Returns("Account Title Span");
            return accountTitleTextSpanMock.Object;
        }

        private static IWebElement SetupAccountDescriptionSpan(string accountId)
        {
            var accountIdSpanMock = new Mock<IWebElement>();
            accountIdSpanMock.Setup(span => span.Text).Returns(accountId);
            accountIdSpanMock.Setup(span => span.ToString()).Returns("Account Description Span");
            return accountIdSpanMock.Object;
        }
    }
}

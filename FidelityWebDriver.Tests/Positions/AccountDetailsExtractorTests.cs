using System.Collections.Generic;
using System.Linq;
using log4net;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.FidelityWebDriver.Positions;

namespace Sonneville.FidelityWebDriver.Tests.Positions
{
    [TestFixture]
    public class AccountDetailsExtractorTests
    {
        private AccountDetailsExtractor _extractor;
        private Mock<IWebDriver> _webDriverMock;
        private List<IAccountDetails> _setupAccountDetails;
        private Dictionary<string, List<IPosition>> _positionsByAccount;
        private Mock<IPositionDetailsExtractor> _positionDetailsExtractorMock;
        private Mock<ILog> _logMock;

        [SetUp]
        public void Setup()
        {
            _positionDetailsExtractorMock = new Mock<IPositionDetailsExtractor>();

            _positionsByAccount = new Dictionary<string, List<IPosition>>();

            _setupAccountDetails = SetupAccountDetails();
            var tableRows = SetupTableRows(_setupAccountDetails);

            var tableBodyMock = new Mock<IWebElement>();
            tableBodyMock.Setup(tableBody => tableBody.FindElements(By.TagName("tr")))
                .Returns(tableRows.AsReadOnly);

            _webDriverMock = new Mock<IWebDriver>();
            _webDriverMock.Setup(webDriver => webDriver.FindElements(By.ClassName("p-positions-tbody")))
                .Returns(new List<IWebElement> {new Mock<IWebElement>().Object, tableBodyMock.Object}.AsReadOnly());

            _logMock = new Mock<ILog>();

            _extractor = new AccountDetailsExtractor(_positionDetailsExtractorMock.Object, _logMock.Object);
        }

        [Test]
        public void ShouldExtractAccountDetails()
        {
            var accountTypeStrings = new Dictionary<AccountType, string>
            {
                {AccountType.InvestmentAccount, "IA"},
                {AccountType.RetirementAccount, "RA"},
                {AccountType.HealthSavingsAccount, "HS"},
                {AccountType.Other, "OA"},
                {AccountType.CreditCard, "CC"},
            };

            foreach (var accountType in accountTypeStrings.Keys)
            {
                var accountGroupDivMock = new Mock<IWebElement>();
                var accountNumberSpans = _setupAccountDetails
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

                _webDriverMock.Setup(webDriver => webDriver.FindElement(By.ClassName(accountTypeStrings[accountType])))
                    .Returns(accountGroupDivMock.Object);
            }

            var actuals = _extractor.ExtractAccountDetails(_webDriverMock.Object).ToList();

            Assert.AreEqual(_setupAccountDetails.Count(), actuals.Count());
            foreach (var actual in actuals)
            {
                var matchingExpected = _setupAccountDetails.Single(
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

        private static List<IAccountDetails> SetupAccountDetails()
        {
            return new List<IAccountDetails>
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
        }

        private List<IWebElement> SetupTableRows(IEnumerable<IAccountDetails> accounts)
        {
            return accounts.SelectMany(SetupAccountRows).ToList();
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

        private IWebElement SetupAccountTotalRow(IAccountDetails account)
        {
            var totalRowMock = new Mock<IWebElement>();
            totalRowMock.Setup(row => row.GetAttribute("class"))
                .Returns("magicgrid--total-row");

            SetupTotalGain(account, totalRowMock);

            SetupPendingActivity(account, totalRowMock);

            return totalRowMock.Object;
        }

        private void SetupTotalGain(IAccountDetails account, Mock<IWebElement> totalRowMock)
        {
            var totalGainDollarSpanMock = new Mock<IWebElement>();
            totalGainDollarSpanMock.Setup(span => span.GetAttribute("class")).Returns("magicgrid--stacked-data-value");
            if (account.TotalGainDollar != 0)
            {
                totalGainDollarSpanMock.Setup(span => span.Text)
                    .Returns($@"
                                            {account.TotalGainDollar:C}");
            }
            else
            {
                totalGainDollarSpanMock.Setup(span => span.Text)
                    .Returns("");
            }

            var totalGainPercentSpanMock = new Mock<IWebElement>();
            totalGainPercentSpanMock.Setup(span => span.GetAttribute("class")).Returns("magicgrid--stacked-data-value");
            if (account.TotalGainPercent != 0)
            {
                totalGainPercentSpanMock.Setup(span => span.Text)
                    .Returns($@"
                                            {account.TotalGainPercent:P}");
            }
            else
            {
                totalGainPercentSpanMock.Setup(span => span.Text)
                    .Returns("");
            }

            var totalGainSpans = new List<IWebElement> {totalGainDollarSpanMock.Object, totalGainPercentSpanMock.Object};
            totalRowMock.Setup(row => row.FindElements(By.ClassName("magicgrid--stacked-data-value")))
                .Returns(totalGainSpans.AsReadOnly());
        }

        private void SetupPendingActivity(IAccountDetails account, Mock<IWebElement> totalRowMock)
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
            }
            else
            {
                var pendingActivityTdMock = new Mock<IWebElement>();
                pendingActivityTdMock.Setup(td => td.Text).Returns("");

                totalRowMock.Setup(row => row.FindElement(It.IsAny<By>()))
                    .Throws(new NoSuchElementException("should not attempt reading pending activity"));
                totalRowMock.Setup(row => row.FindElement(By.ClassName("magicgrid--total-pending-activity-link-cell")))
                    .Returns(pendingActivityTdMock.Object);
            }
        }

        private IWebElement SetupFakeTotalRow()
        {
            var totalRowMock = new Mock<IWebElement>();
            totalRowMock.Setup(row => row.GetAttribute("class"))
                .Returns("magicgrid--total-row");
            totalRowMock.Setup(row => row.FindElement(It.IsAny<By>()))
                .Throws(new NoSuchElementException("should ignore this table row"));

            return totalRowMock.Object;
        }

        private IWebElement SetupPositionRowContent()
        {
            const string contentRowClasses = "content-row ";
            var rowMock = new Mock<IWebElement>();
            rowMock.Setup(row => row.GetAttribute("class")).Returns(contentRowClasses);
            return rowMock.Object;
        }

        private IWebElement SetupPositionRowNormal()
        {
            const string normalRowClasses =
                "normal-row  NG--NotUsedClass  NG--NotUsedSeparateClass  NG--NotUsedSeparateClass magicgrid--defer-display position-preload-row";
            var rowMock = new Mock<IWebElement>();
            rowMock.Setup(row => row.GetAttribute("class")).Returns(normalRowClasses);
            return rowMock.Object;
        }

        private IWebElement SetupIgnoredRow()
        {
            var tableRowMock = new Mock<IWebElement>();
            tableRowMock.Setup(row => row.GetAttribute("class")).Returns("this row should be ignored");
            return tableRowMock.Object;
        }

        private IWebElement SetupAccountTitleRow(string accountName, string accountId)
        {
            var accountTitleRowMock = new Mock<IWebElement>();
            accountTitleRowMock.Setup(row => row.GetAttribute("class")).Returns("magicgrid--account-title-row ");
            var accountNameSpanMock = SetupAccountTitleSpan(accountName);
            accountTitleRowMock.Setup(row => row.FindElement(By.ClassName("magicgrid--account-title-text")))
                .Returns(accountNameSpanMock);
            var accountDescriptionSpanMock = SetupAccountDescriptionSpan(accountId);
            accountTitleRowMock.Setup(row => row.FindElement(By.ClassName("magicgrid--account-title-description")))
                .Returns(accountDescriptionSpanMock);
            return accountTitleRowMock.Object;
        }

        private IWebElement SetupAccountTitleSpan(string accountName)
        {
            var accountTitleTextSpanMock = new Mock<IWebElement>();
            accountTitleTextSpanMock.Setup(span => span.Text).Returns($"{accountName}  - ");
            return accountTitleTextSpanMock.Object;
        }

        private IWebElement SetupAccountDescriptionSpan(string accountId)
        {
            var accountIdSpanMock = new Mock<IWebElement>();
            accountIdSpanMock.Setup(span => span.Text).Returns(accountId);
            return accountIdSpanMock.Object;
        }
    }
}

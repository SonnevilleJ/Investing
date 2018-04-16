using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using log4net;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.Investing.Domain;
using Sonneville.Investing.Fidelity.Utilities;
using Sonneville.Investing.Fidelity.WebDriver.Positions;

namespace Sonneville.Investing.Fidelity.WebDriver.Test.Positions
{
    [TestFixture]
    public class AccountSummariesExtractorTests
    {
        private Mock<IWebDriver> _webDriverMock;
        private List<Dictionary<string, string>> _expectedAccountDetails;
        private Dictionary<string, AccountType> _accountTypeStrings;
        private Mock<ILog> _logMock;
        private AccountSummariesExtractor _extractor;

        [SetUp]
        public void Setup()
        {
            _accountTypeStrings = new Dictionary<string, AccountType>
            {
                {"IA", AccountType.InvestmentAccount},
                {"RA", AccountType.RetirementAccount},
                {"HS", AccountType.HealthSavingsAccount},
                {"OA", AccountType.Other},
                {"CC", AccountType.CreditCard},
            };

            _expectedAccountDetails = new List<Dictionary<string, string>>
            {
                CreateAccountValues("abc1234", "IA", "my taxable investment account", "123.45678"),
                CreateAccountValues("bl54321", "RA", "my BrokerageLink account", "98765.43211"),
                CreateAccountValues("hsa9876", "HS", "my health savings account", "1357.02468"),
                CreateAccountValues("401kplan", "OA", "my 401(k) account", "0"),
                CreateAccountValues("credit1", "CC", "my first credit card", "1234.56"),
                CreateAccountValues("credit2", "CC", "my second credit card", "12.34"),
            };

            _webDriverMock = new Mock<IWebDriver>();

            _logMock = new Mock<ILog>();

            _extractor = new AccountSummariesExtractor(_logMock.Object);
        }

        [Test]
        public void ShouldLogWhenNewAccountTypeFound()
        {
            const string accountType = "NE";
            _expectedAccountDetails.Add(CreateAccountValues("new9999", accountType, "some new account", "0"));
            SetupAccountDivs(_expectedAccountDetails, _webDriverMock);

            var summaries = _extractor.ExtractAccountSummaries(_webDriverMock.Object).ToList();

            _logMock.Verify(log => log.Warn(It.Is<object>(message => message.ToString().Contains(accountType))));
            Assert.AreEqual(_expectedAccountDetails.Count - 1, summaries.Count);
        }

        [Test]
        public void ShouldReturnOneAccountPerAccountOnPage()
        {
            SetupAccountDivs(_expectedAccountDetails, _webDriverMock);

            var accounts = _extractor.ExtractAccountSummaries(_webDriverMock.Object).ToList();

            Assert.AreEqual(_expectedAccountDetails.Count, accounts.Count);
            foreach (var account in accounts)
            {
                var matchingExpected =
                    _expectedAccountDetails.Single(values => values["accountNumber"] == account.AccountNumber);

                Assert.AreEqual(matchingExpected["accountName"], account.Name);
                Assert.AreEqual(_accountTypeStrings[matchingExpected["accountType"]], account.AccountType);
                Assert.AreEqual(NumberParser.ParseDouble(matchingExpected["accountValue"]), account.MostRecentValue);
            }
        }

        private void SetupAccountDivs(IEnumerable<Dictionary<string, string>> expectedAccountDetails, Mock<IWebDriver> webDriverMock)
        {
            var accountDivs = expectedAccountDetails
                .GroupBy(d => d["accountType"])
                .Select(CreateAccountGroupDivs)
                .ToList()
                .AsReadOnly();

            webDriverMock.Setup(driver => driver.FindElements(By.ClassName("account-selector--group-container")))
                .Returns(accountDivs);
        }

        private Dictionary<string, string> CreateAccountValues(string accountNumber, string accountType,
            string accountName, string accountValue)
        {
            return new Dictionary<string, string>
            {
                {"accountNumber", accountNumber},
                {"accountType", accountType},
                {"accountName", accountName},
                {"accountValue", accountValue},
            };
        }

        private IWebElement CreateAccountGroupDivs(
            IGrouping<string, IReadOnlyDictionary<string, string>> accountValuesByAccountType)
        {
            var accountGroupDivMock = new Mock<IWebElement>();
            accountGroupDivMock.Setup(div => div.GetAttribute("data-group-id")).Returns(accountValuesByAccountType.Key);
            accountGroupDivMock.Setup(div => div.FindElements(By.ClassName("js-account")))
                .Returns(CreateAccountDivs(accountValuesByAccountType));
            return accountGroupDivMock.Object;
        }

        private ReadOnlyCollection<IWebElement> CreateAccountDivs(
            IGrouping<string, IReadOnlyDictionary<string, string>> accountValuesByAccountType)
        {
            return accountValuesByAccountType
                .Select(values => CreateWebElementForAccountValues(accountValuesByAccountType.Key, values))
                .ToList()
                .AsReadOnly();
        }

        private IWebElement CreateWebElementForAccountValues(string accountType,
            IReadOnlyDictionary<string, string> values)
        {
            var accountDivMock = new Mock<IWebElement>();
            accountDivMock.Setup(div => div.GetAttribute("data-acct-number"))
                .Returns(values["accountNumber"]);
            accountDivMock.Setup(div => div.GetAttribute("data-acct-name"))
                .Returns(values["accountName"]);
            accountDivMock.Setup(div => div.GetAttribute(GetAttributeByAccountType(accountType)))
                .Returns(values["accountValue"]);
            return accountDivMock.Object;
        }

        private string GetAttributeByAccountType(string accountType)
        {
            return new Dictionary<string, string>
            {
                {"IA", "data-most-recent-value"},
                {"RA", "data-most-recent-value"},
                {"HS", "data-most-recent-value"},
                {"OA", "data-most-recent-value"},
                {"CC", "data-acct-balance"}
            }.TryGetValue(accountType, out var knownValueAttributeName1)
                ? knownValueAttributeName1
                : "unknown";
        }
    }
}

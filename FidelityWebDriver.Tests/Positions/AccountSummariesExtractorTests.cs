using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.FidelityWebDriver.Positions;
using Sonneville.FidelityWebDriver.Utilities;

namespace Sonneville.FidelityWebDriver.Tests.Positions
{
    [TestFixture]
    public class AccountSummariesExtractorTests
    {
        private Mock<IWebDriver> _webDriverMock;
        private List<Dictionary<string, string>> _expectedAccountDetails;
        private Dictionary<string, AccountType> _accountTypeStrings;

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

            var accountDivs = SetupAccountDivs(_expectedAccountDetails);

            _webDriverMock = new Mock<IWebDriver>();
            _webDriverMock.Setup(driver => driver.FindElements(By.ClassName("account-selector--group-container")))
                .Returns(accountDivs);
        }

        [Test]
        public void ShouldReturnOneAccountPerAccountOnPage()
        {
            var accounts = new AccountSummariesExtractor().ExtractAccountSummaries(_webDriverMock.Object).ToList();

            Assert.AreEqual(_expectedAccountDetails.Count(), accounts.Count());
            foreach (var account in accounts)
            {
                var matchingExpected =
                    _expectedAccountDetails.Single(values => values["accountNumber"] == account.AccountNumber);

                Assert.AreEqual(matchingExpected["accountName"], account.Name);
                Assert.AreEqual(_accountTypeStrings[matchingExpected["accountType"]], account.AccountType);
                Assert.AreEqual(NumberParser.ParseDouble(matchingExpected["accountValue"]), account.MostRecentValue);
            }
        }

        private ReadOnlyCollection<IWebElement> SetupAccountDivs(
            IEnumerable<Dictionary<string, string>> expectedAccountDetails)
        {
            return expectedAccountDetails
                .GroupBy(d => d["accountType"])
                .Select(CreateAccountGroupDivs)
                .ToList()
                .AsReadOnly();
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

        private static IWebElement CreateWebElementForAccountValues(string accountType,
            IReadOnlyDictionary<string, string> values)
        {
            var valueAttributeNames = new Dictionary<string, string>
            {
                {"IA", "data-most-recent-value"},
                {"RA", "data-most-recent-value"},
                {"HS", "data-most-recent-value"},
                {"OA", "data-most-recent-value"},
                {"CC", "data-acct-balance"}
            };
            var accountDivMock = new Mock<IWebElement>();
            accountDivMock.Setup(div => div.GetAttribute("data-acct-number"))
                .Returns(values["accountNumber"]);
            accountDivMock.Setup(div => div.GetAttribute("data-acct-name"))
                .Returns(values["accountName"]);
            accountDivMock.Setup(div => div.GetAttribute(valueAttributeNames[accountType]))
                .Returns(values["accountValue"]);
            return accountDivMock.Object;
        }
    }
}
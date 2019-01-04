using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.Investing.Domain;
using Sonneville.Investing.Fidelity.WebDriver.Positions;

namespace Sonneville.Investing.Fidelity.WebDriver.Test.Positions
{
    [TestFixture]
    public class AccountTypesMapperTests
    {
        [SetUp]
        public void Setup()
        {
            _mockWebDriver = new Mock<IWebDriver>();

            _accountTypesMapper = new AccountTypesMapper();
        }

        private AccountTypesMapper _accountTypesMapper;
        private Mock<IWebDriver> _mockWebDriver;

        private void SetupAccountWebElements(Dictionary<string, AccountType> expectedResults)
        {
            var accountNumbersByAccountTypeDictionary = expectedResults.Values.Distinct().Select(entry =>
                    new KeyValuePair<AccountType, IEnumerable<string>>(
                        entry,
                        expectedResults
                            .Where(kvp => kvp.Value.Equals(entry))
                            .Select(kvp => kvp.Key)
                    ))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            foreach (var (key, value) in accountNumbersByAccountTypeDictionary)
            {
                var classNameToFind = AccountTypesMapper.CodesForKnownAccountTypes[key];
                _mockWebDriver.Setup(webDriver => webDriver.FindElement(By.ClassName(classNameToFind)))
                    .Returns(CreateAccountWebElement(value));
            }

            var unusedAccountTypeCodes = AccountTypesMapper.CodesForKnownAccountTypes.Keys
                .Except(expectedResults.Values)
                .Select(accountType => AccountTypesMapper.CodesForKnownAccountTypes[accountType]);
            foreach (var unusedAccountTypeCode in unusedAccountTypeCodes)
                _mockWebDriver.Setup(webDriver => webDriver.FindElement(By.ClassName(unusedAccountTypeCode)))
                    .Throws<NoSuchElementException>();
        }

        private static IWebElement CreateAccountWebElement(IEnumerable<string> accountNumbers)
        {
            var mockWebElement = new Mock<IWebElement>();
            var mockResults = accountNumbers.Select(accountNumber =>
            {
                var mockResult = new Mock<IWebElement>();
                mockResult.Setup(webElement => webElement.Text).Returns(accountNumber);
                return mockResult;
            });
            mockWebElement
                .Setup(webElement => webElement.FindElements(By.ClassName("account-selector--account-number")))
                .Returns(mockResults.Select(mock => mock.Object).ToList().AsReadOnly);
            return mockWebElement.Object;
        }

        [Test]
        public void ShouldReturnDictionaryOfAccountNumberToAccountType_MultipleOfSameType()
        {
            var expectedResults = new Dictionary<string, AccountType>
            {
                {"account 1", AccountType.InvestmentAccount},
                {"account 2", AccountType.InvestmentAccount},
                {"retirement account 1", AccountType.RetirementAccount},
                {"retirement account 2", AccountType.RetirementAccount},
                {"health savings account 1", AccountType.HealthSavingsAccount},
                {"health savings account 2", AccountType.HealthSavingsAccount},
                {"other account 1", AccountType.Other},
                {"other account 2", AccountType.Other},
                {"credit card 1", AccountType.CreditCard},
                {"credit card 2", AccountType.CreditCard},
                {"checking account 1", AccountType.CheckingSavings},
                {"checking account 2", AccountType.CheckingSavings},
            };
            SetupAccountWebElements(expectedResults);

            var dictionary = _accountTypesMapper.MapAccountNumbersToAccountType(_mockWebDriver.Object);

            CollectionAssert.AreEquivalent(expectedResults, dictionary);
        }

        [Test]
        public void ShouldReturnDictionaryOfAccountNumberToAccountType_OneOfEach()
        {
            var expectedResults = new Dictionary<string, AccountType>
            {
                {"account 1", AccountType.InvestmentAccount},
                {"retirement account", AccountType.RetirementAccount},
                {"health savings account", AccountType.HealthSavingsAccount},
                {"other account", AccountType.Other},
                {"credit card", AccountType.CreditCard},
                {"checking account", AccountType.CheckingSavings},
            };
            SetupAccountWebElements(expectedResults);

            var dictionary = _accountTypesMapper.MapAccountNumbersToAccountType(_mockWebDriver.Object);

            CollectionAssert.AreEquivalent(expectedResults, dictionary);
        }

        [Test]
        public void ShouldReturnDictionaryOfAccountNumberToAccountType_SomeMissing()
        {
            var expectedResults = new Dictionary<string, AccountType>
            {
                {"account 1", AccountType.InvestmentAccount},
                {"retirement account", AccountType.RetirementAccount},
                {"checking account", AccountType.CheckingSavings},
            };
            SetupAccountWebElements(expectedResults);

            var dictionary = _accountTypesMapper.MapAccountNumbersToAccountType(_mockWebDriver.Object);

            CollectionAssert.AreEquivalent(expectedResults, dictionary);
        }
    }
}
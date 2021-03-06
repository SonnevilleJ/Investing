using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;
using Sonneville.Investing.Domain;

namespace Sonneville.Investing.Fidelity.WebDriver.Positions
{
    public interface IAccountTypesMapper
    {
        Dictionary<string, AccountType> MapAccountNumbersToAccountType(IWebDriver webDriver);
    }

    public class AccountTypesMapper : IAccountTypesMapper
    {
        public static Dictionary<AccountType, string> CodesForKnownAccountTypes { get; } =
            new Dictionary<AccountType, string>
            {
                {AccountType.InvestmentAccount, "IA"},
                {AccountType.RetirementAccount, "RA"},
                {AccountType.HealthSavingsAccount, "HS"},
                {AccountType.Other, "OA"},
                {AccountType.CreditCard, "CC"},
                {AccountType.CheckingSavings, "SC"}
            };

        public Dictionary<string, AccountType> MapAccountNumbersToAccountType(IWebDriver webDriver)
        {
            return CodesForKnownAccountTypes.ToDictionary(
                map => map.Key,
                map => FindWebElementsOfAccountType(webDriver, map.Value)
            ).SelectMany(kvp => kvp.Value.ToDictionary(
                webElement => webElement.Text,
                webElement => kvp.Key)
            ).ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value
            );
        }

        private static ReadOnlyCollection<IWebElement> FindWebElementsOfAccountType(
            IWebDriver webDriver,
            string classNameToFind)
        {
            try
            {
                return webDriver
                    .FindElement(By.ClassName(classNameToFind))
                    .FindElements(By.ClassName("account-selector--account-number"));
            }
            catch (NoSuchElementException)
            {
                return new List<IWebElement>().AsReadOnly();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using log4net;
using OpenQA.Selenium;
using Sonneville.Investing.Domain;
using Sonneville.Investing.Fidelity.Transactions;
using Sonneville.Investing.Fidelity.Utilities;

namespace Sonneville.Investing.Fidelity.WebDriver.Transactions
{
    public interface IHistoryTransactionParser
    {
        IEnumerable<ITransaction> ParseFidelityTransactions(IWebElement historyRoot);
    }

    public class HistoryTransactionParser : IHistoryTransactionParser
    {
        private readonly ILog _log;
        private readonly ITransactionTypeMapper _transactionTypeMapper;

        public HistoryTransactionParser(ILog log, ITransactionTypeMapper transactionTypeMapper)
        {
            _log = log;
            _transactionTypeMapper = transactionTypeMapper;
        }

        public IEnumerable<ITransaction> ParseFidelityTransactions(IWebElement historyRoot)
        {
            return historyRoot.FindElements(By.TagName("tbody"))[0]
                .FindElements(By.TagName("tr"))
                .Where(row => row.GetAttribute("class").Contains("normal-row") || row.GetAttribute("class").Contains("content-row"))
                .Select((row, index) => new KeyValuePair<int, IWebElement>(index, row))
                .GroupBy(kvp => kvp.Key / 2, kvp => kvp.Value, (i, elements) => elements)
                .Select(transactionRows => ParseTransactionFromRows(transactionRows.ToArray()));
        }

        private ITransaction ParseTransactionFromRows(IReadOnlyList<IWebElement> normalAndContentRows)
        {
            var result = new Transaction();
            normalAndContentRows[0].Click();
            var normalTDs = normalAndContentRows[0].FindElements(By.TagName("td"));
            result.RunDate = ParseDate(normalTDs[0].Text);
            result.AccountName = ParseAccountName(normalTDs[1]);
            result.AccountNumber = ParseAccountNumber(normalTDs[1]);
            result.SecurityDescription = ParseSecurityDescription(normalTDs[2]);
            result.Type = ParseType(result.SecurityDescription);

            var contentBody = normalAndContentRows[1].FindElement(By.TagName("tbody"));
            var tHeaders = contentBody.FindElements(By.TagName("th"));
            var tDatas = contentBody.FindElements(By.TagName("td"));

            var contentDictionary = tHeaders.Zip(tDatas, (th, td) => new KeyValuePair<string, string>(th.Text, td.Text))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            result.Amount = ParseCurrency(contentDictionary[AttributeStrings.Amount]);
            switch (result.Type)
            {
                case TransactionType.Deposit:
                case TransactionType.DepositBrokeragelink:
                case TransactionType.DepositHSA:
                case TransactionType.Withdrawal:
                    break;
                case TransactionType.Buy:
                    result.Symbol = contentDictionary[AttributeStrings.Symbol];
                    result.Quantity = ParseQuantity(contentDictionary[AttributeStrings.Quantity]);
                    result.Price = ParseDecimal(contentDictionary[AttributeStrings.Price]);
                    result.SettlementDate = ParseDate(contentDictionary[AttributeStrings.SettlementDate]);
                    break;
                case TransactionType.Sell:
                    result.Symbol = contentDictionary[AttributeStrings.Symbol];
                    result.Quantity = ParseQuantity(contentDictionary[AttributeStrings.Quantity]);
                    result.Price = ParseDecimal(contentDictionary[AttributeStrings.Price]);
                    if (contentDictionary.Keys.Contains(AttributeStrings.Commission)) result.Commission = ParseCurrency(contentDictionary[AttributeStrings.Commission]);
                    if (contentDictionary.Keys.Contains(AttributeStrings.SettlementDate)) result.SettlementDate = ParseDate(contentDictionary[AttributeStrings.SettlementDate]);
                    break;
                case TransactionType.DividendReceipt:
                case TransactionType.ShortTermCapGain:
                case TransactionType.LongTermCapGain:
                case TransactionType.InterestEarned:
                    result.Symbol = contentDictionary[AttributeStrings.Symbol];
                    break;
                case TransactionType.DividendReinvestment:
                    result.Symbol = contentDictionary[AttributeStrings.Symbol];
                    result.Quantity = ParseQuantity(contentDictionary[AttributeStrings.Quantity]);
                    result.Price = ParseDecimal(contentDictionary[AttributeStrings.Price]);
                    break;
                case TransactionType.Unknown:
                    _log.Warn($"Not parsing some details for unknown transaction type in account `{result.AccountNumber}` with description `{result.SecurityDescription}`");
                    break;
                default:
                    throw new NotImplementedException();
            }

            return result;
        }

        private decimal ParseCurrency(string text)
        {
            return NumberParser.ParseDecimal(text, NumberStyles.Currency);
        }

        private decimal ParseQuantity(string text)
        {
            return NumberParser.ParseDecimal(text.Replace("+ ", ""));
        }

        private decimal ParseDecimal(string text)
        {
            return NumberParser.ParseDecimal(text);
        }

        private TransactionType ParseType(string description)
        {
            return _transactionTypeMapper.ClassifyDescription(description);
        }

        private string ParseSecurityDescription(IWebElement descriptionTd)
        {
            return descriptionTd.Text;
        }

        private string ParseAccountNumber(IWebElement accountTd)
        {
            var accountNumberSpan = accountTd.FindElements(By.TagName("span"))[1];
            return accountNumberSpan.Text;
        }

        private string ParseAccountName(IWebElement accountTd)
        {
            var accountNameSpan = accountTd.FindElements(By.TagName("span"))[0];
            return accountNameSpan.Text;
        }

        private DateTime ParseDate(string text)
        {
            return DateTime.Parse(text, CultureInfo.InvariantCulture);
        }
    }
}

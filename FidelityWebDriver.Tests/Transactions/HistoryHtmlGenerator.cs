using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.FidelityWebDriver.Transactions;

namespace Sonneville.FidelityWebDriver.Tests.Transactions
{
    public class HistoryHtmlGenerator
    {
        public IEnumerable<IWebElement> MapToTableRows(IFidelityTransaction transaction, ICollection<string> excludedKeys = null)
        {
            var contentRow = CreateContentRow(transaction, excludedKeys ?? new string[0]);
            var normalRow = CreateNormalRow(transaction, contentRow);
            yield return normalRow.Object;
            yield return contentRow.Object;
            var garbageRow = new Mock<IWebElement>();
            garbageRow.Setup(row => row.GetAttribute("class")).Returns("This row should be ignored");
            yield return garbageRow.Object;
        }

        private Mock<IWebElement> CreateNormalRow(IFidelityTransaction transaction, Mock<IWebElement> contentRow)
        {
            var trNormalRowExpandableMock = CreateNormalRowMock();
            trNormalRowExpandableMock.Setup(tr => tr.GetAttribute("class")).Returns("normal-row expandable");
            trNormalRowExpandableMock.Setup(tr => tr.Click())
                .Callback(() =>
                {
                    if (trNormalRowExpandableMock.Object.GetAttribute("class").Contains("row-selected"))
                    {
                        trNormalRowExpandableMock.Setup(row => row.GetAttribute("class")).Returns("normal-row expandable");
                        contentRow.Setup(row => row.GetAttribute("class")).Returns("content-row content-row-hidden");
                    }
                    else
                    {
                        trNormalRowExpandableMock.Setup(row => row.GetAttribute("class")).Returns("normal-row expandable row-selected");
                        contentRow.Setup(row => row.GetAttribute("class")).Returns("content-row");
                    }
                });
            var normalRowLocalCopy = trNormalRowExpandableMock;
            trNormalRowExpandableMock.Setup(tr => tr.FindElements(By.TagName("td")))
                .Callback(() => normalRowLocalCopy.Verify(row => row.Click()))
                .Returns(new List<IWebElement>
                {
                    CreateDateTd(transaction.RunDate),
                    CreateAccountTd(transaction.AccountName, transaction.AccountNumber),
                    CreateDescriptionTd(transaction.SecurityDescription),
                }.AsReadOnly);
            return trNormalRowExpandableMock;
        }

        private Mock<IWebElement> CreateContentRow(IFidelityTransaction transaction, ICollection<string> excludeKeys)
        {
            var trContentRowMock = CreateContentRowMock();
            trContentRowMock.Setup(tr => tr.FindElement(By.TagName("tbody")))
                .Returns(CreateActivityTrs(transaction, excludeKeys));
            return trContentRowMock;
        }

        private IWebElement CreateActivityTrs(IFidelityTransaction transaction, ICollection<string> excludedKeys)
        {
            var trs = CreateTrDictionary(transaction)
                .Where(kvp => !excludedKeys.Contains(kvp.Key.Text))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return MockActivityTableBody(trs);
        }

        private static IWebElement MockActivityTableBody(Dictionary<IWebElement, IWebElement> trs)
        {
            var tbodyMock = new Mock<IWebElement>();
            tbodyMock.Setup(tr => tr.FindElements(By.TagName("th")))
                .Returns(trs.Keys.ToList().AsReadOnly);
            tbodyMock.Setup(tr => tr.FindElements(By.TagName("td")))
                .Returns(trs.Values.ToList().AsReadOnly);
            return tbodyMock.Object;
        }

        private IEnumerable<KeyValuePair<IWebElement, IWebElement>> CreateTrDictionary(IFidelityTransaction transaction)
        {
            switch (transaction.Type)
            {
                case TransactionType.Deposit:
                case TransactionType.DepositBrokeragelink:
                case TransactionType.DepositHSA:
                case TransactionType.Withdrawal:
                    yield return CreateActivityKeyValuePair(AttributeStrings.Amount, transaction.Amount?.ToString("C"));
                    break;
                case TransactionType.Buy:
                    yield return CreateActivityKeyValuePair(AttributeStrings.Symbol, transaction.Symbol);
                    yield return CreateActivityKeyValuePair(AttributeStrings.SecurityDescription, transaction.SecurityDescription);
                    yield return CreateActivityKeyValuePair(AttributeStrings.Quantity, transaction.Quantity?.ToString("+ 0.###"));
                    yield return CreateActivityKeyValuePair(AttributeStrings.Price, transaction.Price?.ToString("F"));
                    yield return CreateActivityKeyValuePair(AttributeStrings.Amount, transaction.Amount?.ToString("C"));
                    yield return CreateActivityKeyValuePair(AttributeStrings.SettlementDate, transaction.SettlementDate?.ToString("MM/dd/yyyy"));
                    break;
                case TransactionType.Sell:
                    yield return CreateActivityKeyValuePair(AttributeStrings.Symbol, transaction.Symbol);
                    yield return CreateActivityKeyValuePair(AttributeStrings.SecurityDescription, transaction.SecurityDescription);
                    yield return CreateActivityKeyValuePair(AttributeStrings.Quantity, transaction.Quantity?.ToString("+ 0.###"));
                    yield return CreateActivityKeyValuePair(AttributeStrings.Price, transaction.Price?.ToString("F"));
                    yield return CreateActivityKeyValuePair(AttributeStrings.Amount, transaction.Amount?.ToString("C"));
                    yield return CreateActivityKeyValuePair(AttributeStrings.Commission, transaction.Commission?.ToString("C"));
                    yield return CreateActivityKeyValuePair(AttributeStrings.SettlementDate, transaction.SettlementDate?.ToString("MM/dd/yyyy"));
                    break;
                case TransactionType.DividendReceipt:
                case TransactionType.ShortTermCapGain:
                case TransactionType.LongTermCapGain:
                    yield return CreateActivityKeyValuePair(AttributeStrings.Symbol, transaction.Symbol);
                    yield return CreateActivityKeyValuePair(AttributeStrings.SecurityDescription, transaction.SecurityDescription);
                    yield return CreateActivityKeyValuePair(AttributeStrings.Amount, transaction.Amount?.ToString("C"));
                    break;
                case TransactionType.DividendReinvestment:
                    yield return CreateActivityKeyValuePair(AttributeStrings.Symbol, transaction.Symbol);
                    yield return CreateActivityKeyValuePair(AttributeStrings.SecurityDescription, transaction.SecurityDescription);
                    yield return CreateActivityKeyValuePair(AttributeStrings.Quantity, transaction.Quantity?.ToString("+ 0.###"));
                    yield return CreateActivityKeyValuePair(AttributeStrings.Price, transaction.Price?.ToString("F"));
                    yield return CreateActivityKeyValuePair(AttributeStrings.Amount, transaction.Amount?.ToString("C"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private KeyValuePair<IWebElement, IWebElement> CreateActivityKeyValuePair(string label, string value)
        {
            return new KeyValuePair<IWebElement, IWebElement>(CreateActivityThMock(label).Object, CreateActivityTdMock(value).Object);
        }

        private Mock<IWebElement> CreateNormalRowMock()
        {
            var trMock = new Mock<IWebElement>();
            trMock.Setup(tr => tr.GetAttribute("class")).Returns("normal-row expandable");
            return trMock;
        }

        private Mock<IWebElement> CreateContentRowMock()
        {
            var trMock = new Mock<IWebElement>();
            trMock.Setup(tr => tr.GetAttribute("class")).Returns("content-row");
            return trMock;
        }

        private IWebElement CreateDateTd(DateTime? runDate)
        {
            var dateTd = new Mock<IWebElement>();
            dateTd.Setup(td => td.Text).Returns(runDate?.ToString("MM/dd/yyyy"));
            return dateTd.Object;
        }

        private IWebElement CreateAccountTd(string accountName, string accountNumber)
        {
            var accountTd = new Mock<IWebElement>();
            accountTd.Setup(td => td.FindElements(By.TagName("span")))
                .Returns(new List<IWebElement>
                {
                    CreateSpanMock(accountName).Object,
                    CreateSpanMock(accountNumber).Object
                }.AsReadOnly());
            return accountTd.Object;
        }

        private static Mock<IWebElement> CreateSpanMock(string text)
        {
            var accountNumberSpan = new Mock<IWebElement>();
            accountNumberSpan.Setup(span => span.Text).Returns(text);
            return accountNumberSpan;
        }

        private IWebElement CreateDescriptionTd(string descriptionText)
        {
            var tdMock = new Mock<IWebElement>();
            tdMock.Setup(td => td.Text).Returns(descriptionText);
            return tdMock.Object;
        }

        private Mock<IWebElement> CreateActivityThMock(string label)
        {
            var thMock = new Mock<IWebElement>();
            thMock.Setup(th => th.Text).Returns(label);
            return thMock;
        }

        private Mock<IWebElement> CreateActivityTdMock(string value)
        {
            var tdMock = new Mock<IWebElement>();
            tdMock.Setup(td => td.Text).Returns(value);
            return tdMock;
        }
    }
}
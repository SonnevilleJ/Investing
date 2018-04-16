using System.Collections.Generic;
using OpenQA.Selenium;
using Sonneville.Investing.Fidelity.Utilities;

namespace Sonneville.Investing.Fidelity.WebDriver.Positions.DetailExtractors
{
    public interface IPositionCurrentValueExtractor
    {
        decimal ExtractCurrentValue(IReadOnlyList<IWebElement> tdElements);
    }

    public class PositionCurrentValueExtractor : IPositionCurrentValueExtractor
    {
        public decimal ExtractCurrentValue(IReadOnlyList<IWebElement> tdElements)
        {
            return NumberParser.ParseDecimal(tdElements[4].Text);
        }
    }
}
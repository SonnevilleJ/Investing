using System.Collections.Generic;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Utilities;

namespace Sonneville.FidelityWebDriver.Positions.DetailExtractors
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
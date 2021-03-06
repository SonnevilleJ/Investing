using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using Sonneville.Investing.Fidelity.Utilities;

namespace Sonneville.Investing.Fidelity.WebDriver.Positions.DetailExtractors
{
    public interface IPositionLastPriceExtractor
    {
        decimal ExtractLastPrice(IReadOnlyList<IWebElement> tdElements);
    }

    public class PositionLastPriceExtractor : IPositionLastPriceExtractor
    {
        public decimal ExtractLastPrice(IReadOnlyList<IWebElement> tdElements)
        {
            var text = tdElements[1].FindElements(By.ClassName("magicgrid--stacked-data-value"))
                .First()
                .Text;
            return text.Contains("--")
                ? 0m
                : NumberParser.ParseDecimal(text);
        }
    }
}
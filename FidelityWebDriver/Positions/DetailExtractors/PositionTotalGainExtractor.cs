using System.Collections.Generic;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Utilities;

namespace Sonneville.FidelityWebDriver.Positions.DetailExtractors
{
    public interface IPositionTotalGainExtractor
    {
        decimal ExtractTotalGainDollar(IReadOnlyList<IWebElement> tdElements);

        decimal ExtractTotalGainPercent(IReadOnlyList<IWebElement> tdElements);
    }

    public class PositionTotalGainExtractor : IPositionTotalGainExtractor
    {
        public decimal ExtractTotalGainDollar(IReadOnlyList<IWebElement> tdElements)
        {
            var totalGainDollarText = tdElements[3]
                .FindElements(By.ClassName("magicgrid--stacked-data-value"))[0]
                .Text;
            return totalGainDollarText == "n/a" || totalGainDollarText.Contains("--")
                ? 0
                : NumberParser.ParseDecimal(totalGainDollarText);
        }

        public decimal ExtractTotalGainPercent(IReadOnlyList<IWebElement> tdElements)
        {
            var totalGainPercentText = tdElements[3]
                .FindElements(By.ClassName("magicgrid--stacked-data-value"))[1]
                .Text.TrimEnd('%');
            return totalGainPercentText == "n/a" || totalGainPercentText.Contains("--")
                ? 0
                : NumberParser.ParseDecimal(totalGainPercentText)/100m;
        }
    }
}
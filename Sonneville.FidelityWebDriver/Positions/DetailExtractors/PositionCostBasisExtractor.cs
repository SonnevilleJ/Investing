using System.Collections.Generic;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Utilities;

namespace Sonneville.FidelityWebDriver.Positions.DetailExtractors
{
    public interface IPositionCostBasisExtractor
    {
        decimal ExtractCostBasisPerShare(IReadOnlyList<IWebElement> tdElements);

        decimal ExtractCostBasisTotal(IReadOnlyList<IWebElement> tdElements);
    }

    public class PositionCostBasisExtractor : IPositionCostBasisExtractor
    {
        public decimal ExtractCostBasisPerShare(IReadOnlyList<IWebElement> tdElements)
        {
            var spanText = tdElements[6].FindElements(By.ClassName("magicgrid--stacked-data-value"))[0].Text;
            return spanText == "n/a" || string.IsNullOrWhiteSpace(spanText) || spanText.Contains("--")
                ? 0
                : NumberParser.ParseDecimal(spanText.Replace("/Share", ""));
        }

        public decimal ExtractCostBasisTotal(IReadOnlyList<IWebElement> tdElements)
        {
            var spanText = tdElements[6]
                .FindElements(By.ClassName("magicgrid--stacked-data-value"))[1]
                .FindElement(By.TagName("span"))
                .Text;
            return spanText == "n/a" || string.IsNullOrWhiteSpace(spanText) || spanText.Contains("--")
                ? 0
                : NumberParser.ParseDecimal(spanText);
        }
    }
}
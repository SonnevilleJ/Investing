using System.Collections.Generic;
using OpenQA.Selenium;

namespace Sonneville.FidelityWebDriver.Positions.DetailExtractors
{
    public interface IPositionTickerExtractor
    {
        string ExtractCoreTicker(IReadOnlyList<IWebElement> tdElements);

        string ExtractNonCoreTicker(IReadOnlyList<IWebElement> tdElements);

        string ExtractDescription(IReadOnlyList<IWebElement> tdElements);
    }

    public class PositionTickerExtractor : IPositionTickerExtractor
    {
        public string ExtractCoreTicker(IReadOnlyList<IWebElement> tdElements)
        {
            return tdElements[0].FindElement(By.ClassName("stock-symbol")).Text.Replace("**", "");
        }

        public string ExtractNonCoreTicker(IReadOnlyList<IWebElement> tdElements)
        {
            return tdElements[0].FindElement(By.ClassName("stock-symbol")).Text;
        }

        public string ExtractDescription(IReadOnlyList<IWebElement> tdElements)
        {
            return tdElements[0].FindElement(By.ClassName("stock-name")).Text;
        }
    }
}
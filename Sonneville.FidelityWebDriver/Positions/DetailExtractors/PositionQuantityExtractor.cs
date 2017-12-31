using System.Collections.Generic;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Utilities;

namespace Sonneville.FidelityWebDriver.Positions.DetailExtractors
{
    public interface IPositionQuantityExtractor
    {
        decimal ExtractQuantity(IReadOnlyList<IWebElement> tdElements);

        bool ExtractMargin(IReadOnlyList<IWebElement> tdElements);
    }

    public class PositionQuantityExtractor : IPositionQuantityExtractor
    {
        public decimal ExtractQuantity(IReadOnlyList<IWebElement> tdElements)
        {
            var rawText = tdElements[5].Text;
            var quantityText = ExtractMargin(tdElements)
                ? rawText.Replace("<br>", "").Replace("(Margin)", "").Replace("\"", "")
                : rawText;
            return NumberParser.ParseDecimal(quantityText.Trim());
        }

        public bool ExtractMargin(IReadOnlyList<IWebElement> tdElements)
        {
            return tdElements[5].Text.Contains("(Margin)");
        }
    }
}
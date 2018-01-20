using OpenQA.Selenium;
using Sonneville.Fidelity.WebDriver.Utilities;

namespace Sonneville.Fidelity.WebDriver.Positions
{
    public interface ITotalGainLossExtractor
    {
        decimal ReadTotalDollarGain(IWebElement summaryRow);

        decimal ReadTotalPercentGain(IWebElement summaryRow);
    }

    public class TotalGainLossExtractor : ITotalGainLossExtractor
    {
        public decimal ReadTotalDollarGain(IWebElement summaryRow)
        {
            var totalGainSpans = summaryRow.FindElements(By.ClassName("magicgrid--stacked-data-value"));
            var trimmedGainText = totalGainSpans[0].Text.Trim();
            if (!string.IsNullOrWhiteSpace(trimmedGainText))
            {
                return NumberParser.ParseDecimal(trimmedGainText);
            }

            return default(decimal);
        }

        public decimal ReadTotalPercentGain(IWebElement summaryRow)
        {
            var totalGainSpans = summaryRow.FindElements(By.ClassName("magicgrid--stacked-data-value"));
            var trimmedPercentText = totalGainSpans[1].Text.Trim('%');
            if (!string.IsNullOrWhiteSpace(trimmedPercentText))
            {
                return NumberParser.ParseDecimal(trimmedPercentText) / 100m;
            }

            return default(decimal);
        }
    }
}

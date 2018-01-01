using System.Collections.Generic;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Utilities;

namespace Sonneville.FidelityWebDriver.Positions
{
    public interface ITotalGainLossExtractor
    {
        decimal ReadTotalDollarGain(string trimmedGainText);

        decimal ReadTotalPercentGain(IReadOnlyList<IWebElement> totalGainSpans, string trimmedGainText);
    }

    public class TotalGainLossExtractor : ITotalGainLossExtractor
    {
        public decimal ReadTotalDollarGain(string trimmedGainText)
        {
            if (!string.IsNullOrWhiteSpace(trimmedGainText))
            {
                return NumberParser.ParseDecimal(trimmedGainText);
            }

            return default(decimal);
        }

        public decimal ReadTotalPercentGain(IReadOnlyList<IWebElement> totalGainSpans, string trimmedGainText)
        {
            var trimmedPercentText = totalGainSpans[1].Text.Trim('%');
            if (!string.IsNullOrWhiteSpace(trimmedGainText))
            {
                return NumberParser.ParseDecimal(trimmedPercentText) / 100m;
            }

            return default(decimal);
        }
    }
}
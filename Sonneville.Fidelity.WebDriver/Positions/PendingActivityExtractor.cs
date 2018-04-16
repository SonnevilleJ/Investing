using OpenQA.Selenium;
using Sonneville.Investing.Fidelity.Utilities;

namespace Sonneville.Fidelity.WebDriver.Positions
{
    public interface IPendingActivityExtractor
    {
        decimal ReadPendingActivity(IWebElement tableRow);
    }

    public class PendingActivityExtractor : IPendingActivityExtractor
    {
        public decimal ReadPendingActivity(IWebElement tableRow)
        {
            var pendingActivityDiv = tableRow.FindElement(By.ClassName("magicgrid--total-pending-activity-link-cell"));
            if (!string.IsNullOrWhiteSpace(pendingActivityDiv.Text))
            {
                var rawPendingActivityText = pendingActivityDiv
                    .FindElement(By.ClassName("magicgrid--total-pending-activity-link"))
                    .FindElement(By.ClassName("value"))
                    .Text;
                return NumberParser.ParseDecimal(rawPendingActivityText);
            }

            return default(decimal);
        }
    }
}
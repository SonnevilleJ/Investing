using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace Sonneville.Investing.Fidelity.WebDriver.Positions
{
    public interface IPositionRowsAccumulator
    {
        List<IWebElement> AccumulateRowsForPositions(IEnumerator<IWebElement> e);
    }

    public class PositionRowsAccumulator : IPositionRowsAccumulator
    {
        public List<IWebElement> AccumulateRowsForPositions(IEnumerator<IWebElement> e)
        {
            while (e.MoveNext() && e.Current != null)
            {
                return AccumulatePositionRows(e).ToList();
            }

            throw new NotImplementedException();
        }

        private static IEnumerable<IWebElement> AccumulatePositionRows(IEnumerator<IWebElement> e)
        {
            while (e.MoveNext() && e.Current != null && !IsTotalRow(e.Current))
            {
                if (IsPositionRow(e.Current))
                    yield return e.Current;
            }
        }

        private static bool IsTotalRow(IWebElement tableRow)
        {
            var classes = tableRow.GetAttribute("class");
            return !string.IsNullOrWhiteSpace(classes)
                   && classes.Contains("magicgrid--total-row");
        }

        private static bool IsPositionRow(IWebElement tableRow)
        {
            var classes = tableRow.GetAttribute("class");
            return !string.IsNullOrWhiteSpace(classes)
                   && (classes.Contains("normal-row") || classes.Contains("content-row"));
        }
    }
}

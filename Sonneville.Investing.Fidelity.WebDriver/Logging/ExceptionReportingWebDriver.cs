using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;

namespace Sonneville.Investing.Fidelity.WebDriver.Logging
{
    public class ExceptionReportingWebDriver : WebDriverBase
    {
        private readonly IExceptionReportGenerator _exceptionReportGenerator;

        public ExceptionReportingWebDriver(
            IWebDriver webDriver,
            IExceptionReportGenerator exceptionReportGenerator)
            : base(webDriver)
        {
            _exceptionReportGenerator = exceptionReportGenerator;
        }

        public override IWebElement FindElement(By by)
        {
            return Wrap(base.FindElement(by), _exceptionReportGenerator);
        }

        public override ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return base.FindElements(by)
                .Select(element => Wrap(element, _exceptionReportGenerator))
                .ToList()
                .AsReadOnly();
        }

        private static IWebElement Wrap(IWebElement foundElement, IExceptionReportGenerator exceptionReportGenerator)
        {
            return new ExceptionReportingWebElement(foundElement, exceptionReportGenerator);
        }
    }
}
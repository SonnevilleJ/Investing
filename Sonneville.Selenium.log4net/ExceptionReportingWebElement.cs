using System;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;

namespace Sonneville.Selenium.log4net
{
    public class ExceptionReportingWebElement : WebElementBase
    {
        private readonly IExceptionReportGenerator _exceptionReportGenerator;

        public ExceptionReportingWebElement(IWebElement webElement, IExceptionReportGenerator exceptionReportGenerator)
            : base(webElement)
        {
            _exceptionReportGenerator = exceptionReportGenerator;
        }

        public override void Clear()
        {
            CallBaseMethod(() => base.Clear());
        }

        public override void Click()
        {
            CallBaseMethod(() => base.Click());
        }

        public override void Submit()
        {
            CallBaseMethod(() => base.Submit());
        }

        public override IWebElement FindElement(By by)
        {
            return CallBaseMethod(() => Wrap(base.FindElement(by), _exceptionReportGenerator));
        }

        public override ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return CallBaseMethod(() =>
                base.FindElements(by)
                    .Select(element => Wrap(element, _exceptionReportGenerator))
                    .ToList()
                    .AsReadOnly()
            );
        }

        public override string GetAttribute(string attributeName)
        {
            return CallBaseMethod<string>(() => base.GetAttribute(attributeName));
        }

        public override string GetProperty(string propertyName)
        {
            return CallBaseMethod<string>(() => base.GetProperty(propertyName));
        }

        public override void SendKeys(string text)
        {
            CallBaseMethod(() => base.SendKeys(text));
        }

        public override string GetCssValue(string propertyName)
        {
            return CallBaseMethod<string>(() => base.GetCssValue(propertyName));
        }

        private void CallBaseMethod(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                _exceptionReportGenerator.DocumentException(e);
                throw;
            }
        }

        private T CallBaseMethod<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                _exceptionReportGenerator.DocumentException(e);
                throw;
            }
        }

        private static IWebElement Wrap(IWebElement element, IExceptionReportGenerator exceptionReportGenerator)
        {
            return new ExceptionReportingWebElement(element, exceptionReportGenerator);
        }
    }
}
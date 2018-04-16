using System.Collections.ObjectModel;
using System.Drawing;
using OpenQA.Selenium;

namespace Sonneville.Investing.Fidelity.WebDriver.Logging
{
    public abstract class WebElementBase : IWebElement
    {
        private readonly IWebElement _webElement;

        protected WebElementBase(IWebElement webElement)
        {
            _webElement = webElement;
        }

        public virtual IWebElement FindElement(By by)
        {
            return _webElement.FindElement(by);
        }

        public virtual ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return _webElement.FindElements(by);
        }

        public virtual void Clear()
        {
            _webElement.Clear();
        }

        public virtual void SendKeys(string text)
        {
            _webElement.SendKeys(text);
        }

        public virtual void Submit()
        {
            _webElement.Submit();
        }

        public virtual void Click()
        {
            _webElement.Click();
        }

        public virtual string GetAttribute(string attributeName)
        {
            return _webElement.GetAttribute(attributeName);
        }

        public virtual string GetProperty(string propertyName)
        {
            return _webElement.GetProperty(propertyName);
        }

        public virtual string GetCssValue(string propertyName)
        {
            return _webElement.GetCssValue(propertyName);
        }

        public virtual string TagName => _webElement.TagName;
        public virtual string Text => _webElement.Text;
        public virtual bool Enabled => _webElement.Enabled;
        public virtual bool Selected => _webElement.Selected;
        public virtual Point Location => _webElement.Location;
        public virtual Size Size => _webElement.Size;
        public virtual bool Displayed => _webElement.Displayed;
    }
}
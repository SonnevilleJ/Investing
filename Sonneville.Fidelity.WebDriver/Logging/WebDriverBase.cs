using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace Sonneville.Fidelity.WebDriver.Logging
{
    public abstract class WebDriverBase : IWebDriver
    {
        private readonly IWebDriver _webDriver;

        protected WebDriverBase(IWebDriver webDriver)
        {
            _webDriver = webDriver;
        }

        public virtual string Url
        {
            get => _webDriver.Url;
            set => _webDriver.Url = value;
        }

        public virtual string Title => _webDriver.Title;
        public virtual string PageSource => _webDriver.PageSource;
        public virtual string CurrentWindowHandle => _webDriver.CurrentWindowHandle;
        public virtual ReadOnlyCollection<string> WindowHandles => _webDriver.WindowHandles;

        public virtual IWebElement FindElement(By by)
        {
            return _webDriver.FindElement(@by);
        }

        public virtual ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return _webDriver.FindElements(@by);
        }

        public virtual void Dispose()
        {
            _webDriver?.Dispose();
        }

        public virtual void Close()
        {
            _webDriver.Close();
        }

        public virtual void Quit()
        {
            _webDriver.Quit();
        }

        public virtual IOptions Manage()
        {
            return _webDriver.Manage();
        }

        public virtual INavigation Navigate()
        {
            return _webDriver.Navigate();
        }

        public virtual ITargetLocator SwitchTo()
        {
            return _webDriver.SwitchTo();
        }
    }
}

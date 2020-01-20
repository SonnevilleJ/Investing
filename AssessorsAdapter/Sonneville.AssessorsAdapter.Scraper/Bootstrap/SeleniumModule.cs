using System.IO;
using System.Reflection;
using Ninject.Modules;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Sonneville.AssessorsAdapter.Scraper.Bootstrap
{
    public class SeleniumModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IWebDriver>().ToMethod(context => CreateWebDriver()).InSingletonScope();
        }

        private static ChromeDriver CreateWebDriver()
        {
            var chromeOptions = new ChromeOptions();
            var chromeDriverDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return new ChromeDriver(chromeDriverDirectory, chromeOptions);
        }
    }
}

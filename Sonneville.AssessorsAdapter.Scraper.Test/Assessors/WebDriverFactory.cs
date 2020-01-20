using System.IO;
using System.Reflection;
using OpenQA.Selenium.Chrome;

namespace Sonneville.AssessorsAdapter.Scraper.Test.Assessors
{
    public static class WebDriverFactory
    {
        public static ChromeDriver CreateChromeDriver()
        {
            var chromeOptions = new ChromeOptions();
            var chromeDriverDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return new ChromeDriver(chromeDriverDirectory, chromeOptions);
        }
    }
}

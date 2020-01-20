using System;

namespace Sonneville.Selenium.Config
{
    public class SeleniumConfiguration
    {
        public TimeSpan WebElementDisplayTimeout { get; set; } = TimeSpan.FromMinutes(1);
    }
}
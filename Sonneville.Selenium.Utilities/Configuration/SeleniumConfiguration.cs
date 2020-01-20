using System;

namespace Sonneville.Selenium.Utilities.Configuration
{
    public class SeleniumConfiguration
    {
        public TimeSpan WebElementDisplayTimeout { get; set; } = TimeSpan.FromMinutes(1);
    }
}
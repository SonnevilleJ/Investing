using log4net;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.Fidelity.Shell.Logging;
using Sonneville.Utilities.Sleepers;

namespace Sonneville.Fidelity.Shell.Test.Logging
{
    [TestFixture]
    public class LoggingWebElementTests
    {
        [Test]
        public void ShouldPauseAfterClick()
        {
            var webElementMock = new Mock<IWebElement>();
            var sleeperMock = new Mock<ISleepUtil>();
            sleeperMock.Setup(sleeper => sleeper.Sleep(It.IsAny<int>())).Callback<int>(milliseconds => webElementMock.Verify(webElement => webElement.Click()));
            var loggingWebElement = new LoggingWebElement(webElementMock.Object, sleeperMock.Object, new Mock<ILog>().Object);
            
            loggingWebElement.Click();
            sleeperMock.Verify(sleeper => sleeper.Sleep(It.IsAny<int>()));
        }
    }
}

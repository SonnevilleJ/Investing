using Moq;
using Ninject;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.Fidelity.Shell.AppStartup.Ninject;
using Sonneville.Fidelity.Shell.Interface;

namespace Sonneville.Fidelity.Shell.Test.AppStartup
{
    [TestFixture]
    public class MutableKernelTests
    {
        private static Mock<IWebDriver> _webDriverMock;

        private static IKernel _kernel;

        [SetUp]
        public void Setup()
        {
            // mock out web driver because these tests focus on Ninject bindings, not Selenium
            _webDriverMock = new Mock<IWebDriver>();

            _kernel = new KernelBuilder().Build();
            _kernel.Rebind<IWebDriver>().ToConstant(_webDriverMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _kernel?.Dispose();
        }

        [Test]
        public void ShouldDisposeWebDriverExactlyOnce()
        {
            _kernel.Get<ICommandRouter>().Dispose();

            _webDriverMock.Verify(webDriver => webDriver.Dispose(), Times.Once());
        }
    }
}

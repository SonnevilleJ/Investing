using System.Linq;
using Moq;
using Ninject;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.Fidelity.Shell.AppStartup;
using Sonneville.Fidelity.Shell.Interface;
using Sonneville.FidelityWebDriver;
using Sonneville.FidelityWebDriver.Navigation;
using Sonneville.Utilities.Configuration;

namespace Sonneville.Fidelity.Shell.Test.AppStartup
{
    [TestFixture]
    public class ImmutableKernelTests
    {
        private static Mock<IWebDriver> _webDriverMock;

        private static IKernel _kernel;

        [OneTimeSetUp]
        public void Setup()
        {
            // mock out web driver because these tests focus on Ninject bindings, not Selenium
            _webDriverMock = new Mock<IWebDriver>();

            _kernel = new KernelBuilder().Build();
            _kernel.Rebind<IWebDriver>().ToConstant(_webDriverMock.Object);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _kernel?.Dispose();
        }

        [Test]
        public void ShouldBindConfigStoreAsSingleton()
        {
            var configStore = _kernel.Get<INiniConfigStore>();

            Assert.IsNotNull(configStore);
            Assert.AreSame(configStore, _kernel.Get<INiniConfigStore>());
        }

        [Test]
        public void ShouldBindApp()
        {
            var commandRouter = _kernel.Get<ICommandRouter>();

            Assert.IsNotNull(commandRouter);
        }

        [Test]
        public void ShouldBindCommands()
        {
            var commands = _kernel.GetAll<ICommand>().ToList();

            Assert.IsNotEmpty(commands);
            CollectionAssert.AllItemsAreNotNull(commands);
        }

        [Test]
        public void ShouldBindWebDriverAsSingleton()
        {
            var webDriver = _kernel.Get<IWebDriver>();

            Assert.IsNotNull(webDriver);
            Assert.AreSame(webDriver, _kernel.Get<IWebDriver>());
        }

        [Test]
        public void ShouldGetAllPages()
        {
            var pages = _kernel.GetAll<IPage>().ToList();

            Assert.IsNotEmpty(pages);
            CollectionAssert.AllItemsAreNotNull(pages);
        }

        [Test]
        public void ShouldGetSiteNavigator()
        {
            using (var siteNavigator = _kernel.Get<ISiteNavigator>())
            {
                Assert.IsNotNull(siteNavigator);
            }
        }
    }
}

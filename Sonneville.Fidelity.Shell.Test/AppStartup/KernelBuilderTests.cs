using Ninject;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.Fidelity.Shell.AppStartup;
using Sonneville.Utilities.Configuration;

namespace Sonneville.Fidelity.Shell.Test.AppStartup
{
    [TestFixture]
    public class KernelBuilderTests
    {
        private IKernel _kernel;

        [SetUp]
        public void Setup()
        {
            _kernel = new KernelBuilder().Build();
        }

        [TearDown]
        public void Teardown()
        {
            _kernel.Dispose();
        }

        [Test]
        public void ShouldBindApp()
        {
            var commandRouter = _kernel.Get<ICommandRouter>();

            Assert.IsNotNull(commandRouter);
        }

        [Test]
        public void ShouldBindWebDriverAsSingleton()
        {
            var webDriver = _kernel.Get<IWebDriver>();

            Assert.IsNotNull(webDriver);

            Assert.AreSame(webDriver, _kernel.Get<IWebDriver>());
        }

        [Test]
        public void ShouldBindConfigStoreAsSingleton()
        {
            var configStore = _kernel.Get<INiniConfigStore>();

            Assert.IsNotNull(configStore);
            Assert.AreSame(configStore, _kernel.Get<INiniConfigStore>());
        }
    }
}
using Ninject;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Configuration;
using Sonneville.Fidelity.Shell.AppStartup;
using Sonneville.Fidelity.Shell.Configuration;

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
        public void ShouldBindFidelityConfigurationAsSingleton()
        {
            var configuration = _kernel.Get<FidelityConfiguration>();

            Assert.IsNotNull(configuration);
            Assert.AreSame(configuration, _kernel.Get<FidelityConfiguration>());
            Assert.IsNotNull(configuration.Provider);
        }

        [Test]
        public void ShouldBindPortfolioManagerConfigurationAsSingleton()
        {
            var configuration = _kernel.Get<PortfolioManagerConfiguration>();

            Assert.IsNotNull(configuration);
            Assert.AreSame(configuration, _kernel.Get<PortfolioManagerConfiguration>());
            Assert.IsNotNull(configuration.Provider);
        }
    }
}
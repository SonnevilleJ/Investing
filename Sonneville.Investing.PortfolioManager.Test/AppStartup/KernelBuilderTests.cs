using Ninject;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Configuration;
using Sonneville.Investing.PortfolioManager.AppStartup;
using Sonneville.Investing.PortfolioManager.Configuration;

namespace Sonneville.Investing.PortfolioManager.Test.AppStartup
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
            var app = _kernel.Get<IApp>();

            Assert.IsNotNull(app);
        }

        [Test]
        public void ShouldBindWebDriverAsSingleton()
        {
            var app = _kernel.Get<IWebDriver>();

            Assert.IsNotNull(app);

            Assert.AreSame(app, _kernel.Get<IWebDriver>());
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
using Ninject;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Sonneville.Investing.PortfolioManager.Test
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

    }
}
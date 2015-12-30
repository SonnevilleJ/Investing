using Moq;
using Ninject;
using NUnit.Framework;

namespace Sonneville.Investing.PortfolioManager.Test
{
    [TestFixture]
    public class BootstrapperTests
    {
        private IKernel _kernel;

        [SetUp]
        public void Setup()
        {
            _kernel = Bootstrapper.Kernel;
        }

        [TearDown]
        public void Teardown()
        {
            _kernel.Dispose();
        }

        [Test]
        public void ShouldPassArgumentsToAppAndDispose()
        {
            var appMock = new Mock<IApp>();
            _kernel.Rebind<IApp>().ToConstant(appMock.Object);

            var cliArgs = new[] {"1", "2", "3"};
            Bootstrapper.Main(cliArgs);

            appMock.Verify(app => app.Run(cliArgs), Times.Once());
            appMock.Verify(app => app.Dispose());
            Assert.IsTrue(_kernel.IsDisposed);
        }
    }
}
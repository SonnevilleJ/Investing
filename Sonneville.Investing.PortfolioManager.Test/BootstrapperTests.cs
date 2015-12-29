using Moq;
using Ninject;
using NUnit.Framework;

namespace Sonneville.Investing.PortfolioManager.Test
{
    [TestFixture]
    public class BootstrapperTests
    {
        [Test]
        public void ShouldBindApp()
        {
            var app = Bootstrapper.Kernel.Get<IApp>();

            Assert.IsNotNull(app);
        }

        [Test]
        public void ShouldPassArgumentsToAppAndDispose()
        {
            var appMock = new Mock<IApp>();
            Bootstrapper.Kernel.Rebind<IApp>().ToConstant(appMock.Object);

            var cliArgs = new[] {"1", "2", "3"};
            Bootstrapper.Main(cliArgs);

            appMock.Verify(app => app.Run(cliArgs), Times.Once());
            appMock.Verify(app => app.Dispose(), Times.Once());
        }
    }
}
using Moq;
using Ninject;
using NUnit.Framework;
using Sonneville.AssessorsAdapter.Scraper.Bootstrap;

namespace Sonneville.AssessorsAdapter.Scraper.Test.Bootstrap
{
    [TestFixture]
    public class BootstrapperTests
    {
        [SetUp]
        public void Setup()
        {
            Bootstrapper.InitializeKernel();
            _kernel = Bootstrapper.Kernel;
        }

        [TearDown]
        public void Teardown()
        {
            _kernel.Dispose();
        }

        private IKernel _kernel;

        private Mock<IApp> RebindApp()
        {
            _kernel.Unbind<IApp>();
            var mockApp = new Mock<IApp>();
            _kernel.Bind<IApp>().ToConstant(mockApp.Object);
            return mockApp;
        }

        [Test]
        public void ShouldBindIApp()
        {
            Assert.IsNotNull(_kernel.Get<IApp>());
        }

        [Test]
        public void ShouldPassArgumentsToAppAndDispose()
        {
            var mockApp = RebindApp();

            var cliArgs = new[] {"1", "2", "3"};
            Bootstrapper.Main(cliArgs);

            mockApp.Verify(app => app.Run(cliArgs), Times.Once());
            mockApp.Verify(app => app.Dispose());
            Assert.IsTrue(Bootstrapper.Kernel.IsDisposed);
        }
    }
}

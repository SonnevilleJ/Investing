using System;
using Moq;
using Ninject;
using NUnit.Framework;
using Sonneville.Investing.WebApi.AppStartup;

namespace Sonneville.Investing.WebApi.Test.AppStartup
{
    [TestFixture]
    public class BootstrapperTests
    {
        private IKernel _kernel;

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

        [Test]
        public void ShouldPassArgumentsToAppAndDispose()
        {
            var mockApiServer = RebindApiServer();

            var cliArgs = new[] {"1", "2", "3"};
            Bootstrapper.Main(cliArgs);

            mockApiServer.Verify(app => app.RunAsync(cliArgs), Times.Once());
            mockApiServer.Verify(app => app.Dispose());
            Assert.IsTrue(Bootstrapper.Kernel.IsDisposed);
        }

        [Test]
        public void ShouldBindIApiServer()
        {
            Assert.IsNotNull(_kernel.Get<IApiServer>());
        }

        [Test]
        public void ShouldPrintStackTrace()
        {
            var mockApiServer = RebindApiServer();
            mockApiServer.Setup(server => server.RunAsync(It.IsAny<string[]>()))
                .Throws<NotSupportedException>();

            Assert.Throws<NotSupportedException>(() => Bootstrapper.Main(null));
        }

        private Mock<IApiServer> RebindApiServer()
        {
            _kernel.Unbind<IApiServer>();
            var mockApiServer = new Mock<IApiServer>();
            _kernel.Bind<IApiServer>().ToConstant(mockApiServer.Object);
            return mockApiServer;
        }
    }
}
using Moq;
using Ninject;
using NUnit.Framework;
using Sonneville.Fidelity.Shell.AppStartup;
using Sonneville.Fidelity.Shell.Interface;

namespace Sonneville.Fidelity.Shell.Test.AppStartup
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
            var commandRouterMock = new Mock<ICommandRouter>();
            _kernel.Rebind<ICommandRouter>().ToConstant(commandRouterMock.Object);

            var cliArgs = new[] {"1", "2", "3"};
            Bootstrapper.Main(cliArgs);

            commandRouterMock.Verify(app => app.Run(cliArgs), Times.Once());
            commandRouterMock.Verify(app => app.Dispose());
            Assert.IsTrue(_kernel.IsDisposed);
        }
    }
}
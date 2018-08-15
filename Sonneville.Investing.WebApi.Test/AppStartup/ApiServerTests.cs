using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Moq;
using NUnit.Framework;
using Sonneville.Investing.WebApi.AppStartup;

namespace Sonneville.Investing.WebApi.Test.AppStartup
{
    [TestFixture]
    public class ApiServerTests
    {
        private IPEndPoint _ipEndPoint;
        private Mock<IWebHost> _mockWebHost;
        private Mock<IWebHostFactory> _mockWebHostFactory;
        private CancellationTokenSource _cancellationTokenSource;
        private IApiServer _apiServer;
        private Task _task;

        [SetUp]
        public void Setup()
        {
            _ipEndPoint = new IPEndPoint(IPAddress.Loopback, 5000);

            _mockWebHost = new Mock<IWebHost>();

            _mockWebHostFactory = new Mock<IWebHostFactory>();
            _mockWebHostFactory.Setup(factory => factory.CreateWebHost(_ipEndPoint))
                .Returns(_mockWebHost.Object);

            _cancellationTokenSource = new CancellationTokenSource();

            _apiServer = new ApiServer(_mockWebHostFactory.Object, _ipEndPoint);
        }

        [TearDown]
        public void Teardown()
        {
            StopServer();
            WaitForServerToFinish();
            _task?.Dispose();
        }

        [Test]
        public void ShouldSetRunningAfterStartingServer()
        {
            _mockWebHost.Setup(webHost => webHost.Start())
                .Callback(() => Assert.IsFalse(_apiServer.Running));

            StartServer();

            Assert.IsTrue(_apiServer.Running);
        }

        [Test]
        public void ShouldStopServerWhenCancelled()
        {
            _mockWebHost.Setup(webHost => webHost.StopAsync(default(CancellationToken)))
                .Returns(() => Task.Run(() => { }))
                .Callback(() => Assert.IsTrue(_apiServer.Running));
            StartServer();

            StopServer();
            WaitForServerToFinish();

            Assert.IsFalse(_apiServer.Running);
        }

        private void StartServer()
        {
            _task = _apiServer.RunAsync(null, _cancellationTokenSource.Token);
        }

        private void StopServer()
        {
            _cancellationTokenSource.Cancel();
        }

        private void WaitForServerToFinish()
        {
            _task?.Wait();
        }
    }
}
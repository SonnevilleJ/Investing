using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Sonneville.Investing.WebApi.AppStartup;

namespace Sonneville.Investing.WebApi.Test.AppStartup
{
    [TestFixture]
    public class ApiIntegrationTests
    {
        private CancellationTokenSource _cancellationTokenSource;
        private ApiServer _apiServer;
        private Task _serverTask;
        private IPEndPoint _ipEndPoint;

        [SetUp]
        public void Setup()
        {
            _ipEndPoint = new IPEndPoint(IPAddress.Loopback, 5000);

            _cancellationTokenSource = new CancellationTokenSource();
            _apiServer = new ApiServer(_ipEndPoint);
        }

        [TearDown]
        public void Teardown()
        {
            if (_serverTask != null) StopServer();
            _serverTask?.Dispose();
            _apiServer?.Dispose();
        }

        [Test]
        public void ShouldListenOnDesignatedPort()
        {
            var listeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
            CollectionAssert.DoesNotContain(listeners, _ipEndPoint);
            
            StartServer();

            listeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
            CollectionAssert.Contains(listeners, _ipEndPoint);
        }

        [Test]
        public void ShouldSetStatusProperties()
        {
            Assert.IsFalse(_apiServer.Running);
            Assert.AreEqual(_ipEndPoint, _apiServer.IpEndPoint);

            StartServer();
            Assert.IsTrue(_apiServer.Running);
            Assert.AreEqual(_ipEndPoint, _apiServer.IpEndPoint);

            StopServer();
            Assert.IsFalse(_apiServer.Running);
            Assert.AreEqual(TaskStatus.RanToCompletion, _serverTask.Status);
            Assert.AreEqual(_ipEndPoint, _apiServer.IpEndPoint);
        }

        [Test]
        public async Task ShouldRespondToRequests()
        {
            StartServer();

            var response = await new HttpClient().GetAsync($"http://{_apiServer.IpEndPoint}/");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        private void StartServer()
        {
            _serverTask = _apiServer.RunAsync(null, _cancellationTokenSource.Token);
        }

        private void StopServer()
        {
            _cancellationTokenSource.Cancel();
            _serverTask.Wait();
        }
    }
}
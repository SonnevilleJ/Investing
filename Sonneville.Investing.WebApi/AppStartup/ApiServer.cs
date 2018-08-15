using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Sonneville.Investing.WebApi.AppStartup
{
    public interface IApiServer : IDisposable
    {
        IPEndPoint IpEndPoint { get; }
        bool Running { get; }
        Task RunAsync(string[] args);
        Task RunAsync(string[] args, CancellationToken cancellationToken);
    }

    public class ApiServer : IApiServer
    {
        private readonly IWebHostFactory _webHostFactory;
        public IPEndPoint IpEndPoint { get; }
        public bool Running { get; private set; }

        public ApiServer(IWebHostFactory webHostFactory, IPEndPoint ipEndPoint)
        {
            _webHostFactory = webHostFactory;
            IpEndPoint = ipEndPoint;
        }

        public async Task RunAsync(string[] args)
        {
            await RunAsync(args, CancellationToken.None);
        }

        public async Task RunAsync(string[] args, CancellationToken cancellationToken)
        {
            using (var webHost = StartServer())
            {
                while (true)
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                    }
                    catch (TaskCanceledException)
                    {
                        await StopServer(webHost);
                        break;
                    }
                }
            }
        }

        public void Dispose()
        {
        }

        private IWebHost StartServer()
        {
            var webHost = _webHostFactory.CreateWebHost(IpEndPoint);
            webHost.Start();
            Running = true;
            return webHost;
        }

        private async Task StopServer(IWebHost webHost)
        {
            await webHost.StopAsync();
            Running = false;
        }
    }
}
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Sonneville.Investing.WebApi.AppStartup
{
    public interface IApiServer : IDisposable
    {
        Task RunAsync(string[] args);
        Task RunAsync(string[] args, CancellationToken cancellationToken);
    }

    public class ApiServer : IApiServer
    {
        public IPEndPoint IpEndPoint { get; private set; }
        public bool Running { get; private set; }

        public ApiServer(IPEndPoint ipEndPoint)
        {
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
                webHost.Start();

                while (true)
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine("Cancellation detected...");
                        break;
                    }
                }

                await StopServer(webHost);
            }
        }

        public void Dispose()
        {
        }

        private IWebHost StartServer()
        {
            var webHost = new WebHostBuilder()
                .UseStartup<MvcStartup>()
                .UseKestrel(options => options.Listen(IpEndPoint))
                .Build();
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
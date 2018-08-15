using System.Net;
using Microsoft.AspNetCore.Hosting;

namespace Sonneville.Investing.WebApi.AppStartup
{
    public interface IWebHostFactory
    {
        IWebHost CreateWebHost(IPEndPoint ipEndPoint);
    }

    public class WebHostFactory : IWebHostFactory
    {
        public IWebHost CreateWebHost(IPEndPoint ipEndPoint)
        {
            return new WebHostBuilder()
                .UseStartup<MvcStartup>()
                .UseKestrel(options => options.Listen(ipEndPoint))
                .Build();
        }
    }
}
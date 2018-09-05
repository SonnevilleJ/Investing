using System.Net;
using Ninject.Modules;

namespace Sonneville.Investing.WebApi.AppStartup.Ninject
{
    public class AppModule : NinjectModule
    {
        public override void Load()
        {
            var ipEndPoint = new IPEndPoint(IPAddress.Any, 5000);
            Bind<IPEndPoint>().ToConstant(ipEndPoint);
        }
    }
}

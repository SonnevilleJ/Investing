using System.Net;
using Ninject.Extensions.Conventions;
using Ninject.Modules;

namespace Sonneville.Investing.WebApi.AppStartup.NinjectModules
{
    public class AppModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(syntax => syntax.FromAssembliesMatching("Sonneville.*")
                .SelectAllClasses()
                .BindAllInterfaces()
                .Configure(configurationAction => configurationAction.InSingletonScope()));
            // BindAllInterfaces()      - binds to all implemented interfaces
            // BindDefaultInterface()   - class name must match interface name (i.e. Class to IClass)
            // BindDefaultInterfaces()  - class name must include interface name (i.e. MyClass to IClass)
            // BindSingleInterface()    - class must implement only one interface, no multiple inheritance

            var ipEndPoint = new IPEndPoint(IPAddress.Any, 5000);
            Bind<IPEndPoint>().ToConstant(ipEndPoint);
        }
    }
}

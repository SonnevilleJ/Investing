using Ninject.Extensions.Conventions;
using Ninject.Modules;

namespace Sonneville.Investing.App.Ninject
{
    public class DefaultModule : NinjectModule
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
        }
    }
}
using Ninject;
using Ninject.Modules;
using Sonneville.Investing.App.Ninject;
using Sonneville.Ninject;

namespace Sonneville.Fidelity.Shell.AppStartup.Ninject
{
    public class KernelBuilder
    {
        private readonly INinjectModule[] _modules =
        {
            new DefaultModule(),
            new AppModule(),
            new ConfigModule(),
            new SeleniumModule(),
            new EfCoreModule(),
        };

        public IKernel Build()
        {
            return new StandardKernel(_modules);
        }
    }
}

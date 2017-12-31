using Ninject;
using Ninject.Modules;
using Sonneville.Fidelity.Shell.AppStartup.NinjectModules;

namespace Sonneville.Fidelity.Shell.AppStartup
{
    public class KernelBuilder
    {
        private readonly INinjectModule[] _modules =
        {
            new AppModule(),
            new ConfigModule(),
            new LoggingModule(),
            new SeleniumModule(),
        };

        public IKernel Build()
        {
            return new StandardKernel(_modules);
        }
    }
}

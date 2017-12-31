using Ninject;
using Sonneville.Fidelity.Shell.AppStartup.NinjectModules;

namespace Sonneville.Fidelity.Shell.AppStartup
{
    public class KernelBuilder
    {
        public IKernel Build()
        {
            return new StandardKernel(
                new AppModule(),
                new SeleniumModule(),
                new LoggingModule()
            );
        }
    }
}

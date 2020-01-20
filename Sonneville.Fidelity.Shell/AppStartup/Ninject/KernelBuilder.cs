using Ninject;
using Ninject.Modules;
using Sonneville.Investing.App.Ninject;
using Sonneville.log4net.Ninject;
using Sonneville.Ninject;
using Sonneville.Selenium.log4net;
using Sonneville.Selenium.Ninject;

namespace Sonneville.Fidelity.Shell.AppStartup.Ninject
{
    public class KernelBuilder
    {
        private readonly INinjectModule[] _modules =
        {
            new DefaultModule(),
            new AppModule(),
            new LoggingModule(),
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
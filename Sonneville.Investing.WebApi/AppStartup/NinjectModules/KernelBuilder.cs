using Ninject;
using Sonneville.Investing.App.NinjectModules;

namespace Sonneville.Investing.WebApi.AppStartup.NinjectModules
{
    public class KernelBuilder
    {
        public IKernel Build()
        {
            return new StandardKernel(
                new DefaultModule(),
                new AppModule());
        }
    }
}
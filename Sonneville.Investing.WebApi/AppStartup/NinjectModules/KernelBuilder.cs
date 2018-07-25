using Ninject;

namespace Sonneville.Investing.WebApi.AppStartup.NinjectModules
{
    public class KernelBuilder
    {
        public IKernel Build()
        {
            return new StandardKernel(
                new AppModule());
        }
    }
}
using Ninject;
using Sonneville.Investing.App.Ninject;

namespace Sonneville.Investing.WebApi.AppStartup.Ninject
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
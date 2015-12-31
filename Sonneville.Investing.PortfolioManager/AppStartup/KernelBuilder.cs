using Ninject;

namespace Sonneville.Investing.PortfolioManager.AppStartup
{
    public class KernelBuilder
    {
        public IKernel Build()
        {
            return new StandardKernel(new AppModule());
        }
    }
}
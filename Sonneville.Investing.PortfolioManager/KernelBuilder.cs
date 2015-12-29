using Ninject;

namespace Sonneville.Investing.PortfolioManager
{
    public class KernelBuilder
    {
        public IKernel Build()
        {
            return new StandardKernel(new AppModule());
        }
    }
}
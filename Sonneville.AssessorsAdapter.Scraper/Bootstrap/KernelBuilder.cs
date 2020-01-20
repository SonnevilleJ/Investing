using Ninject;
using Sonneville.Ninject;

namespace Sonneville.AssessorsAdapter.Scraper.Bootstrap
{
    public class KernelBuilder
    {
        public IKernel Build()
        {
            return new StandardKernel(
                new DefaultModule(),
                new SeleniumModule());
        }
    }
}
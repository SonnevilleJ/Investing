using Ninject;
using Sonneville.log4net.Ninject;
using Sonneville.Ninject;
using Sonneville.Selenium.Ninject;

namespace Sonneville.AssessorsAdapter.Scraper.Bootstrap
{
    public class KernelBuilder
    {
        public IKernel Build()
        {
            return new StandardKernel(
                new DefaultModule(),
                new LoggingModule(),
                new SeleniumModule());
        }
    }
}
using Ninject;

namespace Sonneville.AssessorsAdapter.Scraper.Bootstrap
{
    public class KernelBuilder
    {
        public IKernel Build()
        {
            return new StandardKernel(
                new DefaultModule(),
                new SeleniumModule(),
                new CsvModule());
        }
    }
}
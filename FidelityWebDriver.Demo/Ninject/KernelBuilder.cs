using Ninject;

namespace Sonneville.FidelityWebDriver.Demo.Ninject
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
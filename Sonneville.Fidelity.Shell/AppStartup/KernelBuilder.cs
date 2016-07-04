using Ninject;

namespace Sonneville.Fidelity.Shell.AppStartup
{
    public class KernelBuilder
    {
        public IKernel Build()
        {
            return new StandardKernel(new AppModule());
        }
    }
}
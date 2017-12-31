using Ninject;
using Sonneville.FidelityWebDriver.Demo.Ninject;

namespace Sonneville.FidelityWebDriver.Demo
{
    public static class Program
    {
        public static readonly IKernel Kernel = new KernelBuilder().Build();

        public static void Main(string[] args)
        {
            using (var app = Kernel.Get<IApp>())
            {
                app.Run(args);
            }
        }
    }
}

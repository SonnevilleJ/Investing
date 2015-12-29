using Ninject;

namespace Sonneville.Investing.PortfolioManager
{
    public class Bootstrapper
    {
        public static IKernel Kernel { get; }

        static Bootstrapper()
        {
            Kernel = new KernelBuilder().Build();
        }

        public static void Main(string[] args)
        {
            using (var app = Kernel.Get<IApp>())
            {
                app.Run(args);
            }
        }
    }
}
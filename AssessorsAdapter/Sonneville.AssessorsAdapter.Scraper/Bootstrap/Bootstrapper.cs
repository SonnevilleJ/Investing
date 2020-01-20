using Ninject;

namespace Sonneville.AssessorsAdapter.Scraper.Bootstrap
{
    public static class Bootstrapper
    {
        static Bootstrapper()
        {
            InitializeKernel();
        }

        public static IKernel Kernel { get; private set; }

        public static void InitializeKernel()
        {
            Kernel?.Dispose();
            Kernel = new KernelBuilder().Build();
        }

        public static void Main(string[] args)
        {
            using (Kernel)
            using (var app = Kernel.Get<IApp>())
            {
                app.Run(args);
            }
        }
    }
}

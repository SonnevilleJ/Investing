using Ninject;
using Sonneville.Investing.WebApi.AppStartup.NinjectModules;

namespace Sonneville.Investing.WebApi.AppStartup
{
    public static class Bootstrapper
    {
        static Bootstrapper()
        {
            InitializeKernel();
        }

        public static void InitializeKernel()
        {
            Kernel = new KernelBuilder().Build();
        }

        public static IKernel Kernel { get; private set; }

        public static void Main(string[] args)
        {
            using (Kernel)
            using (var apiServer = Kernel.Get<IApiServer>())
            {
                apiServer.RunAsync(args);
            }
        }
    }
}
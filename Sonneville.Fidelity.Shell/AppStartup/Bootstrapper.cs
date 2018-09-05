using Ninject;
using Sonneville.Fidelity.Shell.AppStartup.Ninject;
using Sonneville.Fidelity.Shell.Interface;

namespace Sonneville.Fidelity.Shell.AppStartup
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
            using (var commandRouter = Kernel.Get<ICommandRouter>())
            {
                commandRouter.Run(args);
            }
        }
    }
}
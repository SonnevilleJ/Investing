using Ninject;
using Sonneville.Fidelity.Shell.Interface;

namespace Sonneville.Fidelity.Shell.AppStartup
{
    public static class Bootstrapper
    {
        public static IKernel Kernel { get; } = new KernelBuilder().Build();

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

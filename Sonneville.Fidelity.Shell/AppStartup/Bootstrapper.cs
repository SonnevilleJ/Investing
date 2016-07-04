using Ninject;

namespace Sonneville.Fidelity.Shell.AppStartup
{
    public class Bootstrapper
    {
        public static IKernel Kernel { get; set; }

        static Bootstrapper()
        {
            Kernel = new KernelBuilder().Build();
        }

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
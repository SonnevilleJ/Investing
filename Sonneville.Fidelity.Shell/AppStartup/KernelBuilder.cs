﻿using Ninject;
using Ninject.Modules;
using Sonneville.Fidelity.Shell.AppStartup.NinjectModules;
using Sonneville.Investing.App.NinjectModules;

namespace Sonneville.Fidelity.Shell.AppStartup
{
    public class KernelBuilder
    {
        private readonly INinjectModule[] _modules =
        {
            new DefaultModule(),
            new AppModule(),
            new ConfigModule(),
            new SeleniumModule(),
            new EfCoreModule(),
        };

        public IKernel Build()
        {
            return new StandardKernel(_modules);
        }
    }
}

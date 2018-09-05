using System;
using System.IO;
using Ninject.Modules;
using Sonneville.Fidelity.Shell.Interface;

namespace Sonneville.Fidelity.Shell.AppStartup.Ninject
{
    public class AppModule : NinjectModule
    {
        public override void Load()
        {
            Bind<TextReader>().ToConstant(Console.In).WhenInjectedInto<ICommandRouter>();
            Bind<TextWriter>().ToConstant(Console.Out).WhenInjectedInto<ICommandRouter>();
        }
    }
}

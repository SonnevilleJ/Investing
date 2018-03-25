﻿using System;
using System.IO;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using Sonneville.Fidelity.Shell.Interface;

namespace Sonneville.Fidelity.Shell.AppStartup.NinjectModules
{
    public class AppModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(syntax => syntax.FromAssembliesMatching("Sonneville.*")
                .SelectAllClasses()
                .BindAllInterfaces()
                .Configure(configurationAction => configurationAction.InSingletonScope()));
            // BindAllInterfaces()      - binds to all implemented interfaces
            // BindDefaultInterface()   - class name must match interface name (i.e. Class to IClass)
            // BindDefaultInterfaces()  - class name must include interface name (i.e. MyClass to IClass)
            // BindSingleInterface()    - class must implement only one interface, no multiple inheritance

            Bind<TextReader>().ToConstant(Console.In).WhenInjectedInto<ICommandRouter>();
            Bind<TextWriter>().ToConstant(Console.Out).WhenInjectedInto<ICommandRouter>();
        }
    }
}

using System;
using Ninject.Modules;
using Sonneville.Investing.Persistence.EFCore.EntityFrameworkCore;

namespace Sonneville.Investing.App.NinjectModules
{
    public class EfCoreModule : NinjectModule
    {
        public override void Load()
        {
            Bind<DatabaseConnectionInfo>()
                .ToMethod((context) => throw new NotSupportedException("No database has been configured."));
        }
    }
}

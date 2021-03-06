using log4net;
using Ninject.Activation;

namespace Sonneville.Ninject
{
    public class LogProvider : Provider<ILog>
    {
        protected override ILog CreateInstance(IContext context)
        {
            return LogManager.GetLogger(context.Request.Target.Member.DeclaringType);
        }
    }
}

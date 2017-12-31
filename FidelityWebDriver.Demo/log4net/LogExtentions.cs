using System;
using System.Diagnostics;
using log4net;
using log4net.Core;

namespace Sonneville.FidelityWebDriver.Demo.log4net
{
    public static class LogExtentions
    {
        public static void Trace(this ILog log, string message, int skipFrames = 1, Exception exception = null)
        {
            var declaringType = new StackTrace(skipFrames + 1, false)
                .GetFrame(0)
                .GetMethod()
                .DeclaringType;
            LogManager.GetLogger(declaringType).Logger.Log(declaringType, Level.Trace, message, exception);
        }

        public static void Verbose(this ILog log, string message, int skipFrames = 1, Exception exception = null)
        {
            var declaringType = new StackTrace(skipFrames + 1, false)
                .GetFrame(0)
                .GetMethod()
                .DeclaringType;
            LogManager.GetLogger(declaringType).Logger.Log(declaringType, Level.Verbose, message, exception);
        }
    }
}

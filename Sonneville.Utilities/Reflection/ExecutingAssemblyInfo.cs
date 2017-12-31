using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Sonneville.Utilities.Reflection
{
    public static class ExecutingAssemblyInfo
    {
        public static string Company => GetAssemblyAttribute<AssemblyCompanyAttribute>(1).Company;

        public static string Copyright => GetAssemblyAttribute<AssemblyCopyrightAttribute>(1).Copyright;

        public static string Description => GetAssemblyAttribute<AssemblyDescriptionAttribute>(1).Description;

        public static string Product => GetAssemblyAttribute<AssemblyProductAttribute>(1).Product;

        public static string Title => GetAssemblyAttribute<AssemblyTitleAttribute>(1).Title;

        public static string Trademark => GetAssemblyAttribute<AssemblyTrademarkAttribute>(1).Trademark;

        public static T GetAssemblyAttribute<T>()
        {
            return GetAssemblyAttribute<T>(1);
        }

        private static T GetAssemblyAttribute<T>(int skipFrames)
        {
            return new StackTrace(skipFrames + 1, false)
                .GetFrame(0)
                .GetMethod()
                .DeclaringType
                .Assembly
                .GetCustomAttributes(typeof(T), false)
                .Cast<T>()
                .Single();
        }
    }
}

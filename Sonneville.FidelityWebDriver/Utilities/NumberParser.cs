using System.Globalization;
using log4net;

namespace Sonneville.FidelityWebDriver.Utilities
{
    public static class NumberParser
    {
        private static ILog _log;

        public static void SetLogger(ILog log = null)
        {
            _log = log ?? LogManager.GetLogger(typeof(NumberParser));
        }

        public static double ParseDouble(string input, NumberStyles numberStyles = NumberStyles.Any)
        {
            Log<double>(input, numberStyles);
            return double.Parse(input, numberStyles);
        }

        public static decimal ParseDecimal(string input, NumberStyles numberStyles = NumberStyles.Any)
        {
            Log<decimal>(input, numberStyles);
            return decimal.Parse(input, numberStyles);
        }

        public static int ParseInt(string input, NumberStyles numberStyles = NumberStyles.Any)
        {
            Log<int>(input, numberStyles);
            return int.Parse(input, numberStyles);
        }

        public static long ParseLong(string input, NumberStyles numberStyles = NumberStyles.Any)
        {
            Log<long>(input, numberStyles);
            return long.Parse(input, numberStyles);
        }

        private static void Log<T>(string input, NumberStyles numberStyles)
        {
            _log?.Debug($"Parsing '{input}' into a {typeof(T)} using NumberStyles.{numberStyles}...");
        }
    }
}

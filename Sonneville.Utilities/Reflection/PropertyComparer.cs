using System.Collections;
using System.Linq;
using System.Reflection;

namespace Sonneville.Utilities.Reflection
{
    public class PropertyComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x == null || y == null) return -1;

            var xType = x.GetType();
            var yType = y.GetType();
            if (xType != yType) return -1;

            return xType.IsValueType && yType.IsValueType
                ? CompareValueTypes(x, y)
                : CompareReferenceTypes(x, y);
        }

        private static int CompareValueTypes(object x, object y)
        {
            return x.Equals(y) ? 0 : -1;
        }

        private static int CompareReferenceTypes(object x, object y)
        {
            return x.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(propertyInfo => propertyInfo.GetIndexParameters().Length == 0)
                .All(propertyInfo => propertyInfo.GetValue(x).Equals(propertyInfo.GetValue(y)))
                ? 0
                : -1;
        }
    }
}

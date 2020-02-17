#if !CORE_LIB
using System;

namespace mulova.ui
{
    internal static class EnumUtil
    {
        public static T[] Values<T>() where T : struct, IComparable, IConvertible, IFormattable
        {
            return (T[])Enum.GetValues(typeof(T));
        }
    }
}
#endif
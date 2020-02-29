#if !CORE_LIB
using System;

namespace mulova.switcher
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
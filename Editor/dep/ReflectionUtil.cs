#if !CORE_LIB
using System;
using System.Collections.Generic;
using System.Reflection;

namespace mulova.switcher
{
    public static class ReflectionUtil
    {
        private static int assemblyCounts;
        private static Dictionary<string, Type> types;

        public static Type GetType(string fullName)
        {
            if (fullName.IsEmpty())
            {
                return null;
            }
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (assemblyCounts != assemblies.Length)
            {
                assemblyCounts = assemblies.Length;
                types = new Dictionary<string, Type>();
                foreach (Assembly assembly in assemblies)
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        types[type.FullName] = type;
                    }
                }
            }
            return types.Get(fullName);
        }
    }
}
#endif
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;
#if CORE_LIB
using System.Ex;
using System.Collections.Generic.Ex;
#endif

namespace mulova.ui
{
    internal class CompDataGenerator
    {
        private Dictionary<Type, Type> pool;

        public ICompData GetComponentData(Component c, Type type)
        {
            var dataType = FindDataType(type);
            if (dataType != null)
            {
                var o = Activator.CreateInstance(dataType) as ICompData;
                o.Collect(c);
                return o;
            }
            else
            {
                return null;
            }
        }

        public Type FindDataType(Type type)
        {
            // collect BuildProcessors
            if (pool == null)
            {
                pool = new Dictionary<Type, Type>();
                List<Type> cls = typeof(ICompData).FindTypes();
                foreach (Type t in cls)
                {
                    if (!t.IsAbstract)
                    {
                        var ins = Activator.CreateInstance(t) as ICompData;
                        pool[ins.type] = t;
                    }
                }
            }
            var dataType = pool.Get(type);
            var baseType = type.BaseType;
            while (dataType == null && (baseType != null && baseType != typeof(Object) && baseType != baseType.BaseType))
            {
                dataType = pool.Get(baseType);
                if (dataType != null)
                {
                    pool[type] = dataType;
                    break;
                }
                baseType = baseType.BaseType;
            }
            return dataType;
        }
    }
}
#if !CORE_LIB
using Object = UnityEngine.Object;

namespace mulova.ui
{
    internal interface NamedObj
    {
        Object Obj { get; }
        string Name { get; }
    }
}

#endif
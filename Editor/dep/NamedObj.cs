#if STANDALONE
using Object = UnityEngine.Object;

public interface NamedObj
{
    Object Obj { get; }
    string Name { get; }
}
#endif
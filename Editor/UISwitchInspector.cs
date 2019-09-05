using System.Collections.Generic;
using UnityEditor;
using comunity;

namespace uiswitch
{
    [CustomEditor(typeof(UISwitch))]
    public class UISwitchInspector : Editor
    {
        private UISwitch uiSwitch;
        internal static bool exclusive = true;


        void OnEnable()
        {
            uiSwitch = (UISwitch)target;
        }

        public override void OnInspectorGUI()
        {
#if AUTO_REMOVE
            RemoveUnused();
#endif
            DrawDefaultInspector();
        }

        public static bool[] GetVisibilityUnion(UISwitch uiSwitch)
        {
            bool[] union = new bool[uiSwitch.objs.Count];
            HashSet<List<bool>> set = new HashSet<List<bool>>();
            for (int i = 0; i < uiSwitch.switches.Count; ++i)
            {
                var s = uiSwitch.switches[i];
                if (set.Contains(s.visibility))
                {
                    uiSwitch.switches[i] = new UISwitchSect();
                    EditorUtil.SetDirty(uiSwitch);
                }
                set.Add(uiSwitch.switches[i].visibility);
                for (int j = 0; j < s.visibility.Count; ++j)
                {
                    union[j] |= s.visibility[j];
                }
            }
            return union;
        }

        private void RemoveUnused()
        {
            bool[] union = GetVisibilityUnion(uiSwitch);
            for (int i = union.Length - 1; i >= 0; --i)
            {
                if (!union[i])
                {
                    uiSwitch.objs.RemoveAt(i);
                    for (int j = 0; j < uiSwitch.switches.Count; ++j)
                    {
                        uiSwitch.switches[j].visibility.RemoveAt(i);
                    }
                    EditorUtil.SetDirty(uiSwitch);
                }
            }
        }
    }
}
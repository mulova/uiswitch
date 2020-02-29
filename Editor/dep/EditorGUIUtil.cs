#if !CORE_LIB
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace mulova.switcher
{
    internal class EditorGUIUtil
    {
        public static bool PopupNullable<T>(Rect bound, string label, ref T selection, IList<T> items)
        {
            T sel = selection;
            bool changed = PopupNullable<T>(bound, label, ref sel, items, ObjToString.ScenePathToString);
            selection = sel;
            return changed;
        }

        public static bool PopupNullable<T>(Rect bound, string label, ref T selection, IList<T> items, ToStr toString)
        {
            if (items.Count == 0)
            {
                bool changed = false;
                Color old = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (label == null)
                {
                    int i = EditorGUI.Popup(bound, 1, new string[] { "-", selection.ToString() });
                    if (i == 0)
                    {
                        selection = default(T);
                        changed = true;
                    }
                }
                else
                {
                    int i = EditorGUI.Popup(bound, label, 1, new string[] { "-", selection.ToString() });
                    if (i == 0)
                    {
                        selection = default(T);
                        changed = true;
                    }
                }
                GUI.backgroundColor = old;
                return changed;
            }

            int index = 0;
            string[] str = new string[items.Count + 1];
            str[0] = "-";
            for (int i = 1; i <= items.Count; i++)
            {
                str[i] = toString(items[i - 1]);
                if (object.Equals(items[i - 1], selection))
                {
                    index = i;
                }
            }
            int newIndex = 0;
            if (label == null)
            {
                newIndex = EditorGUI.Popup(bound, index, str);
            }
            else
            {
                newIndex = EditorGUI.Popup(bound, label, index, str);
            }
            if (newIndex == 0)
            {
                selection = default(T);
            }
            else
            {
                selection = items[newIndex - 1];
            }
            return newIndex != index;
        }
    }
}
#endif
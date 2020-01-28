#if STANDALONE
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace mulova.ui
{
    internal class EditorGUILayoutUtil
    {

        public static bool PopupNullable<T>(string label, ref T selection, IList<T> items, params GUILayoutOption[] options)
        {
            T sel = selection;
            bool changed = PopupNullable<T>(label, ref sel, items, ObjToString.ScenePathToString, options);
            selection = sel;
            return changed;
        }

        public static bool PopupNullable<T>(string label, ref T selection, IList<T> items, ToStr toString, params GUILayoutOption[] options)
        {
            if (items.Count == 0)
            {
                bool changed = false;
                Color old = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (label == null)
                {
                    int i = EditorGUILayout.Popup(1, new string[] { "-", selection.ToString() }, options);
                    if (i == 0)
                    {
                        selection = default(T);
                        changed = true;
                    }
                }
                else
                {
                    int i = EditorGUILayout.Popup(label, 1, new string[] { "-", selection.ToString() }, options);
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
                newIndex = EditorGUILayout.Popup(index, str, options);
            }
            else
            {
                newIndex = EditorGUILayout.Popup(label, index, str, options);
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

        public static bool TextField(string label, ref string str, params GUILayoutOption[] options)
        {
            return TextField(label, ref str, EditorStyles.textField, options);
        }

        public static bool TextField(string label, ref string str, GUIStyle style, params GUILayoutOption[] options)
        {
            if (str == null)
            {
                str = "";
            }
            string newStr = label == null ?
            EditorGUILayout.TextField(str, style, options) :
            EditorGUILayout.TextField(label, str, style, options);
            bool changed = newStr != str;
            str = newStr;
            return changed;
        }
    }
}

#endif
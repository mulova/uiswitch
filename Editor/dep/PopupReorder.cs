#if STANDALONE
using System;
using UnityEditor;
using UnityEngine;

namespace mulova.ui
{
    internal class PopupReorder : PropertyReorder<string>
    {
        public string[] options;
        public PopupReorder(SerializedProperty prop, string[] options) : base(prop)
        {
            this.options = options;
        }

        public PopupReorder(SerializedObject ser, string propPath, string[] options) : base(ser, propPath)
        {
            this.options = options;
        }

        protected override void DrawItem(SerializedProperty item, Rect rect, int index, bool isActive, bool isFocused)
        {
            string sel = item.stringValue;
            int i1 = Array.FindIndex(options, o => o == sel);
            var i2 = EditorGUI.Popup(rect, Math.Max(0, i1), options);
            if (i1 != i2)
            {
                item.stringValue = options[i2];
            }
        }
    }
}
#endif
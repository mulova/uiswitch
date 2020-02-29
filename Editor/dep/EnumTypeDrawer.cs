#if !CORE_LIB
using System;
using UnityEditor;
using UnityEngine;

namespace mulova.switcher
{
    [CustomPropertyDrawer(typeof(EnumTypeAttribute))]
    internal class EnumTypeDrawer : PropertyDrawer
    {
        private TypeSelector typeSelector;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (typeSelector == null)
            {
                typeSelector = new TypeSelector(typeof(Enum));
            }
            typeSelector.DrawSelector(position, property);
        }
    }
}
#endif
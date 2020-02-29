#if !CORE_LIB
using System;
using UnityEditor;
using UnityEngine;

namespace mulova.switcher
{
    [CustomPropertyDrawer(typeof(EnumPopupAttribute))]
    internal class EnumPopupDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnumPopupAttribute attr = attribute as EnumPopupAttribute;
            var typeVar = property.serializedObject.FindProperty(attr.enumTypeVar);
            if (typeVar != null)
            {
                var enumType = ReflectionUtil.GetType(typeVar.stringValue);
                bool isEnum = false;
                if (enumType != null)
                {
                    try
                    {
                        Enum e1 = (Enum)enumType.GetEnumValues().GetValue(0);
                        if (!property.stringValue.IsEmpty())
                        {
                            e1 = (Enum)Enum.Parse(enumType, property.stringValue, true);
                        }
                        Enum e2 = EditorGUI.EnumPopup(position, label, e1);
                        if (e1 != e2)
                        {
                            property.stringValue = e2.ToString();
                        }
                        isEnum = true;
                    }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
                    catch
                    {
                    }
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
                }
                if (!isEnum)
                {
                    var newStr = EditorGUI.TextField(position, label, property.stringValue);
                    if (newStr != property.stringValue)
                    {
                        property.stringValue = newStr;
                    }
                }
            }
            else
            {
                EditorGUI.HelpBox(position, "Invalid EnumType Variable Name: " + attr.enumTypeVar, MessageType.Error);
            }
        }
    }
}

#endif
#if STANDALONE
using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumTypeAttribute))]
public class EnumTypeDrawer : PropertyDrawer
{
    private TypeSelector typeSelector;
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (typeSelector == null)
        {
            typeSelector = new TypeSelector(typeof(Enum));
        }
        typeSelector.DrawSelector(position, property);
        //var enumStr = property.stringValue;
        //var enumType = ReflectionUtil.GetType(GetTypeString(property));
        //if (enumType != null && enumType.IsEnum)
        //{
        //    Enum old = null;
        //    try
        //    {
        //        old = (Enum)Enum.Parse(enumType, enumStr, true);
        //    } catch (Exception ex)
        //    {
        //        old = (Enum)Enum.GetValues(enumType).GetValue(0);
        //    }
        //    Enum val = EditorGUI.EnumPopup(position, old);
        //    if (val != old)
        //    {
        //        property.stringValue = val.ToString();
        //        property.serializedObject.ApplyModifiedProperties();
        //    }
        //} else
        //{
        //    var val = EditorGUI.TextField(position, enumStr);
        //    if (val != enumStr)
        //    {
        //        property.stringValue = val.ToString();
        //        property.serializedObject.ApplyModifiedProperties();
        //    }
        //}
    }
}
#endif
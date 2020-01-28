#if STANDALONE
using System;
using UnityEditor;
using UnityEngine;
using Color = UnityEngine.Color;
using Object = UnityEngine.Object;
using UnityEditorInternal;
using System.Collections.Generic;

namespace mulova.ui
{
    public abstract class PropertyDrawerBase : PropertyDrawer
    {
        public int lineHeight
        {
            get
            {
                return (int)EditorGUIUtility.singleLineHeight;
            }
        }
        private SerializedProperty prop;
        protected Rect bound;

        protected virtual int GetLineCount(SerializedProperty p)
        {
            return -1;
        }

        protected SerializedProperty GetProperty(string name)
        {
            return prop.FindPropertyRelative(name);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lineCount = GetLineCount(property);
            if (lineCount > 0)
            {
                return lineCount * base.GetPropertyHeight(property, label);
            }
            else
            {
                return base.GetPropertyHeight(property, label);
            }
        }

        protected abstract void OnGUI(SerializedProperty p, Rect bound);

        protected Rect GetLineRect(int lineNo, int lineCount = 1)
        {
            Rect lineRect = bound;
            lineRect.y += lineHeight * lineNo;
            lineRect.height = lineHeight * lineCount;
            return lineRect;
        }

        protected Rect[] SplitRect(Rect src, float leftWidth)
        {
            Rect left = src;
            Rect right = src;
            left.width = (src.width + EditorGUIUtility.currentViewWidth) * leftWidth;
            right.x = left.x + left.width;
            right.width = src.width + EditorGUIUtility.currentViewWidth - left.width;
            return new Rect[] { left, right };
        }

        protected bool DrawObjectField<T>(Rect r, GUIContent label, ref T o, bool allowSceneObj = true) where T : Object
        {
            Rect controlRect = EditorGUI.PrefixLabel(r, label);
            Rect refRect = controlRect;
            refRect.width = controlRect.width - 15;

            T old = o as T;
            o = EditorGUI.ObjectField(refRect, o, typeof(T), allowSceneObj) as T;
            return o != old;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            this.prop = property;
            this.bound = position;
            Undo.RecordObject(property.serializedObject.targetObject, property.propertyPath);
            EditorGUI.indentLevel = property.depth;
            if (property.isArray)
            {
                ReorderableList list = new ReorderableList(property.serializedObject, property);
                list.DoList(position);
                list.drawElementCallback = DrawArrayElement;
            }
            else
            {
                OnGUI(property, position);
            }
            this.prop = null;
        }

        private void DrawArrayElement(Rect rect, int idx, bool active, bool focused)
        {
            var element = prop.GetArrayElementAtIndex(idx);
            this.bound = rect;
            OnGUI(element, rect);
        }

        protected void DrawTitles(Rect pos, params string[] propNames)
        {
            for (int i = 0; i < propNames.Length; i++)
            {
                float width = 1F / propNames.Length;
                DrawLabel(propNames[i], GetRect(pos, i * width, 0F, width, 1F));
            }
        }

        protected void DrawProperties(Rect pos, SerializedProperty p, params string[] propNames)
        {
            for (int i = 0; i < propNames.Length; i++)
            {
                SerializedProperty prop = p.FindPropertyRelative(propNames[i]);
                float height = 1F / propNames.Length;
                Rect rect = GetRect(pos, 0F, i * height, 1F, height);
                DrawProperty(rect, prop, true);
            }
        }

        protected void DrawProperty(Rect pos, SerializedProperty prop, bool title)
        {
            GUIContent label = GUIContent.none;
            if (title)
            {
                label = new GUIContent(prop.displayName);
            }
            if (prop.propertyType == SerializedPropertyType.AnimationCurve)
            {
                prop.animationCurveValue = EditorGUI.CurveField(pos, label, prop.animationCurveValue);
            }
            else if (prop.propertyType == SerializedPropertyType.ArraySize)
            {
            }
            else if (prop.propertyType == SerializedPropertyType.Boolean)
            {
                prop.boolValue = EditorGUI.Toggle(pos, label, prop.boolValue);
            }
            else if (prop.propertyType == SerializedPropertyType.Bounds)
            {
                prop.boundsValue = EditorGUI.BoundsField(pos, label, prop.boundsValue);
            }
            else if (prop.propertyType == SerializedPropertyType.Character)
            {
                prop.intValue = EditorGUI.IntField(pos, label, prop.intValue);
            }
            else if (prop.propertyType == SerializedPropertyType.Color)
            {
                prop.colorValue = EditorGUI.ColorField(pos, label, prop.colorValue);
            }
            else if (prop.propertyType == SerializedPropertyType.Enum)
            {
                string[] optionNames = prop.enumNames;
                GUIContent[] options = new GUIContent[optionNames.Length];
                for (int i = 0; i < optionNames.Length; i++)
                {
                    options[i] = new GUIContent(optionNames[i]);
                }
                prop.enumValueIndex = EditorGUI.Popup(pos, label, prop.intValue, options);
            }
            else if (prop.propertyType == SerializedPropertyType.Float)
            {
                prop.floatValue = EditorGUI.FloatField(pos, label, prop.floatValue);
                //      } else if (prop.propertyType == SerializedPropertyType.Generic) {
                //          DrawGeneric(prop, pos);
            }
            else if (prop.propertyType == SerializedPropertyType.Gradient)
            {
            }
            else if (prop.propertyType == SerializedPropertyType.Integer)
            {
                prop.intValue = EditorGUI.IntField(pos, label, prop.intValue);
            }
            else if (prop.propertyType == SerializedPropertyType.LayerMask)
            {
                prop.intValue = EditorGUI.LayerField(pos, label, prop.intValue);
            }
            else if (prop.propertyType == SerializedPropertyType.ObjectReference)
            {
                prop.objectReferenceValue = EditorGUI.ObjectField(pos, label, prop.objectReferenceValue, Type.GetType(prop.type), true);
            }
            else if (prop.propertyType == SerializedPropertyType.Rect)
            {
                prop.rectValue = EditorGUI.RectField(pos, label, prop.rectValue);
            }
            else if (prop.propertyType == SerializedPropertyType.String)
            {
                prop.stringValue = EditorGUI.TextField(pos, label, prop.stringValue);
            }
            else if (prop.propertyType == SerializedPropertyType.Vector2)
            {
                prop.vector2Value = EditorGUI.Vector2Field(pos, label.text, prop.vector2Value);
            }
            else if (prop.propertyType == SerializedPropertyType.Vector3)
            {
                prop.vector3Value = EditorGUI.Vector2Field(pos, label.text, prop.vector3Value);
            }
            else if (prop.propertyType == SerializedPropertyType.Generic)
            {
                if (prop.type == typeof(Quaternion).ToString())
                {
                    DrawVec4(prop, label, pos);
                }
            }
        }

        protected void DrawLabel(string title, Rect pos)
        {
            EditorGUI.SelectableLabel(pos, title);
        }

        protected void DrawVec4(SerializedProperty prop, GUIContent label, Rect pos)
        {
            Quaternion q = prop.quaternionValue;
            Vector4 vec4 = new Vector4(q.x, q.y, q.z, q.w);
            vec4 = EditorGUI.Vector4Field(pos, label.text, vec4);
            prop.quaternionValue = new Quaternion(vec4.x, vec4.y, vec4.z, vec4.w);
        }

        private Rect GetRect(Rect src, float x0, float y0, float width, float height)
        {
            return new Rect(x0.Interpolate(src.x, src.x + src.width),
            y0.Interpolate(src.y, src.y + src.height),
            src.width * width,
            src.height * height);
        }

        public static bool Popup<T>(Rect rect, ref T selection, IList<T> items)
        {
            bool ret = Popup<T>(rect, null, ref selection, items);
            return ret;
        }

        public static bool Popup<T>(Rect rect, string label, ref T selection, IList<T> items)
        {
            return Popup<T>(rect, label, ref selection, items);
        }

        public static bool Popup<T>(Rect rect, string label, ref T selection, IList<T> items, GUIStyle style)
        {
            if (items.Count == 0)
            {
                return false;
            }
            int index = -1;
            string[] str = new string[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                str[i] = items[i]?.ToString();
                if (object.ReferenceEquals(items[i], selection) || object.Equals(items[i], selection))
                {
                    index = i;
                }
            }
            int newIndex = 0;

            Rect controlRect = label != null ? EditorGUI.PrefixLabel(rect, new GUIContent(label)) : rect;

            // Show if current value is not the member of popup list
            Color c = GUI.contentColor;
            if (index < 0 && selection != null)
            {
                GUI.contentColor = Color.red;
                index = 0;
            }
            GUI.contentColor = c;
            if (style != null)
            {
                newIndex = EditorGUI.Popup(controlRect, index, str, style);
            }
            else
            {
                newIndex = EditorGUI.Popup(controlRect, index, str);
            }
            if (index != newIndex)
            {
                selection = items[newIndex];
                return true;
            }
            return false;
        }

        public static bool PopupNullable<T>(Rect rect, string label, ref T selection, IList<T> items)
        {
            Rect controlRect = label != null ? EditorGUI.PrefixLabel(rect, new GUIContent(label)) : rect;
            if (items.Count == 0)
            {
                bool changed = false;
                if (selection != null && !string.Empty.Equals(selection) && !selection.Equals(default(T)))
                {
                    selection = default(T);
                    changed = true;
                }
                EditorGUI.Popup(controlRect, 0, new string[] { "-" });
                return changed;
            }


            int index = 0;
            string[] str = new string[items.Count + 1];
            str[0] = "-";
            for (int i = 1; i <= items.Count; i++)
            {
                str[i] = items[i - 1]?.ToString();
                if (object.Equals(items[i - 1], selection))
                {
                    index = i;
                }
            }
            int newIndex = EditorGUI.Popup(controlRect, index, str);
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

        public static bool TextField(Rect r, string label, SerializedProperty p)
        {
            return TextField(r, label, p, EditorStyles.textField);
        }

        public static bool TextField(Rect r, string label, SerializedProperty p, GUIStyle style)
        {
            string str = p.stringValue;
            string newStr = label == null ?
            EditorGUI.TextField(r, str, style) :
            EditorGUI.TextField(r, label, str, style);
            if (newStr != str)
            {
                p.stringValue = newStr;
                return true;
            }
            return false;
        }

        public static void LabelField(Rect r, SerializedProperty p)
        {
            LabelField(r, p, EditorStyles.label);
        }

        public static void LabelField(Rect r, SerializedProperty p, GUIStyle style)
        {
            string str = p.stringValue;
            EditorGUI.LabelField(r, str, style);
        }

        protected void SetDirty()
        {
            prop.serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
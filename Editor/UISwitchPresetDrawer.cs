using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Text.Ex;
using comunity;
using System.Collections.Generic.Ex;

namespace uiswitch
{
    [CustomPropertyDrawer(typeof(UISwitchPreset))]
    public class UISwitchPresetDrawer : PropertyDrawerBase
    {
        private Dictionary<string, ReorderSerialized<string>> vPool = new Dictionary<string, ReorderSerialized<string>>();

        private ReorderSerialized<string> GetDrawer(SerializedProperty p)
        {
            var v = vPool.Get(p.propertyPath);
            if (v == null)
            {
                var prop = p.FindPropertyRelative("keys");
                v = new ReorderSerialized<string>(prop);
                v.drawItem = OnDrawKeys;

                v.onAdd = i => OnAddObject(prop, i);
                v.onRemove = i => OnRemoveObject(prop, i);
                v.onReorder = (i1, i2) => OnReorderObject(prop, i1, i2);
                v.canAdd = () =>
                {
                    var objs = p.serializedObject.FindProperty("objs");
                    return !IsDuplicate(objs, Selection.activeObject);
                };
                vPool[p.propertyPath] = v;
            }
            return v;
        }

        protected override void DrawProperty(SerializedProperty p, Rect bound)
        {
            var n = p.FindPropertyRelative("presetName");
            var keys = p.FindPropertyRelative("keys");
            // Draw Name
            var lines = bound.SplitByHeights(lineHeight);
            var nameBounds = lines[0].SplitByWidths(50);
            Color c = GUI.contentColor;
            if (n.stringValue.IsEmpty())
            {
                GUI.color = Color.red;
            }
            if (GUI.Button(nameBounds[0], new GUIContent("ID")))
            {
                var script = p.serializedObject.targetObject as UISwitch;
                script.SetPreset(n.stringValue);
            }
            EditorGUI.PropertyField(nameBounds[1], n, new GUIContent(""));
            GUI.color = c;

            EditorGUI.PropertyField(lines[1], keys, new GUIContent(""));
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }


        private void OnDrawKeys(SerializedProperty item, Rect rect, int index, bool isActive, bool isFocused)
        {
            var union = UISwitchInspector.GetVisibilityUnion(item.serializedObject.targetObject as UISwitch);
            Color color = GUI.contentColor;
            if (!union[index])
            {
                GUI.contentColor = Color.gray;
            }
            var bounds = rect.SplitByWidths(25, 25);
            // draw index
            EditorGUI.PrefixLabel(bounds[0], new GUIContent(index.ToString()));
            // draw bool
            EditorGUI.PropertyField(bounds[1], item, new GUIContent(""));
            var objs = item.serializedObject.FindProperty("objs");
            var obj = objs.GetArrayElementAtIndex(index);
            var oldObj = obj.objectReferenceValue;
            EditorGUI.PropertyField(bounds[2], obj, new GUIContent(""));
            if (obj.objectReferenceValue == null || IsDuplicate(objs, obj.objectReferenceValue))
            {
                obj.objectReferenceValue = oldObj;
            }
            GUI.contentColor = color;
        }

        private void OnReorderObject(SerializedProperty p, int i1, int i2)
        {
            // reorder objs
            var objs = p.serializedObject.FindProperty("objs");
            objs.MoveArrayElement(i1, i2);
            p.serializedObject.ApplyModifiedProperties();

            // reorder visibility element
            var switches = p.serializedObject.FindProperty("switches");
            for (int i = 0; i < switches.arraySize; ++i)
            {
                var s = switches.GetArrayElementAtIndex(i);
                var visibility = s.FindPropertyRelative("visibility");
                if (p.propertyPath != visibility.propertyPath)
                {
                    visibility.MoveArrayElement(i1, i2);
                }
            }
            p.serializedObject.ApplyModifiedProperties();
        }

        private void OnAddObject(SerializedProperty p, int index)
        {
            p.GetArrayElementAtIndex(index).boolValue = true;
            // Add object to "objs"
            var objs = p.serializedObject.FindProperty("objs");
            objs.InsertArrayElementAtIndex(index);
            var item = objs.GetArrayElementAtIndex(index);
            item.objectReferenceValue = Selection.activeObject;

            // Add visibility element
            var switches = p.serializedObject.FindProperty("switches");
            for (int i = 0; i < switches.arraySize; ++i)
            {
                var s = switches.GetArrayElementAtIndex(i);
                var visibility = s.FindPropertyRelative("visibility");
                if (p.propertyPath != visibility.propertyPath)
                {
                    visibility.InsertArrayElementAtIndex(index);
                    visibility.GetArrayElementAtIndex(index).boolValue = false;
                }
            }
            p.serializedObject.ApplyModifiedProperties();
        }

        private void OnRemoveObject(SerializedProperty p, int index)
        {
            var objs = p.serializedObject.FindProperty("objs");
            objs.DeleteArrayElementAtIndex(index); // clear reference
            objs.DeleteArrayElementAtIndex(index); // remove index

            // Remove visibility element
            var switches = p.serializedObject.FindProperty("switches");
            for (int i = 0; i < switches.arraySize; ++i)
            {
                var s = switches.GetArrayElementAtIndex(i);
                var visibility = s.FindPropertyRelative("visibility");
                if (p.propertyPath != visibility.propertyPath)
                {
                    visibility.DeleteArrayElementAtIndex(index);
                }
            }
            p.serializedObject.ApplyModifiedProperties();
        }
    }
}
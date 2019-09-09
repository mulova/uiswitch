using System.Collections.Generic;
using System.Collections.Generic.Ex;
using System.Text.Ex;
using mulova.comunity;
using UnityEditor;
using UnityEngine;
using UnityEngine.Ex;
using Object = UnityEngine.Object;

namespace mulova.uiswitch
{
    [CustomPropertyDrawer(typeof(UISwitchSect))]
    public class UISwitchSectDrawer : PropertyDrawerBase
    {
        private Dictionary<string, PropertyReorder<bool>> vPool = new Dictionary<string, PropertyReorder<bool>>();
        private Dictionary<string, PropertyReorder<Transform>> tPool = new Dictionary<string, PropertyReorder<Transform>>();
        private static string activeSect;

        private PropertyReorder<bool> GetVisibilityDrawer(SerializedProperty p)
        {
            var v = vPool.Get(p.propertyPath);
            if (v == null)
            {
                var visibility = p.FindPropertyRelative("visibility");
                v = new PropertyReorder<bool>(visibility);
                v.drawItem = OnDrawVisibility;

                v.onAdd = i => OnAddObject(visibility, i);
                v.onRemove = i => OnRemoveObject(visibility, i);
                v.onReorder = (i1, i2) => OnReorderObject(visibility, i1, i2);
                v.canAdd = () =>
                {
                    var objs = p.serializedObject.FindProperty("objs");
                    return !IsDuplicate(objs, Selection.activeObject);
                };
                vPool[p.propertyPath] = v;
            }
            return v;
        }

        private PropertyReorder<Transform> GetTransDrawer(SerializedProperty p)
        {
            var t = tPool.Get(p.propertyPath);
            if (t == null)
            {
                var trans = p.FindPropertyRelative("trans");
                t = new PropertyReorder<Transform>(trans);
                t.onAdd = i => OnAddTrans(p, i);
                t.onRemove = i => OnRemoveTrans(p, i);
                t.onReorder = (i1, i2) => OnReorderTrans(p, i1, i2);
                t.canAdd = () =>
                {
                    if (Selection.activeGameObject == null)
                    {
                        return false;
                    }
                    var sel = Selection.activeGameObject.transform;
                    for (int i=0; i<trans.arraySize; ++i)
                    {
                        if (trans.GetArrayElementAtIndex(i).objectReferenceValue == sel)
                        {
                            return false;
                        }
                    }
                    return true;
                };
                tPool[p.propertyPath] = t;
            }
            return t;
        }

        private double changedTime;
        protected override void OnGUI(SerializedProperty p, Rect bound)
        {
            // Draw Name
            var bounds = bound.SplitByHeights(lineHeight);
            var n = p.FindPropertyRelative("name");
            var nameBounds = bounds[0].SplitByWidthsRatio(0.5f, 0.5f);
            if (n.stringValue.IsEmpty())
            {
                GUI.enabled = false;
            }
            if (GUI.Button(nameBounds[0], new GUIContent(n.stringValue)))
            {
                activeSect = p.propertyPath;
                var script = p.serializedObject.targetObject as UISwitch;
                script.Set(n.stringValue);
            }
            GUI.enabled = true;
            EditorGUI.PropertyField(nameBounds[1], n, new GUIContent(""));

            // Draw Visibility
            var visibility = GetVisibilityDrawer(p);
            bounds[1].x += 30;
            bounds[1].width -= 30;
            var objBound = bounds[1];
            objBound.height = visibility.GetHeight();
            visibility.Draw(objBound);

            var trans = GetTransDrawer(p);
            Color c = GUI.color;
            if (p.propertyPath == activeSect)
            {
                c = UpdatePos(p)? Color.red: Color.yellow;
            }
            using (new EditorGUIUtil.ColorScope(c))
            {
                var tBound = objBound;
                tBound.height = trans.GetHeight();
                tBound.y += objBound.height;
                trans.Draw(tBound);
            }
        }

        private bool UpdatePos(SerializedProperty property)
        {
            var trans = property.FindPropertyRelative("trans");
            var pos = property.FindPropertyRelative("pos");
            bool changed = EditorApplication.timeSinceStartup-changedTime <= 0.2;

            for (int i=0; i<trans.arraySize; ++i)
            {
                var t = trans.GetArrayElementAtIndex(i).objectReferenceValue as Transform;
                if (t.hasChanged)
                {
                    t.hasChanged = false;
                    var p = pos.GetArrayElementAtIndex(i);
                    p.vector3Value = t.localPosition;
                    changedTime = EditorApplication.timeSinceStartup;
                    changed = true;
                }
            }
            return changed;
        }

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        public override float GetPropertyHeight(SerializedProperty p, GUIContent label)
        {
            var visibility = GetVisibilityDrawer(p);
            var separator = 10;
            var trans = GetTransDrawer(p);
            return visibility.GetHeight()
                + trans.GetHeight()
                + lineHeight + separator;
        }

        private bool IsDuplicate(SerializedProperty arr, Object o)
        {
            for (int i = 0; i < arr.arraySize; ++i)
            {
                var a = arr.GetArrayElementAtIndex(i);
                if (a.objectReferenceValue == Selection.activeObject)
                {
                    return true;
                }
            }
            return false;
        }

        private void OnDrawVisibility(SerializedProperty item, Rect rect, int index, bool isActive, bool isFocused)
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

        private void OnReorderObject(SerializedProperty sect, int i1, int i2)
        {
            // reorder objs
            var objs = sect.serializedObject.FindProperty("objs");
            objs.MoveArrayElement(i1, i2);
            sect.serializedObject.ApplyModifiedProperties();

            // reorder visibility element
            var switches = sect.serializedObject.FindProperty("switches");
            for (int i = 0; i < switches.arraySize; ++i)
            {
                var s = switches.GetArrayElementAtIndex(i);
                var visibility = s.FindPropertyRelative("visibility");
                if (sect.propertyPath != visibility.propertyPath)
                {
                    visibility.MoveArrayElement(i1, i2);
                }
            }
            sect.serializedObject.ApplyModifiedProperties();
        }

        private void OnAddObject(SerializedProperty sect, int index)
        {
            sect.GetArrayElementAtIndex(index).boolValue = true;
            // Add object to "objs"
            var objs = sect.serializedObject.FindProperty("objs");
            objs.InsertArrayElementAtIndex(index);
            var item = objs.GetArrayElementAtIndex(index);
            item.objectReferenceValue = Selection.activeObject;

            // Add visibility element
            var switches = sect.serializedObject.FindProperty("switches");
            for (int i = 0; i < switches.arraySize; ++i)
            {
                var s = switches.GetArrayElementAtIndex(i);
                var visibility = s.FindPropertyRelative("visibility");
                if (sect.propertyPath != visibility.propertyPath)
                {
                    visibility.InsertArrayElementAtIndex(index);
                    visibility.GetArrayElementAtIndex(index).boolValue = false;
                }
            }
            sect.serializedObject.ApplyModifiedProperties();
        }

        private void OnRemoveObject(SerializedProperty sect, int index)
        {
            var objs = sect.serializedObject.FindProperty("objs");
            objs.DeleteArrayElementAtIndex(index); // clear reference
            objs.DeleteArrayElementAtIndex(index); // remove index

            // Remove visibility element
            var switches = sect.serializedObject.FindProperty("switches");
            for (int i = 0; i < switches.arraySize; ++i)
            {
                var s = switches.GetArrayElementAtIndex(i);
                var visibility = s.FindPropertyRelative("visibility");
                if (sect.propertyPath != visibility.propertyPath)
                {
                    visibility.DeleteArrayElementAtIndex(index);
                }
            }
            sect.serializedObject.ApplyModifiedProperties();
        }

        private void OnReorderTrans(SerializedProperty sect, int i1, int i2)
        {
            // reorder pos
            var pos = sect.FindPropertyRelative("pos");
            pos.MoveArrayElement(i1, i2);
            sect.serializedObject.ApplyModifiedProperties();
        }

        private void OnAddTrans(SerializedProperty sect, int index)
        {
            var sel = Selection.activeGameObject.transform;
            // Add object to "pos"
            //sect.serializedObject.ApplyModifiedProperties();
            var trans = sect.FindPropertyRelative("trans");
            var t = trans.GetArrayElementAtIndex(index);
            t.objectReferenceValue = sel;
            var pos = sect.FindPropertyRelative("pos");
            pos.InsertArrayElementAtIndex(index);
            var posItem = pos.GetArrayElementAtIndex(index);
            posItem.vector3Value = sel.localPosition;
        }

        private void OnRemoveTrans(SerializedProperty sect, int index)
        {
            var trans = sect.FindPropertyRelative("trans");
            trans.DeleteArrayElementAtIndex(index); // remove index
            var pos = sect.FindPropertyRelative("pos");
            pos.DeleteArrayElementAtIndex(index); // clear reference
            sect.serializedObject.ApplyModifiedProperties();
        }
    }
}
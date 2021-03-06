﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
#if CORE_LIB
using System.Collections.Generic.Ex;
using System.Text.Ex;
using mulova.unicore;
using UnityEngine.Ex;
#endif

namespace mulova.switcher
{
    [CustomPropertyDrawer(typeof(SwitchSet))]
    public class SwitchSetDrawer : PropertyDrawerBase
    {
        private Dictionary<string, PropertyReorder<bool>> vPool = new Dictionary<string, PropertyReorder<bool>>();
        private Dictionary<string, PropertyReorder<Transform>> tPool = new Dictionary<string, PropertyReorder<Transform>>();

        public static readonly Color SelectedColor = Color.green;
        public static readonly Color ChangedSelectedColor = Color.red;

        protected override void OnGUI(SerializedProperty p, Rect bound)
        {
            var uiSwitch = p.serializedObject.targetObject as Switcher;
            // Draw Name
            var bounds = bound.SplitByHeights(lineHeight);
            var n = p.FindPropertyRelative("name");
            var active = SwitcherInspector.IsActive(n.stringValue);
            var nameBounds = bounds[0].SplitByWidthsRatio(0.5f, 0.25f, 0.25f);
            var boundsLeft = bounds[1];
            // indentation
            boundsLeft.x += 30;
            boundsLeft.width -= 30;
            Color c = GUI.color;
            if (active)
            {
                c = UpdatePos(p) ? ChangedSelectedColor : SelectedColor;
            }

            // Draw Title
            using (new EnableScope(!n.stringValue.IsEmpty()))
            {
                using (new ColorScope(c))
                {
                    if (GUI.Button(nameBounds[0], new GUIContent(n.stringValue)))
                    {
                        Undo.RecordObjects(uiSwitch.objs.ToArray(), "switch");
                        bool hasPreset = !uiSwitch.preset.IsEmpty();
                        if (!hasPreset)
                        {
                            SwitcherInspector.SetActive(n.stringValue, n.stringValue);
                        }
                        else
                        {
                            SwitcherInspector.Activate(n.stringValue, !SwitcherInspector.IsActive(n.stringValue));
                        }
                        var script = p.serializedObject.targetObject as Switcher;
                        script.SetKey(n.stringValue);
                    }
                }
            }
            EditorGUI.PropertyField(nameBounds[1], n, new GUIContent(""));
            if (GUI.Button(nameBounds[2], "Collect"))
            {
                uiSwitch.Collect(n.stringValue);
                EditorUtil.SetDirty(uiSwitch);
            }
            // Draw Visibility
            var visibility = GetVisibilityDrawer(p);

            var objBounds = boundsLeft.SplitByHeights((int)visibility.GetHeight());
            boundsLeft = objBounds[1];
            visibility.Draw(objBounds[0]);

            // Draw Transform
            if (uiSwitch.showTrans)
            {
                var trans = GetTransDrawer(p);
                var tBound = boundsLeft.SplitByHeights(transHeight);
                boundsLeft = tBound[1];
                using (new ColorScope(c))
                {
                    trans.Draw(tBound[0]);
                }
            }

            // Draw Actions
            if (uiSwitch.showAction)
            {
                var actionBounds = boundsLeft.SplitByHeights(actionHeight);
                boundsLeft = actionBounds[1];
                var actionProperty = p.FindPropertyRelative("action");
                EditorGUI.PropertyField(actionBounds[0], actionProperty);
            }

            // Draw ICompData
            var dataBounds = boundsLeft.SplitByHeights(dataHeight);
            boundsLeft = dataBounds[1];
#if UNITY_2019_1_OR_NEWER
            var dataProperty = p.FindPropertyRelative("data");
            EditorGUI.PropertyField(dataBounds[0], dataProperty, true);
#endif
        }

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

            void OnDrawVisibility(SerializedProperty item, Rect rect, int index, bool isActive, bool isFocused)
            {
                var union = SwitcherInspector.GetVisibilityUnion(item.serializedObject.targetObject as Switcher);
                using (new ContentColorScope(Color.gray, !union[index]))
                {
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
                }
            }
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
                    // check duplicate
                    for (int i=0; i<trans.arraySize; ++i)
                    {
                        if (trans.GetArrayElementAtIndex(i).objectReferenceValue == sel)
                        {
                            return false;
                        }
                    }
                    // check if in visibility array
                    var objs = p.serializedObject.FindProperty("objs");
                    return IsDuplicate(objs, Selection.activeObject);
                };
                tPool[p.propertyPath] = t;
            }
            return t;
        }

        private double changedTime;
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

        private int dataHeight;
        private int transHeight;
        private int actionHeight;
        private int visHeight;
        public override float GetPropertyHeight(SerializedProperty p, GUIContent label)
        {
            var uiSwitch = p.serializedObject.targetObject as Switcher;
            var separator = 10;
            visHeight = (int)GetVisibilityDrawer(p).GetHeight();
            float height = visHeight;

#if UNITY_2019_1_OR_NEWER
            dataHeight = (int)EditorGUI.GetPropertyHeight(p.FindPropertyRelative("data"));
            height += dataHeight;
#endif
            transHeight = uiSwitch.showTrans? (int)GetTransDrawer(p).GetHeight(): 0;
            height += transHeight;

            actionHeight = uiSwitch.showAction? (int)EditorGUI.GetPropertyHeight(p.FindPropertyRelative("action")): 0;
            height += actionHeight;

            height += lineHeight + separator;
            return height;
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
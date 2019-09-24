using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Text.Ex;
using System.Collections.Generic.Ex;
using UnityEngine.Ex;
using mulova.unicore;

namespace mulova.ui
{
    [CustomPropertyDrawer(typeof(UISwitchPreset))]
    public class UISwitchPresetDrawer : PropertyDrawerBase
    {
        private Dictionary<string, PopupReorder> pool = new Dictionary<string, PopupReorder>();

        private PopupReorder GetKeysDrawer(SerializedProperty p)
        {
            var v = pool.Get(p.propertyPath);
            string[] options = (p.serializedObject.targetObject as UISwitch).GetAllKeys().ToArray();
            if (v == null)
            {
                var prop = p.FindPropertyRelative("keys");
                v = new PopupReorder(prop, options);
                pool[p.propertyPath] = v;
            } else
            {
                v.options = options;
            }
            return v;
        }

        protected override void OnGUI(SerializedProperty p, Rect bound)
        {
            var n = p.FindPropertyRelative("presetName");
            // Draw Name
            var lines = bound.SplitByHeights(lineHeight);
            var nameBounds = lines[0].SplitByWidthsRatio(0.5f, 0.5f);
            Color c = GUI.contentColor;
            if (n.stringValue.IsEmpty())
            {
                GUI.color = Color.red;
            }
            if (GUI.Button(nameBounds[0], new GUIContent(n.stringValue)))
            {
                var script = p.serializedObject.targetObject as UISwitch;
                script.SetPreset(n.stringValue);
            }
            EditorGUI.PropertyField(nameBounds[1], n, new GUIContent(""));
            GUI.color = c;

            var drawer = GetKeysDrawer(p);
            drawer.Draw(lines[1]);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return lineHeight // presetName
                + GetKeysDrawer(property).GetHeight()  // keys
                + 5; // separator
        }

    }
}
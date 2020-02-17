using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if CORE_LIB
using System.Collections.Generic.Ex;
using System.Text.Ex;
using mulova.unicore;
using UnityEngine.Ex;
#endif

namespace mulova.ui
{
    [CustomPropertyDrawer(typeof(UISwitchPreset))]
    public class UISwitchPresetDrawer : PropertyDrawer
    {
        private Dictionary<string, PopupReorder> pool = new Dictionary<string, PopupReorder>(); //

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

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var n = property.FindPropertyRelative("presetName");
            // Draw Name
            var lines = position.SplitByHeights((int)EditorGUIUtility.singleLineHeight);
            var nameBounds = lines[0].SplitByWidthsRatio(0.5f, 0.5f);
            var keysProp = property.FindPropertyRelative("keys");
            List<string> presetKeys = new List<string>();
            for (int i=0; i<keysProp.arraySize; ++i)
            {
                presetKeys.Add(keysProp.GetArrayElementAtIndex(i).stringValue);
            }
            using (new ColorScope(Color.green, UISwitchInspector.IsPreset(presetKeys)))
            {
                using (new ColorScope(Color.red, n.stringValue.IsEmpty()))
                {
                    if (GUI.Button(nameBounds[0], new GUIContent(n.stringValue)))
                    {
                        var script = property.serializedObject.targetObject as UISwitch;
                        script.SetPreset(n.stringValue);
                        UISwitchInspector.SetActive(presetKeys.ToArray());
                    }
                    EditorGUI.PropertyField(nameBounds[1], n, new GUIContent(""));
                }
                // indentation
                lines[1].x += 30;
                lines[1].width -= 30;
                var drawer = GetKeysDrawer(property);
                drawer.Draw(lines[1]);
            }

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (int)EditorGUIUtility.singleLineHeight // presetName
                + GetKeysDrawer(property).GetHeight()  // keys
                + 5; // separator
        }

    }
}
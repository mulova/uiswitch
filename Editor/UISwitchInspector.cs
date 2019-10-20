using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using mulova.unicore;

namespace mulova.ui
{
    [CustomEditor(typeof(UISwitch))]
    public class UISwitchInspector : Editor
    {
        public static bool showSceneUI = true;
        public bool autoRemove;
        private UISwitch uiSwitch;
        internal static bool exclusive = true;
        private double changedTime = double.MaxValue;

        void OnEnable()
        {
            uiSwitch = (UISwitch)target;
            EditorApplication.update += OnUpdate;
            changedTime = double.MaxValue;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnUpdate;
        }

        private void OnSceneGUI()
        {
            Handles.BeginGUI();
            GUILayout.BeginHorizontal();
            showSceneUI = GUILayout.Toggle(showSceneUI, "");
            if (showSceneUI)
            {
                using (new GUILayout.VerticalScope())
                {
                    foreach (var p in uiSwitch.preset)
                    {
                        if (GUILayout.Button(p.presetName, GUILayout.MaxWidth(300)))
                        {
                            uiSwitch.SetPreset(p.presetName);
                        }
                    }
                }
                using (new GUILayout.VerticalScope())
                {
                    foreach (var s in uiSwitch.switches)
                    {
                        using (new EditorGUIUtil.ColorScope(Color.red, s.name == UISwitchSetDrawer.activeSet))
                        {
                            if (GUILayout.Button(s.name, GUILayout.MaxWidth(300)))
                            {
                                uiSwitch.Set(s.name);
                                UISwitchSetDrawer.activeSet = s.name;
                            }
                        }
                    }
                }
            }
            GUILayout.EndHorizontal();
            Handles.EndGUI();
        }

        public override void OnInspectorGUI()
        {
            if (autoRemove)
            {
                RemoveUnused();
            }
            DrawDefaultInspector();
        }

        void OnUpdate()
        {
            bool changed = false;
            foreach (var s in uiSwitch.switches)
            {
                foreach (var t in s.trans)
                {
                    changed |= t.hasChanged;
                }
            }
            if (changed)
            {
                EditorUtility.SetDirty(this);
                changedTime = EditorApplication.timeSinceStartup;
                Repaint();
            } else if (EditorApplication.timeSinceStartup - changedTime > 0.2)
            {
                changedTime = double.MaxValue;
                Repaint();
            }
        }


        public static bool[] GetVisibilityUnion(UISwitch uiSwitch)
        {
            bool[] union = new bool[uiSwitch.objs.Count];
            HashSet<List<bool>> set = new HashSet<List<bool>>();
            for (int i = 0; i < uiSwitch.switches.Count; ++i)
            {
                var s = uiSwitch.switches[i];
                if (set.Contains(s.visibility))
                {
                    uiSwitch.switches[i] = new UISwitchSet();
                    EditorUtil_SetDirty(uiSwitch);
                }
                set.Add(uiSwitch.switches[i].visibility);
                for (int j = 0; j < s.visibility.Count; ++j)
                {
                    union[j] |= s.visibility[j];
                }
            }
            return union;
        }

        private void RemoveUnused()
        {
            bool[] union = GetVisibilityUnion(uiSwitch);
            for (int i = union.Length - 1; i >= 0; --i)
            {
                if (!union[i])
                {
                    uiSwitch.objs.RemoveAt(i);
                    for (int j = 0; j < uiSwitch.switches.Count; ++j)
                    {
                        uiSwitch.switches[j].visibility.RemoveAt(i);
                    }
                    EditorUtil_SetDirty(uiSwitch);
                }
            }
        }

        public static void EditorUtil_SetDirty(Object o)
        {
            if (Application.isPlaying || o == null)
            {
                return;
            }
            GameObject go = null;
            if (o is GameObject)
            {
                go = o as GameObject;
            }
            else if (o is Component)
            {
                go = (o as Component).gameObject;
            }
            if (go != null && go.scene.IsValid())
            {
                EditorSceneManager.MarkSceneDirty(go.scene);
            }
            else
            {
                EditorUtility.SetDirty(o);
            }
        }
    }
}
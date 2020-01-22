using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using mulova.unicore;
using System.Collections.Generic.Ex;
using mulova.commons;

namespace mulova.ui
{
    [CustomEditor(typeof(UISwitch))]
    public class UISwitchInspector : Editor
    {
        public bool autoRemove;
        private UISwitch uiSwitch;
        internal static bool exclusive = true;
        private double changedTime = double.MaxValue;
        internal static HashSet<string> activeSet = new HashSet<string>();

        internal static bool IsPreset(IList<string> actives)
        {
            if (activeSet.Count != actives.Count)
            {
                return false;
            }
            foreach (var a in actives)
            if (actives != null)
            {
                if (!activeSet.Contains(a))
                {
                    return false;
                }
            }
            return true;
        }

        internal static void SetActive(params string[] actives)
        {
            activeSet.Clear();
            if (actives != null)
            {
                activeSet.AddAll(actives);
            }
        }

        internal static bool IsActive(string active)
        {
            return activeSet.Contains(active);
        }

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
            bool locked = GUILayout.Toggle(ActiveEditorTracker.sharedTracker.isLocked, "Lock");
            if (locked ^ ActiveEditorTracker.sharedTracker.isLocked)
            {
                ActiveEditorTracker.sharedTracker.isLocked = locked;
            }
            if (ActiveEditorTracker.sharedTracker.isLocked)
            {
                if (!uiSwitch.preset.IsEmpty())
                {
                    using (new GUILayout.VerticalScope())
                    {
                        GUILayout.Label("Preset");
                        foreach (var p in uiSwitch.preset)
                        {
                            using (new ColorScope(Color.green, IsPreset(p.keys)))
                            {
                                if (GUILayout.Button(p.presetName, GUILayout.MaxWidth(200)))
                                {
                                    uiSwitch.SetPreset(p.presetName);
                                    SetActive(p.keys);
                                }
                            }
                        }
                    }
                }
                GUILayout.Space(10);
                using (new GUILayout.VerticalScope())
                {
                    GUILayout.Label("Option");
                    foreach (var s in uiSwitch.switches)
                    {
                        using (new ColorScope(Color.green, IsActive(s.name)))
                        {
                            if (GUILayout.Button(s.name, GUILayout.MaxWidth(200)))
                            {
                                uiSwitch.Set(s.name);
                                SetActive(s.name);
                            }
                        }

                        if (IsActive(s.name))
                        {
                            int labelWidth = 150;
                            foreach (var t in s.trans)
                            {
                                var uiPos = HandleUtility.WorldToGUIPoint(t.position);
                                uiPos.x -= labelWidth/ 2;
                                uiPos.y -= 40;
                                var rect = new Rect(uiPos, new Vector2(labelWidth, 20));
                                GUI.Button(rect, t.name);
                            }
                        }
                    }
                }
            }
            Handles.EndGUI();

            foreach (var s in uiSwitch.switches)
            {
                if (IsActive(s.name))
                {
                    foreach (var t in s.trans)
                    {
                        var pos = Handles.PositionHandle(t.position, t.rotation);
                        if (pos != t.position)
                        {
                            t.position = pos;
                            EditorUtil.SetDirty(t);
                            EditorUtil.SetDirty(uiSwitch);
                        }
                    }
                }
            }
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
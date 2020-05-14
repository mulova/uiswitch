using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if CORE_LIB
using System.Collections.Generic.Ex;
using System.Ex;
using mulova.unicore;
using mulova.commons;
using System.Text.Ex;
#endif

namespace mulova.switcher
{
    [CustomEditor(typeof(Switcher))]
    public class SwitcherInspector : Editor
    {
        private Switcher uiSwitch;
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

        internal static void Activate(string id, bool active)
        {
            if (active)
            {
                activeSet.Add(id);
            } else
            {
                activeSet.Remove(id);
            }
        }

        internal static bool IsActive(string active)
        {
            return activeSet.Contains(active);
        }

        private void OnEnable()
        {
            uiSwitch = (Switcher)target;
            EditorApplication.update += OnUpdate;
            changedTime = double.MaxValue;

            uiSwitch.showTrans = uiSwitch.switches.Find(s => s.trans.Count > 0) != null;
            uiSwitch.showAction = uiSwitch.switches.Find(s => s.action != null && s.action.GetPersistentEventCount() > 0) != null;
            uiSwitch.showPreset = uiSwitch.preset.Count > 0;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnUpdate;
            activeSet.Clear();
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
                                uiSwitch.SetKey(s.name);
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

            if (locked)
            {
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
                                EditorUtil.SetDirty(t); //
                                EditorUtil.SetDirty(uiSwitch);
                            }
                        }
                    }
                }
            }
        }

        private string createSwitcherErr;
        private ObjPropertyReorder<GameObject> diffList;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (uiSwitch.switches.Count == 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Separator();

                if (diffList == null)
                {
                    diffList = new ObjPropertyReorder<GameObject>(serializedObject, "objs");
                    diffList.onRemove = OnRemoveObject;
                    diffList.title = "Diff Roots";

                    void OnRemoveObject(int index)
                    {
                        var objs = serializedObject.FindProperty("objs");
                        objs.DeleteArrayElementAtIndex(index); // remove index
                        serializedObject.ApplyModifiedProperties();
                    }
                }
                serializedObject.Update();
                diffList.Draw();
                serializedObject.ApplyModifiedProperties();
                if (uiSwitch.objs.Count >= 2)
                {
                    if (GUILayout.Button("Extract Diff"))
                    {
                        serializedObject.Update();
                        var roots = uiSwitch.objs.ToArray();
                        Undo.RecordObjects(roots, "Create Switcher");
                        createSwitcherErr = SwitcherMenu.CreateSwitcher(roots);
                        if (createSwitcherErr.IsEmpty())
                        {
                            Undo.DestroyObjectImmediate(uiSwitch);
                            Selection.activeGameObject = roots[0];
                        }
                    }
                }
                if (!createSwitcherErr.IsEmpty())
                {
                    EditorGUILayout.HelpBox(createSwitcherErr, MessageType.Error);
                }
            } else if (uiSwitch.switches.Count > 0)
            {
                if (uiSwitch.showPreset)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("preset"), true);
                }
                EditorGUILayout.Separator();
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (!uiSwitch.showTrans)
                    {
                        if (GUILayout.Button("Transforms"))
                        {
                            uiSwitch.showTrans = true;
                        }
                    }
                    if (!uiSwitch.showAction)
                    {
                        if (GUILayout.Button("Actions"))
                        {
                            uiSwitch.showAction = true;
                        }
                    }
                    if (!uiSwitch.showPreset)
                    {
                        if (GUILayout.Button("Preset"))
                        {
                            uiSwitch.showPreset = true;
                        }
                    }
                }
            }
        }

        private bool IsFirstExtra(Transform c1, Transform c2)
        {
            if (c1 == null)
            {
                return false;
            }
            if (c2 == null)
            {
                return true;
            }
            var parent = c1.parent;
            int i = c1.GetSiblingIndex()+1;
            while (i < parent.childCount)
            {
                if (parent.GetChild(i).name == c2.name)
                {
                    return true;
                }
            }
            return false;
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

        public static bool[] GetVisibilityUnion(Switcher uiSwitch)
        {
            bool[] union = new bool[uiSwitch.objs.Count];
            HashSet<List<bool>> set = new HashSet<List<bool>>();
            for (int i = 0; i < uiSwitch.switches.Count; ++i)
            {
                var s = uiSwitch.switches[i];
                if (set.Contains(s.visibility))
                {
                    uiSwitch.switches[i] = new SwitchSet();
                    EditorUtil.SetDirty(uiSwitch);
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
                    EditorUtil.SetDirty(uiSwitch);
                }
            }
        }
    }
}
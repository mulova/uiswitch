using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System;
#if !STANDALONE
using System.Collections.Generic.Ex;
using mulova.unicore;
using mulova.commons;
using UnityEditorInternal;
using System.Linq;
#endif

namespace mulova.ui
{
    [CustomEditor(typeof(UISwitch))]
    public class UISwitchInspector : Editor
    {
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
            uiSwitch = (UISwitch)target;
            EditorApplication.update += OnUpdate;
            changedTime = double.MaxValue;

            uiSwitch.showTrans = uiSwitch.switches.Find(s => s.trans.Count > 0) != null;
            uiSwitch.showAction = uiSwitch.switches.Find(s => s.action.GetPersistentEventCount() > 0) != null;
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
                            using (new ColorScope(Color.green, IsPreset(p.keys))) //
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
                        using (new ColorScope(Color.green, IsActive(s.name))) //
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
                            EditorUtil.SetDirty(t); //
                            EditorUtil.SetDirty(uiSwitch);
                        }
                    }
                }
            }
        }

        private ObjPropertyReorder<GameObject> diffList;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (uiSwitch.switches.Count == 0)
            {
                EditorGUILayout.Space(30);
                EditorGUILayout.Separator();

                if (diffList == null)
                {
                    diffList = new ObjPropertyReorder<GameObject>(serializedObject, "objs");
                    diffList.title = "Diff Roots";
                }
                serializedObject.Update();
                diffList.Draw();
                if (uiSwitch.objs.Count >= 2)
                {
                    if (GUILayout.Button("Extract Diff"))
                    {
                        CreateMissingObject();
                        EditorUtil.SetDirty(uiSwitch);
                        //diffList.serializedProperty.ClearArray();
                    }
                }
                serializedObject.ApplyModifiedProperties();
            } else if (uiSwitch.showTrans || uiSwitch.showAction)
            {
                EditorGUILayout.Space(30);
                EditorGUILayout.Separator();
                if (!uiSwitch.showTrans)
                {
                    if (GUILayout.Button("Show Transforms"))
                    {
                        uiSwitch.showTrans = true;
                    }
                }
                if (!uiSwitch.showAction)
                {
                    if (GUILayout.Button("Show Actions"))
                    {
                        uiSwitch.showAction = true;
                    }
                }
            }

        }

        private void CreateMissingObject()
        {
            var trans = uiSwitch.objs.ConvertAll(o=> o.transform);
            
            for (int i1=0; i1<trans.Count; ++i1)
            {
                for (int i2=0; i2 < trans.Count; ++i2)
                {
                    if (i1 == i2) { continue; }
                    for (int j=0; j<trans[i1].childCount; ++j)
                    {
                        var c1 = trans[i1].GetChild(j);
                        var c2 = j < trans[i2].childCount ? trans[i2].GetChild(j): null;
                        if (c1.name != c2?.name)
                        {
                            if (IsFirstExtra(c1, c2))
                            {
                                CloneSibling(c1, c2, trans[i2]);
                            }
                        }
                    }
                }
            }
        }

        private static void CloneSibling(Transform c1, Transform c2, Transform parent)
        {
            if (c1 != null)
            {
                var newChild1 = Instantiate(c1, parent, false);
                newChild1.name = c1.name;
                Undo.RegisterCreatedObjectUndo(newChild1.gameObject, c1.name);
                if (c2 != null)
                {
                    newChild1.SetSiblingIndex(c2.GetSiblingIndex());
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
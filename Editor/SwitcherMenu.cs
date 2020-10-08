using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if CORE_LIB
using System.Collections.Generic.Ex;
using System.Ex;
#endif

namespace mulova.switcher
{
    public static class SwitcherMenu
    {
        public static List<GameObject> sortedSelection
        {
            get
            {
                var roots = new List<GameObject>(Selection.gameObjects);
                roots.Sort((a, b) => a.transform.GetSiblingIndex() - b.transform.GetSiblingIndex());
                return roots;
            }
        }

        [MenuItem("GameObject/Switcher/Generate", true, 30)]
        public static bool IsCreateSwitcher()
        {
            foreach (var o in Selection.gameObjects)
            {
                if (o.TryGetComponent<Switcher>(out var s))
                {
                    return false;
                }
            }
            return Selection.gameObjects.Length > 1;
        }

        [MenuItem("GameObject/Switcher/Generate", false, 30)]
        public static void CreateSwitcher()
        {
            var selected = sortedSelection;
            var switchers = selected.FindAll(o => o.GetComponent<Switcher>() != null);
            if (switchers.Count == 0)
            {
                var pos = new List<TransformData>();
                var err = CreateSwitcher(selected);
                if (string.IsNullOrEmpty(err))
                {
                    for (int i = 1; i < selected.Count; ++i)
                    {
                        pos.Add(CompDataGenerator.instance.GetComponentData(selected[i].transform) as TransformData);
                        Undo.DestroyObjectImmediate(selected[i]);
                    }
                    SpreadOut(selected[0].GetComponent<Switcher>(), pos);
                    Selection.activeGameObject = selected[0];
                } else
                {
                    Debug.LogError(err);
                    EditorUtility.DisplayDialog("Error", "Check out the log for more detail", "OK");
                }
            } else if (switchers.Count != selected.Count)
            {
                // this method is called several times (the same count as the selected game object)
                EditorUtility.DisplayDialog("Error", "Remove switcher first", "OK");
            }
        }

        public static void SpreadOut(Switcher s, List<TransformData> pos = null)
        {
            for (int i=1; i<s.switches.Count; ++i)
            {
                var name = s.switches[i].name;
                var clone = Object.Instantiate(s, s.transform.parent);
                if (pos != null && pos.Count > i-1)
                {
                    pos[i - 1].ApplyTo(clone.transform);
                }
                Undo.RegisterCreatedObjectUndo(clone.gameObject, name);
                clone.SetKey(name);
                clone.name = name; 
            }
        }

        [MenuItem("GameObject/Switcher/Spread Out", true, 31)]
        public static bool IsSpreadOut()
        {
            return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Switcher>();
        }


        [MenuItem("GameObject/Switcher/Spread Out", false, 31)]
        public static void SpreadOut()
        {
            SpreadOut(Selection.activeGameObject.GetComponent<Switcher>());
        }

        public static string CreateSwitcher(List<GameObject> roots)
        {
            var duplicates = GameObjectDiff.GetDuplicateSiblingNames(roots);
            if (duplicates.Count > 0)
            {
                return "Duplicate sibling names: " + duplicates.Join(",");
            }
            else
            {
                if (!GameObjectDiff.IsChildrenMatches(roots.ConvertAll(o => o.transform)))
                {
                    var parents = roots.ConvertAll(o => o.transform);
                    GameObjectDiff.CreateMissingChildren(parents);
                }
                var err = GameObjectDiff.GetComponentMismatch(roots.ConvertAll(o => o.transform));
                if (err.Count == 0)
                {
                    Undo.RecordObjects(roots.ToArray(), "Diff");
                    ExtractDiff(roots);
                    return null;
                }
                else
                {
                    return "Component Mismatch\n" + err.Join(",");
                }
            }
        }

        private static void ExtractDiff(List<GameObject> roots)
        {
            // just set data for the first object
            var root0 = roots[0];
            var diffs = GameObjectDiff.CreateDiff(roots.ToArray());
            var tDiffs = GameObjectDiff.FindAll<TransformData>(diffs);
            // remove TransformData from diffs
            for (int i = 0; i < diffs.Length; ++i)
            {
                diffs[i] = diffs[i].FindAll(d => !(d is TransformData));
            }

            var ui = root0.GetComponent<Switcher>();
            if (ui == null)
            {
                ui = root0.AddComponent<Switcher>();
                Undo.RegisterCreatedObjectUndo(ui, root0.name);
            }
            // Get Visibility Diffs
            var vDiffs = new List<List<TransformData>>();
            for (int i = 0; i < tDiffs.Length; ++i)
            {
                vDiffs.Add(new List<TransformData>());
            }
            for (int c = 0; c < tDiffs[0].Count; ++c)
            {
                bool diff = false;
                for (int i = 1; i < tDiffs.Length && !diff; ++i)
                {
                    diff |= tDiffs[0][c].active != tDiffs[i][c].active;
                }
                if (diff)
                {
                    for (int i = 0; i < tDiffs.Length; ++i)
                    {
                        vDiffs[i].Add(tDiffs[i][c]);
                    }
                }
            }
            ui.objs = vDiffs[0].ConvertAll(v => v.target.gameObject);

            // Get Position Diffs
            var posDiffs = new List<List<TransformData>>();
            for (int i = 0; i < tDiffs.Length; ++i)
            {
                posDiffs.Add(new List<TransformData>());
            }
            for (int c = 0; c < tDiffs[0].Count; ++c)
            {
                bool diff = false;
                for (int i = 1; i < tDiffs.Length && !diff; ++i)
                {
                    diff |= !tDiffs[0][c].TransformEquals(tDiffs[i][c]);
                }
                if (diff)
                {
                    for (int i = 0; i < tDiffs.Length; ++i)
                    {
                        posDiffs[i].Add(tDiffs[i][c]);
                    }
                }
            }

            for (int i = 0; i < roots.Count; ++i)
            {
                var s = new SwitchSet();
                s.name = roots[i].name;
#if UNITY_2019_1_OR_NEWER
                s.data = diffs[i];
#endif
                s.trans = posDiffs[i].ConvertAll(t => t.trans);
                s.pos = posDiffs[i].ConvertAll(t => t.pos);
                s.visibility = vDiffs[i].ConvertAll(t => t.enabled);
                ui.switches.Add(s);
            }
            EditorUtility.SetDirty(ui);
            //diffList.serializedProperty.ClearArray();
        }
    }
}
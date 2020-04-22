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

        [MenuItem("GameObject/Switcher", true, 30)]
        public static bool IsCreateSwitcher()
        {
            return Selection.gameObjects.Length > 1;
        }

        [MenuItem("GameObject/Switcher", false, 30)]
        public static void CreateSwitcher()
        {
            CreateSwitcher(Selection.gameObjects);
            Selection.activeGameObject = Selection.gameObjects[0];
        }

        public static string CreateSwitcher(GameObject[] roots)
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
                    Undo.RecordObjects(roots, "Diff");
                    ExtractDiff(roots);
                    return null;
                }
                else
                {
                    return "Component Mismatch\n" + err.Join(",");
                }
            }
        }

        private static void ExtractDiff(GameObject[] roots)
        {
            // just set data for the first object
            var root0 = roots[0];
            var diffs = GameObjectDiff.CreateDiff(roots);
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

            for (int i = 0; i < roots.Length; ++i)
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
            //diffList.serializedProperty.ClearArray();
        }
    }
}
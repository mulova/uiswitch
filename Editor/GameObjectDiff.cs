using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Linq;
#if CORE_LIB
using System.Ex;
using System.Collections.Generic.Ex;
#endif

namespace mulova.switcher
{
    public static class GameObjectDiff
    {
        public static void CreateMissingChildren(IList<Transform> roots)
        {
            var children = GetChildUnion(roots);

            foreach (var p in roots)
            {
                int insertIndex = 0;
                for (int ic = 0; ic < children.Count; ++ic)
                {
                    var c = children[ic];
                    // find matching child
                    Transform found = null;
                    for (int i = 0; i < p.childCount && !found; ++i)
                    {
                        if (p.GetChild(i).name == c.name)
                        {
                            insertIndex = i + 1;
                            found = p.GetChild(i);
                        }
                    }
                    if (found == null)
                    {
                        var clone = CloneSibling(c, p, insertIndex);
                        clone.gameObject.SetActive(false);
                        ++insertIndex;
                    }
                }
            }

            // sort children
            for (int i=0; i<roots[0].childCount; ++i)
            {
                var c = roots[0].GetChild(i);

                var childRoots = new List<Transform>();
                for (int j=0; j < roots.Count; ++j)
                {
                    childRoots.Add(roots[j].GetChild(i));
                    if (j != 0)
                    {
                        roots[j].Find(c.name).SetSiblingIndex(i);
                    }
                }
                CreateMissingChildren(childRoots);
            }
        }

        public static List<ICompData>[] CreateDiff(GameObject[] roots)
        {
            var parents = roots.ConvertAll(o => o.transform);
            var store = parents.ConvertAll(p => new List<ICompData>());
            GetDiffRecursively(parents, store, true);
            return store;
        }

        private static void GetDiffRecursively(Transform[] parents, List<ICompData>[] store, bool isRoot)
        {
            var comps = parents.ConvertAll(p => p.GetComponents<Component>().ToArray());
            for (int i = 0; i < comps[0].Length; ++i)
            {
                GetComponentDiff(comps, i, store, isRoot);
            }
            // child diff
            for (int i=0; i < parents[0].childCount; ++i)
            {
                var children = parents.ConvertAll(p => p.GetChild(i));
                GetDiffRecursively(children, store, false);
            }
        }

        /// <summary>
        /// return Component data if all components' data are the same.
        /// </summary>
        /// <returns>The diff.</returns>
        /// <param name="comps">return Component data if all components' data are the same.</param>
        private static void GetComponentDiff(Component[][] comps, int index, List<ICompData>[] store, bool isRoot = false)
        {
            var arr = new ICompData[comps.Length];
            bool diff = false;
            for (int i = 0; i < arr.Length; ++i)
            {
                arr[i] = CompDataGenerator.instance.GetComponentData(comps[i][index], isRoot);
                if (i != 0)
                {
                    // Reset the position of root transform
                    if (isRoot && (arr[0] is TransformData))
                    {
                        if (arr[0].GetType() == typeof(RectTransformData))
                        {
                            var src = arr[0] as RectTransformData;
                            var dst = arr[i] as RectTransformData;
                            dst.anchoredPosition = src.anchoredPosition;
                        }
                        else
                        {
                            var src = arr[0] as TransformData;
                            var dst = arr[i] as TransformData;
                            dst.pos = src.pos;
                        }
                    }
                    if (!diff && arr[i] != null && !arr[i].Equals(arr[0]))
                    {
                        diff = true;
                    }
                }
            }
            if (diff)
            {
                for (int i = 0; i < arr.Length; ++i)
                {
                    arr[i].target = arr[0].target;
                    store[i].Add(arr[i]);
                }
            }
        }

        public static List<string> GetDuplicateSiblingNames(IList<GameObject> objs)
        {
            List<string> dup = new List<string>();
            foreach (var o in objs)
            {
                if (o != null)
                {
                    dup.AddRange(GetDuplicateSiblingNames(o.transform));
                }
            }
            return dup;

            List<string> GetDuplicateSiblingNames(Transform parent)
            {
                List<string> d = new List<string>();
                HashSet<string> names = new HashSet<string>();
                foreach (Transform child in parent)
                {
                    if (!names.Add(child.name))
                    {
                        d.Add(child.name);
                    }
                    d.AddRange(GetDuplicateSiblingNames(child));
                }
                return d;
            }
        }

        public static bool IsChildrenMatches(IList<Transform> objs)
        {
            int childCount = objs[0].childCount;
            for (int i = 1; i < objs.Count; ++i)
            {
                if (objs[i].childCount != childCount)
                {
                    return false;
                }
            }
            Transform[] children = new Transform[objs.Count];
            for (int c=0; c<childCount; ++c)
            {
                for (int i=0; i<objs.Count; ++i)
                {
                    children[i] = objs[i].GetChild(c);
                    if (i != 0 && children[0].name != children[i].name)
                    {
                        return false;
                    }
                }
                if (!IsChildrenMatches(children))
                {
                    return false;
                }
            }
            return true;
        }

        public static List<string> GetComponentMismatch(IList<Transform> objs)
        {
            List<string> err = new List<string>();
            var c0 = objs[0].GetComponents<Component>();
            for (int i=1; i<objs.Count; ++i)
            {
                var c = objs[i].GetComponents<Component>();
                if (c0.Length != c.Length)
                {
                    err.Add($"Component Count Mismatch '{objs[0].name}': {c0.Length-1} vs '{objs[i].name}': {c.Length-1}");
                } else
                {
                    for (int j=0; j<c.Length; ++j)
                    {
                        if (c0[j].GetType() != c[j].GetType())
                        {
                            err.Add($"{c0[j].name}.{c0[j].GetType().Name} <-> {c[j].name}.{c[j].GetType().Name}");
                        }
                    }
                }
            }
            for (int c=0; c<objs[0].childCount; ++c)
            {
                List<Transform> children = new List<Transform>();
                for (int i=0; i<objs.Count; ++i)
                {
                    children.Add(objs[i].GetChild(c));
                }
                err.AddRange(GetComponentMismatch(children));
            }
            return err;
        }

        public static int GetSiblingIndex(string objName, Transform parent)
        {
            for (int i = 0; i < parent.childCount; ++i)
            {
                var c = parent.GetChild(i);
                if (c.name == objName)
                {
                    return i;
                }
            }
            return -1;
        }

        public static Transform CloneSibling(Transform c1, Transform parent, int siblingIndex)
        {
            if (c1 != null)
            {
                var child = Object.Instantiate(c1, parent, false);
                child.name = c1.name;
                Undo.RegisterCreatedObjectUndo(child.gameObject, c1.name);
                child.SetSiblingIndex(siblingIndex);
                return child;
            }
            return null;
        }

        private static List<Transform> GetChildUnion(IList<Transform> parents)
        {
            List<Transform> sorted = new List<Transform>(parents);
            sorted.Sort(SortByChildCount);

            int SortByChildCount(Transform t1, Transform t2)
            {
                return t2.childCount - t1.childCount;
            }

            List<Transform> names = new List<Transform>();
            foreach (var p in parents)
            {
                for (int i = 0; i < p.childCount; ++i)
                {
                    var c = p.GetChild(i);
                    int index = names.FindIndex(t => t.name == c.name);
                    if (index < 0)
                    {
                        for (var j = i + 1; j < p.childCount && index < 0; ++j)
                        {
                            index = names.FindIndex(t => t.name == p.GetChild(j).name);
                        }
                        if (index < 0)
                        {
                            names.Add(c);
                        }
                        else
                        {
                            names.Insert(index, c);
                        }
                    }
                }
            }
            return names;
        }

        internal static List<T>[] FindAll<T>(List<ICompData>[] diffs) where T : ICompData
        {
            var list = new List<T>[diffs.Length];
            for (int i=0; i<diffs.Length; ++i)
            {
                list[i] = diffs[i].FindAll(c => c is T).ConvertAll(t => (T)t);
            }
            return list;
        }
    }
}
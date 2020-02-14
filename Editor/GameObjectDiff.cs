using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Ex;
using System;
using Object = UnityEngine.Object;
#if !STANDALONE
using System.Collections.Generic.Ex;
#endif

namespace mulova.ui
{
    public static class GameObjectDiff
    {
        public static void CreateMissingSiblings(List<GameObject> roots)
        {
            var parents = roots.ConvertAll(o => o.transform);
            var children = GetChildUnion(parents);

            foreach (var p in parents)
            {
                int insertIndex = 0;
                for (int ic = 0; ic < children.Count; ++ic)
                {
                    var c = children[ic];
                    // find matching child
                    bool found = false;
                    for (int i = 0; i < p.childCount && !found; ++i)
                    {
                        if (p.GetChild(i).name == c.name)
                        {
                            insertIndex = i + 1;
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        var clone = CloneSibling(c, p, insertIndex);
                        clone.gameObject.SetActive(false);
                        ++insertIndex;
                    }
                }
            }
        }

        public static List<List<ICompData>> CreateDiff(List<GameObject> roots)
        {
            var parents = roots.ConvertAll(o => o.transform);
            var store = parents.ConvertAll(p => new List<ICompData>());
            GetDiffRecursively(parents, store, false);
            return store;
        }

        private static void GetDiffRecursively(List<Transform> parents, List<List<ICompData>> store, bool objDiff = true)
        {
            var comps = parents.ConvertAll(p => p.GetComponents<Component>());
            if (objDiff)
            {
                GetObjDiff(parents, store);
            }
            for (int i = 0; i < comps[0].Length; ++i)
            {
                GetComponentDiff(comps, i, store);
            }
            for (int i=0; i < parents[0].childCount; ++i)
            {
                var children = parents.ConvertAll(p => p.GetChild(i));
                GetDiffRecursively(children, store);
            }
        }

        /// <summary>
        /// return Component data if all components' data are the same.
        /// </summary>
        /// <returns>The diff.</returns>
        /// <param name="comps">return Component data if all components' data are the same.</param>
        private static void GetComponentDiff(List<Component[]> comps, int index, List<List<ICompData>> store)
        {
            var arr = new ICompData[comps.Count];
            bool diff = false;
            for (int i = 0; i < arr.Length; ++i)
            {
                arr[i] = GetComponentData(comps[i][index], comps[i][index].GetType());
                if (!diff && i != 0 && !arr[i].Equals(arr[0]))
                {
                    diff = true;
                }
            }
            if (diff)
            {
                for (int i=0; i<arr.Length; ++i)
                {
                    arr[i].target = arr[0].target;
                    store[i].Add(arr[i]);
                }
            }
        }

        private static void GetObjDiff(List<Transform> objs, List<List<ICompData>> store)
        {
            var arr = new ICompData[objs.Count];
            bool diff = false;
            for (int i = 0; i < arr.Length; ++i)
            {
                arr[i] = GetComponentData(objs[i], typeof(GameObject));
                if (!diff && i != 0 && !arr[i].Equals(arr[0]))
                {
                    diff = true;
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

        private static Dictionary<Type, Type> pool;
        private static ICompData GetComponentData(Component c, Type type)
        {
            // collect BuildProcessors
            if (pool == null)
            {
                pool = new Dictionary<Type, Type>();
                List<Type> cls = typeof(ICompData).FindTypes();
                foreach (Type t in cls)
                {
                    if (!t.IsAbstract)
                    {
                        var ins = Activator.CreateInstance(t) as ICompData;
                        pool[ins.type] = t;
                    }
                }
            }
            var dataType = pool.Get(type);
            if (dataType != null)
            {
                var o = Activator.CreateInstance(dataType) as ICompData;
                o.Collect(c);
                return o;
            } else
            {
                return null;
            }
        }

        public static List<string> GetDuplicateSiblingNames(List<GameObject> objs)
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

        public static List<string> GetComponentMismatch(List<Transform> objs)
        {
            List<string> err = new List<string>();
            var c0 = objs[0].GetComponents<Component>();
            for (int i=1; i<objs.Count; ++i)
            {
                var c = objs[i].GetComponents<Component>();
                if (c0.Length != c.Length)
                {
                    err.Add($"{objs[0].name}({c0.Length}) <-> {objs[i].name}({c.Length})");
                } else
                {
                    for (int j=0; j<c.Length; ++j)
                    {
                        if (c0[j].GetType() != c[j].GetType())
                        {
                            err.Add($"{objs[0].name}.{c0[j].GetType().Name} <-> {c[j].GetType().Name}");
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

        private static List<Transform> GetChildUnion(List<Transform> parents)
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

        internal static List<List<T>> FindAll<T>(List<List<ICompData>> diffs) where T : ICompData
        {
            var list = new List<List<T>>();
            foreach (var d in diffs)
            {
                var filtered = d.FindAll(c => c is T).ConvertAll(t => (T)t);
                list.Add(filtered);
            }
            return list;
        }
    }
}
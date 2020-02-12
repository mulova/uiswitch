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
        public static List<List<ICompData>> CreateDiff(List<GameObject> roots)
        {
            var parents = roots.ConvertAll(o => o.transform);
            var store = parents.ConvertAll(p => new List<ICompData>());
            GetDiffRecursively(parents, store);
            return store;
        }

        private static void GetDiffRecursively(List<Transform> parents, List<List<ICompData>> store)
        {
            var comps = parents.ConvertAll(p => p.GetComponents<Component>());
            for (int i = 0; i < comps[0].Length; ++i)
            {
                GetDiff(comps, i, store);
            }
            for (int i=0; i < parents[0].childCount; ++i)
            {
                var children = parents.ConvertAll(p => p.GetChild(i));
                GetDiffRecursively(children, store);
            }
        }

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


        /// <summary>
        /// return Component data if all components' data are the same.
        /// </summary>
        /// <returns>The diff.</returns>
        /// <param name="comps">return Component data if all components' data are the same.</param>
        private static void GetDiff(List<Component[]> comps, int index, List<List<ICompData>> store)
        {
            var arr = new ICompData[comps.Count];
            bool diff = false;
            for (int i = 0; i < arr.Length; ++i)
            {
                arr[i] = GetComponentData(comps[i][index]);
                if (!diff && i != 0 && !arr[i].Equals(arr[0]))
                {
                    diff = true;
                }
            }
            if (diff)
            {
                for (int i=0; i<arr.Length; ++i)
                {
                    store[i].Add(arr[i]);
                }
            }
        }

        private static Dictionary<Type, Type> pool;
        private static ICompData GetComponentData(Component c)
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
            var dataType = pool.Get(c.GetType());
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
        }

        public static List<string> GetDuplicateSiblingNames(Transform parent)
        {
            List<string> dup = new List<string>();
            HashSet<string> names = new HashSet<string>();
            foreach (Transform child in parent)
            {
                if (!names.Add(child.name))
                {
                    dup.Add(child.name);
                }
                dup.AddRange(GetDuplicateSiblingNames(child));
            }
            return dup;
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
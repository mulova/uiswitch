using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if !STANDALONE
using System.Collections.Generic.Ex;
using System.Linq;
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

        public static void CreateDiff(List<Component> comps)
        {
            foreach (var c in comps)
            {
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
    }
}
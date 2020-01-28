#if STANDALONE
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace mulova.ui
{
    internal static class EditorUtil
    {
        public static ILogger log = Debug.unityLogger;

        public static void SetDirty(Object o)
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

        public static string GetScenePath(this Transform trans)
        {
            var (path, _, _) = trans.GetScenePath(null);
            return path;
        }

        public static (string, string[], int[]) GetScenePath(this Transform trans, Transform root)
        {
            Stack<Transform> stack = new Stack<Transform>();
            Transform t = trans;
            while (t != null && t != root)
            {
                stack.Push(t);
                t = t.parent;
            }
            var paths = new string[stack.Count];
            var index = new int[stack.Count];
            StringBuilder str = new StringBuilder();
            int i = 0;
            while (stack.Count > 0)
            {
                var s = stack.Pop();
                paths[i] = s.name;
                index[i] = s.GetSiblingIndex();
                str.Append('/').Append(s.name);
            }
            return (str.ToString(), paths, index);
        }

        public static float Interpolate(this float val, float min, float max)
        {
            float diff = max - min;
            float inter = diff * val;
            return min + inter;
        }

        public static List<Type> FindClasses(this Type type)
        {
            List<Type> found = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    foreach (Type t in assembly.GetTypes())
                    {
                        if (type.IsAssignableFrom(t))
                        {
                            found.Add(t);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
            return found;
        }
    }
}
#endif
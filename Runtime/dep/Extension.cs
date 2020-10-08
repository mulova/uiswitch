#if !CORE_LIB
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace mulova.switcher
{
    public static class Extension
    {
        public static ILogger log = Debug.unityLogger;
        public static void AddAll<T>(this HashSet<T> hashSet, IEnumerable<T> objs)
        {
            foreach (T t in objs)
            {
                hashSet.Add(t);
            }
        }

        public static bool IsEmpty<T>(this ICollection<T> col)
        {
            if (col == null)
            {
                return true;
            }
            return col.Count <= 0;
        }

        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static Rect[] SplitByHeights(this Rect src, params int[] heights)
        {
            Rect[] rects = new Rect[heights.Length + 1];
            for (int i = 0; i < heights.Length; ++i)
            {
                rects[i] = src;
                rects[i].height = heights[i];
                if (i > 0)
                {
                    rects[i].y = rects[i - 1].y + heights[i - 1];
                }
            }
            rects[heights.Length] = src;
            rects[heights.Length].y = rects[heights.Length - 1].y + heights[heights.Length - 1];
            rects[heights.Length].height = src.x + src.height - rects[heights.Length].x;
            return rects;
        }

        public static V Get<K, V>(this IDictionary<K, V> dict, K key)
        {
            if (dict != null && key != null)
            {
                V val = default(V);
                if (dict.TryGetValue(key, out val))
                {
                    return val;
                }
            }
            return default(V);
        }

        public static V Get<K, V>(this IDictionary<K, V> dict, K key, V defaultVal)
        {
            V val = defaultVal;
            if (dict != null && key != null && dict.TryGetValue(key, out val))
            {
                return val;
            }
            return defaultVal;
        }

        public static Rect[] SplitByWidthsRatio(this Rect src, params float[] ratio)
        {
            Rect[] rects = new Rect[ratio.Length];
            for (int i = 0; i < rects.Length; ++i)
            {
                rects[i] = src;
                if (i > 0)
                {
                    rects[i].x = rects[i - 1].x + ratio[i - 1] * src.width;
                }
                rects[i].width = src.width * ratio[i];
            }
            return rects;
        }

        public static Rect[] SplitByWidths(this Rect src, params int[] widths)
        {
            Rect[] rects = new Rect[widths.Length + 1];
            for (int i = 0; i < widths.Length; ++i)
            {
                rects[i] = src;
                rects[i].width = widths[i];
                if (i > 0)
                {
                    rects[i].x = rects[i - 1].x + widths[i - 1];
                }
            }
            rects[widths.Length] = src;
            rects[widths.Length].x = rects[widths.Length - 1].x + widths[widths.Length - 1];
            rects[widths.Length].width = src.x + src.width - rects[widths.Length].x;
            return rects;
        }

        public static T Find<T>(this IEnumerable<T> src, Predicate<T> predicate)
        {
            if (src != null && predicate != null)
            {
                foreach (T t in src)
                {
                    if (predicate(t))
                    {
                        return t;
                    }
                }
            }
            return default(T);
        }

        public static int GetCount<T>(this IList<T> list)
        {
            if (list == null)
            {
                return 0;
            }
            return list.Count;
        }

        public static T[] ShallowClone<T>(this T[] source)
        {
            if (source == null)
            {
                return null;
            }
            T[] dest = (T[])Array.CreateInstance(typeof(T), source.Length);
            Array.Copy(source, dest, source.Length);
            return dest;
        }

        public static bool EqualsIgnoreCase(this string str1, string str2)
        {
            if (str1 == null)
            {
                return false;
            }
            return str1.Equals(str2, StringComparison.OrdinalIgnoreCase);
        }

        public static string Join(this IEnumerable list, string separator)
        {
            int count = 0;
            StringBuilder str = new StringBuilder(256);
            foreach (object o in list)
            {
                if (o != null)
                {
                    if (count != 0)
                    {
                        str.Append(separator);
                    }
                    string t = o.ToString();
                    if (!t.IsEmpty())
                    {
                        str.Append(t);
                        count++;
                    }
                }
            }
            return str.ToString();
        }

        public static List<T> FindAll<T>(this IEnumerable<T> src, Predicate<T> predicate)
        {
            List<T> dst = new List<T>();
            if (src != null)
            {
                foreach (T t in src)
                {
                    if (predicate(t))
                    {
                        dst.Add(t);
                    }
                }
            }
            return dst;
        }

        public static List<Type> FindTypes(this Type type)
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

        public static bool ApproximatelyEquals(this Vector3 v1, Vector3 v2)
        {
            return Mathf.Approximately(v1.x, v2.x)
                && Mathf.Approximately(v1.y, v2.y)
                && Mathf.Approximately(v1.z, v2.z);
        }

        public static bool ApproximatelyEquals(this Vector2 v1, Vector2 v2)
        {
            return Mathf.Approximately(v1.x, v2.x)
                && Mathf.Approximately(v1.y, v2.y);
        }

        public static bool ApproximatelyEquals(this Quaternion first, Quaternion second)
        {
            return (Mathf.Approximately(first.x, second.x) && Mathf.Approximately(first.y, second.y)
                && Mathf.Approximately(first.z, second.z) && Mathf.Approximately(first.w, second.w)) ||
                (Mathf.Approximately(first.x, -second.x) && Mathf.Approximately(first.y, -second.y)
                    && Mathf.Approximately(first.z, -second.z) && Mathf.Approximately(first.w, -second.w));
        }

        public static D[] ConvertAll<S, D>(this S[] src) where D:S {
            if (src == null) {
                return null;
            }
            D[] dst = new D[src.Length];
            for (int i=0; i<src.Length; ++i) {
                dst[i] = (D)src[i];
            }
            return dst;
        }
        
        public static D[] ConvertAll<S, D>(this S[] src, Converter<S, D> converter) {
            if (src == null) {
                return null;
            }
            return Array.ConvertAll(src, converter);
        }
    }
}


#endif
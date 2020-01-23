﻿#if STANDALONE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class Extension
{
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

    #region Logger
    public const string TAG = "Unity";

    public static bool IsLoggable(this ILogger l, LogType type)
    {
        if (!l.logEnabled)
        {
            return false;
        }
        return l.IsLogTypeAllowed(type);
    }

    public static void Debug(this ILogger l, string format, object arg1)
    {
        if (!l.IsLogTypeAllowed(LogType.Log))
        {
            return;
        }
        l.Log(TAG, string.Format(format, arg1));
    }

    public static void Debug(this ILogger l, string format, object arg1, object arg2)
    {
        if (!l.IsLogTypeAllowed(LogType.Log))
        {
            return;
        }
        l.Log(TAG, string.Format(format, arg1, arg2));
    }

    public static void Debug(this ILogger l, string format, object arg1, object arg2, object arg3)
    {
        if (!l.IsLogTypeAllowed(LogType.Log))
        {
            return;
        }
        l.Log(TAG, string.Format(format, arg1, arg2, arg3));
    }

    public static void Debug(this ILogger l, string format, params object[] args)
    {
        if (!l.IsLoggable(LogType.Log))
        {
            return;
        }
        l.LogFormat(LogType.Log, format, args);
    }

    public static void Warn(this ILogger l, string format, object arg1)
    {
        if (!l.IsLogTypeAllowed(LogType.Warning))
        {
            return;
        }
        l.Log(TAG, string.Format(format, arg1));
    }

    public static void Warn(this ILogger l, string format, object arg1, object arg2)
    {
        if (!l.IsLogTypeAllowed(LogType.Warning))
        {
            return;
        }
        l.Log(TAG, string.Format(format, arg1, arg2));
    }

    public static void Warn(this ILogger l, string format, object arg1, object arg2, object arg3)
    {
        if (!l.IsLogTypeAllowed(LogType.Warning))
        {
            return;
        }
        l.Log(TAG, string.Format(format, arg1, arg2, arg3));
    }

    public static void Warn(this ILogger l, string format, params object[] args)
    {
        if (!l.IsLoggable(LogType.Warning))
        {
            return;
        }
        l.LogFormat(LogType.Warning, format, args);
    }

    public static void Error(this ILogger l, string format, object arg1)
    {
        if (!l.IsLogTypeAllowed(LogType.Error))
        {
            return;
        }
        l.Log(TAG, string.Format(format, arg1));
    }

    public static void Error(this ILogger l, string format, object arg1, object arg2)
    {
        if (!l.IsLogTypeAllowed(LogType.Error))
        {
            return;
        }
        l.Log(TAG, string.Format(format, arg1, arg2));
    }

    public static void Error(this ILogger l, string format, object arg1, object arg2, object arg3)
    {
        if (!l.IsLogTypeAllowed(LogType.Error))
        {
            return;
        }
        l.Log(TAG, string.Format(format, arg1, arg2, arg3));
    }

    public static void Error(this ILogger l, string format, params object[] args)
    {
        if (!l.IsLoggable(LogType.Error))
        {
            return;
        }
        l.LogFormat(LogType.Error, format, args);
    }

    public static void Error(this ILogger l, Exception ex)
    {
        if (!l.IsLoggable(LogType.Exception))
        {
            return;
        }
        l.LogException(ex);
    }
    #endregion
}

#endif
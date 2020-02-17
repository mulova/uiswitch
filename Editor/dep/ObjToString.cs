#if !CORE_LIB
using System;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace mulova.ui
{
    public delegate string ToStr(object o);
    public delegate Object ToObject(object o);

    internal class ObjToString
    {

        private ToStr toString;
        private ToObject toObject;
        private bool showAsTooltip;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToStringRow"/> class.
        /// </summary>
        /// <param name="toStr">use ScenePathToString when null</param>
        /// <param name="toObj">use default casting when null</param>
        public ObjToString(ToStr toStr, ToObject toObj, bool showAsTooltip)
        {
            this.toString = toStr != null ? toStr : ScenePathToString;
            this.toObject = toObj != null ? toObj : DefaultToObject;
            this.showAsTooltip = showAsTooltip;
        }

        /// <summary>
        /// </summary>
        /// <returns><c>true</c>, if row was drawn, <c>false</c> otherwise.</returns>
        /// <param name="obj">Object.</param>
        /// <param name="filter">Filter.</param>
        public string DrawRow<T>(T obj)
        {
            string name = toString(obj);
            Object o = toObject(obj);

            if (o != null)
            {
                if (showAsTooltip)
                {
                    EditorGUILayout.ObjectField(o, o.GetType(), true, GUILayout.ExpandWidth(true));
                }
                else
                {
                    EditorGUILayout.ObjectField(o, o.GetType(), true, GUILayout.Width(100));
                    if (GUILayout.Button(name, EditorStyles.miniLabel))
                    {
                        EditorGUIUtility.PingObject(o);
                        Selection.activeObject = o;
                    }
                }
            }
            else if (!name.IsEmpty())
            {
                EditorGUILayout.LabelField(name, EditorStyles.miniLabel);
            }
            return name;
        }

        public static Object DefaultToObject(object o)
        {
            if (o is NamedObj)
            {
                return (o as NamedObj).Obj;
            }
            else if (o is Object)
            {
                return o as Object;
            }
            return null;
        }

        /// <summary>
        /// Get asset path or scene path from the object
        /// </summary>
        /// <returns>convert object to asset path / scene path. if none, return o.ToString() </returns>
        /// <param name="o">O.</param>
        public static string ScenePathToString(object o)
        {
            if (o == null)
            {
                return string.Empty;
            }
            try
            {
                if (o is AnimationClip)
                {
                    return (o as AnimationClip).name;
                }
                else if (o is Object)
                {
                    string str = AssetDatabase.GetAssetPath(o as Object);
                    if (!str.IsEmpty())
                    {
                        return str;
                    }
                    else
                    {
                        if (o is GameObject)
                        {
                            return (o as GameObject).transform.GetScenePath();
                        }
                        else if (o is Component)
                        {
                            Component c = o as Component;
                            if (c == null || c.gameObject == null)
                            {
                                return string.Empty;
                            }
                            return c.transform.GetScenePath();
                        }
                    }
                }
                else if (o is NamedObj)
                {
                    return (o as NamedObj).Name;
                }
                return DefaultToString(o);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return string.Empty;
            }
        }

        public static string DefaultToString(object o)
        {
            if (o == null)
            {
                return string.Empty;
            }
            if (o is Object)
            {
                Object obj = o as Object;
                if (obj != null)
                {
                    if (o is GameObject)
                    {
                        return (o as GameObject).name;
                    }
                    else if (o is Component)
                    {
                        Component c = o as Component;
                        if (c == null || c.gameObject == null)
                        {
                            return string.Empty;
                        }
                        return c.name;
                    }
                    else
                    {
                        return (o as Object).name;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            else if (o is NamedObj)
            {
                return (o as NamedObj).Name;
            }
            return o.ToString();
        }
    }
}

#endif
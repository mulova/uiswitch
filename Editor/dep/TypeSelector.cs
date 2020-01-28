#if STANDALONE
using System;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;
using System.Collections.Generic;

namespace mulova.ui
{
    internal class TypeSelector
    {
        public bool fullNameSearch = false;
        private string typeName;
        public Type type = typeof(Object);
        private Dictionary<string, Type> types;
        private List<Type> typeMatches = new List<Type>();
        private Type[] typeMatchesArr = new Type[0];

        public TypeSelector(Type baseType)
        {
            SetBaseType(baseType);
        }

        public void SetSelected(Type type)
        {
            if (type != null)
            {
                this.type = type;
                this.typeName = type.FullName;
            }
        }

        public void SetBaseType(Type baseType)
        {
            List<Type> classes = baseType.FindClasses();
            types = new Dictionary<string, Type>();
            foreach (Type t in classes)
            {
                types[t.FullName] = t;
            }
        }

        public bool DrawSelector(ref string typeStr)
        {
            typeName = typeStr;
            bool ret = DrawSelector();
            typeStr = typeName;
            return ret;
        }

        /// <summary>
        /// Draws the selector.
        /// </summary>
        /// <returns><c>true</c>, if selection was changed, <c>false</c> otherwise.</returns>
        public bool DrawSelector()
        {
            Type oldType = type;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Type", GUILayout.Width(50));
            if (((type == null && EditorGUILayoutUtil.TextField(null, ref typeName))
            || (type != null && EditorGUILayoutUtil.TextField(null, ref typeName, EditorStyles.toolbarTextField)))
            && typeName.Length > 0)
            {
                type = types.Get(typeName);
                if (!typeName.IsEmpty() && typeName.Length >= 2)
                {
                    typeMatches.Clear();
                    foreach (KeyValuePair<string, Type> pair in types)
                    {
                        var index = pair.Key.LastIndexOf(typeName, StringComparison.OrdinalIgnoreCase);
                        if (index >= 0 && (fullNameSearch
                        || (!fullNameSearch && index >= pair.Key.LastIndexOf('.'))))
                        {
                            typeMatches.Add(pair.Value);
                        }
                    }
                    typeMatchesArr = typeMatches.ToArray();
                }
            }
            Type newType = type;
            EditorGUILayoutUtil.PopupNullable<Type>(null, ref newType, typeMatchesArr);
            if (newType != null)
            {
                type = newType;
            }
            EditorGUILayout.EndHorizontal();
            return oldType != type;
        }

        public bool DrawSelector(Rect bound, SerializedProperty p)
        {
            Type oldType = type;
            var bounds = bound.SplitByWidths((int)EditorGUIUtility.labelWidth, (int)(bound.width - EditorGUIUtility.labelWidth) / 2);
            EditorGUI.LabelField(bounds[0], p.displayName);
            var textStyle = type != null ? EditorStyles.toolbarTextField : EditorStyles.textField;
            p.stringValue = EditorGUI.TextField(bounds[1], p.stringValue, textStyle);
            if (typeName != p.stringValue && p.stringValue.Length > 0)
            {
                typeName = p.stringValue;
                type = types.Get(typeName);
                if (!typeName.IsEmpty() && typeName.Length >= 2)
                {
                    typeMatches.Clear();
                    foreach (KeyValuePair<string, Type> pair in types)
                    {
                        var index = pair.Key.LastIndexOf(typeName, StringComparison.OrdinalIgnoreCase);
                        bool exactMatch = index == 0 && typeName.Length == pair.Key.Length;
                        bool fullNameFound = index >= 0 && fullNameSearch;
                        bool nameFound = index >= 0 && !fullNameSearch && index >= pair.Key.LastIndexOf('.');
                        if (exactMatch || fullNameFound || nameFound)
                        {
                            typeMatches.Add(pair.Value);
                        }
                    }
                    typeMatchesArr = typeMatches.ToArray();
                }
            }
            if (EditorGUIUtil.PopupNullable<Type>(bounds[2], null, ref type, typeMatchesArr))
            {
                if (type != null)
                {
                    p.stringValue = type.FullName;
                }
                else
                {
                    p.stringValue = "";
                }
            }
            return oldType != type;
        }
    }
}

#endif
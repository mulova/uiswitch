#if !CORE_LIB
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;
using Object = UnityEngine.Object;

namespace mulova.switcher
{
    internal class ObjPropertyReorder<T> : PropertyReorder<T> where T : Object
    {
        public bool allowSceneObjects = true;
        public bool allowDuplicate = false;
        public bool editable = true;

        public ObjPropertyReorder(SerializedObject ser, string varName, bool allowSceneObjects = true) : base(ser, varName) {
            this.allowSceneObjects = allowSceneObjects;
            this.setItem = SetItem;
            this.getItem = GetItem;
            this.createItems = CreateItems;
            this.canAdd = CanAdd;
        }

        private bool CanAdd()
        {
            if (allowDuplicate)
            {
                return true;
            }
            var sel = CreateItems();
            if (sel != null)
            {
                for (int i=0; i<count; ++i)
                {
                    foreach (var s in sel)
                    {
                        if (s == this[i])
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private IList<T> Convert(IList<Object> objs)
        {
            var list = new List<T>();
            if (objs != null)
            {
                foreach (var o in objs)
                {
                    if (o is T)
                    {
                        list.Add(o as T);
                    }
                    else if (o is GameObject)
                    {
                        list.Add((o as GameObject).GetComponent<T>());
                    }
                    else
                    {
                        list.Add(default(T));
                    }
                }
            }
            return list;
        }

        private IList<T> CreateItems()
        {
            return Convert(Selection.objects);
        }

        private T GetItem(SerializedProperty p)
        {
            return p.objectReferenceValue as T;
        }

        private void SetItem(SerializedProperty p, T value)
        {
            p.objectReferenceValue = value;
        }

        protected override void DrawItem(SerializedProperty item, Rect rect, int index, bool isActive, bool isFocused)
        {
            var o1 = this[index];
            var o2 = EditorGUI.ObjectField(rect, o1, typeof(T), allowSceneObjects);
            if (editable && o1 != o2)
            {
                this[index] = o2 as T;
            }
        }
    }
}


#endif
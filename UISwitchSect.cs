//----------------------------------------------
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System;
using mulova.commons;
using System.Text.Ex;
using System.Ex;

namespace uiswitch
{
    [System.Serializable]
    public struct UISwitchSect : ICloneable
    {
        [EnumPopup("enumType")] public string name;
        public List<bool> visibility;
        public Transform[] trans;
        public Vector3[] pos;
        [NonSerialized] public Action action;

        public bool isValid
        {
            get
            {
                return !name.IsEmpty() && visibility != null;
            }
        }

        public object Clone()
        {
            UISwitchSect e = new UISwitchSect();
            e.name = this.name;
            e.visibility = new List<bool>(visibility);
            e.trans = (Transform[])trans.Clone();
            e.pos = (Vector3[])pos.Clone();
            return e;
        }

        public override string ToString()
        {
            return name;
        }

        internal GameObject FindObject(List<GameObject> objs, Func<GameObject, bool> predicate)
        {
            for (int i = 0; i < visibility.Count; ++i)
            {
                if (visibility[i] && predicate(objs[i]))
                {
                    return objs[i];
                }
            }
            return null;
        }

        internal void ForEach(List<GameObject> objs, Action<GameObject> func)
        {
            for (int i = 0; i < visibility.Count; ++i)
            {
                if (visibility[i])
                {
                    func(objs[i]);
                }
            }
        }

        internal void RemoveAt(int i)
        {
            visibility.RemoveAt(i);
        }
    }
}
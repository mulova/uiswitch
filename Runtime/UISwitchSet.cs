//----------------------------------------------
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if !STANDALONE
using System.Ex;
using System.Text.Ex;
using mulova.unicore;
#endif

namespace mulova.ui
{
    [Serializable]
    public class UISwitchSet : ICloneable
    {
        [EnumPopup("enumType")] public string name;
        public List<bool> visibility = new List<bool>();
        public List<Transform> trans = new List<Transform>();
        public List<Vector3> pos = new List<Vector3>();
        public UnityEvent action;

        public bool isValid
        {
            get
            {
                return !name.IsEmpty() && visibility != null;
            }
        }

        public object Clone()
        {
            UISwitchSet e = new UISwitchSet();
            e.name = this.name;
            e.visibility = new List<bool>(visibility);
            e.trans = new List<Transform>(trans);
            e.pos = new List<Vector3>(pos);
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
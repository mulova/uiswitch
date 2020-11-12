//----------------------------------------------
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if CORE_LIB
using System.Ex;
using System.Text.Ex;
using mulova.unicore;
#endif

namespace mulova.switcher
{
    [Serializable]
    public class SwitchSet : ICloneable
    {
        [EnumPopup("enumType")] public string name;
        public List<bool> visibility = new List<bool>();
        public List<Transform> trans = new List<Transform>();
        public List<Vector3> pos = new List<Vector3>();
#if UNITY_2019_1_OR_NEWER
        [SerializeReference] [SerializeReferenceButton] public List<ICompData> data = new List<ICompData>();
#endif
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
            SwitchSet e = new SwitchSet();
            e.name = this.name;
            e.visibility = new List<bool>(visibility);
            e.trans = new List<Transform>(trans);
            e.pos = new List<Vector3>(pos);
#if UNITY_2019_1_OR_NEWER
            e.data = new List<ICompData>(data);
#endif
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
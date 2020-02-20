﻿using System;
using UnityEngine;

namespace mulova.ui
{
    [Serializable]
    public class ObjData : ICompData
    {
        public bool active;
        public GameObject obj;

        public Type type => typeof(GameObject);

        public Component target
        {
            get { return obj.transform; }
            set { obj = value.gameObject; }
        }

        public void ApplyTo(Component c)
        {
            var o = c.gameObject;
            o.SetActive(active);
        }

        public void Collect(Component c)
        {
            obj = c.gameObject;
            active = obj.gameObject.activeSelf;
        }

        public override bool Equals(object obj)
        {
            var that = (ObjData)obj;
            return this.active == that.active;
        }

        public override int GetHashCode()
        {
            return active.GetHashCode();
        }
    }
}


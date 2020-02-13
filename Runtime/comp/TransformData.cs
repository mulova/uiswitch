using System;
using UnityEngine;

namespace mulova.ui
{
    [Serializable]
    public struct TransformData : ICompData
    {
        public Vector3 pos;
        public bool active;
        public Transform trans;

        public Type type => typeof(Transform);

        public Component target
        {
            get { return trans; }
            set { trans = value as Transform; }
        }

        public void ApplyTo(Component c)
        {
            var t = c as Transform;
            t.localPosition = pos;
        }

        public void Collect(Component c)
        {
            trans = c as Transform;
            pos = trans.localPosition;
            active = trans.gameObject.activeSelf;
        }

        public override bool Equals(object obj)
        {
            var that = (TransformData)obj;
            return this.pos == that.pos && this.active == that.active;
        }

        public override int GetHashCode()
        {
            return pos.GetHashCode() + active.GetHashCode();
        }
    }
}


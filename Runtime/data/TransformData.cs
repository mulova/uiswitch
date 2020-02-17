using System;
using UnityEngine;

namespace mulova.ui
{
    [Serializable]
    public class TransformData : ICompData
    {
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
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
            t.localRotation = rot;
            t.localScale = scale;
        }

        public void Collect(Component c)
        {
            trans = c as Transform;
            pos = trans.localPosition;
            rot = trans.localRotation;
            scale = trans.localScale;
        }

        public override bool Equals(object obj)
        {
            var that = (TransformData)obj;
            return this.pos == that.pos
             && this.rot == that.rot
             && this.scale == that.scale;
        }

        public override int GetHashCode()
        {
            return pos.GetHashCode()
             + rot.GetHashCode()
             + scale.GetHashCode()
             + trans.name.GetHashCode();
        }
    }
}


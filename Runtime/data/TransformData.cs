using System;
using UnityEngine;
#if CORE_LIB
using UnityEngine.Ex;
#endif

namespace mulova.switcher
{
    [Serializable]
    public class TransformData : ICompData
    {
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
        public Transform trans;
        public bool enabled;

        public bool active => enabled;
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
            t.gameObject.SetActive(enabled);
        }

        public void Collect(Component c)
        {
            trans = c as Transform;
            pos = trans.localPosition;
            rot = trans.localRotation;
            scale = trans.localScale;
            enabled = c.gameObject.activeSelf;
        }

        public override bool Equals(object obj)
        {
            var that = (TransformData)obj;
            return this.pos == that.pos
             && this.rot == that.rot
             && this.scale == that.scale
             && this.enabled == that.enabled;
        }

        public override int GetHashCode()
        {
            return pos.GetHashCode()
             + rot.GetHashCode()
             + scale.GetHashCode()
             + trans.name.GetHashCode()
             + enabled.GetHashCode();
        }

        public bool TransformEquals(TransformData that)
        {
            return this.pos.ApproximatelyEquals(that.pos)
                && this.rot.ApproximatelyEquals(that.rot)
                && this.scale.ApproximatelyEquals(that.scale);
        }
    }
}


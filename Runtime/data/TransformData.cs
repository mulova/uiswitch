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
        public Transform trans;
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
        public bool enabled;
        public bool isRoot;

        public bool active => enabled;
        public virtual Type type => typeof(Transform);

        public Component target
        {
            get { return trans; }
            set { trans = value as Transform; }
        }

        public virtual void ApplyTo(Component c)
        {
            var t = c as Transform;
            if (!isRoot)
            {
                t.localPosition = pos;
            }
            t.localRotation = rot;
            t.localScale = scale;
            t.gameObject.SetActive(enabled);
        }

        public virtual void Collect(Component c)
        {
            trans = c as Transform;
            pos = trans.localPosition;
            rot = trans.localRotation;
            scale = trans.localScale;
            enabled = c.gameObject.activeSelf;
            isRoot = c.GetComponent<Switcher>() != null;
        }

        public override bool Equals(object obj)
        {
            var that = (TransformData)obj;
            return (isRoot || this.pos == that.pos)
             && this.rot == that.rot
             && this.scale == that.scale
             && this.enabled == that.enabled
             && this.isRoot == that.isRoot;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash = hash * 37 + rot.GetHashCode();
            hash = hash * 37 + scale.GetHashCode();
            hash = hash * 37 + trans.name.GetHashCode();
            hash = hash * 37 + enabled.GetHashCode();
            hash = hash * 37 + isRoot.GetHashCode();
            return hash;
        }

        public virtual bool TransformEquals(TransformData that)
        {
            return (isRoot || this.pos.ApproximatelyEquals(that.pos))
                && this.rot.ApproximatelyEquals(that.rot)
                && this.scale.ApproximatelyEquals(that.scale);
        }

        public override string ToString()
        {
            return target != null ? target.name : null;
        }
    }
}


using System;
using UnityEngine;
using UnityEngine.UI;

namespace mulova.switcher
{
    [Serializable]
    public abstract class GraphicData<G> : ICompData where G : Graphic
    {
        public bool enabled;
        public bool raycastTarget;
        public Color color;

        public Type type => typeof(G);
        public bool active => enabled;

        public G g;
        public Component target
        {
            get { return g; }
            set { g = value as G; }
        }

        protected abstract void ApplyTo(G g);
        protected abstract void Collect(G g);
        protected abstract bool Equals(GraphicData<G> g);
        protected abstract int ComputeHashCode();

        public void ApplyTo(Component c)
        {
            var g = c as G;
            g.raycastTarget = raycastTarget;
            g.color = color;
            g.enabled = enabled;
            ApplyTo(g);
        }

        public void Collect(Component c)
        {
            g = c as G;
            raycastTarget = g.raycastTarget;
            color = g.color;
            enabled = g.enabled;
            Collect(g);
        }

        public override bool Equals(object obj)
        {
            var that = obj as GraphicData<G>;
            return this.raycastTarget == that.raycastTarget
                && this.color == that.color
                && this.enabled == that.enabled
                && Equals(that);
        }

        public override int GetHashCode()
        {
            var hash = color.GetHashCode();
            hash = hash * 37 + enabled.GetHashCode();
            hash = hash * 37 + raycastTarget.GetHashCode();
            hash = hash * 37 + ComputeHashCode();
            return hash;
        }

        public override string ToString()
        {
            return target != null ? target.name : null;
        }
    }
}


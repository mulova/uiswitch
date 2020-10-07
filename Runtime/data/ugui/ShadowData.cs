using System;
using UnityEngine;
using UnityEngine.UI;

namespace mulova.switcher
{
    [Serializable]
    public class ShadowData : ICompData
    {
        public Shadow comp;
        public bool enabled;
        public Color effectColor;
        public Vector2 effectDistance;
        public bool useGraphicAlpha;

        public Type type => typeof(Shadow);
        public bool active => enabled;

        public Component target
        {
            get { return comp; }
            set { comp = value as Shadow; }
        }

        public void ApplyTo(Component c)
        {
            var s = c as Shadow;
            s.effectColor = effectColor;
            s.effectDistance = effectDistance;
            s.useGraphicAlpha = useGraphicAlpha;
            s.enabled = enabled;
        }

        public void Collect(Component c)
        {
            comp = c as Shadow;
            effectColor = comp.effectColor;
            effectDistance = comp.effectDistance;
            useGraphicAlpha = comp.useGraphicAlpha;
            enabled = comp.enabled;
        }

        public override bool Equals(object obj)
        {
            var that = (ShadowData)obj;
            return this.effectColor == that.effectColor
                && this.effectDistance == that.effectDistance
                && this.useGraphicAlpha == that.useGraphicAlpha
                && this.enabled == that.enabled;
        }

        public override int GetHashCode()
        {
            var hash = effectColor.GetHashCode();
            hash = hash * 37 + effectDistance.GetHashCode();
            hash = hash * 37 + useGraphicAlpha.GetHashCode();
            hash = hash * 37 + enabled.GetHashCode();
            return hash;
        }
    }
}


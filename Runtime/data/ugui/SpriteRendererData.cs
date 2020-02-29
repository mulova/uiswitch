using System;
using UnityEngine;

namespace mulova.switcher
{
    [Serializable]
    public class SpriteRendererData : ICompData
    {
        public SpriteRenderer rend;
        public Sprite sprite;
        public bool enabled;

        public Type type => typeof(SpriteRenderer);
        public bool active => enabled;

        public Component target
        {
            get { return rend; }
            set { rend = value as SpriteRenderer; }
        }

        public void ApplyTo(Component c)
        {
            var r = c as SpriteRenderer;
            r.sprite = sprite;
            r.enabled = enabled;
        }

        public void Collect(Component c)
        {
            rend = c as SpriteRenderer;
            sprite = rend.sprite;
            enabled = rend.enabled;
        }

        public override bool Equals(object obj)
        {
            var that = (SpriteRendererData)obj;
            return this.sprite == that.sprite
                && this.enabled == that.enabled;
        }

        public override int GetHashCode()
        {
            return sprite.GetHashCode()
                + enabled.GetHashCode();
        }
    }
}


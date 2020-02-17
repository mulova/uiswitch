using System;
using UnityEngine;

namespace mulova.ui
{
    [Serializable]
    public class SpriteRendererData : ICompData
    {
        public SpriteRenderer rend;
        public Sprite sprite;

        public Type type => typeof(SpriteRenderer);

        public Component target
        {
            get { return rend; }
            set { rend = value as SpriteRenderer; }
        }

        public void ApplyTo(Component c)
        {
            var r = c as SpriteRenderer;
            r.sprite = sprite;
        }

        public void Collect(Component c)
        {
            rend = c as SpriteRenderer;
            sprite = rend.sprite;
        }

        public override bool Equals(object obj)
        {
            var that = (SpriteRendererData)obj;
            return this.sprite == that.sprite;
        }

        public override int GetHashCode()
        {
            return sprite.GetHashCode();
        }
    }
}


using System;
using UnityEngine;

namespace mulova.switcher
{
    [Serializable]
    public abstract class UIWidgetData<T> : ICompData where T : Component
    {
        // UIWidget Data
        public Color color = Color.white;
        public UIWidget.Pivot pivot = UIWidget.Pivot.Center;
        public int width = 100;
        public int height = 100;
        public int depth = 0;
        public bool active => enabled;

        public bool enabled;
        public Type type => typeof(T);
        public abstract Component target { get; set; }

        protected abstract void ApplyTo(T c);
        protected abstract void Collect(T c);
        protected abstract bool DataEquals(object that);
        protected abstract int GetDataHash();

        public void ApplyTo(Component c)
        {
            var w = c as UIWidget;
            w.enabled = enabled;
            w.color = color;
            w.pivot = pivot;
            w.width = width;
            w.height = height;
            w.depth = depth;

            ApplyTo(w as T);
        }

        public virtual void Collect(Component c)
        {
            var w = c as UIWidget;
            enabled = w.enabled;
            color = w.color;
            pivot = w.pivot;
            width = w.width;
            height = w.height;
            depth = w.depth;

            Collect(w as T);
        }

        public override bool Equals(object obj)
        {
            var that = (UIWidgetData<T>)obj;
            return this.color == that.color
                && this.pivot == that.pivot
                && this.width == that.width
                && this.height == that.height
                && this.depth == that.depth
                && DataEquals(obj);
            ;
        }

        public override int GetHashCode()
        {
            return color.GetHashCode()
                + pivot.GetHashCode()
                + width.GetHashCode()
                + height.GetHashCode()
                + depth.GetHashCode()
                + enabled.GetHashCode()
                + this.GetDataHash()
            ;
        }
    }
}


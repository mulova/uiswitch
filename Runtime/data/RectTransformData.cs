using System;
using UnityEngine;
#if CORE_LIB
using UnityEngine.Ex;
#endif

namespace mulova.switcher
{
    [Serializable]
    public class RectTransformData : TransformData
    {
        public Vector2 anchoredPosition;
        public Vector2 anchorMax;
        public Vector2 anchorMin;
        public Vector2 pivot;
        public Vector2 sizeDelta;

        public RectTransform rect => trans as RectTransform;
        public override Type type => typeof(RectTransform);

        public override void ApplyTo(Component c)
        {
            base.ApplyTo(c);
            var r = c as RectTransform;
            r.pivot = pivot;
            r.anchoredPosition = anchoredPosition;
            r.sizeDelta = sizeDelta;
            r.anchorMax = anchorMax;
            r.anchorMin = anchorMin;

        }

        public override void Collect(Component c)
        {
            base.Collect(c);
            var r = rect;
            pivot = r.pivot;
            anchoredPosition = r.anchoredPosition;
            sizeDelta = r.sizeDelta;
            anchorMax = r.anchorMax;
            anchorMin = r.anchorMin;
        }

        public override bool Equals(object obj)
        {
            var that = (RectTransformData)obj;
            return base.Equals(obj)
             && this.anchoredPosition == that.anchoredPosition
             && this.anchorMax == that.anchorMax
             && this.anchorMin == that.anchorMin
             && this.pivot == that.pivot
             && this.sizeDelta == that.sizeDelta;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash = hash * 37 + pivot.GetHashCode();
            hash = hash * 37 + anchoredPosition.GetHashCode();
            hash = hash * 37 + sizeDelta.GetHashCode();
            hash = hash * 37 + anchorMax.GetHashCode();
            hash = hash * 37 + anchorMin.GetHashCode();
            return hash;
        }

        public override bool TransformEquals(TransformData t)
        {
            var that = t as RectTransformData;
            return base.TransformEquals(that)
                && this.anchoredPosition.ApproximatelyEquals(that.anchoredPosition)
                && this.anchorMax.ApproximatelyEquals(that.anchorMax)
                && this.anchorMin.ApproximatelyEquals(that.anchorMin)
                && this.pivot.ApproximatelyEquals(that.pivot)
                && this.sizeDelta.ApproximatelyEquals(that.sizeDelta);
        }
    }
}


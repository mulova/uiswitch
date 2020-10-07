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
        public Vector3 anchoredPosition3D;
        public Vector2 anchorMax;
        public Vector2 anchorMin;
        public Vector2 offsetMax;
        public Vector2 offsetMin;
        public Vector2 pivot;
        public Vector2 sizeDelta;

        public RectTransform rect => trans as RectTransform;
        public override Type type => typeof(RectTransform);

        public override void ApplyTo(Component c)
        {
            base.ApplyTo(c);
            var r = c as RectTransform;
            r.anchoredPosition = anchoredPosition;
            r.anchoredPosition3D = anchoredPosition3D;
            r.anchorMax = anchorMax;
            r.anchorMin = anchorMin;
            r.offsetMax = offsetMax;
            r.offsetMin = offsetMin;
            r.pivot = pivot;
            r.sizeDelta = sizeDelta;

        }

        public override void Collect(Component c)
        {
            base.Collect(c);
            var r = rect;
            anchoredPosition = r.anchoredPosition;
            anchoredPosition3D = r.anchoredPosition3D;
            anchorMax = r.anchorMax;
            anchorMin = r.anchorMin;
            offsetMax = r.offsetMax;
            offsetMin = r.offsetMin;
            pivot = r.pivot;
            sizeDelta = r.sizeDelta;
        }

        public override bool Equals(object obj)
        {
            var that = (RectTransformData)obj;
            return base.Equals(obj)
             && this.anchoredPosition == that.anchoredPosition
             && this.anchoredPosition3D == that.anchoredPosition3D
             && this.anchorMax == that.anchorMax
             && this.anchorMin == that.anchorMin
             && this.offsetMax == that.offsetMax
             && this.offsetMin == that.offsetMin
             && this.pivot == that.pivot
             && this.sizeDelta == that.sizeDelta;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash = hash * 37 + anchoredPosition.GetHashCode();
            hash = hash * 37 + anchoredPosition3D.GetHashCode();
            hash = hash * 37 + anchorMax.GetHashCode();
            hash = hash * 37 + anchorMin.GetHashCode();
            hash = hash * 37 + offsetMax.GetHashCode();
            hash = hash * 37 + offsetMin.GetHashCode();
            hash = hash * 37 + pivot.GetHashCode();
            hash = hash * 37 + sizeDelta.GetHashCode();
            return hash;
        }

        public override bool TransformEquals(TransformData t)
        {
            var that = t as RectTransformData;
            return base.TransformEquals(that)
                && this.anchoredPosition.ApproximatelyEquals(that.anchoredPosition)
                && this.anchoredPosition3D.ApproximatelyEquals(that.anchoredPosition3D)
                && this.anchorMax.ApproximatelyEquals(that.anchorMax)
                && this.anchorMin.ApproximatelyEquals(that.anchorMin)
                && this.offsetMax.ApproximatelyEquals(that.offsetMax)
                && this.offsetMin.ApproximatelyEquals(that.offsetMin)
                && this.pivot.ApproximatelyEquals(that.pivot)
                && this.sizeDelta.ApproximatelyEquals(that.sizeDelta);
        }
    }
}


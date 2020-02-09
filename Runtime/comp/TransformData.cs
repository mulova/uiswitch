using System;
using UnityEngine;

namespace mulova.ui
{
    [Serializable]
    public struct TransformData : ICompData<Transform>
    {
        public Vector3 pos;

        public static TransformData[] GetDiff(Transform[] comps)
        {
            var arr = new TransformData[comps.Length];
            bool diff = false;
            for (int i=0; i<arr.Length; ++i)
            {
                arr[i].Collect(comps[i]);
                if (!diff && i != 0 && !arr[i].Equals(arr[0]))
                {
                    diff = true;
                }
            }
            if (diff)
            {
                return arr;
            } else
            {
                return null;
            }
        }

        public void ApplyTo(Transform t)
        {
            t.localPosition = pos;
        }

        public void Collect(Transform t)
        {
            pos = t.localPosition;
        }

        public override bool Equals(object obj)
        {
            var that = (TransformData)obj;
            return this.pos == that.pos;
        }

        public override int GetHashCode()
        {
            return pos.GetHashCode();
        }
    }
}


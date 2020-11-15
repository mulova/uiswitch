using System;
using UnityEngine;
using UnityEngine.UI;

namespace mulova.switcher
{
    [Serializable]
    public class ImageData : GraphicData<Image>
    {
        public Sprite sprite;
        public Image.Type imageType;
        public float fillAmount;
        public bool fillCenter;
        public bool fillClockwise;
        public Image.FillMethod fillMethod;
        public int fillOrigin;
        public bool useSpriteMesh;
        public bool preserveAspect;
        public bool maskable;

        protected override void ApplyTo(Image i)
        {
            i.sprite = sprite;
            i.type = imageType;
            i.fillAmount = fillAmount;
            i.fillCenter = fillCenter;
            i.fillClockwise = fillClockwise;
            i.fillMethod = fillMethod;
            i.fillOrigin = fillOrigin;
            i.useSpriteMesh = useSpriteMesh;
            i.preserveAspect = preserveAspect;
            i.maskable = maskable;
        }

        protected override void Collect(Image img)
        {
            sprite = img.sprite;
            imageType = img.type;
            fillAmount = img.fillAmount;
            fillCenter = img.fillCenter;
            fillClockwise = img.fillClockwise;
            fillMethod = img.fillMethod;
            fillOrigin = img.fillOrigin;
            useSpriteMesh = img.useSpriteMesh;
            preserveAspect = img.preserveAspect;
            maskable = img.maskable;
        }

        protected override bool Equals(GraphicData<Image> obj)
        {
            var that = (ImageData)obj;
            return this.sprite == that.sprite
                && this.type == that.type
                && this.fillAmount == that.fillAmount
                && this.fillCenter == that.fillCenter
                && this.fillClockwise == that.fillClockwise
                && this.fillMethod == that.fillMethod
                && this.fillOrigin == that.fillOrigin
                && this.useSpriteMesh == that.useSpriteMesh
                && this.preserveAspect == that.preserveAspect
                && this.maskable == that.maskable;
        }

        protected override int ComputeHashCode()
        {
            var hash = sprite.GetHashCode();
            hash = hash * 37 + type.GetHashCode();
            hash = hash * 37 + fillAmount.GetHashCode();
            hash = hash * 37 + fillCenter.GetHashCode();
            hash = hash * 37 + fillClockwise.GetHashCode();
            hash = hash * 37 + fillMethod.GetHashCode();
            hash = hash * 37 + fillOrigin.GetHashCode();
            hash = hash * 37 + useSpriteMesh.GetHashCode();
            hash = hash * 37 + preserveAspect.GetHashCode();
            hash = hash * 37 + maskable.GetHashCode();
            return hash;
        }
    }
}


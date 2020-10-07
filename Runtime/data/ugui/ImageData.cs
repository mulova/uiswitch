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
        public bool useSpriteMesh;
        public bool preserveAspect;
        public bool maskable;

        protected override void ApplyTo(Image i)
        {
            i.sprite = sprite;
            i.type = imageType;
            i.useSpriteMesh = useSpriteMesh;
            i.preserveAspect = preserveAspect;
            i.maskable = maskable;
        }

        protected override void Collect(Image img)
        {
            sprite = img.sprite;
            imageType = img.type;
            useSpriteMesh = img.useSpriteMesh;
            preserveAspect = img.preserveAspect;
            maskable = img.maskable;
        }

        protected override bool Equals(GraphicData<Image> obj)
        {
            var that = (ImageData)obj;
            return this.sprite == that.sprite
                && this.type == that.type
                && this.useSpriteMesh == that.useSpriteMesh
                && this.preserveAspect == that.preserveAspect
                && this.maskable == that.maskable;
        }

        protected override int ComputeHashCode()
        {
            var hash = sprite.GetHashCode();
            hash = hash * 37 + type.GetHashCode();
            hash = hash * 37 + useSpriteMesh.GetHashCode();
            hash = hash * 37 + preserveAspect.GetHashCode();
            hash = hash * 37 + maskable.GetHashCode();
            return hash;
        }
    }
}


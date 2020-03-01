using System;
using UnityEngine;
using static UIBasicSprite;
using Object = UnityEngine.Object;

namespace mulova.switcher
{
    [Serializable]
    public class UISpriteData : UIWidgetData<UISprite>
    {
        public UISprite sprite;
        public Object atlas;
        public string spriteName;
        public FillDirection fillDirection = FillDirection.Radial360;
        public float fillAmount = 1f;
        public bool invert = false;
        public Flip flip = Flip.Nothing;
        public bool applyGradient = false;
        public Color gradientTop = Color.white;
        public Color gradientBottom = new Color(0.7f, 0.7f, 0.7f);

        public override Component target
        {
            get { return sprite; }
            set { sprite = value as UISprite; }
        }

        protected override void ApplyTo(UISprite s)
        {
            s.atlas = atlas as INGUIAtlas;
            s.spriteName = spriteName;
            s.fillDirection = fillDirection;
            s.fillAmount = fillAmount;
            s.invert = invert;
            s.flip = flip;
            s.applyGradient = applyGradient;
            s.gradientTop = gradientTop;
            s.gradientBottom = gradientBottom;
        }

        protected override void Collect(UISprite s)
        {
            sprite = s;
            atlas = sprite.atlas as Object;
            spriteName = sprite.spriteName;
            fillDirection = sprite.fillDirection;
            fillAmount = sprite.fillAmount;
            invert = sprite.invert;
            flip = sprite.flip;
            applyGradient = sprite.applyGradient;
            gradientTop = sprite.gradientTop;
            gradientBottom = sprite.gradientBottom;
        }

        protected override bool DataEquals(object o)
        {
            var that = o as UISpriteData;
            return this.spriteName == that.spriteName
                && this.atlas == that.atlas
                && this.fillDirection == that.fillDirection
                && this.fillAmount.ApproximatelyEquals(that.fillAmount)
                && this.invert == that.invert
                && this.flip == that.flip
                && this.applyGradient == that.applyGradient
                && this.gradientTop == that.gradientTop
                && this.gradientBottom == that.gradientBottom
            ;
        }

        protected override int GetDataHash()
        {
            return spriteName.GetHashCode()
                + atlas?.GetHashCode() ?? 0
                + fillDirection.GetHashCode()
                + fillAmount.GetHashCode()
                + invert.GetHashCode()
                + flip.GetHashCode()
                + applyGradient.GetHashCode()
                + gradientTop.GetHashCode()
                + gradientBottom.GetHashCode()
            ;
        }
    }
}


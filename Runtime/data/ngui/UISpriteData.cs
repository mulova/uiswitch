using System;
using UnityEngine;
using static UIBasicSprite;

namespace mulova.ui
{
    [Serializable]
    public class UISpriteData : ICompData
    {
        public UISprite sprite;
        [SerializeReference] public INGUIAtlas atlas;
        public string spriteName;
        public FillDirection fillDirection = FillDirection.Radial360;
        public float fillAmount = 1f;
        public bool invert = false;
        public Flip flip = Flip.Nothing;
        public bool applyGradient = false;
        public Color gradientTop = Color.white;
        public Color gradientBottom = new Color(0.7f, 0.7f, 0.7f);

        public System.Type type => typeof(UISprite);

        public Component target
        {
            get { return sprite; }
            set { sprite = value as UISprite; }
        }

        public void ApplyTo(Component c)
        {
            var s = c as UISprite;
            s.atlas = atlas;
            s.spriteName = spriteName;
            s.fillDirection = fillDirection;
            s.fillAmount = fillAmount;
            s.invert = invert;
            s.flip = flip;
            s.applyGradient = applyGradient;
            s.gradientTop = gradientTop;
            s.gradientBottom = gradientBottom;
        }

        public void Collect(Component c)
        {
            sprite = c as UISprite;
            atlas = sprite.atlas;
            spriteName = sprite.spriteName;
            fillDirection = sprite.fillDirection;
            fillAmount = sprite.fillAmount;
            invert = sprite.invert;
            flip = sprite.flip;
            applyGradient = sprite.applyGradient;
            gradientTop = sprite.gradientTop;
            gradientBottom = sprite.gradientBottom;
        }

        public override bool Equals(object obj)
        {
            var that = (UISpriteData)obj;
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

        public override int GetHashCode()
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


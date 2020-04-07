using System;
using UnityEngine;
using static UILabel;

namespace mulova.switcher
{
    [Serializable]
    public class UILabelData : UIWidgetData<UILabel>
    {
        public bool applyText;

        public UILabel lbl;
        public string text;
        public int fontSize;
        public FontStyle fontStyle;
        public NGUIText.Alignment alignment;
        public bool encoding = true;
        public int maxLineCount = 0;
        public Effect effectStyle;
        public Color effectColor;
        public NGUIText.SymbolStyle symbolStyle;
        public Vector2 effectDistance;
        public Overflow overflow;
        public bool applyGradient;
        public Color gradientTop;
        public Color gradientBottom;
        public int spacingX;
        public int spacingY;
        public bool useFloatSpacing;
        public float floatSpacingX;
        public float floatSpacingY;
        public bool overflowEllipsis;
        public int overflowWidth;
        public Modifier modifier;

        public override Component target
        {
            get { return lbl; }
            set { lbl = value as UILabel; }
        }

        protected override void ApplyTo(UILabel l)
        {
            l.enabled = enabled;
            if (applyText)
            {
                l.text = text;
            }
            l.fontSize = fontSize;
            l.fontStyle = fontStyle;
            l.alignment = alignment;
            l.supportEncoding = encoding;
            l.maxLineCount = maxLineCount;
            l.effectStyle = effectStyle;
            l.effectColor = effectColor;
            l.symbolStyle = symbolStyle;
            l.effectDistance = effectDistance;
            l.overflowMethod = overflow;
            l.applyGradient = applyGradient;
            l.gradientTop = gradientTop;
            l.gradientBottom = gradientBottom;
            l.spacingX = spacingX;
            l.spacingY = spacingY;
            l.useFloatSpacing = useFloatSpacing;
            l.floatSpacingX = floatSpacingX;
            l.floatSpacingY = floatSpacingY;
            l.overflowEllipsis = overflowEllipsis;
            l.overflowWidth = overflowWidth;
            l.modifier = modifier;
        }

        protected override void Collect(UILabel l)
        {
            lbl = l;
            enabled = l.enabled;
            if (applyText)
            {
                text = l.text;
            }
            fontSize = l.fontSize;
            fontStyle = l.fontStyle;
            alignment = l.alignment;
            encoding = l.supportEncoding;
            maxLineCount = l.maxLineCount;
            effectStyle = l.effectStyle;
            effectColor = l.effectColor;
            symbolStyle = l.symbolStyle;
            effectDistance = l.effectDistance;
            overflow = l.overflowMethod;
            applyGradient = l.applyGradient;
            gradientTop = l.gradientTop;
            gradientBottom = l.gradientBottom;
            spacingX = l.spacingX;
            spacingY = l.spacingY;
            useFloatSpacing = l.useFloatSpacing;
            floatSpacingX = l.floatSpacingX;
            floatSpacingY = l.floatSpacingY;
            overflowEllipsis = l.overflowEllipsis;
            overflowWidth = l.overflowWidth;
            modifier = l.modifier;
        }

        protected override bool DataEquals(object o)
        {
            UILabelData that = o as UILabelData;
            return this.text == that.text
            && this.enabled == that.enabled
            && this.fontSize == that.fontSize
            && this.fontStyle == that.fontStyle
            && this.alignment == that.alignment
            && this.encoding == that.encoding
            && this.maxLineCount == that.maxLineCount
            && this.effectStyle == that.effectStyle
            && this.effectColor == that.effectColor
            && this.symbolStyle == that.symbolStyle
            && this.effectDistance == that.effectDistance
            && this.overflow == that.overflow
            && this.applyGradient == that.applyGradient
            && this.gradientTop == that.gradientTop
            && this.gradientBottom == that.gradientBottom
            && this.spacingX == that.spacingX
            && this.spacingY == that.spacingY
            && this.useFloatSpacing == that.useFloatSpacing
            && this.floatSpacingX == that.floatSpacingX
            && this.floatSpacingY == that.floatSpacingY
            && this.overflowEllipsis == that.overflowEllipsis
            && this.overflowWidth == that.overflowWidth
            && this.modifier == that.modifier;
        }

        protected override int GetDataHash()
        {
            return text.GetHashCode() 
            + enabled.GetHashCode()
            + fontSize.GetHashCode()
            + alignment.GetHashCode()
            + encoding.GetHashCode()
            + maxLineCount.GetHashCode()
            + effectStyle.GetHashCode()
            + effectColor.GetHashCode()
            + symbolStyle.GetHashCode()
            + effectDistance.GetHashCode()
            + overflow.GetHashCode()
            + applyGradient.GetHashCode()
            + gradientTop.GetHashCode()
            + gradientBottom.GetHashCode()
            + spacingX.GetHashCode()
            + spacingY.GetHashCode()
            + useFloatSpacing.GetHashCode()
            + floatSpacingX.GetHashCode()
            + floatSpacingY.GetHashCode()
            + overflowEllipsis.GetHashCode()
            + overflowWidth.GetHashCode()
            + modifier.GetHashCode()
                ;
        }
    }
}


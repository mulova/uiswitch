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
        public Effect effectStyle;
        public Color effectColor;
        public Vector2 effectDistance;
#if SWITCHER_DETAILED
        public Overflow overflow;
        public int overflowWidth;
        public NGUIText.Alignment alignment;
        public FontStyle fontStyle;
        public int fontSize;
        public bool encoding = true;
        public int maxLineCount = 0;
        public bool applyGradient;
        public Color gradientTop;
        public Color gradientBottom;
        public NGUIText.SymbolStyle symbolStyle;
        public int spacingX;
        public int spacingY;
        public bool useFloatSpacing;
        public float floatSpacingX;
        public float floatSpacingY;
        public bool overflowEllipsis;
        public Modifier modifier;
#endif

        public override Component target
        {
            get { return lbl; }
            set { lbl = value as UILabel; }
        }

        protected override void ApplyTo(UILabel l)
        {
            l.enabled = enabled;
            l.effectStyle = effectStyle;
            l.effectColor = effectColor;
            l.effectDistance = effectDistance;
#if SWITCHER_DETAILED
            l.overflowMethod = overflow;
            l.overflowWidth = overflowWidth;
            l.alignment = alignment;
            l.fontSize = fontSize;
            l.fontStyle = fontStyle;
            l.supportEncoding = encoding;
            l.maxLineCount = maxLineCount;
            l.symbolStyle = symbolStyle;
            l.applyGradient = applyGradient;
            l.gradientTop = gradientTop;
            l.gradientBottom = gradientBottom;
            l.spacingX = spacingX;
            l.spacingY = spacingY;
            l.useFloatSpacing = useFloatSpacing;
            l.floatSpacingX = floatSpacingX;
            l.floatSpacingY = floatSpacingY;
            l.overflowEllipsis = overflowEllipsis;
            l.modifier = modifier;
#endif
            if (applyText)
            {
                l.text = text;
            }
        }

        protected override void Collect(UILabel l)
        {
            lbl = l;
            enabled = l.enabled;
            effectStyle = l.effectStyle;
            effectColor = l.effectColor;
            effectDistance = l.effectDistance;
#if SWITCHER_DETAILED
            overflow = l.overflowMethod;
            overflowWidth = l.overflowWidth;
            alignment = l.alignment;
            fontSize = l.fontSize;
            fontStyle = l.fontStyle;
            encoding = l.supportEncoding;
            maxLineCount = l.maxLineCount;
            symbolStyle = l.symbolStyle;
            applyGradient = l.applyGradient;
            gradientTop = l.gradientTop;
            gradientBottom = l.gradientBottom;
            spacingX = l.spacingX;
            spacingY = l.spacingY;
            useFloatSpacing = l.useFloatSpacing;
            floatSpacingX = l.floatSpacingX;
            floatSpacingY = l.floatSpacingY;
            overflowEllipsis = l.overflowEllipsis;
            modifier = l.modifier;
#endif
            if (applyText)
            {
                text = l.text;
            }
        }

        protected override bool DataEquals(object o)
        {
            UILabelData that = o as UILabelData;
            return this.text == that.text
            && this.enabled == that.enabled
            && this.effectStyle == that.effectStyle
            && this.effectColor == that.effectColor
            && this.effectDistance == that.effectDistance
#if SWITCHER_DETAILED
            && this.overflow == that.overflow
            && this.overflowWidth == that.overflowWidth
            && this.alignment == that.alignment
            && this.fontSize == that.fontSize
            && this.fontStyle == that.fontStyle
            && this.encoding == that.encoding
            && this.maxLineCount == that.maxLineCount
            && this.symbolStyle == that.symbolStyle
            && this.applyGradient == that.applyGradient
            && this.gradientTop == that.gradientTop
            && this.gradientBottom == that.gradientBottom
            && this.spacingX == that.spacingX
            && this.spacingY == that.spacingY
            && this.useFloatSpacing == that.useFloatSpacing
            && this.floatSpacingX == that.floatSpacingX
            && this.floatSpacingY == that.floatSpacingY
            && this.overflowEllipsis == that.overflowEllipsis
            && this.modifier == that.modifier
#endif
            ;
        }

        protected override int GetDataHash()
        {
            return text.GetHashCode() 
            + enabled.GetHashCode()
            + effectStyle.GetHashCode()
            + effectColor.GetHashCode()
            + effectDistance.GetHashCode()
#if SWITCHER_DETAILED
            + overflow.GetHashCode()
            + overflowWidth.GetHashCode()
            + alignment.GetHashCode()
            + fontSize.GetHashCode()
            + encoding.GetHashCode()
            + maxLineCount.GetHashCode()
            + symbolStyle.GetHashCode()
            + applyGradient.GetHashCode()
            + gradientTop.GetHashCode()
            + gradientBottom.GetHashCode()
            + spacingX.GetHashCode()
            + spacingY.GetHashCode()
            + useFloatSpacing.GetHashCode()
            + floatSpacingX.GetHashCode()
            + floatSpacingY.GetHashCode()
            + overflowEllipsis.GetHashCode()
            + modifier.GetHashCode()
#endif
                ;
        }
    }
}


using System;
using UnityEngine;
using static UILabel;

namespace mulova.switcher
{
    [Serializable]
    public class UILabelData : ICompData
    {
        public UILabel lbl;
        public Color color;
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
        public bool enabled;

        public Type type => typeof(UILabel);
        public bool active => enabled;

        public Component target
        {
            get { return lbl; }
            set { lbl = value as UILabel; }
        }

        public void ApplyTo(Component c)
        {
            var l = c as UILabel;
            lbl.enabled = enabled;
            lbl.color = color;
            lbl.text = text;
            lbl.fontSize = fontSize;
            lbl.fontStyle = fontStyle;
            lbl.alignment = alignment;
            lbl.supportEncoding = encoding;
            lbl.maxLineCount = maxLineCount;
            lbl.effectStyle = effectStyle;
            lbl.effectColor = effectColor;
            lbl.symbolStyle = symbolStyle;
            lbl.effectDistance = effectDistance;
            lbl.overflowMethod = overflow;
            lbl.applyGradient = applyGradient;
            lbl.gradientTop = gradientTop;
            lbl.gradientBottom = gradientBottom;
            lbl.spacingX = spacingX;
            lbl.spacingY = spacingY;
            lbl.useFloatSpacing = useFloatSpacing;
            lbl.floatSpacingX = floatSpacingX;
            lbl.floatSpacingY = floatSpacingY;
            lbl.overflowEllipsis = overflowEllipsis;
            lbl.overflowWidth = overflowWidth;
            lbl.modifier = modifier;
        }

        public void Collect(Component c)
        {
            lbl = c as UILabel;
            enabled = lbl.enabled;
            color = lbl.color;
            text = lbl.text;
            fontSize = lbl.fontSize;
            fontStyle = lbl.fontStyle;
            alignment = lbl.alignment;
            encoding = lbl.supportEncoding;
            maxLineCount = lbl.maxLineCount;
            effectStyle = lbl.effectStyle;
            effectColor = lbl.effectColor;
            symbolStyle = lbl.symbolStyle;
            effectDistance = lbl.effectDistance;
            overflow = lbl.overflowMethod;
            applyGradient = lbl.applyGradient;
            gradientTop = lbl.gradientTop;
            gradientBottom = lbl.gradientBottom;
            spacingX = lbl.spacingX;
            spacingY = lbl.spacingY;
            useFloatSpacing = lbl.useFloatSpacing;
            floatSpacingX = lbl.floatSpacingX;
            floatSpacingY = lbl.floatSpacingY;
            overflowEllipsis = lbl.overflowEllipsis;
            overflowWidth = lbl.overflowWidth;
            modifier = lbl.modifier;
        }

        public override bool Equals(object obj)
        {
            var that = (UILabelData)obj;
            return this.text == that.text
            && this.enabled == that.enabled
            && this.color == that.color
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

        public override int GetHashCode()
        {
            return text.GetHashCode() 
            + enabled.GetHashCode()
            + color.GetHashCode()
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


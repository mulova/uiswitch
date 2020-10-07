using System;
using UnityEngine;
using UnityEngine.UI;

namespace mulova.switcher
{
    [Serializable]
    public class TextData : GraphicData<Text>
    {
        public Text comp;
        public bool maskable;
        public string text;
        public Font font;
        public int fontSize;
        public FontStyle fontStyle;
        public float lineSpacing;
        public bool supportRichText;
        public TextAnchor alignment;
        public bool alignByGeometry;
        public HorizontalWrapMode horizontalOverflow;
        public VerticalWrapMode verticalOverflow;
        public bool resizeTextForBestFit;

        protected override void ApplyTo(Text t)
        {
            t.text = text;
            t.font = font;
            t.fontSize = fontSize;
            t.fontStyle = fontStyle;
            t.lineSpacing = lineSpacing;
            t.supportRichText = supportRichText;
            t.alignment = alignment;
            t.alignByGeometry = alignByGeometry;
            t.horizontalOverflow = horizontalOverflow;
            t.verticalOverflow = verticalOverflow;
            t.resizeTextForBestFit = resizeTextForBestFit;
            t.maskable = maskable;
        }

        protected override void Collect(Text t)
        {
            text = t.text;
            font = t.font;
            fontSize = t.fontSize;
            fontStyle = t.fontStyle;
            lineSpacing = t.lineSpacing;
            supportRichText = t.supportRichText;
            alignment = t.alignment;
            alignByGeometry = t.alignByGeometry;
            horizontalOverflow = t.horizontalOverflow;
            verticalOverflow = t.verticalOverflow;
            resizeTextForBestFit = t.resizeTextForBestFit;
            maskable = t.maskable;
        }

        protected override bool Equals(GraphicData<Text> obj)
        {
            var that = (TextData)obj;
            return this.text == that.text
                && this.font == that.font
                && this.fontSize == that.fontSize
                && this.fontStyle == that.fontStyle
                && this.lineSpacing == that.lineSpacing
                && this.supportRichText == that.supportRichText
                && this.alignment == that.alignment
                && this.alignByGeometry == that.alignByGeometry
                && this.horizontalOverflow == that.horizontalOverflow
                && this.verticalOverflow == that.verticalOverflow
                && this.resizeTextForBestFit == that.resizeTextForBestFit
                && this.maskable == that.maskable;
        }

        protected override int ComputeHashCode()
        {
            var hash = text.GetHashCode();
            if (font != null)
            {
                hash = hash * 37 + font.GetHashCode();
            }
            hash = hash * 37 + fontSize.GetHashCode();
            hash = hash * 37 + fontStyle.GetHashCode();
            hash = hash * 37 + lineSpacing.GetHashCode();
            hash = hash * 37 + supportRichText.GetHashCode();
            hash = hash * 37 + alignment.GetHashCode();
            hash = hash * 37 + alignByGeometry.GetHashCode();
            hash = hash * 37 + horizontalOverflow.GetHashCode();
            hash = hash * 37 + verticalOverflow.GetHashCode();
            hash = hash * 37 + resizeTextForBestFit.GetHashCode();
            hash = hash * 37 + maskable.GetHashCode();
            return hash;
        }
    }
}


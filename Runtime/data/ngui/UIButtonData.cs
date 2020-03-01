using System;
using UnityEngine;
using static UIButtonColor;

namespace mulova.switcher
{
    [Serializable]
    public class UIButtonData : UIWidgetData<UIButton>
    {
        public UIButton button;
        public string hoverSprite;
        public string pressedSprite;
        public string disabledSprite;
        public Color hover = new Color(225f / 255f, 200f / 255f, 150f / 255f, 1f);
        public Color pressed = new Color(183f / 255f, 163f / 255f, 123f / 255f, 1f);
        public Color disabledColor = Color.grey;
        public float duration = 0.2f;
        public State state;

        public override Component target
        {
            get { return button; }
            set { button = value as UIButton; }
        }

        protected override void ApplyTo(UIButton b)
        {
            b.enabled = enabled;
            b.hoverSprite = hoverSprite;
            b.pressedSprite = pressedSprite;
            b.disabledSprite = disabledSprite;
            b.hover = hover;
            b.pressed = pressed;
            b.disabledColor = disabledColor;
            b.duration = duration;
            b.state = state;
        }

        protected override void Collect(UIButton b)
        {
            button = b;
            enabled = button.enabled;
            hoverSprite = button.hoverSprite;
            pressedSprite = button.pressedSprite;
            disabledSprite = button.disabledSprite;
            hover = button.hover;
            pressed = button.pressed;
            disabledColor = button.disabledColor;
            duration = button.duration;
            state = button.state;
        }

        protected override bool DataEquals(object obj)
        {
            var that = (UIButtonData)obj;
            return this.enabled == that.enabled
                && this.hoverSprite == that.hoverSprite
                && this.pressedSprite == that.pressedSprite
                && this.disabledSprite == that.pressedSprite
                && this.hover == that.hover
                && this.pressed == that.pressed
                && this.disabledColor == that.disabledColor
                && this.duration.ApproximatelyEquals(that.duration)
                && this.state == that.state
                ;
        }

        protected override int GetDataHash()
        {
            return enabled.GetHashCode() 
                + hoverSprite?.GetHashCode() ?? 0
                + pressedSprite?.GetHashCode() ?? 0
                + disabledSprite?.GetHashCode() ?? 0
                + hover.GetHashCode()
                + pressed.GetHashCode()
                + disabledColor.GetHashCode()
                + duration.GetHashCode()
                + state.GetHashCode()
            ;
        }
    }
}


using System;
using UnityEngine;
using static UIButtonColor;

namespace mulova.ui
{
    [Serializable]
    public class UIButtonData : ICompData
    {
        public UIButton button;
        public Type type => typeof(UIButton);
        public string hoverSprite;
        public string pressedSprite;
        public string disabledSprite;
        public Color hover = new Color(225f / 255f, 200f / 255f, 150f / 255f, 1f);
        public Color pressed = new Color(183f / 255f, 163f / 255f, 123f / 255f, 1f);
        public Color disabledColor = Color.grey;
        public float duration = 0.2f;
        public State state;


        public Component target
        {
            get { return button; }
            set { button = value as UIButton; }
        }

        public void ApplyTo(Component c)
        {
            var b = c as UIButton;
            b.hoverSprite = hoverSprite;
            b.pressedSprite = pressedSprite;
            b.disabledSprite = disabledSprite;
            b.hover = hover;
            b.pressed = pressed;
            b.disabledColor = disabledColor;
            b.duration = duration;
            b.state = state;
        }

        public void Collect(Component c)
        {
            button = c as UIButton;
            hoverSprite = button.hoverSprite;
            pressedSprite = button.pressedSprite;
            disabledSprite = button.disabledSprite;
            hover = button.hover;
            pressed = button.pressed;
            disabledColor = button.disabledColor;
            duration = button.duration;
            state = button.state;
        }

        public override bool Equals(object obj)
        {
            var that = (UIButtonData)obj;
            return this.hoverSprite == that.hoverSprite
                && this.pressedSprite == that.pressedSprite
                && this.disabledSprite == that.pressedSprite
                && this.hover == that.hover
                && this.pressed == that.pressed
                && this.disabledColor == that.disabledColor
                && this.duration.ApproximatelyEquals(that.duration)
                && this.state == that.state
                ;
        }

        public override int GetHashCode()
        {
            return hoverSprite?.GetHashCode() ?? 0
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


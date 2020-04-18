using System;
using UnityEngine;
using static UIButtonColor;

namespace mulova.switcher
{
    [Serializable]
    public class UIButtonData : ICompData
    {
        public UIButton button;
        public bool enabled;
        public string hoverSprite;
        public string pressedSprite;
        public string disabledSprite;
        public Color hover = new Color(225f / 255f, 200f / 255f, 150f / 255f, 1f);
        public Color pressed = new Color(183f / 255f, 163f / 255f, 123f / 255f, 1f);
        public Color disabledColor = Color.grey;
#if SWITCHER_DETAILED
        public float duration = 0.2f;
        public State state;
        public GameObject tweenTarget;
#endif

        public Type type => typeof(UIButton);
        public bool active => enabled;

        public Component target
        {
            get { return button; }
            set { button = value as UIButton; }
        }

        public void ApplyTo(Component c)
        {
            var b = c as UIButton;
            b.enabled = enabled;
            b.hoverSprite = hoverSprite;
            b.pressedSprite = pressedSprite;
            b.disabledSprite = disabledSprite;
            b.hover = hover;
            b.pressed = pressed;
            b.disabledColor = disabledColor;
#if SWITCHER_DETAILED
            b.duration = duration;
            b.state = state;
            b.tweenTarget = tweenTarget;
#endif
        }

        public void Collect(Component b)
        {
            button = b as UIButton;
            enabled = button.enabled;
            hoverSprite = button.hoverSprite;
            pressedSprite = button.pressedSprite;
            disabledSprite = button.disabledSprite;
            hover = button.hover;
            pressed = button.pressed;
            disabledColor = button.disabledColor;
#if SWITCHER_DETAILED
            duration = button.duration;
            state = button.state;
            tweenTarget = button.tweenTarget;
#endif
        }

        public override bool Equals(object obj)
        {
            var that = (UIButtonData)obj;
            return this.enabled == that.enabled
                && this.hoverSprite == that.hoverSprite
                && this.pressedSprite == that.pressedSprite
                && this.disabledSprite == that.pressedSprite
                && this.hover == that.hover
                && this.pressed == that.pressed
                && this.disabledColor == that.disabledColor
#if SWITCHER_DETAILED
                && this.duration.ApproximatelyEquals(that.duration)
                && this.state == that.state
#endif
                ;
        }

        public override int GetHashCode()
        {
            return enabled.GetHashCode() 
                + hoverSprite?.GetHashCode() ?? 0
                + pressedSprite?.GetHashCode() ?? 0
                + disabledSprite?.GetHashCode() ?? 0
                + hover.GetHashCode()
                + pressed.GetHashCode()
                + disabledColor.GetHashCode()
#if SWITCHER_DETAILED
                + duration.GetHashCode()
                + state.GetHashCode()
#endif
            ;
        }
    }
}


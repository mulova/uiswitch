using System;
using UnityEngine.UI;

namespace mulova.switcher
{
    [Serializable]
    public class ButtonData : SelectableData<Button>
    {
        public Button.ButtonClickedEvent onClick;

        protected override void ApplyTo(Button i)
        {
            i.onClick = onClick;
        }

        protected override void Collect(Button b)
        {
            this.onClick = b.onClick;
        }

        protected override bool Equals(SelectableData<Button> obj)
        {
            var that = (ButtonData)obj;
            return this.onClick == that.onClick;
        }

        protected override int ComputeHashCode()
        {
            var hash = onClick.GetHashCode();
            return hash;
        }
    }

}


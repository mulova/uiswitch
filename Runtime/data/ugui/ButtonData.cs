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
            if (this.onClick.GetPersistentEventCount() == that.onClick.GetPersistentEventCount())
            {
                for (int i=0; i<onClick.GetPersistentEventCount(); ++i)
                {
                    if (onClick.GetPersistentMethodName(i) != that.onClick.GetPersistentMethodName(i))
                    {
                        return false;
                    }
                    //if (onClick.GetPersistentTarget(i) != that.onClick.GetPersistentTarget(i))
                    //{
                    //    return false;
                    //}
                }
                return true;
            } else
            {
                return false;
            }
        }

        protected override int ComputeHashCode()
        {
            var hash = onClick.GetHashCode();
            return hash;
        }
    }

}


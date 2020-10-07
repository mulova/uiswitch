using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Selectable;

namespace mulova.switcher
{
    [Serializable]
    public abstract class SelectableData<S> : ICompData where S : Selectable
    {
        public bool enabled;

        public Type type => typeof(S);
        public bool active => enabled;

        public S comp;
        public bool interactable;
        public Transition transition;
        public Navigation navigation;
        public Graphic targetGraphic;
        public ColorBlock colors;
        public SpriteState spriteStates;
        public AnimationTriggers animationTriggers;

        public Component target
        {
            get { return comp; }
            set { comp = value as S; }
        }

        protected abstract void ApplyTo(S s);
        protected abstract void Collect(S s);
        protected abstract bool Equals(SelectableData<S> data);
        protected abstract int ComputeHashCode();

        public void ApplyTo(Component c)
        {
            var s = c as S;
            s.interactable = interactable;
            s.transition = transition;
            s.navigation = navigation;
            s.targetGraphic = targetGraphic;
            s.colors = colors;
            s.spriteState = spriteStates;
            s.animationTriggers = animationTriggers;
            s.enabled = enabled;
            ApplyTo(s);
        }

        public void Collect(Component c)
        {
            comp = c as S;
            interactable = comp.interactable;
            transition = comp.transition;
            navigation = comp.navigation;
            targetGraphic = comp.targetGraphic;
            colors = comp.colors;
            spriteStates = comp.spriteState;
            animationTriggers = comp.animationTriggers;
            enabled = comp.enabled;
            Collect(comp);
        }

        public override bool Equals(object obj)
        {
            var that = obj as SelectableData<S>;
            return this.interactable == that.interactable
                && this.transition == that.transition
                && this.navigation.Equals(that.navigation)
                && this.targetGraphic == that.targetGraphic
                && this.colors.Equals(that.colors)
                && this.spriteStates.Equals(that.spriteStates)
                && TriggerEquals(this.animationTriggers, that.animationTriggers)
                && this.enabled == that.enabled
                && Equals(that);
        }

        private bool TriggerEquals(AnimationTriggers t1, AnimationTriggers t2)
        {
            if (t1 == t2)
            {
                return true;
            }
            if (t1 == null || t2 == null)
            {
                return false;
            }
            return t1.disabledTrigger == t2.disabledTrigger
                && t1.highlightedTrigger == t2.highlightedTrigger
                && t1.normalTrigger == t2.normalTrigger
                && t1.pressedTrigger == t2.pressedTrigger
                && t1.selectedTrigger == t2.selectedTrigger;
        }

        public override int GetHashCode()
        {
            var hash = interactable.GetHashCode();
            hash = hash * 37 + transition.GetHashCode();
            hash = hash * 37 + navigation.GetHashCode();
            hash = hash * 37 + targetGraphic.GetHashCode();
            hash = hash * 37 + colors.GetHashCode();
            hash = hash * 37 + spriteStates.GetHashCode();
            if (animationTriggers != null)
            {
                hash = hash * 37 + animationTriggers.GetHashCode();
            }
            hash = hash * 37 + ComputeHashCode();
            return hash;
        }
    }
}


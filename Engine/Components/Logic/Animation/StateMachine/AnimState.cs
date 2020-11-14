using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Animation;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Files;

namespace TheraEngine.Components.Logic.Animation
{
    public class AnimState : TObject
    {
        private EventList<AnimStateTransition> _transitions = new EventList<AnimStateTransition>();
        private GlobalFileRef<PoseGenBase> _animation;
        private float _startSecond = 0.0f;
        private float _endSecond = 0.0f;

        [Browsable(false)]
        public AnimStateMachineComponent Owner { get; internal set; }

        /// <summary>
        /// All possible transitions to move out of this state and into another state.
        /// </summary>
        [TSerialize]
        public EventList<AnimStateTransition> Transitions
        {
            get => _transitions;
            set
            {
                if (_transitions != null)
                {
                    _transitions.PostAnythingAdded -= _transitions_PostAnythingAdded;
                    _transitions.PostAnythingRemoved -= _transitions_PostAnythingRemoved;
                }
                _transitions = value ?? new EventList<AnimStateTransition>();
                _transitions.PostAnythingAdded += _transitions_PostAnythingAdded;
                _transitions.PostAnythingRemoved += _transitions_PostAnythingRemoved;
                foreach (AnimStateTransition transition in _transitions)
                    _transitions_PostAnythingAdded(transition);
            }
        }

        /// <summary>
        /// The pose retrieval animation to use to retrieve a result.
        /// </summary>
        [TSerialize]
        public GlobalFileRef<PoseGenBase> Animation
        {
            get => _animation;
            set => Set(ref _animation, value);
        }
        [TSerialize]
        public float StartSecond
        {
            get => _startSecond;
            set => Set(ref _startSecond, value);
        }
        [TSerialize]
        public float EndSecond
        {
            get => _endSecond;
            set => Set(ref _endSecond, value);
        }

        public AnimState() { }
        public AnimState(GlobalFileRef<PoseGenBase> animation)
        {
            Animation = animation;
        }
        public AnimState(GlobalFileRef<PoseGenBase> animation, params AnimStateTransition[] transitions)
        {
            Animation = animation;
            Transitions = new EventList<AnimStateTransition>(transitions);
        }
        public AnimState(GlobalFileRef<PoseGenBase> animation, List<AnimStateTransition> transitions)
        {
            Animation = animation;
            Transitions = new EventList<AnimStateTransition>(transitions);
        }
        public AnimState(GlobalFileRef<PoseGenBase> animation, EventList<AnimStateTransition> transitions)
        {
            Animation = animation;
            Transitions = new EventList<AnimStateTransition>(transitions);
        }

        /// <summary>
        /// Attempts to find any transitions that evaluate to true and returns the one with the highest priority.
        /// </summary>
        public AnimStateTransition TryTransition()
        {
            AnimStateTransition[] transitions =
                Transitions.
                FindAll(x => x.Condition()).
                OrderBy(x => x.Priority).
                ToArray();

            return transitions.Length > 0 ? transitions[0] : null;
        }
        public void Tick(float delta)
        {
            Animation?.File?.Tick(delta);
        }
        public SkeletalAnimationPose GetFrame()
        {
            return Animation?.File?.GetPose();
        }
        public void OnStarted()
        {

        }
        public void OnEnded()
        {

        }
        private void _transitions_PostAnythingRemoved(AnimStateTransition item)
        {
            if (item.Owner == this)
                item.Owner = null;
        }
        private void _transitions_PostAnythingAdded(AnimStateTransition item)
        {
            item.Owner = this;
        }
    }
}

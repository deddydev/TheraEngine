using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Animation;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Files;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Components.Logic.Animation
{
    public class AnimState
    {
        private EventList<AnimStateTransition> _transitions = new EventList<AnimStateTransition>();
        private GlobalFileRef<SkelAnimPoseGenBase> _poseRef;

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
        public GlobalFileRef<SkelAnimPoseGenBase> Animation { get; set; }
        [TSerialize]
        public float StartSecond { get; set; } = 0.0f;
        [TSerialize]
        public float EndSecond { get; set; } = -1.0f;
        
        public AnimState() { }
        public AnimState(GlobalFileRef<SkelAnimPoseGenBase> animation)
        {
            Animation = animation;
        }
        public AnimState(GlobalFileRef<SkelAnimPoseGenBase> animation, params AnimStateTransition[] transitions)
        {
            Animation = animation;
            Transitions = new EventList<AnimStateTransition>(transitions);
        }
        public AnimState(GlobalFileRef<SkelAnimPoseGenBase> animation, List<AnimStateTransition> transitions)
        {
            Animation = animation;
            Transitions = new EventList<AnimStateTransition>(transitions);
        }
        public AnimState(GlobalFileRef<SkelAnimPoseGenBase> animation, EventList<AnimStateTransition> transitions)
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
                FindAll(x => x.ConditionMethod()).
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

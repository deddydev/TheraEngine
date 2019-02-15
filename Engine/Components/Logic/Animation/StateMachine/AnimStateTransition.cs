using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Animation;
using TheraEngine.Core.Attributes;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Files;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Components.Logic.Animation
{
    /// <summary>
    /// Describes a condition and how to transition to a new state.
    /// </summary>
    public class AnimStateTransition
    {
        [Browsable(false)]
        public AnimState Owner { get; internal set; }

        public event Action Started;
        public event Action Finished;

        /// <summary>
        /// The index of the next state to go to if this transition's condition method returns true.
        /// </summary>
        [TDropDownIndexSelector("Owner.Owner.States")]
        [TSerialize]
        public int DestinationStateIndex { get; set; }
        /// <summary>
        /// The condition to test every frame if this transition should occur.
        /// </summary>
        [TSerialize]
        public Func<bool> ConditionMethod { get; set; }
        /// <summary>
        /// How quickly the current state should blend into the next, in seconds.
        /// </summary>
        [TSerialize]
        public float BlendDuration { get; set; }
        /// <summary>
        /// The interpolation method to use to blend to the next state.
        /// </summary>
        [TSerialize]
        public EAnimBlendType BlendType { get; set; }
        /// <summary>
        /// If <see cref="BlendType"/> == <see cref="EAnimBlendType.Custom"/>, 
        /// uses these keyframes to interpolate between 0.0f and 1.0f.
        /// </summary>
        [TSerialize]
        public KeyframeTrack<FloatKeyframe> CustomBlendFunction { get; set; }
        /// <summary>
        /// If multiple transitions evaluate to true at the same time, this dictates which transition will occur.
        /// </summary>
        [TSerialize]
        public int Priority { get; set; } = 0;

        internal void OnFinished()
        {
            Started?.Invoke();
        }
        internal void OnStarted()
        {
            Finished?.Invoke();
        }
    }
}

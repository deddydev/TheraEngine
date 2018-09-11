using TheraEngine.Rendering.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Files;
using System.Linq;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using TheraEngine.Animation;

namespace TheraEngine.Components.Logic.Animation
{
    [FileDef("Animation State Machine Component")]
    public class AnimStateMachineComponent : LogicComponent
    {
        [TSerialize("InitialStateIndex", XmlNodeType = EXmlNodeType.Attribute)]
        private int _initialStateIndex;
        [TSerialize("States", XmlNodeType = EXmlNodeType.Attribute)]
        private List<AnimState> _states;
        [TSerialize("Skeleton", XmlNodeType = EXmlNodeType.Attribute)]
        private GlobalFileRef<Skeleton> _skeleton;
        
        private BlendManager _blendManager;
        private AnimState _currentState;

        public AnimStateMachineComponent()
        {
            _initialStateIndex = -1;
            _states = new List<AnimState>();
            _skeleton = new GlobalFileRef<Skeleton>();
        }
        public AnimStateMachineComponent(Skeleton skeleton)
        {
            _initialStateIndex = -1;
            _states = new List<AnimState>();
            _skeleton = skeleton;
        }

        [PostDeserialize]
        private void InitPostDeserialize()
        {

        }

        public AnimState InitialState
        {
            get => _initialStateIndex >= 0 ? _states[_initialStateIndex] : null;
            set
            {
                bool wasNull = _initialStateIndex < 0;
                int index = _states.IndexOf(value);
                if (index >= 0)
                    _initialStateIndex = index;
                else if (value != null)
                {
                    _initialStateIndex = _states.Count;
                    _states.Add(value);
                }
                else
                    _initialStateIndex = -1;

                if (IsSpawned && wasNull && _initialStateIndex >= 0)
                {
                    _currentState = InitialState;
                    _blendManager = new BlendManager(InitialState.Animation);
                    RegisterTick(ETickGroup.PrePhysics, ETickOrder.Animation, Tick);
                }
            }
        }
        public GlobalFileRef<Skeleton> Skeleton
        {
            get => _skeleton;
            set => _skeleton = value;
        }

        public override void OnSpawned()
        {
            if (_initialStateIndex >= 0)
            {
                _currentState = InitialState;
                _blendManager = new BlendManager(InitialState.Animation);
                RegisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, Tick);
            }
        }
        public override void OnDespawned()
        {
            if (_initialStateIndex >= 0)
            {
                UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, Tick);
                _blendManager = null;
            }
        }
        protected internal void Tick(float delta)
        {
            AnimStateTransition transition = _currentState?.TryTransition();
            if (transition != null)
            {
                _currentState = _states[transition.DestinationStateIndex];
                _blendManager.QueueState(_currentState.Animation, transition.BlendDuration, transition.BlendType, transition.CustomBlendFunction);
            }
            _blendManager?.Tick(delta, Skeleton.File);
        }
    }
    public class AnimState
    {
        [TSerialize]
        public List<AnimStateTransition> Transitions { get; set; } = new List<AnimStateTransition>();
        [TSerialize]
        public GlobalFileRef<SkeletalAnimation> Animation { get; set; }

        public AnimState() { }
        public AnimState(SkeletalAnimation animation)
        {
            Animation = animation;
        }
        public AnimState(SkeletalAnimation animation, params AnimStateTransition[] transitions)
        {
            Animation = animation;
            Transitions = transitions.ToList();
        }
        public AnimState(SkeletalAnimation animation, List<AnimStateTransition> transitions)
        {
            Animation = animation;
            Transitions = new List<AnimStateTransition>(transitions);
        }

        public AnimStateTransition TryTransition()
            => Transitions.Find(x => x.ConditionMethod());
    }
    public enum AnimBlendType
    {
        Linear,             //start + (end - start) * time
        CosineEaseInOut,    //start + (end - start) * (1.0f - cos(time)) / 2.0f
        QuadraticEaseStart, //start + (end - start) * time^power
        QuadraticEaseEnd,   //start + (end - start) * (1.0f - (1.0f - time)^power)
        Custom,             //start + (end - start) * customInterp(time)
    }
    public class BlendManager
    {
        private class BlendInfo
        {
            /// <summary>
            /// How long the blend lasts in seconds.
            /// </summary>
            [TSerialize(IsXmlAttribute = true)]
            public float Duration
            {
                get => 1.0f / _invDuration;
                set => _invDuration = 1.0f / value;
            }
            /// <summary>
            /// How long until the blend is done in seconds.
            /// </summary>
            public float ElapsedTime { get; private set; }
            /// <summary>
            /// The blending method to use.
            /// </summary>
            [TSerialize]
            public AnimBlendType Type
            {
                get => _blendType;
                set
                {
                    _blendType = value;
                    switch (_blendType)
                    {
                        default:
                        case AnimBlendType.Linear:
                            _blendFunction = (time) => time;
                            break;
                        case AnimBlendType.CosineEaseInOut:
                            _blendFunction = (time) => Interp.InterpCosineTo(0.0f, 1.0f, time);
                            break;
                        case AnimBlendType.QuadraticEaseStart:
                            _blendFunction = (time) => Interp.InterpQuadraticEaseStart(0.0f, 1.0f, time);
                            break;
                        case AnimBlendType.QuadraticEaseEnd:
                            _blendFunction = (time) => Interp.InterpQuadraticEaseEnd(0.0f, 1.0f, time);
                            break;
                        case AnimBlendType.Custom:
                            _blendFunction = (time) => _customBlendFunction.First.Interpolate(time, EVectorInterpValueType.Position);
                            break;
                    }
                }
            }

            private float _invDuration;
            private AnimBlendType _blendType;
            private Func<float, float> _blendFunction;
            [TSerialize("CustomFunction", Order = 0)]
            private KeyframeTrack<FloatKeyframe> _customBlendFunction;

            public BlendInfo(float blendDuration, AnimBlendType blendType, KeyframeTrack<FloatKeyframe> customFunction)
            {
                Duration = blendDuration;
                ElapsedTime = 0.0f;
                Type = blendType;
                _customBlendFunction = customFunction;
            }

            /// <summary>
            /// Increments the blending time.
            /// Returns true if the blend is done.
            /// </summary>
            internal bool Tick(float delta) => (ElapsedTime += delta) > Duration;
            
            //Multiplying is faster than division, store duration as inverse
            /// <summary>
            /// Returns a value from 0.0f - 1.0f indicating a linear time between two animations.
            /// </summary>
            public float GetBlendTime() => _blendFunction((ElapsedTime * _invDuration).ClampMax(1.0f));
        }

        private LinkedList<SkeletalAnimation> _stateQueue;
        private LinkedList<BlendInfo> _blendQueue;

        public BlendManager(SkeletalAnimation initialState)
        {
            _blendQueue = new LinkedList<BlendInfo>();
            _stateQueue = new LinkedList<SkeletalAnimation>();
            if (initialState != null)
            {
                _stateQueue.AddFirst(initialState);
                initialState.Start();
            }
        }

        public void QueueState(
            SkeletalAnimation destinationState,
            float blendDuration,
            AnimBlendType type,
            KeyframeTrack<FloatKeyframe> customBlendMethod)
        {
            if (destinationState == null)
                return;
            destinationState.Start();
            _stateQueue.AddLast(destinationState);
            if (_stateQueue.Count > 1)
                _blendQueue.AddLast(new BlendInfo(blendDuration, type, customBlendMethod));
        }
        
        internal void Tick(float delta, Skeleton skeleton)
        {
            if (skeleton == null)
                return;

            var stateNode = _stateQueue.First;

            //Tick first animation
            //stateNode.Value.Tick(delta);

            //Get frame of first animation
            SkeletalAnimationFrame frame = stateNode?.Value?.GetFrame();
            if (frame == null)
                return;

            if (_blendQueue.Count > 0)
            {
                //Execute all blends on top of the first animation's frame
                var blendNode = _blendQueue.First;
                while (blendNode != null)
                {
                    //Get the blend info for the current animation and the next
                    BlendInfo blend = blendNode.Value;
                    
                    //Tick the animation to be blended with next
                    stateNode = stateNode.Next;
                    //stateNode.Value.Tick(delta);

                    //Update blending information
                    if (blend.Tick(delta))
                    {
                        //Frame is now entirely the next animation.
                        frame = stateNode.Value.GetFrame();

                        //All previous animations are now irrelevant. Remove them
                        while (_stateQueue.First != stateNode)
                        {
                            _stateQueue.First.Value.Stop();
                            _stateQueue.RemoveFirst();
                        }
                        //Remove all previous blends
                        while (_blendQueue.First != blendNode)
                            _blendQueue.RemoveFirst();
                        //And this one
                        _blendQueue.RemoveFirst();

                        //Get next blend info
                        blendNode = _blendQueue.First;
                    }
                    else
                    {
                        //Update frame by blending it with the next animation using the current blending time
                        frame.BlendWith(stateNode.Value.GetFrame(), blend.GetBlendTime());

                        //Increment to next blend info
                        blendNode = blendNode.Next;
                    }
                }
            }

            //Update the skeleton with the final blended frame
            frame.UpdateSkeleton(skeleton);
        }
    }
    /// <summary>
    /// Describes a condition and how to transition to a new state.
    /// </summary>
    public class AnimStateTransition
    {
        /// <summary>
        /// The index of the next state to go to if this transition's condition method returns true.
        /// </summary>
        public int DestinationStateIndex { get; set; }
        /// <summary>
        /// The condition to test every frame if this transition should occur.
        /// </summary>
        public Func<bool> ConditionMethod { get; set; }
        /// <summary>
        /// How quickly the current state should blend into the next, in seconds.
        /// </summary>
        public float BlendDuration { get; set; }
        /// <summary>
        /// The interpolation method to use to blend to the next state.
        /// </summary>
        public AnimBlendType BlendType { get; set; }
        /// <summary>
        /// If <see cref="BlendType"/> == <see cref="AnimBlendType.Custom"/>, 
        /// uses these keyframes to interpolate between 0.0f and 1.0f.
        /// </summary>
        public KeyframeTrack<FloatKeyframe> CustomBlendFunction { get; set; }
    }
}

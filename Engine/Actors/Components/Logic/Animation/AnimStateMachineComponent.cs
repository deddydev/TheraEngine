using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Animation;
using TheraEngine.Core.Maths;
using TheraEngine.Files;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Components.Logic.Animation
{
    public abstract class SkeletalAnimationResult : TFileObject
    {
        public abstract SkeletalAnimationFrame GetFrame();
        public abstract float CurrentTime { get; set; }
        public abstract void Tick(float delta);
    }
    public class SkeletalAnimationPose : SkeletalAnimationResult
    {
        public override float CurrentTime
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public override SkeletalAnimationFrame GetFrame()
        {
            throw new NotImplementedException();
        }
        public override void Tick(float delta)
        {
            throw new NotImplementedException();
        }
    }
    [FileDef("Animation State Machine Component")]
    public class AnimStateMachineComponent : LogicComponent
    {
        [TSerialize("InitialStateIndex", XmlNodeType = EXmlNodeType.Attribute)]
        private int _initialStateIndex = -1;
        [TSerialize("States", XmlNodeType = EXmlNodeType.Attribute)]
        private EventList<AnimState> _states;
        [TSerialize("Skeleton", XmlNodeType = EXmlNodeType.Attribute)]
        private LocalFileRef<Skeleton> _skeleton;
        [TSerialize("Animations", XmlNodeType = EXmlNodeType.Attribute)]
        private EventList<GlobalFileRef<SkeletalAnimation>> _animations;

        private BlendManager _blendManager;
        private AnimState _currentState;

        public AnimStateMachineComponent()
        {
            _initialStateIndex = -1;
            States = new EventList<AnimState>();
            _skeleton = new LocalFileRef<Skeleton>();
        }
        public AnimStateMachineComponent(Skeleton skeleton)
        {
            _initialStateIndex = -1;
            States = new EventList<AnimState>();
            _skeleton = skeleton;
        }

        //[PostDeserialize]
        //private void InitPostDeserialize()
        //{

        //}

        public AnimState InitialState
        {
            get => States.IndexInRange(_initialStateIndex) ? States[_initialStateIndex] : null;
            set
            {
                bool wasNull = !States.IndexInRange(_initialStateIndex);
                int index = States.IndexOf(value);
                if (index >= 0)
                    _initialStateIndex = index;
                else if (value != null)
                {
                    _initialStateIndex = States.Count;
                    States.Add(value);
                }
                else
                    _initialStateIndex = -1;

                if (IsSpawned && wasNull && States.IndexInRange(_initialStateIndex))
                {
                    _currentState = InitialState;
                    _blendManager = new BlendManager(InitialState);
                    RegisterTick(ETickGroup.PrePhysics, ETickOrder.Animation, Tick);
                }
            }
        }
        public LocalFileRef<Skeleton> Skeleton
        {
            get => _skeleton;
            set => _skeleton = value;
        }
        public EventList<AnimState> States
        {
            get => _states;
            set => _states = value;
        }

        public override void OnSpawned()
        {
            if (States.IndexInRange(_initialStateIndex))
            {
                _currentState = InitialState;
                _blendManager = new BlendManager(InitialState);
                RegisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, Tick);
            }
        }
        public override void OnDespawned()
        {
            if (States.IndexInRange(_initialStateIndex))
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
                _currentState = States[transition.DestinationStateIndex];
                _blendManager.QueueState(_currentState, transition.BlendDuration, transition.BlendType, transition.CustomBlendFunction);
            }
            _blendManager?.Tick(delta, Skeleton.File);
        }
    }
    public class AnimState
    {
        /// <summary>
        /// All possible transitions to move out of this state and into another state.
        /// </summary>
        [TSerialize]
        public List<AnimStateTransition> Transitions { get; set; } = new List<AnimStateTransition>();
        /// <summary>
        /// The pose retrieval animation to use to retrieve a result.
        /// </summary>
        [TSerialize]
        public GlobalFileRef<SkeletalAnimationResult> Animation { get; set; }
        [TSerialize]
        public float StartSecond { get; set; } = 0.0f;
        [TSerialize]
        public float EndSecond { get; set; } = -1.0f;


        
        public AnimState() { }
        public AnimState(GlobalFileRef<SkeletalAnimationResult> animation)
        {
            Animation = animation;
        }
        public AnimState(GlobalFileRef<SkeletalAnimationResult> animation, params AnimStateTransition[] transitions)
        {
            Animation = animation;
            Transitions = transitions.ToList();
        }
        public AnimState(GlobalFileRef<SkeletalAnimationResult> animation, List<AnimStateTransition> transitions)
        {
            Animation = animation;
            Transitions = new List<AnimStateTransition>(transitions);
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

        }
        public SkeletalAnimationFrame GetFrame()
        {
            return null;
        }
        public void Begin()
        {

        }
        public void End()
        {

        }
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
                get => _invDuration == 0.0f ? 0.0f : 1.0f / _invDuration;
                set => _invDuration = value == 0.0f ? 0.0f : 1.0f / value;
            }
            /// <summary>
            /// How long until the blend is done in seconds.
            /// </summary>
            [TSerialize(State = true)]
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

            public BlendInfo() { }
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
            public float GetBlendTime() => _blendFunction(_invDuration == 0.0f ? 1.0f : (ElapsedTime * _invDuration).ClampMax(1.0f));
        }

        private LinkedList<AnimState> _stateQueue;
        private LinkedList<BlendInfo> _blendQueue;

        public BlendManager(AnimState initialState)
        {
            _blendQueue = new LinkedList<BlendInfo>();
            _stateQueue = new LinkedList<AnimState>();
            if (initialState != null)
            {
                _stateQueue.AddFirst(initialState);
                //initialState.Start();
            }
        }

        public void QueueState(
            AnimState destinationState,
            float blendDuration,
            AnimBlendType type,
            KeyframeTrack<FloatKeyframe> customBlendMethod)
        {
            if (destinationState == null)
                return;
            //destinationState.Start();
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
            AnimState anim = stateNode?.Value;
            SkeletalAnimationFrame baseFrame = null;
            if (anim != null)
            {
                baseFrame = anim.GetFrame();
                anim.Tick(delta);
            }

            if (baseFrame == null)
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
                    anim = stateNode?.Value;
                    if (anim != null)
                    {
                        //Update blending information
                        if (blend.Tick(delta))
                        {
                            //Frame is now entirely the next animation.
                            baseFrame = anim.GetFrame();

                            //All previous animations are now irrelevant. Remove them
                            while (_stateQueue.First != stateNode)
                            {
                                //_stateQueue.First.Value.Stop();
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
                            baseFrame?.BlendWith(stateNode.Value.GetFrame(), blend.GetBlendTime());

                            //Increment to next blend info
                            blendNode = blendNode.Next;
                        }

                        anim.Tick(delta);
                    }
                }
            }

            //Update the skeleton with the final blended frame
            baseFrame.UpdateSkeleton(skeleton);
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
        /// <summary>
        /// If multiple transitions evaluate to true at the same time, this dictates which transition will occur.
        /// </summary>
        public int Priority { get; set; } = 0;
    }
}

using TheraEngine.Rendering.Models;
using TheraEngine.Worlds.Actors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Files;
using System.Linq;

namespace TheraEngine.Animation
{
    [FileClass("CSTM", "Animation State Machine Component")]
    public class AnimStateMachineComponent : LogicComponent
    {
        [Serialize("InitialStateIndex", true)]
        private int _initialStateIndex;
        [Serialize("States", true)]
        private List<AnimState> _states;
        [Serialize("SkeletonRef", true)]
        private SingleFileRef<Skeleton> _skeleton;
        
        [State]
        private BlendManager _blendManager;
        [State]
        private AnimState _currentState;

        public AnimStateMachineComponent()
        {
            _initialStateIndex = -1;
            _states = new List<AnimState>();
            _skeleton = new SingleFileRef<Skeleton>();
        }

        public AnimState InitialState
        {
            get => _initialStateIndex >= 0 ? _states[_initialStateIndex] : null;
            set
            {
                int index = _states.IndexOf(value);
                if (index >= 0)
                    _initialStateIndex = index;
                else
                {
                    _initialStateIndex = _states.Count;
                    _states.Add(value);
                }
            }
        }
        public SingleFileRef<Skeleton> Skeleton
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
                RegisterTick(ETickGroup.PrePhysics, ETickOrder.BoneAnimation, Tick);
            }
        }
        public override void OnDespawned()
        {
            if (_initialStateIndex >= 0)
            {
                UnregisterTick(ETickGroup.PrePhysics, ETickOrder.BoneAnimation, Tick);
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
            _blendManager.Tick(delta, Skeleton.File);
        }
    }
    public class AnimState
    {
        [Serialize]
        public List<AnimStateTransition> Transitions { get; set; } = new List<AnimStateTransition>();
        [Serialize]
        public SingleFileRef<ModelAnimation> Animation { get; set; }

        public AnimState() { }
        public AnimState(ModelAnimation animation)
        {
            Animation = animation;
        }
        public AnimState(ModelAnimation animation, params AnimStateTransition[] transitions)
        {
            Animation = animation;
            Transitions = transitions.ToList();
        }
        public AnimState(ModelAnimation animation, List<AnimStateTransition> transitions)
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
        QuadraticEaseEnd,   //start + (end - start) * (1.0f - (1.0f - x)^power)
        Custom,             //start + (end - start) * customInterp(time)
    }
    public class BlendManager
    {
        private class BlendInfo
        {
            /// <summary>
            /// How long the blend lasts in seconds.
            /// </summary>
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
                            _blendFunction = (time) => CustomMath.InterpCosineTo(0.0f, 1.0f, time);
                            break;
                        case AnimBlendType.QuadraticEaseStart:
                            _blendFunction = (time) => CustomMath.InterpQuadraticEaseStart(0.0f, 1.0f, time);
                            break;
                        case AnimBlendType.QuadraticEaseEnd:
                            _blendFunction = (time) => CustomMath.InterpQuadraticEaseEnd(0.0f, 1.0f, time);
                            break;
                        case AnimBlendType.Custom:
                            _blendFunction = (time) => _customBlendFunction.First.Interpolate(time);
                            break;
                    }
                }
            }

            private float _invDuration;
            private AnimBlendType _blendType;
            private Func<float, float> _blendFunction;
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

        private LinkedList<ModelAnimation> _stateQueue;
        private LinkedList<BlendInfo> _blendQueue;

        public BlendManager(ModelAnimation initialState)
        {
            _blendQueue = new LinkedList<BlendInfo>();
            _stateQueue = new LinkedList<ModelAnimation>();
            _stateQueue.AddFirst(initialState);
        }

        public void QueueState(
            ModelAnimation destinationState,
            float blendDuration,
            AnimBlendType type,
            KeyframeTrack<FloatKeyframe> customBlendMethod)
        {
            _stateQueue.AddLast(destinationState);
            _blendQueue.AddLast(new BlendInfo(blendDuration, type, customBlendMethod));
        }
        
        internal void Tick(float delta, Skeleton skeleton)
        {
            if (skeleton == null)
                return;

            var stateNode = _stateQueue.First;

            //Tick first animation
            stateNode.Value.Tick(delta);

            //Get frame of first animation
            ModelAnimationFrame frame = stateNode.Value.GetFrame();

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
                    stateNode.Value.Tick(delta);

                    //Update blending information
                    if (blend.Tick(delta))
                    {
                        //Frame is now entirely the next animation.
                        frame = stateNode.Value.GetFrame();

                        //All previous animations are now irrelevant. Remove them
                        while (_stateQueue.First != stateNode)
                            _stateQueue.RemoveFirst();

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
    public class AnimStateTransition
    {
        public int DestinationStateIndex { get; set; }
        public Func<bool> ConditionMethod { get; set; }
        public float BlendDuration { get; set; }
        public AnimBlendType BlendType { get; set; }
        public KeyframeTrack<FloatKeyframe> CustomBlendFunction { get; set; }
    }
}

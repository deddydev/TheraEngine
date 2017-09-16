using TheraEngine.Rendering.Models;
using TheraEngine.Worlds.Actors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Files;

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
                _blendManager = new BlendManager(this, InitialState.Animation);
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
            _currentState.TryTransition(this);
            _blendManager.Tick(delta, Skeleton.File);
        }
        public void SetCurrentState(AnimState destinationState, float blendDuration, AnimBlendType type, KeyframeTrack<FloatKeyframe> customBlendMethod)
        {
            _currentState = destinationState;
            _blendManager.QueueState(destinationState.Animation, blendDuration, type, customBlendMethod);
        }
    }
    public class AnimState
    {
        public List<AnimStateTransition> Transitions { get; set; } = new List<AnimStateTransition>();
        public ModelAnimation Animation { get; set; }

        public void TryTransition(AnimStateMachineComponent machine)
        {
            foreach (AnimStateTransition t in Transitions)
                if (t.TryTransition(machine))
                    break;
        }
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
            public float BlendDuration { get; set; }
            /// <summary>
            /// How long until the blend is done in seconds.
            /// </summary>
            public float ElapsedBlendTime { get; private set; }
            /// <summary>
            /// The blending method to use.
            /// </summary>
            public AnimBlendType Type { get; set; }

            public BlendInfo(float blendDuration, AnimBlendType type)
            {
                BlendDuration = blendDuration;
                ElapsedBlendTime = 0.0f;
                Type = type;
            }

            internal void Tick(float delta)
            {
                ElapsedBlendTime = (ElapsedBlendTime + delta).ClampMax(BlendDuration);
            }
        }

        private LinkedList<ModelAnimation> _stateQueue;
        private LinkedList<BlendInfo> _blendQueue;
        private LinkedList<KeyframeTrack<FloatKeyframe>> _blendMethodQueue;
        private AnimStateMachineComponent _machine;

        public BlendManager(AnimStateMachineComponent machine, ModelAnimation initialState)
        {
            _stateQueue = new LinkedList<ModelAnimation>();
            _blendQueue = new LinkedList<BlendInfo>();
            _blendMethodQueue = new LinkedList<KeyframeTrack<FloatKeyframe>>();
            _stateQueue.AddFirst(initialState);
            _machine = machine;
        }
        public void QueueState(
            ModelAnimation destinationState,
            float blendDuration,
            AnimBlendType type,
            KeyframeTrack<FloatKeyframe> customBlendMethod)
        {
            _stateQueue.AddLast(destinationState);
            if (type == AnimBlendType.Custom)
                if (customBlendMethod != null)
                    _blendMethodQueue.AddLast(customBlendMethod);
                else
                    type = AnimBlendType.Linear;

            _blendQueue.AddLast(new BlendInfo(blendDuration, type));
        }
        
        internal void Tick(float delta, Skeleton skeleton)
        {
            var stateNode = _stateQueue.First;
            var blendMethodNode = _blendMethodQueue.First;
            stateNode.Value.Tick(delta);
            ModelAnimationFrame frame = stateNode.Value.GetFrame();
            foreach (BlendInfo blendInfo in _blendQueue)
            {
                stateNode = stateNode.Next;

                blendInfo.Tick(delta);
                float funcTime = blendInfo.ElapsedBlendTime / blendInfo.BlendDuration;
                switch (blendInfo.Type)
                {
                    default:
                    case AnimBlendType.Linear:
                        break;
                    case AnimBlendType.CosineEaseInOut:
                        funcTime = CustomMath.InterpCosineTo(0.0f, 1.0f, funcTime);
                        break;
                    case AnimBlendType.QuadraticEaseStart:
                        funcTime = CustomMath.InterpQuadraticEaseStart(0.0f, 1.0f, funcTime);
                        break;
                    case AnimBlendType.QuadraticEaseEnd:
                        funcTime = CustomMath.InterpQuadraticEaseEnd(0.0f, 1.0f, funcTime);
                        break;
                    case AnimBlendType.Custom:
                        funcTime = blendMethodNode.Value.First.Interpolate(funcTime);
                        blendMethodNode = blendMethodNode.Next;
                        break;
                }

                stateNode.Value.Tick(delta);
                frame.BlendWith(stateNode.Value.GetFrame(), funcTime);
            }
            frame.UpdateSkeleton(skeleton);
        }
    }
    public class AnimStateTransition
    {
        public AnimState DestinationState { get; set; }
        public Func<bool> ConditionMethod { get; set; }
        public float BlendDuration { get; set; }
        public AnimBlendType BlendType { get; set; }
        public KeyframeTrack<FloatKeyframe> CustomBlendFunction { get; set; }

        public bool TryTransition(AnimStateMachineComponent machine)
        {
            bool canTransition = ConditionMethod();
            if (canTransition)
                machine.SetCurrentState(DestinationState, BlendDuration, BlendType, CustomBlendFunction);
            return canTransition;
        }
    }
}

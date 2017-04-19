using CustomEngine.Files;
using CustomEngine.Rendering.Models;
using CustomEngine.Worlds.Actors;
using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Animation
{
    public class AnimBlendState
    {
        AnimBlendState _previous = null;
        AnimState _current;
        float _remainingBlendTime;

        public void Apply(Skeleton skeleton)
        {

        }

        /// <summary>
        /// Returns true if done blending.
        /// </summary>
        public bool TickBlendTime(float delta)
        {
            _remainingBlendTime -= delta;
            return _remainingBlendTime <= 0.0f;
        }
    }

    [ObjectHeader()]
    [FileClass()]
    public class AnimStateMachine : LogicComponent
    {
        private AnimState _initialState;
        private Dictionary<string, AnimState> _states;
        private AnimState _previousState = null;
        private AnimState _currentState;
        private Skeleton _skeleton;
        private float _remainingBlendTime;
        
        public AnimState CurrentState => _currentState;
        public AnimState InitialState
        {
            get => _initialState;
            set => _initialState = value;
        }

        public override void OnSpawned()
        {
            _currentState = _initialState;
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Animation);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            UnregisterTick();
            base.OnDespawned();
        }
        protected internal override void Tick(float delta)
        {
            _currentState.TryTransition(this);
        }
        public void Start()
        {

        }
        public void End()
        {

        }
        public void SetCurrentState(AnimState destinationState, float blendDuration, AnimBlendType type, KeyframeTrack<FloatKeyframe> customBlendMethod)
        {

        }
    }
    public class AnimState
    {
        public ModelAnimation _animation;
        public List<AnimStateTransition> _transitions = new List<AnimStateTransition>();
        public void TryTransition(AnimStateMachine machine)
        {
            foreach (AnimStateTransition t in _transitions)
                if (t.TryTransition(machine))
                    return;
        }
        public void Tick(Skeleton skel)
        {

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
    public class AnimStateTransition
    {
        AnimState _destinationState;
        Func<bool> _transitionMethod;
        float _blendDuration;
        AnimBlendType _type;
        KeyframeTrack<FloatKeyframe> _customBlendMethod;

        public AnimStateTransition()
        {

        }

        public bool TryTransition(AnimStateMachine machine)
        {
            bool canTransition = _transitionMethod();
            if (canTransition)
                machine.SetCurrentState(_destinationState, _blendDuration, _type, _customBlendMethod);
            return canTransition;
        }
    }
}

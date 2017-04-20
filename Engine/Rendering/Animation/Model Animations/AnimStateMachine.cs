﻿using CustomEngine.Files;
using CustomEngine.Rendering.Models;
using CustomEngine.Worlds.Actors;
using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Animation
{
    public class AnimBlendState
    {
        public event Action DoneBlending;

        public AnimBlendState(AnimState state)
        {
            _current = state;
            _previous = null;
            _remainingBlendTime = 0.0f;
        }
        public AnimBlendState(AnimState state, AnimBlendState prevState)
        {
            _current = state;
            _previous = prevState;
            _remainingBlendTime = 0.0f;
        }
        public AnimBlendState(
            AnimState state,
            AnimBlendState prevState,
            float blendTime,
            AnimBlendType type,
            KeyframeTrack<FloatKeyframe> customBlendMethod,
            Action onDoneBlending)
        {
            _current = state;
            _previous = prevState;
            _remainingBlendTime = _totalBlendTime = blendTime;
            _type = type;
            _customBlendMethod = customBlendMethod;
            DoneBlending += onDoneBlending;
        }

        AnimBlendState _previous;
        AnimState _current;
        float _totalBlendTime;
        float _remainingBlendTime;
        AnimBlendType _type;
        KeyframeTrack<FloatKeyframe> _customBlendMethod;

        public ModelAnimationFrame GetFrame()
        {
            if (_previous == null)
                return _current._animation.GetFrame();
            else
                return _previous.GetFrame().BlendedWith(_current._animation.GetFrame(), GetBlendTime());
        }

        public float GetBlendTime()
        {
            float funcTime = 1.0f - (_remainingBlendTime / _totalBlendTime);
            switch (_type)
            {
                default:
                case AnimBlendType.Linear:
                    return CustomMath.Lerp(0.0f, 1.0f, funcTime);
                case AnimBlendType.CosineEaseInOut:
                    return CustomMath.InterpCosineTo(0.0f, 1.0f, funcTime);
                case AnimBlendType.QuadraticEaseStart:
                    return CustomMath.InterpQuadraticEaseStart(0.0f, 1.0f, funcTime);
                case AnimBlendType.QuadraticEaseEnd:
                    return CustomMath.InterpQuadraticEaseEnd(0.0f, 1.0f, funcTime);
                case AnimBlendType.Custom:
                    return _customBlendMethod.First.Interpolate(funcTime);
            }
        }

        public void Apply(Skeleton skeleton)
        {
            GetFrame().UpdateSkeleton(skeleton);
        }

        /// <summary>
        /// Returns true if done blending.
        /// </summary>
        public bool TickBlendTime(float delta)
        {
            _previous?.TickBlendTime(delta);
            if (_remainingBlendTime <= 0.0f)
                return true;
            _remainingBlendTime -= delta;
            bool done = _remainingBlendTime <= 0.0f;
            if (done)
                DoneBlending?.Invoke();
            return done;
        }
    }

    [ObjectHeader()]
    [FileClass()]
    public class AnimStateMachine : LogicComponent
    {
        private AnimState _initialState;
        private Dictionary<string, AnimState> _states;
        private AnimBlendState _currentState;
        private Skeleton _skeleton;
        private float _remainingBlendTime;
        
        public AnimState InitialState
        {
            get => _initialState;
            set => _initialState = value;
        }

        public override void OnSpawned()
        {
            _currentState = new AnimBlendState(_initialState);
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

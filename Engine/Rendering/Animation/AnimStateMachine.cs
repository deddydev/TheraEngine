using CustomEngine.Files;
using CustomEngine.Rendering.Models;
using CustomEngine.Worlds.Actors;
using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Animation
{
    [ObjectHeader()]
    [FileClass()]
    public class AnimStateMachine : LogicComponent
    {
        private AnimState _initialState;
        private Dictionary<string, AnimState> _states;
        private AnimState _currentState;
        private Skeleton _skeleton;

        internal void SetCurrentState(AnimState state)
            => _currentState = state;
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
        Custom,
    }
    public class AnimStateTransition
    {
        AnimState _destinationState;
        Func<bool> _method;
        float _blendDuration;
        AnimBlendType _type;

        public AnimStateTransition()
        {

        }

        public bool TryTransition(AnimStateMachine machine)
        {
            bool canTransition = _method();
            if (canTransition)
                machine.SetCurrentState(_destinationState);
            return canTransition;
        }
    }
}

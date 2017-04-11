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
        private AnimState _currentState;
        private Skeleton _skeleton;

        public override void OnSpawned()
        {
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
            _currentState.TryTransition();
        }
    }
    public class AnimState
    {
        public List<AnimStateTransition> _transitions = new List<AnimStateTransition>();
        public AnimState TryTransition()
        {
            foreach (AnimStateTransition t in _transitions)
            {
                AnimState nextState = t.TryTransition();
                if (nextState != null)
                    return nextState;
            }
            return null;
        }
    }
    public class AnimStateTransition
    {
        AnimState _destinationState;
        public AnimState TryTransition()
        {
            bool canTransition = false;
            return canTransition ? _destinationState : null;
        }
    }
}

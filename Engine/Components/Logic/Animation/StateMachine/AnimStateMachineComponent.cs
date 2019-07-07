using Extensions;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Animation;
using TheraEngine.Core.Files;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Components.Logic.Animation
{
    [TFileDef("Animation State Machine Component")]
    public class AnimStateMachineComponent : LogicComponent, IGlobalFilesContext<SkeletalAnimation>
    {
        [TSerialize(NodeType = ENodeType.Attribute)]
        internal int InitialStateIndex { get; set; } = -1;
        [TSerialize("Skeleton", NodeType = ENodeType.Attribute)]
        public LocalFileRef<Skeleton> Skeleton { get; set; }
        [TSerialize("States", NodeType = ENodeType.Attribute)]
        public EventList<AnimState> States
        {
            get => _states;
            set
            {
                if (_states != null)
                {
                    _states.PostAnythingAdded -= _states_PostAnythingAdded;
                    _states.PostAnythingRemoved -= _states_PostAnythingRemoved;
                }
                _states = value;
                _states.PostAnythingAdded += _states_PostAnythingAdded;
                _states.PostAnythingRemoved += _states_PostAnythingRemoved;
                foreach (AnimState state in _states)
                    _states_PostAnythingAdded(state);
            }
        }
        [TSerialize("Animations")]
        internal ConcurrentDictionary<string, SkeletalAnimation> AnimationTable { get; set; }
        ConcurrentDictionary<string, SkeletalAnimation> IGlobalFilesContext<SkeletalAnimation>.GlobalFileInstances => AnimationTable;

        public AnimState InitialState
        {
            get => States.IndexInRange(InitialStateIndex) ? States[InitialStateIndex] : null;
            set
            {
                bool wasNull = !States.IndexInRange(InitialStateIndex);
                int index = States.IndexOf(value);
                if (index >= 0)
                    InitialStateIndex = index;
                else if (value != null)
                {
                    InitialStateIndex = States.Count;
                    States.Add(value);
                }
                else
                    InitialStateIndex = -1;

                if (wasNull && IsSpawned && States.IndexInRange(InitialStateIndex))
                {
                    _currentState = InitialState;
                    _blendManager = new BlendManager(InitialState);
                    RegisterTick(ETickGroup.PrePhysics, ETickOrder.Animation, Tick);
                }
            }
        }

        private EventList<AnimState> _states;
        private BlendManager _blendManager;
        private AnimState _currentState;

        public AnimStateMachineComponent()
        {
            InitialStateIndex = -1;
            States = new EventList<AnimState>();
            Skeleton = new LocalFileRef<Skeleton>();
        }
        public AnimStateMachineComponent(Skeleton skeleton)
        {
            InitialStateIndex = -1;
            States = new EventList<AnimState>();
            Skeleton = skeleton;
        }
        
        public override void OnSpawned()
        {
            if (States.IndexInRange(InitialStateIndex))
            {
                _currentState = InitialState;
                _blendManager = new BlendManager(InitialState);
                RegisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, Tick);
            }
        }
        public override void OnDespawned()
        {
            if (States.IndexInRange(InitialStateIndex))
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
                _blendManager.QueueState(_currentState, transition);
            }
            _blendManager?.Tick(delta, Skeleton?.File);
        }
        private void _states_PostAnythingRemoved(AnimState item)
        {
            if (item.Owner == this)
                item.Owner = null;
        }
        private void _states_PostAnythingAdded(AnimState item)
        {
            item.Owner = this;
        }
    }
}

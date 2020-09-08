using Extensions;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TheraEngine.Animation;
using TheraEngine.ComponentModel;
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
            set => Set(ref _states, value, UnlinkStates, LinkStates);
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
                    _blendManager = new BlendManager(InitialState);
                    RegisterTick(ETickGroup.PrePhysics, ETickOrder.Animation, Tick);
                }
            }
        }

        private EventList<AnimState> _states;
        private BlendManager _blendManager;

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

        protected override void OnSpawned()
        {
            if (!States.IndexInRange(InitialStateIndex))
                return;
            
            _blendManager = new BlendManager(InitialState);
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Animation, Tick);
        }
        protected override void OnDespawned()
        {
            if (!States.IndexInRange(InitialStateIndex))
                return;
            
            UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Animation, Tick);
            _blendManager = null;
        }

        protected internal void Tick(float delta)
            => _blendManager?.Tick(delta, States, Skeleton?.File);

        private void LinkStates()
        {
            if (_states is null)
                return;

            foreach (AnimState state in _states)
                StateAdded(state);

            _states.PostAnythingAdded += StateAdded;
            _states.PostAnythingRemoved += StateRemoved;
        }
        private void UnlinkStates()
        {
            if (_states is null)
                return;

            _states.PostAnythingAdded -= StateAdded;
            _states.PostAnythingRemoved -= StateRemoved;

            foreach (AnimState state in _states)
                StateRemoved(state);
        }
        private void StateRemoved(AnimState state)
        {
            if (state?.Owner == this)
                state.Owner = null;
        }
        private void StateAdded(AnimState state)
        {
            if (state != null)
                state.Owner = this;
        }
    }
}

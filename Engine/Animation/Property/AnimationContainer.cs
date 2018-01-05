using TheraEngine.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using TheraEngine.Input.Devices;
using System.ComponentModel.Design;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using TheraEngine.Animation.Property;

namespace TheraEngine.Animation
{
    [FileExt("animtree")]
    [FileDef("Property Animation Tree")]
    public class AnimationContainer : FileObject
    {
        public event Action<AnimationContainer> AnimationStarted;
        public event Action<AnimationContainer> AnimationEnded;

        private int _totalAnimCount = 0;
        private AnimFolder _root;
        internal List<TObject> Owners { get; } = new List<TObject>();
        
        [TSerialize("EndedAnimations")]
        private int _endedAnimations = 0;
        [TSerialize("State")]
        private AnimationState _state = AnimationState.Stopped;
        [TSerialize("TickGroup")]
        private ETickGroup _group = ETickGroup.PostPhysics;
        [TSerialize("TickOrder")]
        private ETickOrder _order = ETickOrder.Animation;
        [TSerialize("TickPausedBehavior")]
        private EInputPauseType _pausedBehavior = EInputPauseType.TickAlways;

        [PostDeserialize]
        private void PostDeserialize()
        {
            if (_state == AnimationState.Playing)
            {
                _state = AnimationState.Stopped;
                Start(_group, _order, _pausedBehavior);
            }
        }

        //public AnimationContainer(Action<bool> func, PropAnimBool anim) : this()
        //{

        //}
        //public AnimationContainer(Action<string> func, PropAnimString anim) : this()
        //{

        //}
        //public AnimationContainer(Action<float> func, PropAnimFloat anim) : this()
        //{

        //}

        public AnimationContainer()
        {

        }
        
        public AnimationContainer(AnimFolder rootFolder) : this()
        {
            RootFolder = rootFolder;
        }
        public AnimationContainer(string animationName, string propertyName, bool method, BasePropAnim anim) : this()
        {
            Name = animationName;
            string[] parts = propertyName.Split('.');
            bool first = true;
            AnimFolder last = null;
            foreach (string i in parts)
            {
                if (first)
                {
                    last = RootFolder = new AnimFolder(i);
                    first = false;
                }
                else
                {
                    AnimFolder folder = new AnimFolder(i);
                    last.Children.Add(folder);
                    last = folder;
                }
            }
            if (last != null)
                last.SetAnimation(anim, method);
        }
        private void OwnersModified()
        {
            if (Owners.Count == 0 && IsTicking)
                UnregisterTick(_group, _order, Tick, _pausedBehavior);
            else if (_state == AnimationState.Playing && Owners.Count != 0 && !IsTicking)
                RegisterTick(_group, _order, Tick, _pausedBehavior);
        }

        [TSerialize]
        public AnimFolder RootFolder
        {
            get => _root;
            set
            {
                _root = value;
                _totalAnimCount = _root != null ? _root.Register(this) : 0;
            }
        }

        public AnimationState State
        {
            get => _state;
            set
            {
                if (_state == value)
                    return;
                switch (value)
                {
                    case AnimationState.Playing:
                        if (_state == AnimationState.Paused)
                            SetPaused(false);
                        else
                            Start(_group, _order, _pausedBehavior);
                        break;
                    case AnimationState.Paused:
                        SetPaused(true);
                        break;
                    case AnimationState.Stopped:
                        Stop();
                        break;
                }
            }
        }

        public ETickGroup Group
        {
            get => _group;
            set
            {
                if (_group == value)
                    return;
                if (_state != AnimationState.Stopped)
                {
                    UnregisterTick(_group, _order, Tick, _pausedBehavior);
                    RegisterTick(value, _order, Tick, _pausedBehavior);
                }
                _group = value;
            }
        }
        public ETickOrder Order
        {
            get => _order;
            set
            {
                if (_order == value)
                    return;
                if (_state != AnimationState.Stopped)
                {
                    UnregisterTick(_group, _order, Tick, _pausedBehavior);
                    RegisterTick(_group, value, Tick, _pausedBehavior);
                }
                _order = value;
            }
        }
        public EInputPauseType PausedBehavior
        {
            get => _pausedBehavior;
            set
            {
                if (_pausedBehavior == value)
                    return;
                if (_state != AnimationState.Stopped)
                {
                    UnregisterTick(_group, _order, Tick, _pausedBehavior);
                    RegisterTick(_group, _order, Tick, value);
                }
                _pausedBehavior = value;
            }
        }

        internal void AnimationHasEnded()
        {
            if (++_endedAnimations >= _totalAnimCount)
                Stop();
        }
        public Dictionary<string, BasePropAnim> GetAllAnimations()
        {
            Dictionary<string, BasePropAnim> anims = new Dictionary<string, BasePropAnim>();
            _root.CollectAnimations("", anims);
            return anims;
        }
        [Browsable(true)]
        public void Start() => Start(_group, _order, _pausedBehavior);
        public void Start(ETickGroup group, ETickOrder order, EInputPauseType pausedBehavior)
        {
            if (_state == AnimationState.Playing)
                return;

            _group = group;
            _order = order;
            _pausedBehavior = pausedBehavior;
            _state = AnimationState.Playing;

            _root?.StartAnimations();
            AnimationStarted?.Invoke(this);
            RegisterTick(_group, _order, Tick, _pausedBehavior);
        }
        [Browsable(true)]
        public void TogglePause() => SetPaused(_state != AnimationState.Paused);
        [Browsable(true)]
        public void SetPaused(bool pause)
        {
            if (_state == AnimationState.Stopped)
            {
                if (!pause)
                {
                    Start(_group, _order, _pausedBehavior);
                }
            }
            else if (pause)
            {
                if (_state == AnimationState.Paused)
                    return;

                _state = AnimationState.Paused;
                UnregisterTick(_group, _order, Tick, _pausedBehavior);
            }
            else
            {
                if (_state != AnimationState.Paused)
                    return;

                _state = AnimationState.Playing;
                RegisterTick(_group, _order, Tick, _pausedBehavior);
            }
        }
        /// <summary>
        /// Stops the animation in its entirety.
        /// </summary>
        [Browsable(true)]
        public void Stop()
        {
            if (_state == AnimationState.Stopped)
                return;

            if (_endedAnimations < _totalAnimCount)
                _root?.StopAnimations();

            _state = AnimationState.Stopped;
            UnregisterTick(_group, _order, Tick, _pausedBehavior);
            AnimationEnded?.Invoke(this);
        }
        protected internal void Tick(float delta)
        {
            foreach (TObject b in Owners)
                _root?._tick(b, delta);
        }
    }
}

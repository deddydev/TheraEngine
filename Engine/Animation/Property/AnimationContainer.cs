using TheraEngine.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using TheraEngine.Input.Devices;
using System.ComponentModel.Design;
using TheraEngine.Core.Reflection.Attributes.Serialization;

namespace TheraEngine.Animation
{
    public class AnimFolder
    {
        public AnimFolder()
        {

        }

        /// <summary>
        /// Constructor to create a subtree without an animation at this level.
        /// </summary>
        /// <param name="propertyName">The name of this property and optionally sub-properties separated by a period.</param>
        /// <param name="children">Any sub-properties this property owns and you want to animate.</param>
        public AnimFolder(string propertyName, params AnimFolder[] children) : this()
        {
            int splitIndex = propertyName.IndexOf('.');
            if (splitIndex >= 0)
            {
                string remainingPath = propertyName.Substring(splitIndex + 1, propertyName.Length - splitIndex - 1);
                _children.Add(new AnimFolder(remainingPath));
                propertyName = propertyName.Substring(0, splitIndex);
            }
            if (children != null)
                _children.AddRange(children);
            _propertyName = propertyName;
            SetAnimation(null, false);
        }
        /// <summary>
        /// Constructor to create a subtree with an animation attached at this level.
        /// </summary>
        /// <param name="propertyOrMethodName">The name of the property or method to animate.</param>
        /// <param name="isMethod"></param>
        /// <param name="animation"></param>
        public AnimFolder(string propertyOrMethodName, bool isMethod, BasePropertyAnimation animation) : this()
        {
            if (!isMethod)
            {
                int splitIndex = propertyOrMethodName.IndexOf('.');
                if (splitIndex >= 0)
                {
                    string remainingPath = propertyOrMethodName.Substring(splitIndex + 1, propertyOrMethodName.Length - splitIndex - 1);
                    _children.Add(new AnimFolder(remainingPath));
                    propertyOrMethodName = propertyOrMethodName.Substring(0, splitIndex);
                }
            }
            _propertyName = propertyOrMethodName;
            SetAnimation(animation, isMethod);
        }

        //Cached at runtime
        private PropertyInfo _propertyCache;
        private MethodInfo _methodCache;
        //TODO: need <object, bool> dictionary because multiple objs might tick in this anim
        private bool _propertyNotFound = false;
        internal Action<object, float> _tick = null;
        
        [TSerialize("IsMethodAnimation")]
        private bool _isMethodAnimation = false;
        [TSerialize("PropertyName")]
        private string _propertyName = null;
        [TSerialize("Animation")]
        private GlobalFileRef<BasePropertyAnimation> _animation = new GlobalFileRef<BasePropertyAnimation>();
        [TSerialize("Children")]
        private MonitoredList<AnimFolder> _children = new MonitoredList<AnimFolder>();

        [Category("Animation Folder")]
        public GlobalFileRef<BasePropertyAnimation> Animation => _animation;

        public void CollectAnimations(string path, Dictionary<string, BasePropertyAnimation> animations)
        {
            if (!string.IsNullOrEmpty(path))
                path += ".";

            path += _propertyName;

            if (Animation != null)
                animations.Add(path, Animation);
            foreach (AnimFolder p in _children)
                p.CollectAnimations(path, animations);
        }
        
        [Category("Animation Folder")]
        public MonitoredList<AnimFolder> Children => _children;
        [Category("Animation Folder")]
        public string PropertyName
        {
            get => _propertyName;
            set => _propertyName = value;
        }
        [Category("Animation Folder")]
        public bool IsMethodAnimation
        {
            get => _isMethodAnimation;
            set => SetAnimationType(value);
        }

        public void SetAnimation(BasePropertyAnimation anim, bool method)
        {
            Animation.File = anim;
            SetAnimationType(method);
        }
        public void SetAnimationType(bool method)
        {
            _isMethodAnimation = method;
            _tick = method ? (Action<object, float>)MethodTick : PropertyTick;
        }

        internal void MethodTick(object obj, float delta)
        {
            bool noObject = obj == null;
            bool noProperty = _propertyNotFound;

            if (noObject || noProperty)
                return;

            if (_methodCache == null)
            {
                Type type = obj.GetType();
                while (type != null)
                {
                    if ((_methodCache = type.GetMethod(_propertyName)) == null)
                        type = type.BaseType;
                    else
                        break;
                }
                if (_propertyNotFound = _methodCache == null)
                    return;
            }

            Animation.File?.Tick(obj, _methodCache, delta);
        }
        internal void PropertyTick(object obj, float delta)
        {
            bool noObject = obj == null;
            bool noProperty = _propertyNotFound;

            if (noObject || noProperty)
                return;

            if (_propertyCache == null)
            {
                Type type = obj.GetType();
                while (type != null)
                {
                    if ((_propertyCache = type.GetProperty(_propertyName)) == null)
                        type = type.BaseType;
                    else
                        break;
                }
                if (_propertyNotFound = _propertyCache == null)
                    return;
            }

            if (Animation != null)
                Animation.File?.Tick(obj, _propertyCache, delta);
            else
            {
                object value = _propertyCache.GetValue(obj);
                foreach (AnimFolder f in _children)
                    f._tick(value, delta);
            }
        }

        internal int Register(AnimationContainer container)
        {
            bool animExists = Animation.File != null;
            int count = (animExists ? 1 : 0);
            if (animExists)
                Animation.File.AnimationEnded += container.AnimationHasEnded;
            foreach (AnimFolder folder in _children)
                count += folder.Register(container);
            return count;
        }
        internal void StartAnimations()
        {
            _propertyNotFound = false;
            _propertyCache = null;

            Animation.File?.Start();
            foreach (AnimFolder folder in _children)
                folder.StartAnimations();
        }
        internal void StopAnimations()
        {
            Animation.File?.Stop();
            foreach (AnimFolder folder in _children)
                folder.StopAnimations();
        }
    }
    [FileClass("TANIM", "Property Animation Tree")]
    public class AnimationContainer : FileObject
    {
        public event Action<AnimationContainer> AnimationStarted;
        public event Action<AnimationContainer> AnimationEnded;

        private int _totalAnimCount = 0;
        private AnimFolder _root;
        internal MonitoredList<TObject> _owners = new MonitoredList<TObject>();

        [TSerialize("EndedAnimations")]
        private int _endedAnimations = 0;
        [TSerialize("State")]
        private AnimationState _state;
        [TSerialize("TickGroup")]
        private ETickGroup _group;
        [TSerialize("TickOrder")]
        private ETickOrder _order;
        [TSerialize("TickPausedBehavior")]
        private InputPauseType _pausedBehavior;

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
            _owners.PostModified += OwnersModified;
        }
        public AnimationContainer(AnimFolder rootFolder) : this()
        {
            RootFolder = rootFolder;
        }
        public AnimationContainer(string animationName, string propertyName, bool method, BasePropertyAnimation anim) : this()
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
            if (_owners.Count == 0 && IsTicking)
                UnregisterTick(_group, _order, Tick, _pausedBehavior);
            else if (_state == AnimationState.Playing && _owners.Count != 0 && !IsTicking)
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
        
        internal void AnimationHasEnded()
        {
            if (++_endedAnimations >= _totalAnimCount)
                Stop();
        }
        public Dictionary<string, BasePropertyAnimation> GetAllAnimations()
        {
            Dictionary<string, BasePropertyAnimation> anims = new Dictionary<string, BasePropertyAnimation>();
            _root.CollectAnimations("", anims);
            return anims;
        }
        [Browsable(true)]
        public void Start() => Start(_group, _order, _pausedBehavior);
        public void Start(ETickGroup group, ETickOrder order, InputPauseType pausedBehavior)
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
                return;

            if (pause)
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
            foreach (TObject b in _owners)
                _root?._tick(b, delta);
        }
    }
}

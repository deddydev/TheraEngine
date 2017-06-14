using TheraEngine.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Xml;
using System.ComponentModel;

namespace TheraEngine.Rendering.Animation
{
    public class AnimFolder
    {
        public AnimFolder(string propertyName, params AnimFolder[] children)
        {
            _propertyName = propertyName;
            _animation = null;
            if (children != null)
                _children = children.ToList();
            _tick = PropertyTick;
        }
        public AnimFolder(string propertyName, bool method, BasePropertyAnimation animation)
        {
            _propertyName = propertyName;
            _animation = animation;
            _tick = method ? (Action<object, float>)MethodTick : PropertyTick;
        }

        private PropertyInfo _propertyCache;
        private MethodInfo _methodCache;

        //TODO: need object, bool dictionary because multiple objs might tick in this anim
        private bool _propertyNotFound = false;

        public string _propertyName;
        private List<AnimFolder> _children = new List<AnimFolder>();
        public BasePropertyAnimation _animation;

        public void CollectAnimations(string path, Dictionary<string, BasePropertyAnimation> animations)
        {
            if (!string.IsNullOrEmpty(path))
                path += ".";

            path += _propertyName;

            if (_animation != null)
                animations.Add(path, _animation);
            foreach (AnimFolder p in _children)
                p.CollectAnimations(path, animations);
        }

        internal Action<object, float> _tick = null;

        public List<AnimFolder> Children => _children;

        public void MethodTick(object obj, float delta)
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

            _animation?.Tick(obj, _methodCache, delta);
        }
        public void PropertyTick(object obj, float delta)
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

            if (_animation != null)
                _animation?.Tick(obj, _propertyCache, delta);
            else
            {
                object value = _propertyCache.GetValue(obj);
                foreach (AnimFolder f in _children)
                    f._tick(value, delta);
            }
        }

        public int Register(AnimationContainer container)
        {
            bool animExists = _animation != null;
            int count = (animExists ? 1 : 0);
            if (animExists)
                _animation.AnimationEnded += container.AnimationHasEnded;
            foreach (AnimFolder folder in _children)
                count += folder.Register(container);
            return count;
        }
        public void StartAnimations()
        {
            _propertyNotFound = false;
            _propertyCache = null;

            _animation?.Start();
            foreach (AnimFolder folder in _children)
                folder.StartAnimations();
        }
        public void StopAnimations()
        {
            _animation?.Stop();
            foreach (AnimFolder folder in _children)
                folder.StopAnimations();
        }
    }
    public class AnimationContainer : FileObject
    {
        public event Action<AnimationContainer> AnimationStarted;
        public event Action<AnimationContainer> AnimationEnded;

        [Serialize("TotalAnimCount")]
        private int _totalAnimCount = 0;
        [Serialize("EndedAnimations")]
        private int _endedAnimations = 0;
        [Serialize("IsPlaying")]
        private bool _isPlaying;
        [Serialize("RootFolder")]
        private AnimFolder _root;
        public MonitoredList<ObjectBase> _owners = new MonitoredList<ObjectBase>();

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
        public AnimationContainer(string propertyName, bool method, BasePropertyAnimation anim) : this()
        {
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
            {
                last._animation = anim;
                last._tick = method ? (Action<object, float>)last.MethodTick : last.PropertyTick;
            }
        }
        private void OwnersModified()
        {
            if (_owners.Count == 0 && IsTicking)
                UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Animation, Tick);
            else if (_owners.Count != 0 && !IsTicking)
                RegisterTick(ETickGroup.PrePhysics, ETickOrder.Animation, Tick);
        }

        public AnimFolder RootFolder
        {
            get { return _root; }
            set
            {
                _root = value;
                _totalAnimCount = _root != null ? _root.Register(this) : 0;
            }
        }
        
        internal void AnimationHasEnded()
        {
            if (++_endedAnimations >= _totalAnimCount)
                Stop(true);
        }
        public Dictionary<string, BasePropertyAnimation> GetAllAnimations()
        {
            Dictionary<string, BasePropertyAnimation> anims = new Dictionary<string, BasePropertyAnimation>();
            _root.CollectAnimations("", anims);
            return anims;
        }
        public void Start()
        {
            if (_isPlaying)
                return;

            _root?.StartAnimations();

            _isPlaying = true;
            AnimationStarted?.Invoke(this);
        }
        public void Stop() => Stop(false);
        private void Stop(bool animationsAllEnded)
        {
            if (!_isPlaying)
                return;

            if (!animationsAllEnded)
                _root?.StopAnimations();

            _isPlaying = false;
            AnimationEnded?.Invoke(this);
        }
        protected internal void Tick(float delta)
        {
            foreach (ObjectBase b in _owners)
                Tick(delta, b);
        }
        internal void Tick(float delta, ObjectBase obj)
        {
            if (_isPlaying)
                _root?._tick(obj, delta);
        }
    }
}

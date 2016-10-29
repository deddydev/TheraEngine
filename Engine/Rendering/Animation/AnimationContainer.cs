using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CustomEngine.Rendering.Animation
{
    public class AnimFolder
    {
        public AnimFolder(string propertyName, BasePropertyAnimation animation, params AnimFolder[] children)
        {
            _propertyName = propertyName;
            _animation = animation;
            if (children != null)
                _children = children.ToList();
        }

        private PropertyInfo _propertyCache;
        private bool _propertyNotFound;

        public string _propertyName;
        public List<AnimFolder> _children = new List<AnimFolder>();
        public BasePropertyAnimation _animation;

        public void CollectAnimations(string path, Dictionary<string, BasePropertyAnimation> animations)
        {
            if (!String.IsNullOrEmpty(path))
                path += ".";

            path += _propertyName;

            if (_animation != null)
                animations.Add(path, _animation);
            foreach (AnimFolder p in _children)
                p.CollectAnimations(path, animations);
        }

        public void Tick(object obj, float delta)
        {
            bool noObject = obj == null;
            bool noProperty = _propertyNotFound;

            if (noObject || noProperty)
                return;

            if (_propertyCache == null && (_propertyCache = obj.GetType().GetProperty(_propertyName)) == null)
            {
                _propertyNotFound = true;
                return;
            }

            _animation?.Tick(obj, _propertyCache, delta);

            object value = _propertyCache.GetValue(obj);
            foreach (AnimFolder f in _children)
                f.Tick(value, delta);
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
        public event EventHandler AnimationStarted;
        public event EventHandler AnimationEnded;

        private int _totalAnimCount = 0;
        private int _endedAnimations = 0;
        private bool _isPlaying;
        private AnimFolder _root;

        public AnimationContainer() { }
        public AnimationContainer(AnimFolder rootFolder)
        {
            RootFolder = rootFolder;
        }
        public AnimationContainer(string propertyName, BasePropertyAnimation anim)
        {
            string[] parts = propertyName.Split('.');
            bool first = true;
            AnimFolder last = null;
            foreach (string i in parts)
            {
                if (first)
                {
                    last = RootFolder = new AnimFolder(i, null);
                    first = false;
                }
                else
                    last._children.Add(new AnimFolder(i, null));
            }
            if (last != null)
                last._animation = anim;
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
        internal void AnimationHasEnded(object sender, EventArgs e)
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
        private void Start()
        {
            if (!_isPlaying)
                return;

            _root?.StartAnimations();

            _isPlaying = true;
            AnimationStarted?.Invoke(this, EventArgs.Empty);
        }
        private void Stop(bool animationsAllEnded = false)
        {
            if (!_isPlaying)
                return;

            if (!animationsAllEnded)
                _root?.StopAnimations();

            _isPlaying = false;
            AnimationEnded?.Invoke(this, EventArgs.Empty);
        }
        internal void Tick(float delta, ObjectBase obj)
        {
            if (_isPlaying)
                _root?.Tick(obj, delta);
        }
    }
}

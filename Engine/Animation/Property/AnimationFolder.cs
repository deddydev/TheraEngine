using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Files;

namespace TheraEngine.Animation.Property
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
        public AnimFolder(string propertyOrMethodName, bool isMethod, BasePropAnim animation) : this()
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
        private GlobalFileRef<BasePropAnim> _animation = new GlobalFileRef<BasePropAnim>();
        [TSerialize("Children")]
        private EventList<AnimFolder> _children = new EventList<AnimFolder>();

        [Category("Animation Folder")]
        public GlobalFileRef<BasePropAnim> Animation => _animation;

        public void CollectAnimations(string path, Dictionary<string, BasePropAnim> animations)
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
        public EventList<AnimFolder> Children => _children;
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

        public void SetAnimation(BasePropAnim anim, bool method)
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
}

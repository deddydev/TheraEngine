using TheraEngine.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using TheraEngine.Input.Devices;
using System.ComponentModel.Design;

namespace TheraEngine.Animation
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
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

            _animation?.Tick(obj, _methodCache, delta);
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
    public enum AnimationState
    {
        /// <summary>
        /// Stopped means that the animation is not playing and is set to its initial start position.
        /// </summary>
        Stopped,
        /// <summary>
        /// Paused means that the animation is not currently playing
        /// but is at some arbitrary point in the animation, ready to start up at that point again.
        /// </summary>
        Paused,
        /// <summary>
        /// Playing means that the animation is currently progressing forward.
        /// </summary>
        Playing,
    }
    [FileClass("TANIM", "Property Animation Tree")]
    public class AnimationContainer : FileObject, IComponent
    {
        public event Action<AnimationContainer> AnimationStarted;
        public event Action<AnimationContainer> AnimationEnded;
        public event EventHandler Disposed;

        private int _totalAnimCount = 0;
        private AnimFolder _root;

        [Serialize("EndedAnimations")]
        private int _endedAnimations = 0;
        [Serialize("State")]
        private AnimationState _state;
        [Serialize("TickGroup")]
        private ETickGroup _group;
        [Serialize("TickOrder")]
        private ETickOrder _order;
        [Serialize("TickPausedBehavior")]
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
            {
                last._animation = anim;
                last._tick = method ? (Action<object, float>)last.MethodTick : last.PropertyTick;
            }
        }
        private void OwnersModified()
        {
            if (_owners.Count == 0 && IsTicking)
                UnregisterTick(_group, _order, Tick, _pausedBehavior);
            else if (_state == AnimationState.Playing && _owners.Count != 0 && !IsTicking)
                RegisterTick(_group, _order, Tick, _pausedBehavior);
        }

        [Serialize]
        public AnimFolder RootFolder
        {
            get => _root;
            set
            {
                _root = value;
                _totalAnimCount = _root != null ? _root.Register(this) : 0;
            }
        }

        [Browsable(false)]
        public ISite Site { get => new DesignerVerbSite(this); set => throw new NotImplementedException(); }

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
            foreach (ObjectBase b in _owners)
                _root?._tick(b, delta);
        }

        public void Dispose()
        {

        }

        public class DesignerVerbSite : IMenuCommandService, ISite
        {
            // our target object
            protected object _Component;

            public DesignerVerbSite(object component)
            {
                _Component = component;
            }

            #region IMenuCommandService Members
            // IMenuCommandService provides DesignerVerbs, seen as commands in the PropertyGrid control

            public void AddCommand(MenuCommand command)
            {
                throw new NotImplementedException();
            }

            public void AddVerb(DesignerVerb verb)
            {
                throw new NotImplementedException();
            }

            public MenuCommand FindCommand(CommandID commandID)
            {
                throw new NotImplementedException();
            }

            public bool GlobalInvoke(CommandID commandID)
            {
                throw new NotImplementedException();
            }

            public void RemoveCommand(MenuCommand command)
            {
                throw new NotImplementedException();
            }

            public void RemoveVerb(DesignerVerb verb)
            {
                throw new NotImplementedException();
            }

            public void ShowContextMenu(CommandID menuID, int x, int y)
            {
                throw new NotImplementedException();
            }

            // ** Item of interest ** Return the DesignerVerbs collection
            public DesignerVerbCollection Verbs
            {
                get
                {
                    DesignerVerbCollection Verbs = new DesignerVerbCollection();
                    // Use reflection to enumerate all the public methods on the object
                    MethodInfo[] mia = _Component.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
                    foreach (MethodInfo mi in mia)
                    {
                        // Ignore any methods without a [Browsable(true)] attribute
                        object[] attrs = mi.GetCustomAttributes(typeof(BrowsableAttribute), true);
                        if (attrs == null || attrs.Length == 0)
                            continue;
                        if (!((BrowsableAttribute)attrs[0]).Browsable)
                            continue;
                        // Add a DesignerVerb with our VerbEventHandler
                        // The method name will appear in the command pane
                        Verbs.Add(new DesignerVerb(mi.Name, new EventHandler(VerbEventHandler)));
                    }
                    return Verbs;
                }
            }

            // ** Item of interest ** Handle invokaction of the DesignerVerbs
            private void VerbEventHandler(object sender, EventArgs e)
            {
                // The verb is the sender
                DesignerVerb verb = sender as DesignerVerb;
                // Enumerate the methods again to find the one named by the verb
                MethodInfo[] mia = _Component.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach (MethodInfo mi in mia)
                {
                    object[] attrs = mi.GetCustomAttributes(typeof(BrowsableAttribute), true);
                    if (attrs == null || attrs.Length == 0)
                        continue;
                    if (!((BrowsableAttribute)attrs[0]).Browsable)
                        continue;
                    if (verb.Text == mi.Name)
                    {
                        // Invoke the method on our object (no parameters)
                        mi.Invoke(_Component, null);
                        return;
                    }
                }
            }

            #endregion

            #region ISite Members
            // ISite required to represent this object directly to the PropertyGrid

            public IComponent Component
            {
                get { throw new NotImplementedException(); }
            }

            // ** Item of interest ** Implement the Container property
            public IContainer Container
            {
                // Returning a null Container works fine in this context
                get { return null; }
            }

            // ** Item of interest ** Implement the DesignMode property
            public bool DesignMode
            {
                // While this *is* called, it doesn't seem to matter whether we return true or false
                get { return true; }
            }

            public string Name
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            #endregion

            #region IServiceProvider Members
            // IServiceProvider is the mechanism used by the PropertyGrid to discover our IMenuCommandService support

            // ** Item of interest ** Respond to requests for IMenuCommandService
            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(IMenuCommandService))
                    return this;
                return null;
            }

            #endregion
        }
    }
}

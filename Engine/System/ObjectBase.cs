using CustomEngine;
using CustomEngine.Rendering;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using CustomEngine.Rendering.Animation;

namespace System
{
    public delegate void ResourceEventHandler(ObjectBase node);
    public delegate void RenamedEventHandler(ObjectBase node, string oldName);
    public delegate void ObjectPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);
    public enum ETickGroup
    {
        PrePhysics = 0,
        PostPhysics = 1,
        DuringPhysics = 2,
    }
    public enum ETickOrder
    {
        Timers = 0, //Call timing events
        Input = 1, //Call input events
        Logic = 2, //Call update tick
        Scene = 3, //Update scene
    }
    public class ObjectBase// : INotifyPropertyChanged
    {
        //public static List<ObjectBase> _changedObjects = new List<ObjectBase>();

        public event PropertyChangedEventHandler PropertyChanged;
        public event RenamedEventHandler Renamed;
        public event ResourceEventHandler Disposing, UpdateProperties, UpdateEditor;

        protected string _name;
        protected bool _changed;
        private ETickGroup? _tickGroup = null;
        private ETickOrder? _tickOrder = null;
        protected bool _isTicking = false;

        //[Browsable(false)]
        //public virtual ResourceType ResourceType { get { return ResourceType.Object; } }

        [Default]
#if EDITOR
        [Category("State")]
#endif
        public string Name
        {
            get { return _name; }
#if EDITOR
            set
            {
                string oldName = _name;
                _name = value;
                OnRenamed(oldName);
            }
#endif
        }
        
        //[Category("Tick"), PreChanged("UnregisterTick"), PostChanged("RegisterTick")]
        public ETickGroup? TickGroup
        {
            get { return _tickGroup; }
            set { _tickGroup = value; }
        }
        //[Category("Tick"), PreChanged("UnregisterTick"), PostChanged("RegisterTick")]
        public ETickOrder? TickOrder
        {
            get { return _tickOrder; }
            set { _tickOrder = value; }
        }

#if EDITOR
        /// <summary>
        /// 
        /// </summary>
        [Category("State"), State, EditorOnly]
        public bool HasChanged
        {
            get { return _changed; }
            set { _changed = value; }
        }
        /// <summary>
        /// If this class needs to be rebuilt.
        /// </summary>
        [Category("State"), State, EditorOnly]
        public virtual bool IsDirty
        {
            get { return HasChanged; }
            set { HasChanged = value; }
        }
#endif
        /// <summary>
        /// Specifies that this object wants tick calls.
        /// </summary>
        public void RegisterTick(ETickGroup group, ETickOrder order)
        {
            if (_isTicking)
                return;
            _isTicking = true;
            Engine.RegisterTick(this, group, order);
        }
        /// <summary>
        /// Specifies that this object will not have any tick calls.
        /// </summary>
        public void UnregisterTick()
        {
            if (!_isTicking)
                return;
            _isTicking = false;
            Engine.UnregisterTick(this);
        }
        /// <summary>
        /// Updates logic for this class
        /// </summary>
        /// <param name="delta">The amount of time that has passed since the last tick update</param>
        internal virtual void Tick(float delta) { }

        public void OnPropertyChanged(PropertyInfo info, object previousValue)
        {
            if (info.Name == "_changed")
                return;

            string output = "Changed property " + info.Name + " in " + GetType().ToString() + " \"" + Name + "\"";
            Console.WriteLine(output);

            _changed = true;
            //_changedObjects.Add(this);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info.Name));
        }
        protected virtual void OnUpdateProperties() { UpdateProperties?.Invoke(this); }
        protected virtual void OnUpdateEditor() { UpdateEditor?.Invoke(this); }
        protected virtual void OnDisposing() { Disposing?.Invoke(this); }
        protected virtual void OnRenamed(string oldName) { Renamed?.Invoke(this, oldName); }

        private List<AnimationContainer> _animations = new List<AnimationContainer>();
        public void AddAnimation(AnimationContainer anim, bool startNow = false)
        {
            anim.AnimationEnded += RemoveAnimation;
            _animations.Add(anim);
            anim._owners.Add(this);
            if (startNow)
                anim.Start();
        }
        public void RemoveAnimation(AnimationContainer anim)
        {
            _animations.Remove(anim);
            anim._owners.Remove(this);
        }
        public override string ToString()
        {
            return _name;
        }
    }
//    [PSerializable]
//    public class NotifyPropertyChangedAttribute : LocationInterceptionAspect
//    {
//        public override void OnSetValue(LocationInterceptionArgs args)
//        {
//            if (args.Value != args.GetCurrentValue())
//            {
//                PropertyInfo info = args.Location.PropertyInfo;
//                if (info == null)
//                    return;

//                object currentValue = args.GetCurrentValue();

//                Default def = info.GetCustomAttribute<Default>();
//                if (def != null)
//                {
//#if EDITOR
//                    return;
//#endif
//                }

//                PreChanged pre = info.GetCustomAttribute<PreChanged>();
//                if (pre != null)
//                {
//                    MethodInfo method = GetType().GetMethod(pre._methodName).MakeGenericMethod(info.PropertyType);
//                    if (method != null)
//                        method.Invoke(this, new object[] { currentValue });
//                }

//                args.Value = args.Value;
//                args.ProceedSetValue();

//                PostChanged post = info.GetCustomAttribute<PostChanged>();
//                if (post != null)
//                {
//                    MethodInfo method = GetType().GetMethod(post._methodName).MakeGenericMethod(info.PropertyType);
//                    if (method != null)
//                        method.Invoke(this, new object[] { args.Value });
//                    else
//                    {
//                        method = GetType().GetMethod(post._methodName).MakeGenericMethod(info.PropertyType);
//                        if (method != null)
//                            method.Invoke(this, new object[] { args.Value });
//                    }
//                }

//                PostCall call = info.GetCustomAttribute<PostCall>();
//                if (post != null)
//                {
//                    MethodInfo method = GetType().GetMethod(post._methodName).MakeGenericMethod(info.PropertyType);
//                    if (method != null)
//                        method.Invoke(this, new object[] { args.Value });
//                }

//                ((ObjectBase)args.Instance).OnPropertyChanged(args.Location.PropertyInfo, currentValue);
//            }
//        }
//    }
}

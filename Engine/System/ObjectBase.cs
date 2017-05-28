using CustomEngine;
using CustomEngine.Rendering;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using CustomEngine.Rendering.Animation;
using System.Linq;

namespace System
{
    public class TickInfo : Tuple<ETickGroup, ETickOrder, DelTick>
    {
        public TickInfo(ETickGroup group, ETickOrder order, DelTick function)
            : base(group, order, function) { }
    }
    public delegate void DelTick(float delta);
    public delegate void ResourceEventHandler(ObjectBase node);
    public delegate void RenamedEventHandler(ObjectBase node, string oldName);
    public delegate void ObjectPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);
    public enum ETickGroup
    {
        PrePhysics      = 0,
        DuringPhysics   = 5,
        PostPhysics     = 10,
    }
    public enum ETickOrder
    {
        Timers      = 0, //Call timing events
        Input       = 1, //Call input events
        Animation   = 2, //Update animation positions
        Logic       = 3, //Gameplay calculations
        Scene       = 4, //Update scene
    }
    public class ObjectBase
    {
        [Browsable(false)]
        public event PropertyChangedEventHandler PropertyChanged;
        [Browsable(false)]
        public event RenamedEventHandler Renamed;
        [Browsable(false)]
        public event ResourceEventHandler Disposing, UpdateProperties, UpdateEditor;

        [Serialize("Name", IsXmlAttribute = true)]
        protected string _name;
        [Serialize("UserData")]
        private object _userData;

        [Browsable(false)]
        public bool IsTicking => _tickFunctions.Count > 0;

        public void RegisterTick(ETickGroup group, ETickOrder order, DelTick tickFunc)
        {
            _tickFunctions.Add(new TickInfo(group, order, tickFunc));
            Engine.RegisterTick(group, order, tickFunc);
        }
        public void UnregisterTick(ETickGroup group, ETickOrder order, DelTick tickFunc)
        {
            int info = _tickFunctions.FindIndex(x => x.Item1 == group && x.Item2 == order && x.Item3 == tickFunc);
            if (info != null)
            Engine.UnregisterTick(group, order, tickFunc);
        }

        protected bool _changed;
        private ThreadSafeList<TickInfo> _tickFunctions = new ThreadSafeList<TickInfo>();
        //protected bool _isTicking = false;

        [Serialize("Animations")]
        private List<AnimationContainer> _animations;

        [Browsable(false)]
        public object UserData
        {
            get => _userData;
            set => _userData = value;
        }

        //public ObjectHeader ClassHeader 
        //    => (ObjectHeader)Attribute.GetCustomAttribute(GetType(), typeof(ObjectHeader));
        
        public string Name
        {
            get { return _name; }
            set
            {
                string oldName = _name;
                _name = value;
                OnRenamed(oldName);
            }
        }

        //[Browsable(false)]
        ////[Category("Tick"), PreChanged("UnregisterTick"), PostChanged("RegisterTick")]
        //public ETickGroup? TickGroup
        //{
        //    get { return _tickGroup; }
        //    set { _tickGroup = value; }
        //}
        //[Browsable(false)]
        ////[Category("Tick"), PreChanged("UnregisterTick"), PostChanged("RegisterTick")]
        //public ETickOrder? TickOrder
        //{
        //    get { return _tickOrder; }
        //    set { _tickOrder = value; }
        //}

        /// <summary>
        /// Specifies that this object wants tick calls.
        /// </summary>
        //public void RegisterTick(ETickGroup group, ETickOrder order)
        //{
        //    Engine.RegisterTick(this, group, order);
        //}

        /// <summary>
        /// Specifies that this object will not have any tick calls.
        /// </summary>
        //public void UnregisterTick()
        //{
        //    Engine.UnregisterTick(this);
        //}

        /// <summary>
        /// Updates logic for this class
        /// </summary>
        /// <param name="delta">The amount of time that has passed since the last tick update</param>
        //protected internal virtual void Tick(float delta) { }

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

        public void AddAnimation(AnimationContainer anim, bool startNow = false)
        {
            anim.AnimationEnded += RemoveAnimation;
            if (_animations == null)
                _animations = new List<AnimationContainer>();
            _animations.Add(anim);
            anim._owners.Add(this);
            if (startNow)
                anim.Start();
        }
        public void RemoveAnimation(AnimationContainer anim)
        {
            if (_animations == null)
                return;
            _animations.Remove(anim);
            if (_animations.Count == 0)
                _animations = null;
            anim._owners.Remove(this);
        }
        public override string ToString() => _name;
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

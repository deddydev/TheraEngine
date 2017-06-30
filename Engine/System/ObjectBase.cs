using TheraEngine;
using TheraEngine.Rendering;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using TheraEngine.Rendering.Animation;
using System.Linq;
using System;
using TheraEngine.Input.Devices;
using System.Diagnostics;

namespace TheraEngine
{
    public interface IObjectBase
    {
        string Name { get; set; }
        bool IsTicking { get; }
        object UserData { get; set; }
#if EDITOR
        EditorState EditorState { get; set; }
#endif
        void RegisterTick(ETickGroup group, ETickOrder order, DelTick tickFunc, InputPauseType pausedBehavior = InputPauseType.TickAlways);
        void UnregisterTick(ETickGroup group, ETickOrder order, DelTick tickFunc, InputPauseType pausedBehavior = InputPauseType.TickAlways);
        void AddAnimation(AnimationContainer anim, bool startNow = false);
        void RemoveAnimation(AnimationContainer anim);
    }
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
        DuringPhysics   = 15,
        PostPhysics     = 30,
    }
    public enum ETickOrder
    {
        Timers      = 0, //Call timing events
        Input       = 3, //Call input events
        Animation   = 6, //Update animation positions
        Logic       = 9, //Gameplay calculations
        Scene       = 12, //Update scene
    }

    public abstract class ObjectBase
    {
        public static event Action<ObjectBase> OnConstructed;

        public ObjectBase()
        {
            OnConstructed?.Invoke(this);
        }

        [Browsable(false)]
        public event PropertyChangedEventHandler PropertyChanged;
        [Browsable(false)]
        public event RenamedEventHandler Renamed;

        [Serialize("Name", IsXmlAttribute = true)]
        protected string _name;
        private object _userData;
#if EDITOR
        private EditorState _editorState = new EditorState();
#endif
        protected bool _changed;
        private ThreadSafeList<TickInfo> _tickFunctions = new ThreadSafeList<TickInfo>();
        [Serialize("Animations")]
        private List<AnimationContainer> _animations;

        [Browsable(false)]
        public bool IsTicking => _tickFunctions.Count > 0;

        public void RegisterTick(ETickGroup group, ETickOrder order, DelTick tickFunc, InputPauseType pausedBehavior = InputPauseType.TickAlways)
        {
            _tickFunctions.Add(new TickInfo(group, order, tickFunc));
            Engine.RegisterTick(group, order, tickFunc, pausedBehavior);
        }
        public void UnregisterTick(ETickGroup group, ETickOrder order, DelTick tickFunc, InputPauseType pausedBehavior = InputPauseType.TickAlways)
        {
            int index = _tickFunctions.FindIndex(x => x.Item1 == group && x.Item2 == order && x.Item3 == tickFunc);
            if (index >= 0)
                _tickFunctions.RemoveAt(index);
            Engine.UnregisterTick(group, order, tickFunc, pausedBehavior);
        }


#if EDITOR
        [Serialize]
        [Browsable(false)]
        public EditorState EditorState
        {
            get => _editorState;
            set => _editorState = value;
        }
#endif

        [Serialize]
        [Browsable(false)]
        public object UserData
        {
            get => _userData;
            set => _userData = value;
        }
        
        public string Name
        {
            get => string.IsNullOrEmpty(_name) ? GetType().Name : _name;
            set
            {
                string oldName = _name;
                _name = value;
                OnRenamed(oldName);
            }
        }
        
        protected internal void OnPropertyChanged(PropertyInfo info, object previousValue)
        {
            if (info.Name == "_changed")
                return;

            string output = "Changed property " + info.Name + " in " + GetType().ToString() + " \"" + Name + "\"";
            Debug.WriteLine(output);

            _changed = true;

#if EDITOR
            if (_editorState != null)
                _editorState.ChangedProperties.Add(info);
#endif

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info.Name));
        }
        
        protected virtual void OnRenamed(string oldName) { Renamed?.Invoke(this, oldName); }

        public void AddAnimation(AnimationContainer anim, bool startNow = false)
        {
            if (anim == null)
                return;
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
            if (_animations == null || anim == null)
                return;
            _animations.Remove(anim);
            if (_animations.Count == 0)
                _animations = null;
            anim._owners.Remove(this);
        }
        public override string ToString() => Name;
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

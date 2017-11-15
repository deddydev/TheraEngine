using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Animation;
using System;
using TheraEngine.Input.Devices;
using System.Runtime.CompilerServices;

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
        void AddAnimation(AnimationContainer anim, bool startNow = false, ETickGroup group = ETickGroup.PostPhysics, ETickOrder order = ETickOrder.BoneAnimation, InputPauseType pausedBehavior = InputPauseType.TickAlways);
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
        Timers          = 0, //Call timing events
        Input           = 3, //Call input events
        BoneAnimation   = 6, //Update model animation positions
        Logic           = 9, //Gameplay calculations
        Scene           = 12, //Update scene
    }

    public abstract class ObjectBase : INotifyPropertyChanged
    {
        public static event Action<ObjectBase> OnConstructed;

        public ObjectBase()
        {
            OnConstructed?.Invoke(this);
        }
        
        [TSerialize("Name", XmlNodeType = EXmlNodeType.Attribute)]
        protected string _name = null;
        [TSerialize("UserData")]
        private object _userData = null;
        
        [TSerialize]
        [Browsable(false)]
        [Category("Object")]
        public object UserData
        {
            get => _userData;
            set => _userData = value;
        }

        #region Name
        [Category("Object")]
        public string Name
        {
            get => string.IsNullOrEmpty(_name) ? GetType().GetFriendlyName().Replace("<", "[").Replace(">", "]") : _name;
            set
            {
                string oldName = _name;
                _name = value;
                OnRenamed(oldName);
            }
        }
        public event RenamedEventHandler Renamed;
        protected virtual void OnRenamed(string oldName)
            => Renamed?.Invoke(this, oldName);
        #endregion

        #region Ticking
        private ThreadSafeList<TickInfo> _tickFunctions = new ThreadSafeList<TickInfo>();
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
        #endregion

#if EDITOR

        private EditorState _editorState = null;

        [TSerialize]
        [Browsable(false)]
        [Category("Object")]
        public EditorState EditorState
        {
            get => _editorState ?? (_editorState = new EditorState());
            set => _editorState = value;
        }

        [Browsable(false)]
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
            => PropertyChanged?.Invoke(this, e);
        protected void SetPropertyField<T>(ref T field, T newValue, [CallerMemberName] string propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                Engine.PrintLine("Changed property {0} in {1} \"{2}\"", propertyName, GetType().GetFriendlyName(), ToString());

                EditorState.AddChange(propertyName, field, newValue);

                field = newValue;
                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }
        }
#endif

        #region Animation

        private List<AnimationContainer> _animations = null;

        [TSerialize]
        [Browsable(false)]
        [Category("Object")]
        public List<AnimationContainer> Animations
        {
            get => _animations;
            set => _animations = value;
        }

        public void AddAnimation(
            AnimationContainer anim,
            bool startNow = false,
            ETickGroup group = ETickGroup.PostPhysics,
            ETickOrder order = ETickOrder.BoneAnimation,
            InputPauseType pausedBehavior = InputPauseType.TickOnlyWhenUnpaused)
        {
            if (anim == null)
                return;
            anim.AnimationEnded += RemoveAnimation;
            if (_animations == null)
                _animations = new List<AnimationContainer>();
            _animations.Add(anim);
            anim._owners.Add(this);
            if (startNow)
                anim.Start(group, order, pausedBehavior);
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
        #endregion

        public override string ToString() => Name;
    }
}

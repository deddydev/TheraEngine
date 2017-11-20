using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Animation;
using System;
using TheraEngine.Input.Devices;
using System.Runtime.CompilerServices;
using TheraEngine.Core.Reflection.Attributes;
using TheraEngine.Core.Reflection.Attributes.Serialization;

namespace TheraEngine
{
    public interface IObjectBase
    {
        string Name { get; set; }
        bool IsTicking { get; }
        object UserData { get; set; }
        MonitoredList<AnimationContainer> Animations { get; }
#if EDITOR
        EditorState EditorState { get; set; }
#endif
        void RegisterTick(
            ETickGroup group,
            ETickOrder order,
            DelTick tickFunc,
            InputPauseType pausedBehavior = InputPauseType.TickAlways);
        void UnregisterTick(
            ETickGroup group,
            ETickOrder order,
            DelTick tickFunc,
            InputPauseType pausedBehavior = InputPauseType.TickAlways);
        void AddAnimation(
            AnimationContainer anim,
            bool startNow = false,
            bool removeOnEnd = true,
            ETickGroup group = ETickGroup.PostPhysics,
            ETickOrder order = ETickOrder.BoneAnimation,
            InputPauseType pausedBehavior = InputPauseType.TickAlways);
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

    public abstract class ObjectBase// : INotifyPropertyChanged
    {
        public static event Action<ObjectBase> OnConstructed;

        public ObjectBase()
        {
            OnConstructed?.Invoke(this);
        }
        
        [TSerialize("Name", XmlNodeType = EXmlNodeType.Attribute)]
        protected string _name = null;
        private object _userData = null;
        
        [TSerialize]
        [BrowsableIf("_userData != null")]
        [Category("Object")]
        public object UserData
        {
            get => _userData;
            set => _userData = value;
        }

        #region Name
        [Category("Object")]
        public virtual string Name
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

        [TSerialize("EditorState")]
        private EditorState _editorState = null;

        [Browsable(false)]
        public EditorState EditorState
        {
            get => _editorState ?? (_editorState = new EditorState());
            set => _editorState = value;
        }

        //[Browsable(false)]
        //public event PropertyChangedEventHandler PropertyChanged;
        //protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        //    => PropertyChanged?.Invoke(this, e);
        //protected void SetPropertyField<T>(ref T field, T newValue, [CallerMemberName] string propertyName = "")
        //{
        //    if (!EqualityComparer<T>.Default.Equals(field, newValue))
        //    {
        //        Engine.PrintLine("Changed property {0} in {1} \"{2}\"", propertyName, GetType().GetFriendlyName(), ToString());

        //        EditorState.AddChange(propertyName, field, newValue);

        //        field = newValue;
        //        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        //    }
        //}
#endif

        #region Animation

        [TSerialize]
        private MonitoredList<AnimationContainer> _animations = null;

        [PostDeserialize]
        private void PostDeserialize()
        {
            if (_animations != null)
            {
                _animations.PostAdded += _animations_PostAdded;
                _animations.PostInserted += _animations_PostInserted;
                _animations.PostAddedRange += _animations_PostAddedRange;
                _animations.PostInsertedRange += _animations_PostInsertedRange;
                _animations.PostRemoved += _animations_PostRemoved;
                _animations.PostRemovedRange += _animations_PostRemovedRange;
            }
        }

        [Category("Object")]
        public MonitoredList<AnimationContainer> Animations
        {
            get
            {
                if (_animations == null)
                {
                    _animations = new MonitoredList<AnimationContainer>();
                    _animations.PostAdded += _animations_PostAdded;
                    _animations.PostInserted += _animations_PostInserted;
                    _animations.PostAddedRange += _animations_PostAddedRange;
                    _animations.PostInsertedRange += _animations_PostInsertedRange;
                    _animations.PostRemoved += _animations_PostRemoved;
                    _animations.PostRemovedRange += _animations_PostRemovedRange;
                }
                return _animations;
            }
        }

        private void _animations_PostRemovedRange(IEnumerable<AnimationContainer> items)
        {
            foreach (AnimationContainer item in items)
                _animations_PostRemoved(item);
        }

        private void _animations_PostRemoved(AnimationContainer item)
        {
            if (_animations.Count == 0)
            {
                _animations.PostAdded -= _animations_PostAdded;
                _animations.PostInserted -= _animations_PostInserted;
                _animations.PostAddedRange -= _animations_PostAddedRange;
                _animations.PostInsertedRange -= _animations_PostInsertedRange;
                _animations.PostRemoved -= _animations_PostRemoved;
                _animations.PostRemovedRange -= _animations_PostRemovedRange;
                _animations = null;
            }
            item._owners.Remove(this);
        }

        private void _animations_PostInsertedRange(IEnumerable<AnimationContainer> items, int index)
        {
            foreach (AnimationContainer item in items)
                _animations_PostAdded(item);
        }

        private void _animations_PostAddedRange(IEnumerable<AnimationContainer> items)
        {
            foreach (AnimationContainer item in items)
                _animations_PostAdded(item);
        }

        private void _animations_PostInserted(AnimationContainer item, int index)
        {
            _animations_PostAdded(item);
        }

        private void _animations_PostAdded(AnimationContainer item)
        {
            item._owners.Add(this);
        }

        public void AddAnimation(
            AnimationContainer anim,
            bool startNow = false,
            bool removeOnEnd = true,
            ETickGroup group = ETickGroup.PostPhysics,
            ETickOrder order = ETickOrder.BoneAnimation,
            InputPauseType pausedBehavior = InputPauseType.TickOnlyWhenUnpaused)
        {
            if (anim == null)
                return;
            if (removeOnEnd)
                anim.AnimationEnded += RemoveAnimationSelf;
            Animations.Add(anim);
            if (startNow)
                anim.Start(group, order, pausedBehavior);
        }
        private void RemoveAnimationSelf(AnimationContainer anim)
        {
            anim.AnimationEnded -= RemoveAnimationSelf;
            Animations.Remove(anim);
        }
        #endregion

        public override string ToString() => Name;
    }
}

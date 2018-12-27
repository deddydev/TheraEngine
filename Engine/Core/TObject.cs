using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Animation;
using TheraEngine.Core.Reflection.Attributes;
using TheraEngine.Editor;
using TheraEngine.Input.Devices;
using TheraEngine.Scripting;

namespace TheraEngine
{
    public interface IObject
    {
        string Name { get; set; }
        object UserObject { get; set; }

#if EDITOR
        EditorState EditorState { get; set; }
#endif

        #region Ticking
        bool IsTicking { get; }
        void RegisterTick(
            ETickGroup group,
            ETickOrder order,
            DelTick tickFunc,
            EInputPauseType pausedBehavior = EInputPauseType.TickAlways);
        void UnregisterTick(
            ETickGroup group,
            ETickOrder order,
            DelTick tickFunc,
            EInputPauseType pausedBehavior = EInputPauseType.TickAlways);
        #endregion
        
        #region Animation
        IReadOnlyList<AnimationTree> Animations { get; }
        void AddAnimation(
            AnimationTree anim,
            bool startNow = false,
            bool removeOnEnd = true,
            ETickGroup group = ETickGroup.PostPhysics,
            ETickOrder order = ETickOrder.Animation,
            EInputPauseType pausedBehavior = EInputPauseType.TickAlways);
        bool RemoveAnimation(AnimationTree anim);
        #endregion
    }

    public delegate void ResourceEventHandler(TObject node);
    public delegate void RenamedEventHandler(TObject node, string oldName);
    public delegate void ObjectPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);
    public abstract class TObject : IObject
    {
        /// <summary>
        /// Event called any time a new object is created.
        /// </summary>
        public static event Action<TObject> OnConstructed;

        public TObject()
        {
            OnConstructed?.Invoke(this);
        }
        
        [TString(false, false, false)]
        [TSerialize(nameof(Name), NodeType = ENodeType.Attribute)]
        protected string _name = null;
        [Browsable(false)]
        public Guid Guid { get; } = Guid.NewGuid();
        
        [TSerialize]
        //[BrowsableIf("_userData != null")]
        [Browsable(false)]
        [Category("Object")]
        public object UserObject { get; set; } = null;

        #region Name
        [Browsable(false)]
        [TString(false, false, false)]
        [Category("Object")]
        public virtual string Name
        {
            get => _name ?? GetType().GetFriendlyName("[", "]");
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
        private ThreadSafeList<(ETickGroup Group, ETickOrder Order, DelTick Tick)> _tickFunctions
            = new ThreadSafeList<(ETickGroup Group, ETickOrder Order, DelTick Tick)>();
        [Browsable(false)]
        public bool IsTicking => _tickFunctions.Count > 0;
        /// <summary>
        /// Registers a method to be called on every update tick.
        /// </summary>
        /// <param name="group">The group to execute before, during, or after physics simulation.</param>
        /// <param name="order">Specifies when to run the method within the group.</param>
        /// <param name="tickFunc">The method to call.</param>
        /// <param name="pausedBehavior">Whether to tick when paused or not.</param>
        public void RegisterTick(ETickGroup group, ETickOrder order, DelTick tickFunc, EInputPauseType pausedBehavior = EInputPauseType.TickAlways)
        {
            if (_tickFunctions.FindIndex(t => t.Group == group && t.Order == order && t.Tick == tickFunc) >= 0)
                return;

            _tickFunctions.Add((group, order, tickFunc));
            Engine.RegisterTick(group, order, tickFunc, pausedBehavior);
        }
        public void UnregisterTick(ETickGroup group, ETickOrder order, DelTick tickFunc, EInputPauseType pausedBehavior = EInputPauseType.TickAlways)
        {
            int index = _tickFunctions.FindIndex(x => x.Group == group && x.Order == order && x.Tick == tickFunc);
            if (index >= 0)
                _tickFunctions.RemoveAt(index);
            Engine.UnregisterTick(group, order, tickFunc, pausedBehavior);
        }
        #endregion

        public static void RunScript(string path, params object[] arguments)
            => RunScript(new PythonScript(path));
        public static void RunScript(PythonScript script, params object[] arguments)
        {
            //script.Run();
        }

#if EDITOR

        private EditorState _editorState = null;

        //[BrowsableIf("_editorState != null")]
        //[TSerialize]
        [Browsable(false)]
        public EditorState EditorState
        {
            get => _editorState ?? (_editorState = new EditorState(this));
            set
            {
                _editorState = value;
                if (_editorState != null)
                    _editorState.Object = this;
            }
        }

        internal protected virtual void OnHighlightChanged(bool highlighted) { }
        internal protected virtual void OnSelectedChanged(bool selected) { }

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
        
        [TSerialize(nameof(Animations))]
        private List<AnimationTree> _animations = null;

        [Browsable(false)]
        public IReadOnlyList<AnimationTree> Animations => _animations;

        /// <summary>
        /// Adds a property animation tree to this TObject.
        /// </summary>
        /// <param name="anim">The animation tree to add.</param>
        /// <param name="startNow">If the animation should be run immediately.</param>
        /// <param name="removeOnEnd">If the animation should be removed from this TObject upon finishing.</param>
        /// <param name="group">The group to tick this animation in.</param>
        /// <param name="order">The order within the group to tick this animation in.</param>
        /// <param name="pausedBehavior">Ticking behavior of the animation while paused.</param>
        public void AddAnimation(
            AnimationTree anim,
            bool startNow = false,
            bool removeOnEnd = true,
            ETickGroup group = ETickGroup.PostPhysics,
            ETickOrder order = ETickOrder.Animation,
            EInputPauseType pausedBehavior = EInputPauseType.TickOnlyWhenUnpaused)
        {
            if (anim == null)
                return;
            if (removeOnEnd)
                anim.AnimationEnded += RemoveAnimationSelf;
            if (_animations == null)
                _animations = new List<AnimationTree>();
            _animations.Add(anim);
            anim.Owners.Add(this);
            anim.Group = group;
            anim.Order = order;
            anim.PausedBehavior = pausedBehavior;
            if (startNow)
                anim.Start();
        }
        public bool RemoveAnimation(AnimationTree anim)
        {
            if (anim == null)
                return false;
            anim.AnimationEnded -= RemoveAnimationSelf;
            anim.Owners.Remove(this);
            bool removed = _animations.Remove(anim);
            if (_animations.Count == 0)
                _animations = null;
            return removed;
        }
        private void RemoveAnimationSelf(BaseAnimation anim)
        {
            anim.AnimationEnded -= RemoveAnimationSelf;
            AnimationTree cont = anim as AnimationTree;
            cont.Owners.Remove(this);
            _animations.Remove(cont);
        }
        #endregion

        /// <summary>
        /// Prints a line to output.
        /// Identical to Engine.Print().
        /// </summary>
        protected static void Print(string message, params object[] args) => Engine.Print(message, args);
        /// <summary>
        /// Prints a line to output.
        /// Identical to Engine.PrintLine().
        /// </summary>
        protected static void PrintLine(string message, params object[] args) => Engine.PrintLine(message, args);

        public override string ToString() => Name;
    }
}

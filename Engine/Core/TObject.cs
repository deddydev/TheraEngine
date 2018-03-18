using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Animation;
using System;
using TheraEngine.Input.Devices;
using TheraEngine.Scripting;
using TheraEngine.Editor;
using TheraEngine.Core.Reflection.Attributes;
using TheraEngine.Files.Serialization;
using System.Runtime.Serialization;
using System.Reflection;

namespace TheraEngine
{
    public interface IObjectBase
    {
        string Name { get; set; }
        object UserData { get; set; }

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
        IReadOnlyList<AnimationContainer> Animations { get; }
        void AddAnimation(
            AnimationContainer anim,
            bool startNow = false,
            bool removeOnEnd = true,
            ETickGroup group = ETickGroup.PostPhysics,
            ETickOrder order = ETickOrder.Animation,
            EInputPauseType pausedBehavior = EInputPauseType.TickAlways);
        bool RemoveAnimation(AnimationContainer anim);
        #endregion
    }

    public delegate void ResourceEventHandler(TObject node);
    public delegate void RenamedEventHandler(TObject node, string oldName);
    public delegate void ObjectPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);
    public abstract class TObject
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
        [TSerialize(nameof(Name), XmlNodeType = EXmlNodeType.Attribute)]
        protected string _name = null;
        private object _userData = null;
        
        [TSerialize]
        //[BrowsableIf("_userData != null")]
        [Browsable(false)]
        [Category("Object")]
        public object UserData
        {
            get => _userData;
            set => _userData = value;
        }

        #region Name
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
        private ThreadSafeList<TickInfo> _tickFunctions = new ThreadSafeList<TickInfo>();
        [Browsable(false)]
        public bool IsTicking => _tickFunctions.Count > 0;
        public void RegisterTick(ETickGroup group, ETickOrder order, DelTick tickFunc, EInputPauseType pausedBehavior = EInputPauseType.TickAlways)
        {
            _tickFunctions.Add(new TickInfo(group, order, tickFunc));
            Engine.RegisterTick(group, order, tickFunc, pausedBehavior);
        }
        public void UnregisterTick(ETickGroup group, ETickOrder order, DelTick tickFunc, EInputPauseType pausedBehavior = EInputPauseType.TickAlways)
        {
            int index = _tickFunctions.FindIndex(x => x.Item1 == group && x.Item2 == order && x.Item3 == tickFunc);
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

        [TSerialize(nameof(EditorState))]
        private EditorState _editorState = null;

        [Browsable(false)]
        //[BrowsableIf("_editorState != null")]
        public EditorState EditorState
        {
            get => _editorState ?? (_editorState = new EditorState(this));
            set => _editorState = value;
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
        
        [TSerialize]
        private List<AnimationContainer> _animations = null;

        [Browsable(false)]
        public IReadOnlyList<AnimationContainer> Animations => _animations;

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
            AnimationContainer anim,
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
                _animations = new List<AnimationContainer>();
            _animations.Add(anim);
            anim.Owners.Add(this);
            anim.Group = group;
            anim.Order = order;
            anim.PausedBehavior = pausedBehavior;
            if (startNow)
                anim.Start();
        }
        public bool RemoveAnimation(AnimationContainer anim)
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
        private void RemoveAnimationSelf(AnimationContainer anim)
        {
            anim.AnimationEnded -= RemoveAnimationSelf;
            anim.Owners.Remove(this);
            _animations.Remove(anim);
        }
        #endregion

        public override string ToString() => Name;
    }
}

using Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using TheraEngine.Animation;
using TheraEngine.Core;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Reflection.Attributes;
using TheraEngine.Editor;
using TheraEngine.Input.Devices;

namespace TheraEngine
{
    public interface IObject : IObjectSlim, INotifyPropertyChanged
    {
        event RenamedEventHandler Renamed;

        string Name { get; set; }
        object UserObject { get; set; }
        bool ConstructedProgrammatically { get; set; }
        Guid Guid { get; set; }

#if EDITOR
        bool HasEditorState { get; }
        EditorState EditorState { get; set; }
        void OnSelectedChanged(bool selected);
        void OnHighlightChanged(bool highlighted);
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
        IEventList<AnimationTree> Animations { get; set; }

        //void AddAnimation(
        //    AnimationTree anim,
        //    bool startNow = false,
        //    bool removeOnEnd = true,
        //    ETickGroup group = ETickGroup.PostPhysics,
        //    ETickOrder order = ETickOrder.Animation,
        //    EInputPauseType pausedBehavior = EInputPauseType.TickAlways);
        //bool RemoveAnimation(AnimationTree anim);
        #endregion
    }
    public interface IObjectExtension
    {
        IObject Owner { get; set; }
    }
    /// <summary>
    /// Base engine object with support for custom functionality.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TObjectExt<T> : TObject where T : IObjectExtension, new()
    {
        public TObjectExt()
            => Ext = new T() { Owner = this };

        public T Ext { get; }
    }
    /// <summary>
    /// Base engine object. Supports name, guid, user object, property bindings, ticking, animations, etc.
    /// </summary>
    public abstract class TObject : TObjectSlim, IObject
    {
        public TObject()
        {

        }

        #region Misc

        [TSerialize]
        private object _userObject = null;

        [Browsable(false)]
        [TSerialize(NodeType = ENodeType.Attribute)]
        public Guid Guid { get; set; } = Guid.NewGuid();

        /// <summary>
        /// If true, this object was originally constructed via code.
        /// If false, this object was originally deserialized from a file.
        /// </summary>
        [Browsable(false)]
        public bool ConstructedProgrammatically { get; set; } = true;

        //protected internal bool IsDeserializing { get; set; } = false;
        //protected internal bool IsSerializing { get; set; } = false;

        [BrowsableIf("UserObject != null")]
        //[Browsable(false)]
        [Category("Object")]
        public object UserObject
        {
            get => _userObject;
            set => Set(ref _userObject, value); 
        }

        #endregion

        #region Name

        [TString(false, false, false)]
        [TSerialize(nameof(Name), NodeType = ENodeType.Attribute)]
        public string _name = null;

        [Browsable(false)]
        [TString(false, false, false)]
        [Category("Object")]
        public virtual string Name
        {
            get => _name ?? GetType().GetFriendlyName("[", "]");
            set
            {
                string oldName = _name;
                if (Set(ref _name, value))
                    OnRenamed(oldName);
            }
        }

        public event RenamedEventHandler Renamed;

        protected virtual void OnRenamed(string oldName)
            => Renamed?.Invoke(this, oldName);

        #endregion

        #region Property Management

        [Serializable]
        public class SerializablePropertyChangedEventArgs : PropertyChangedEventArgs, ISerializable
        {
            public SerializablePropertyChangedEventArgs(string propertyName) : base(propertyName) { }
            protected SerializablePropertyChangedEventArgs(SerializationInfo info, StreamingContext context)
                : base((string)info.GetValue(nameof(PropertyName), typeof(string))) { }

            [SecurityPermission(SecurityAction.LinkDemand, 
                Flags = SecurityPermissionFlag.SerializationFormatter)]
            void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
                => info.AddValue(nameof(PropertyName), PropertyName);
        }
        [Serializable]
        public class SerializableCancellablePropertyChangingEventArgs : PropertyChangingEventArgs, ISerializable
        {
            public bool Cancel { get; set; } = false;

            public SerializableCancellablePropertyChangingEventArgs(string propertyName) : base(propertyName) { }
            protected SerializableCancellablePropertyChangingEventArgs(SerializationInfo info, StreamingContext context)
                : base((string)info.GetValue(nameof(PropertyName), typeof(string)))
                => Cancel = (bool)info.GetValue(nameof(Cancel), typeof(bool));

            [SecurityPermission(SecurityAction.LinkDemand,
                Flags = SecurityPermissionFlag.SerializationFormatter)]
            void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(PropertyName), PropertyName);
                info.AddValue(nameof(Cancel), Cancel);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        //public event PropertyChangingEventHandler PropertyChanging;

        ///// <summary>
        ///// Returns true if cancelled.
        ///// </summary>
        ///// <param name="propertyName"></param>
        ///// <returns></returns>
        //protected virtual bool OnPropertyChanging([CallerMemberName] string propertyName = null)
        //{
        //    var args = new SerializableCancellablePropertyChangingEventArgs(propertyName);
        //    PropertyChanging?.Invoke(this, args);
        //    return args.Cancel;
        //}
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new SerializablePropertyChangedEventArgs(propertyName));
        protected void OnPropertiesChanged(params string[] propertyNames)
            => propertyNames.ForEach(x => OnPropertyChanged(x));
        
        /// <summary>
        /// Helper method to set a property's backing field.
        /// Checks if the new value is equal to the new value and cancels if true.
        /// Otherwise, reports property changing event, sets value, then reports property changed event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="propertyName"></param>
        protected virtual bool Set<T>(
            ref T fieldValue, 
            T newValue, 
            Action beforeSet = null, 
            Action afterSet = null,
            bool executeMethodsIfNull = false, 
            [CallerMemberName] string propertyName = null)
        {
            if (Equals(fieldValue, newValue))
                return false;

            //if (OnPropertyChanging(propertyName))
            //    return false;

            if (executeMethodsIfNull || !EqualityComparer<T>.Default.Equals(fieldValue, default))
                beforeSet?.Invoke();

            fieldValue = newValue;

            if (executeMethodsIfNull || !EqualityComparer<T>.Default.Equals(newValue, default))
                afterSet?.Invoke();

            OnPropertyChanged(propertyName);

            return true;
        }
        /// <summary>
        /// Helper method to set a property's backing field.
        /// Checks if the new value is equal to the new value and cancels if true.
        /// Otherwise, reports property changing event, sets value, then reports property changed event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="propertyName"></param>
        protected virtual bool SetExplicit<T>(
            ref T fieldValue,
            T newValue,
            Action<T> beforeSet = null,
            Action<T> afterSet = null,
            bool executeMethodsIfNull = false,
            [CallerMemberName] string propertyName = null)
        {
            if (Equals(fieldValue, newValue))
                return false;

            //if (OnPropertyChanging(propertyName))
            //    return false;

            if (executeMethodsIfNull || !EqualityComparer<T>.Default.Equals(fieldValue, default))
                beforeSet?.Invoke(fieldValue);

            fieldValue = newValue;

            if (executeMethodsIfNull || !EqualityComparer<T>.Default.Equals(newValue, default))
                afterSet?.Invoke(newValue);

            OnPropertyChanged(propertyName);

            return true;
        }
        protected virtual bool SetRef<T, TRef>(
            ref TRef fieldValue,
            TRef newValue,
            Action<T> onLoaded = null,
            Action<T> onUnloaded = null,
            bool allowNullRef = false,
            [CallerMemberName] string propertyName = null)
            where T : class, IFileObject
            where TRef : IFileRef<T>, new()
        {
            if (Equals(fieldValue, newValue))
                return false;

            //if (OnPropertyChanging(propertyName))
            //    return false;

            if (fieldValue != null)
            {
                if (onLoaded != null)
                    fieldValue.Loaded -= onLoaded;
                if (onUnloaded != null)
                {
                    fieldValue.Unloaded -= onUnloaded;
                    if (fieldValue.IsLoaded)
                        onUnloaded(fieldValue.File);
                }
            }

            if (!allowNullRef && newValue is null)
                newValue = new TRef();

            fieldValue = newValue;

            if (fieldValue != null)
            {
                if (onLoaded != null)
                {
                    if (fieldValue.IsLoaded)
                        onLoaded(fieldValue.File);
                    fieldValue.Loaded += onLoaded;
                }
                if (onUnloaded != null)
                    fieldValue.Unloaded += onUnloaded;
            }

            OnPropertyChanged(propertyName);

            return true;
        }

        [Category("Object")]
        [BrowsableIf("Bindings != null")]
        [TSerialize]
        public Dictionary<string, PropertyBinding> Bindings { get; private set; }

        public void BindProperty(string propertyName, TObject source, string sourcePropertyName, EBindingMode mode = EBindingMode.OneWay, TPropertyConverter converter = null)
        {
            if (Bindings is null)
                Bindings = new Dictionary<string, PropertyBinding>();

            Bindings.Add(propertyName, new PropertyBinding(this, source, propertyName, sourcePropertyName, mode, converter));
        }
        public void BindProperty(string propertyName, TObject source, string sourcePropertyName, Func<object, object> converter, bool toSource = false)
        {
            if (Bindings is null)
                Bindings = new Dictionary<string, PropertyBinding>();

            Bindings.Add(propertyName, new PropertyBinding(this, source, propertyName, sourcePropertyName, converter, toSource));
        }
        public void ClearPropertyBinding(string propertyName)
        {
            if (Bindings.ContainsKey(propertyName))
                Bindings.Remove(propertyName);

            if (Bindings.Count == 0)
                Bindings = null;
        }
        public void ClearAllPropertyBindings(string propertyName)
        {
            if (Bindings.ContainsKey(propertyName))
                Bindings.Remove(propertyName);

            if (Bindings.Count == 0)
                Bindings = null;
        }

        #endregion

        #region Ticking
        private List<(ETickGroup Group, ETickOrder Order, DelTick Tick)> _tickFunctions
            = new List<(ETickGroup Group, ETickOrder Order, DelTick Tick)>();
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

        #region Editor

#if EDITOR

        private EditorState _editorState = null;

        [Browsable(false)]
        public bool HasEditorState => _editorState != null;
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

        void IObject.OnHighlightChanged(bool highlighted) => OnHighlightChanged(highlighted);
        void IObject.OnSelectedChanged(bool selected) => OnSelectedChanged(selected);
        protected internal virtual void OnHighlightChanged(bool highlighted) { }
        protected internal virtual void OnSelectedChanged(bool selected) { }

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

        #endregion

        #region Animation

        [TSerialize(nameof(Animations))]
        private IEventList<AnimationTree> _animations = null;

        [Category("Object")]
        [BrowsableIf("Animations != null")]
        public IEventList<AnimationTree> Animations
        {
            get => _animations;
            set => Set(ref _animations, value,
                () =>
                {
                    _animations.PostAnythingAdded -= AnimationAdded;
                    _animations.PostAnythingRemoved -= AnimationRemoved;
                    foreach (var anim in _animations)
                        AnimationRemoved(anim);
                },
                () =>
                {
                    _animations.PostAnythingAdded += AnimationAdded;
                    _animations.PostAnythingRemoved += AnimationRemoved;
                    foreach (var anim in _animations)
                        AnimationAdded(anim);
                });
        }

        private void AnimationAdded(AnimationTree item)
        {
            if (item is null)
                return;

            item.PropertyChanged += AnimTree_PropertyChanged;

            if (item.RemoveOnEnd)
                item.AnimationEnded += RemoveAnimationSelf;

            item.Owners.Add(this);
        }
        private void AnimationRemoved(AnimationTree animTree)
        {
            if (animTree is null)
                return;

            animTree.PropertyChanged -= AnimTree_PropertyChanged;

            if (animTree.RemoveOnEnd)
                animTree.AnimationEnded -= RemoveAnimationSelf;

            animTree.Owners.Remove(this);
        }

        private void AnimTree_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is AnimationTree animTree))
                return;
            
            if (e.PropertyName.EqualsInvariant(nameof(AnimationTree.RemoveOnEnd)))
            {
                if (animTree.RemoveOnEnd)
                    animTree.AnimationEnded += RemoveAnimationSelf;
                else
                    animTree.AnimationEnded -= RemoveAnimationSelf;
            }
        }

        /// <summary>
        /// Adds a property animation tree to this TObject.
        /// </summary>
        /// <param name="anim">The animation tree to add.</param>
        /// <param name="startNow">If the animation should be run immediately.</param>
        /// <param name="removeOnEnd">If the animation should be removed from this TObject upon finishing.</param>
        /// <param name="group">The group to tick this animation in.</param>
        /// <param name="order">The order within the group to tick this animation in.</param>
        /// <param name="pausedBehavior">Ticking behavior of the animation while paused.</param>
        //public void AddAnimation(
        //    AnimationTree anim,
        //    bool startNow = false,
        //    bool removeOnEnd = true,
        //    ETickGroup group = ETickGroup.PostPhysics,
        //    ETickOrder order = ETickOrder.Animation,
        //    EInputPauseType pausedBehavior = EInputPauseType.TickOnlyWhenUnpaused)
        //{
        //    if (anim is null)
        //        return;
        //    if (removeOnEnd)
        //        anim.AnimationEnded += RemoveAnimationSelf;
        //    if (_animations is null)
        //        _animations = new EventList<AnimationTree>();
        //    _animations.Add(anim);
        //    anim.Group = group;
        //    anim.Order = order;
        //    anim.PausedBehavior = pausedBehavior;
        //    anim.TickSelf = true;
        //    if (startNow)
        //        anim.Start();
        //}
        //public bool RemoveAnimation(AnimationTree anim)
        //{
        //    if (anim is null)
        //        return false;
        //    anim.Owners.Remove(this);
        //    bool removed = _animations.Remove(anim);
        //    if (_animations.Count == 0)
        //        _animations = null;
        //    return removed;
        //}
        private void RemoveAnimationSelf(BaseAnimation anim)
            => _animations.Remove((AnimationTree)anim);

        public delegate float DelAnimateFloat(float value, float time);
        public void AnimateProperty(string name, float seconds, PropAnimMethod<float>.DelGetValue interp)
        {
            //TODO: pool these animation trees?
            PropAnimMethod<float> anim = new PropAnimMethod<float>(seconds, false, interp);
            AnimationTree tree = new AnimationTree("AnimatePropertyMethod", name, anim)
            {
                RemoveOnEnd = true
            };
            Animations.Add(tree);
        }

        protected void StartAllAnimations(bool beginOnSpawnOnly)
        {
            Animations?.ForEach(anim =>
            {
                if (anim.State == EAnimationState.Stopped && (!beginOnSpawnOnly || anim.BeginOnSpawn))
                    anim.Start();
            });
        }
        protected void StopAllAnimations()
        {
            Animations?.ForEach(anim =>
            {
                if (anim.State != EAnimationState.Stopped)
                    anim.Stop();
            });
        }

        #endregion
    }

    public delegate void RenamedEventHandler(TObject node, string oldName);
}

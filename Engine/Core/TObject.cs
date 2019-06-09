﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Serialization;
using TheraEngine.Animation;
using TheraEngine.Core.Reflection;
using TheraEngine.Core.Reflection.Attributes;
using TheraEngine.Editor;
using TheraEngine.Input.Devices;

namespace TheraEngine
{
    public interface IObject
    {
        event RenamedEventHandler Renamed;

        string Name { get; set; }
        object UserObject { get; set; }
        bool ConstructedProgrammatically { get; set; }
        AppDomain Domain { get; }
        bool HasEditorState { get; }

#if EDITOR
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
        Guid Guid { get; set; }

        //void AddAnimation(
        //    AnimationTree anim,
        //    bool startNow = false,
        //    bool removeOnEnd = true,
        //    ETickGroup group = ETickGroup.PostPhysics,
        //    ETickOrder order = ETickOrder.Animation,
        //    EInputPauseType pausedBehavior = EInputPauseType.TickAlways);
        //bool RemoveAnimation(AnimationTree anim);
        #endregion

        TypeProxy RetrieveTypeProxy();
    }

    public class MarshalSponsor : MarshalByRefObject, ISponsor
    {
        public static readonly TimeSpan RenewalTimeSpan = TimeSpan.FromSeconds(1);

        public bool Release { get; set; } = false;
        public bool IsReleased { get; private set; } = false;

        public TimeSpan Renewal(ILease lease)
        {
            // if any of these cases is true
            if (lease == null || lease.CurrentState != LeaseState.Renewing || Release)
            {
                IsReleased = true;
                return TimeSpan.Zero; // don't renew
            }
            IsReleased = lease.CurrentState == LeaseState.Expired;
            return RenewalTimeSpan;
        }
    }
    public delegate void ResourceEventHandler(TObject node);
    public delegate void RenamedEventHandler(TObject node, string oldName);
    public delegate void ObjectPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);
    public abstract class TObject : MarshalByRefObject, IObject, ISerializable
    {
        public AppDomain Domain => AppDomain.CurrentDomain;
        TypeProxy IObject.RetrieveTypeProxy() => GetType();
        
        internal MarshalSponsor Sponsor { get; set; }

        [TString(false, false, false)]
        [TSerialize(nameof(Name), NodeType = ENodeType.Attribute)]
        public string _name = null;

        [Browsable(false)]
        //[TSerialize(NodeType = ENodeType.Attribute)]
        public Guid Guid { get; set; } = Guid.NewGuid();

        /// <summary>
        /// If true, this object was originally constructed via code.
        /// If false, this object was originally deserialized from a file.
        /// </summary>
        [Browsable(false)]
        public bool ConstructedProgrammatically { get; set; } = true;

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
            get => _name ?? this.GetType().GetFriendlyName("[", "]");
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
        [BrowsableIf("_animations != null")] // && _animations.Count > 0
        public IEventList<AnimationTree> Animations
        {
            get => _animations;
            set
            {
                if (_animations != null)
                {
                    _animations.PostAnythingAdded -= _animations_PostAnythingAdded;
                    _animations.PostAnythingRemoved -= _animations_PostAnythingRemoved;
                }
                _animations = value;
                if (_animations != null)
                {
                    _animations.PostAnythingAdded += _animations_PostAnythingAdded;
                    _animations.PostAnythingRemoved += _animations_PostAnythingRemoved;
                }
            }
        }

        private void _animations_PostAnythingAdded(AnimationTree item)
        {
            if (item.RemoveOnEnd)
                item.AnimationEnded += RemoveAnimationSelf;
            item.Owners.Add(this);
        }
        private void _animations_PostAnythingRemoved(AnimationTree item)
        {
            if (item.RemoveOnEnd)
                item.AnimationEnded -= RemoveAnimationSelf;
            item.Owners.Remove(this);
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
        //    if (anim == null)
        //        return;
        //    if (removeOnEnd)
        //        anim.AnimationEnded += RemoveAnimationSelf;
        //    if (_animations == null)
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
        //    if (anim == null)
        //        return false;
        //    anim.Owners.Remove(this);
        //    bool removed = _animations.Remove(anim);
        //    if (_animations.Count == 0)
        //        _animations = null;
        //    return removed;
        //}
        private void RemoveAnimationSelf(BaseAnimation anim)
            => _animations.Remove((AnimationTree)anim);
        #endregion

        #region Debug
        
        /// <summary>
        /// Prints a line to output.
        /// Identical to Engine.PrintLine().
        /// </summary>
        protected static void PrintLine(string message, params object[] args) => Debug.Print(message, args);
        /// <summary>
        /// Prints a line to output.
        /// Identical to Engine.PrintLine().
        /// </summary>
        protected static void PrintLine(string message) => Debug.Print(message);

        public override string ToString() => Name;

        #endregion

        public void GetObjectData(SerializationInfo info, StreamingContext context) => throw new NotImplementedException();
    }
}

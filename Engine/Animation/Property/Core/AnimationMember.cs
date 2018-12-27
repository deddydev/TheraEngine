using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using TheraEngine.Core.Files;

namespace TheraEngine.Animation
{
    public enum EAnimationMemberType
    {
        Field,
        Property,
        Method,
    }
    public class AnimationMember
    {
        public AnimationMember() { }
        /// <summary>
        /// Constructor to create a subtree without an animation at this level.
        /// </summary>
        /// <param name="memberName">The name of this property and optionally sub-properties separated by a period.</param>
        /// <param name="children">Any sub-properties this property owns and you want to animate.</param>
        public AnimationMember(string memberName, params AnimationMember[] children) : this()
        {
            int splitIndex = memberName.IndexOf('.');
            if (splitIndex >= 0)
            {
                string remainingPath = memberName.Substring(splitIndex + 1, memberName.Length - splitIndex - 1);
                _children.Add(new AnimationMember(remainingPath));
                memberName = memberName.Substring(0, splitIndex);
            }
            if (children != null)
                _children.AddRange(children);
            _memberName = memberName;
            SetAnimation(null, EAnimationMemberType.Field);
        }
        /// <summary>
        /// Constructor to create a subtree with an animation attached at this level.
        /// </summary>
        /// <param name="memberName">The name of the field, property or method to animate.</param>
        /// <param name="memberType"></param>
        /// <param name="animation"></param>
        public AnimationMember(string memberName, EAnimationMemberType memberType, BasePropAnim animation) : this()
        {
            if (memberType != EAnimationMemberType.Property)
            {
                int splitIndex = memberName.IndexOf('.');
                if (splitIndex >= 0)
                {
                    string remainingPath = memberName.Substring(splitIndex + 1, memberName.Length - splitIndex - 1);
                    _children.Add(new AnimationMember(remainingPath));
                    memberName = memberName.Substring(0, splitIndex);
                }
            }
            _memberName = memberName;
            SetAnimation(animation, memberType);
        }

        //Cached at runtime
        private PropertyInfo _propertyCache;
        private MethodInfo _methodCache;
        private FieldInfo _fieldCache;

        //TODO: need <object, bool> dictionary because multiple objs might tick in this anim
        private bool _memberNotFound = false;
        internal Action<object, float> _tick = null;

        [TSerialize("MemberType")]
        private EAnimationMemberType _memberType = EAnimationMemberType.Property;
        [TSerialize("MemberName")]
        private string _memberName = null;
        [TSerialize("Animation")]
        private GlobalFileRef<BasePropAnim> _animation = new GlobalFileRef<BasePropAnim>();
        [TSerialize("Children")]
        private EventList<AnimationMember> _children = new EventList<AnimationMember>();

        [Category("Animation Member")]
        public GlobalFileRef<BasePropAnim> Animation => _animation;

        [Category("Animation Member")]
        public EventList<AnimationMember> Children => _children;
        [Category("Animation Member")]
        public string MemberName
        {
            get => _memberName;
            set => _memberName = value;
        }
        [Category("Animation Member")]
        public EAnimationMemberType MemberType
        {
            get => _memberType;
            set
            {
                _memberType = value;
                _memberNotFound = false;
                switch (_memberType)
                {
                    case EAnimationMemberType.Field:
                        _tick = FieldTick;
                        break;
                    case EAnimationMemberType.Property:
                        _tick = PropertyTick;
                        break;
                    case EAnimationMemberType.Method:
                        _tick = MethodTick;
                        break;
                }
            }
        }
        public void CollectAnimations(string path, Dictionary<string, BasePropAnim> animations)
        {
            if (!string.IsNullOrEmpty(path))
                path += "." + _memberName;
            else
                path = _memberName;

            if (Animation != null)
                animations.Add(path, Animation);

            foreach (AnimationMember member in _children)
                member.CollectAnimations(path, animations);
        }
        public void SetAnimation(BasePropAnim anim, EAnimationMemberType memberType)
        {
            Animation.File = anim;
            MemberType = memberType;
        }
        internal void MethodTick(object obj, float delta)
        {
            if (obj == null || _memberNotFound)
                return;

            if (_methodCache == null)
            {
                Type type = obj.GetType();
                while (type != null)
                {
                    if ((_methodCache = type.GetMethod(_memberName)) == null)
                        type = type.BaseType;
                    else
                        break;
                }
                if (_memberNotFound = _methodCache == null)
                    return;
            }

            Animation.File?.Tick(obj, _methodCache, delta);
        }
        internal void PropertyTick(object obj, float delta)
        {
            if (obj == null || _memberNotFound)
                return;

            if (_propertyCache == null)
            {
                Type type = obj.GetType();
                while (type != null)
                {
                    if ((_propertyCache = type.GetProperty(_memberName)) == null)
                        type = type.BaseType;
                    else
                        break;
                }
                if (_memberNotFound = _propertyCache == null)
                    return;
            }

            if (Animation.IsLoaded)
                Animation.File.Tick(obj, _propertyCache, delta);
            else
            {
                object value = _propertyCache.GetValue(obj);
                foreach (AnimationMember f in _children)
                    f._tick(value, delta);
            }
        }
        internal void FieldTick(object obj, float delta)
        {
            if (obj == null || _memberNotFound)
                return;

            if (_fieldCache == null)
            {
                Type type = obj.GetType();
                while (type != null)
                {
                    if ((_fieldCache = type.GetField(_memberName)) == null)
                        type = type.BaseType;
                    else
                        break;
                }
                if (_memberNotFound = _fieldCache == null)
                    return;
            }

            if (Animation.IsLoaded)
                Animation.File.Tick(obj, _fieldCache, delta);
            else
            {
                object value = _fieldCache.GetValue(obj);
                foreach (AnimationMember f in _children)
                    f._tick(value, delta);
            }
        }
        /// <summary>
        /// Registers to the AnimationHasEnded method in the animation tree
        /// and returns the total amount of animations this member and its child members contain.
        /// </summary>
        /// <param name="tree">The animation tree that owns this member.</param>
        /// <returns>The total amount of animations this member and its child members contain.</returns>
        internal int Register(AnimationTree tree)
        {
            bool animExists = Animation.File != null;
            int count = animExists ? 1 : 0;

            //TODO: call Animation.File.AnimationEnded -= tree.AnimationHasEnded somewhere
            if (animExists)
                Animation.File.AnimationEnded += tree.AnimationHasEnded;

            foreach (AnimationMember folder in _children)
                count += folder.Register(tree);

            return count;
        }
        internal void StartAnimations()
        {
            _memberNotFound = false;
            _fieldCache = null;
            _propertyCache = null;
            _methodCache = null;

            Animation.File?.Start();
            foreach (AnimationMember folder in _children)
                folder.StartAnimations();
        }
        internal void StopAnimations()
        {
            _memberNotFound = false;
            _fieldCache = null;
            _propertyCache = null;
            _methodCache = null;

            Animation.File?.Stop();
            foreach (AnimationMember folder in _children)
                folder.StopAnimations();
        }
    }
}

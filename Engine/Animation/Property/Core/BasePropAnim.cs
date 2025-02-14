﻿using System.Reflection;
using TheraEngine.ComponentModel;

namespace TheraEngine.Animation
{
    /// <summary>
    /// Base class for animations that animate properties such as Vec3, bool and float.
    /// </summary>
    [TFileExt("propanim")]
    [TFileDef("Property Animation")]
    public abstract class BasePropAnim : BaseAnimation
    {
        public const string PropAnimCategory = "Property Animation";

        public BasePropAnim(float lengthInSeconds, bool looped)
            : base(lengthInSeconds, looped)
        {
            //Animation container will tick all of its children
            _tickSelf = false;
        }

        /// <summary>
        /// Call to set this animation's current value to an object's property and then advance the animation by the given delta.
        /// </summary>
        public void Tick(object obj, FieldInfo field, float delta)
        {
            field.SetValue(obj, GetCurrentValueGeneric());
            Progress(delta);
        }
        /// <summary>
        /// Call to set this animation's current value to an object's property and then advance the animation by the given delta.
        /// </summary>
        public void Tick(object obj, PropertyInfo property, float delta)
        {
            property.SetValue(obj, GetCurrentValueGeneric());
            Progress(delta);
        }
        private readonly object[] _memberParamCache = new object[1];
        /// <summary>
        /// Call to set this animation's current value to an object's method that takes it as a single argument and then advance the animation by the given delta.
        /// </summary>
        public void Tick(object obj, MethodInfo method, float delta)
        {
            _memberParamCache[0] = GetCurrentValueGeneric();
            method.Invoke(obj, _memberParamCache);
            Progress(delta);
        }

        /// <summary>
        /// Retrieves the value for the animation's current time.
        /// Used by the internal animation implementation to set property/field values and call methods,
        /// so must be overridden.
        /// </summary>
        protected abstract object GetCurrentValueGeneric();
        /// <summary>
        /// Retrieves the value for the given second.
        /// Used by the internal animation implementation to set property/field values and call methods,
        /// so must be overridden.
        /// </summary>
        protected abstract object GetValueGeneric(float second);
    }
}
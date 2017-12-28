using System;
using System.ComponentModel;
using System.Reflection;

namespace TheraEngine.Animation
{
    /// <summary>
    /// Base class for animations that animate properties such as Vec3, bool and float.
    /// </summary>
    [FileExt("propanim")]
    [FileDef("Property Animation")]
    public abstract class BasePropAnim : BaseAnimation
    {
        public BasePropAnim(float lengthInSeconds, bool looped, bool isBaked = false)
            : base(lengthInSeconds, looped, isBaked) { }
        public BasePropAnim(int frameCount, float fPS, bool looped, bool isBaked = false)
            : base(frameCount, fPS, looped, isBaked) { }
        public void Tick(object obj, PropertyInfo property, float delta)
        {
            if (_state != AnimationState.Playing)
                return;
            property.SetValue(obj, GetValueGeneric(_currentTime));
            Progress(delta);
        }
        public void Tick(object obj, MethodInfo method, float delta)
        {
            if (_state != AnimationState.Playing)
                return;
            method.Invoke(obj, new object[] { GetValueGeneric(_currentTime) });
            Progress(delta);
        }
        protected abstract object GetValueGeneric(float frame);
    }
}
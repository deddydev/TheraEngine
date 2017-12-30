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
        public override void Start()
        {
            if (_state == AnimationState.Playing)
                return;
            PreStarted();
            _state = AnimationState.Playing;
            CurrentTime = 0.0f;
            OnAnimationStarted();
            PostStarted();
        }
        public override void Stop()
        {
            if (_state == AnimationState.Stopped)
                return;
            PreStopped();
            CurrentTime = 0.0f;
            _state = AnimationState.Stopped;
            OnAnimationEnded();
            PostStopped();
        }
        public override void Pause()
        {
            if (_state != AnimationState.Playing)
                return;
            PrePaused();
            _state = AnimationState.Paused;
            OnAnimationPaused();
            PostPaused();
        }
        protected abstract object GetValueGeneric(float frame);
    }
}
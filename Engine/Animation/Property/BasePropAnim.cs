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
        public const string PropAnimCategory = "Property Animation";

        public BasePropAnim(float lengthInSeconds, bool looped, bool isBaked = false)
            : base(lengthInSeconds, looped, isBaked) { }
        public BasePropAnim(int frameCount, float framesPerSecond, bool looped, bool isBaked = false)
            : base(frameCount, framesPerSecond, looped, isBaked) { }
        public void Tick(object obj, PropertyInfo property, float delta)
        {
            if (_state != EAnimationState.Playing)
                return;
            property.SetValue(obj, GetValueGeneric());
            Progress(delta);
        }
        public void Tick(object obj, MethodInfo method, float delta)
        {
            if (_state != EAnimationState.Playing)
                return;
            method.Invoke(obj, new object[] { GetValueGeneric(_currentTime) });
            Progress(delta);
        }
        public override void Start()
        {
            if (_state == EAnimationState.Playing)
                return;
            PreStarted();
            _state = EAnimationState.Playing;
            OnAnimationStarted();
            PostStarted();
        }
        public override void Stop()
        {
            if (_state == EAnimationState.Stopped)
                return;
            PreStopped();
            _state = EAnimationState.Stopped;
            OnAnimationEnded();
            PostStopped();
        }
        public override void Pause()
        {
            if (_state != EAnimationState.Playing)
                return;
            PrePaused();
            _state = EAnimationState.Paused;
            OnAnimationPaused();
            PostPaused();
        }
        /// <summary>
        /// Retrieves the value for the animation's current time.
        /// Used by the internal animation implementation to set property values and call methods,
        /// so must be overridden.
        /// </summary>
        protected abstract object GetValueGeneric();
        /// <summary>
        /// Retrieves the value for the given second.
        /// Used by the internal animation implementation to set property values and call methods,
        /// so must be overridden.
        /// </summary>
        protected abstract object GetValueGeneric(float second);
    }
}
using System;
using System.ComponentModel;
using System.Reflection;

namespace TheraEngine.Animation
{
    public abstract class PropAnimKeyframed<T> : BasePropAnimKeyframed where T : Keyframe
    {
        public delegate T2 DelGetValue<T2>(float second);
        protected KeyframeTrack<T> _keyframes;

        public PropAnimKeyframed(float lengthInSeconds, bool looped, bool isBaked = false) 
            : base(lengthInSeconds, looped, isBaked)
        {
            _keyframes = new KeyframeTrack<T>();
        }
        public PropAnimKeyframed(int frameCount, float FPS, bool looped, bool isBaked = false)
            : base(frameCount, FPS, looped, isBaked)
        {
            _keyframes = new KeyframeTrack<T>();
        }
        
        [Category("Property Animation")]
        protected override BaseKeyframeTrack InternalKeyframes => _keyframes;

        [TSerialize]
        [Category("Property Animation")]
        public KeyframeTrack<T> Keyframes => _keyframes;

        /// <summary>
        /// Appends the keyframes of the given animation to the end of this one.
        /// Basically, where this animation currently ends is where the given will begin, all in one animation.
        /// </summary>
        public void Append(PropAnimKeyframed<T> other)
            => Keyframes.Append(other.Keyframes);
    }
}
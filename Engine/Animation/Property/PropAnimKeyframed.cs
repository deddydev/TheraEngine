using System.ComponentModel;

namespace TheraEngine.Animation
{
    public abstract class PropAnimKeyframed<T> : BasePropAnimKeyframed where T : Keyframe
    {
        public delegate T2 DelGetValue<T2>(float second);

        [TSerialize(nameof(Keyframes))]
        protected KeyframeTrack<T> _keyframes;

        public PropAnimKeyframed(float lengthInSeconds, bool looped, bool isBaked = false) 
            : base(lengthInSeconds, looped, isBaked) => ConstructKeyframes();
        public PropAnimKeyframed(int frameCount, float FPS, bool looped, bool isBaked = false)
            : base(frameCount, FPS, looped, isBaked) => ConstructKeyframes();
        
        private void ConstructKeyframes()
        {
            _keyframes = new KeyframeTrack<T>();
            _keyframes.LengthChanged += _keyframes_LengthChanged;
        }

        private void _keyframes_LengthChanged(float oldLength, float newLength)
        {
            _lengthInSeconds = newLength;
            SetBakedFramecount();
            OnLengthChanged();
        }

        protected override BaseKeyframeTrack InternalKeyframes => _keyframes;

        [Category(PropAnimCategory)]
        public KeyframeTrack<T> Keyframes => _keyframes;

        /// <summary>
        /// Appends the keyframes of the given animation to the end of this one.
        /// Basically, where this animation currently ends is where the given will begin, all in one animation.
        /// </summary>
        public void Append(PropAnimKeyframed<T> other)
            => Keyframes.Append(other.Keyframes);
    }
}
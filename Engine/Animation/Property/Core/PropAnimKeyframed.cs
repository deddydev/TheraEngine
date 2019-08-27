using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEngine.Animation
{
    public abstract class PropAnimKeyframed<T> : BasePropAnimKeyframed, IEnumerable<T> where T : Keyframe, new()
    {
        public delegate T2 DelGetValue<T2>(float second);

        [TSerialize("KeyframeTrack")]
        protected KeyframeTrack<T> _keyframes;

        public PropAnimKeyframed()
            : this(0.0f, false) { }
        public PropAnimKeyframed(float lengthInSeconds, bool looped, bool useKeyframes = true) 
            : base(lengthInSeconds, looped, useKeyframes) => ConstructKeyframes();
        public PropAnimKeyframed(int frameCount, float framesPerSecond, bool looped, bool useKeyframes = true)
            : base(frameCount, framesPerSecond, looped, useKeyframes) => ConstructKeyframes();
        
        private KeyframeTrack<T> ConstructKeyframes()
        {
            _keyframes = new KeyframeTrack<T>();
            _keyframes.LengthChanged += KeyframesLengthChanged;
            return _keyframes;
        }

        private bool _updatingLength = false;
        private void KeyframesLengthChanged(float oldValue, BaseKeyframeTrack track)
        {
            if (_updatingLength)
                return;
            _updatingLength = true;
            SetLength(track.LengthInSeconds, false, true);
            _updatingLength = false;
        }

        protected override BaseKeyframeTrack InternalKeyframes => Keyframes;

        [Category("Keyframed Property Animation")]
        public KeyframeTrack<T> Keyframes => _keyframes ?? ConstructKeyframes();

        /// <summary>
        /// Appends the keyframes of the given animation to the end of this one.
        /// Basically, where this animation currently ends is where the given will begin, all in one animation.
        /// </summary>
        public void Append(PropAnimKeyframed<T> other)
            => Keyframes.Append(other.Keyframes);
        
        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)Keyframes).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)Keyframes).GetEnumerator();
    }
}
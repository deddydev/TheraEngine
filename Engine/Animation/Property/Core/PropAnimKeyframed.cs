﻿using System.Collections;
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
        public PropAnimKeyframed(float lengthInSeconds, bool looped, bool isBaked = false) 
            : base(lengthInSeconds, looped, isBaked) => ConstructKeyframes();
        public PropAnimKeyframed(int frameCount, float framesPerSecond, bool looped, bool isBaked = false)
            : base(frameCount, framesPerSecond, looped, isBaked) => ConstructKeyframes();
        
        private void ConstructKeyframes()
        {
            _keyframes = new KeyframeTrack<T>();
            _keyframes.LengthChanged += KeyframesLengthChanged;
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

        protected override BaseKeyframeTrack InternalKeyframes => _keyframes;

        [Category("Keyframed Property Animation")]
        public KeyframeTrack<T> Keyframes => _keyframes;

        /// <summary>
        /// Appends the keyframes of the given animation to the end of this one.
        /// Basically, where this animation currently ends is where the given will begin, all in one animation.
        /// </summary>
        public void Append(PropAnimKeyframed<T> other)
            => Keyframes.Append(other.Keyframes);
        
        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_keyframes).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)_keyframes).GetEnumerator();
    }
}
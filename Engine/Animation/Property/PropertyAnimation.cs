using System;
using System.ComponentModel;
using System.Reflection;
using TheraEngine.Files;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Animation
{
    [FileClass("PANM", "Property Animation")]
    public abstract class BasePropertyAnimation : BaseAnimation
    {
        /// <summary>
        /// Determines which method to use, baked or keyframed.
        /// Keyframed takes up less memory and calculates in-between frames on the fly, which allows for time dilation.
        /// Baked takes up more memory but requires no calculations. However, the animation cannot be sped up at all, nor slowed down without artifacts.
        /// </summary>
        [Category("Property Animation"), Serialize(IsXmlAttribute = true)]
        public bool UseKeyframes
        {
            get => _useKeyframes;
            set
            {
                _useKeyframes = value;
                UseKeyframesChanged();
            }
        }
        public override void SetLength(float lengthInSeconds, bool stretchAnimation)
        {
            InternalKeyframes.SetLength(lengthInSeconds, stretchAnimation);
            base.SetLength(lengthInSeconds, stretchAnimation);
        }
        public void Tick(object obj, PropertyInfo property, float delta)
        {
            if (!_isPlaying)
                return;
            property.SetValue(obj, GetValue(_currentTime));
            Progress(delta);
        }
        public void Tick(object obj, MethodInfo method, float delta)
        {
            if (!_isPlaying)
                return;
            method.Invoke(obj, new object[] { GetValue(_currentTime) });
            Progress(delta);
        }

        protected abstract object GetValue(float frame);
        protected abstract void UseKeyframesChanged();
        
        [Category("Property Animation")]
        protected abstract BaseKeyframeTrack InternalKeyframes { get; }

        /// <summary>
        /// Bakes the interpolated data for fastest access by the game.
        /// However, this method takes up more space and does not support time dilation (speeding up and slowing down with proper in-betweens)
        /// </summary>
        public abstract void Bake(float framesPerSecond);
    }
    public abstract class PropertyAnimation<T> : BasePropertyAnimation where T : Keyframe
    {
        public delegate T2 GetValue<T2>(float second);
        protected KeyframeTrack<T> _keyframes;

        public PropertyAnimation(float lengthInSeconds, bool looped, bool useKeyframes)
        {
            _bakedFrameCount = (int)Math.Ceiling(lengthInSeconds * 60.0f);
            _bakedFPS = 60.0f;
            _lengthInSeconds = lengthInSeconds;
            _keyframes = new KeyframeTrack<T>();
            Looped = looped;
            UseKeyframes = useKeyframes;
        }
        public PropertyAnimation(int frameCount, float FPS, bool looped, bool useKeyframes)
        {
            _bakedFrameCount = frameCount;
            _bakedFPS = FPS;
            _lengthInSeconds = frameCount / FPS;
            _keyframes = new KeyframeTrack<T>();
            Looped = looped;
            UseKeyframes = useKeyframes;
        }
        
        [Category("Property Animation")]
        protected override BaseKeyframeTrack InternalKeyframes => _keyframes;

        [Category("Property Animation")]
        [Serialize]
        public KeyframeTrack<T> Keyframes => _keyframes;

        public void Append(PropertyAnimation<T> other)
            => Keyframes.Append(other.Keyframes);
    }
}

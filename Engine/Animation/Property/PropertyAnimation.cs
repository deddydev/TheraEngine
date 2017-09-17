using System.ComponentModel;
using System.Reflection;
using TheraEngine.Files;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Animation
{
    [FileClass("PANIM", "Property Animation")]
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
            set { _useKeyframes = value; UseKeyframesChanged(); }
        }
        public override void SetFrameCount(int frameCount, bool stretchAnimation)
        {
            InternalKeyframes.SetFrameCount(frameCount, stretchAnimation);
            base.SetFrameCount(frameCount, stretchAnimation);
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
        public abstract void Resize(int newSize);
        public abstract void Stretch(int newSize);
        public abstract void Bake();
    }
    public abstract class PropertyAnimation<T> : BasePropertyAnimation where T : Keyframe
    {
        protected KeyframeTrack<T> _keyframes;

        public PropertyAnimation(int frameCount, bool looped, bool useKeyframes)
        {
            _bakedFrameCount = frameCount;
            _keyframes = new KeyframeTrack<T>(this);
            Looped = looped;
            UseKeyframes = useKeyframes;
        }
        
        [Category("Property Animation")]
        protected override BaseKeyframeTrack InternalKeyframes => _keyframes;

        [Category("Property Animation")]
        [Serialize]
        public KeyframeTrack<T> Keyframes => _keyframes;

        public abstract void Append(PropertyAnimation<T> other);
    }
}

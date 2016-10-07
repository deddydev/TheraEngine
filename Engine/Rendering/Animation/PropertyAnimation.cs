using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Animation
{
    public abstract class PropertyAnimation<T> : ObjectBase, IPropertyAnimation where T : Keyframe
    {
        protected KeyframeTrack<T> _keyframes;
        private int _frameCount;

        public PropertyAnimation() { _keyframes = new KeyframeTrack<T>(this); }

        [Category("Property Animation"), Browsable(true), PostChanged("FrameCountUpdated")]
        public int FrameCount
        {
            get { return _frameCount; }
            set { _frameCount = value; }
        }
        [Category("Property Animation"), Browsable(true)]
        public IKeyframeTrack Keyframes { get { return _keyframes; } }

        private void FrameCountUpdated(int oldCount) { }
        public abstract void Resize(int newSize);
        public abstract void Stretch(int newSize);
        public abstract void Append(PropertyAnimation<T> other);
        public abstract void Bake();
    }
}

using System;
using System.Reflection;

namespace CustomEngine.Rendering.Animation
{
    public abstract class PropertyAnimation<T> : ObjectBase, INameable where T : Keyframe
    {
        protected KeyframeTrack<T> _keyframes;

        public PropertyAnimation() { _keyframes = new KeyframeTrack<T>(this); }

        private int _frameCount;
        [Event("FrameCountUpdated")]
        public int FrameCount
        {
            get { return _frameCount; }
            set
            {
                int oldCount = _frameCount;
                _frameCount = value;
                FrameCountUpdated(oldCount);
            }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        private void FrameCountUpdated(int oldCount) { }
        public abstract void Resize(int newSize);
        public abstract void Stretch(int newSize);
        public abstract void Append(PropertyAnimation<T> other);
        public abstract void Bake();
    }
}

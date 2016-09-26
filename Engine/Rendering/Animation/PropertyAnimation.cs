using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using CustomEngine.Components;

namespace CustomEngine.Rendering.Animation
{
    public abstract class PropertyAnimation<T> : ObjectBase where T : Keyframe
    {
        protected KeyframeTrack<T> _keyframes;

        public PropertyAnimation() { _keyframes = new KeyframeTrack<T>(this); }

        private int _frameCount;
        public int FrameCount
        {
            get { return _frameCount; }
            set
            {
                int oldCount = _frameCount;
                _frameCount = value;
                FrameCountUpdated(oldCount);
                Changed(MethodBase.GetCurrentMethod());
            }
        }

        private void FrameCountUpdated(int oldCount) { }
        public abstract void Resize(int newSize);
        public abstract void Stretch(int newSize);
        public abstract void Append(PropertyAnimation<T> other);
        public abstract void Bake();
    }
}

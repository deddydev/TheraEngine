﻿using CustomEngine.Files;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Animation
{
    public abstract class BasePropertyAnimation : FileObject
    {
        public event EventHandler AnimationStarted;
        public event EventHandler AnimationEnded;

        protected int _frameCount;
        protected float _currentFrame;
        protected bool _looped;
        protected bool _isPlaying;
        protected bool _useKeyframes;

        [Category("Property Animation"), Default, PostChanged("UseKeyframesChanged")]
        public bool UseKeyframes
        {
            get { return _useKeyframes; }
            set { _useKeyframes = value; }
        }
        [Category("Property Animation"), Default]
        public int FrameCount { get { return _frameCount; } }
        [Category("Property Animation"), Default]
        public bool Looped
        {
            get { return _looped; }
            set { _looped = value; }
        }
        [Category("Property Animation"), State]
        public float CurrentFrame
        {
            get { return _currentFrame; }
            set { _currentFrame = value; }
        }
        [Category("Property Animation"), State]
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                if (value)
                    Start();
                else
                    Stop();
            }
        }
        public void Start()
        {
            if (_isPlaying)
                return;
            _currentFrame = 0.0f;
            _isPlaying = true;
            AnimationStarted?.Invoke(this, null);
        }
        public void Stop()
        {
            if (!_isPlaying)
                return;
            _isPlaying = false;
            AnimationEnded?.Invoke(this, null);
        }
        public void Tick(object obj, PropertyInfo property, float delta)
        {
            if (!_isPlaying)
                return;
            
            property.SetValue(obj, GetValue(_currentFrame));

            _currentFrame += delta;
            if (_currentFrame > _frameCount - 1)
                if (_looped)
                    _currentFrame = _currentFrame.RemapToRange(0.0f, _frameCount);
                else
                {
                    Stop();
                    return;
                }
        }
        protected abstract void UseKeyframesChanged(bool oldUseKeyframes);
        protected abstract object GetValue(float frame);
        
        [Category("Property Animation"), Default]
        protected abstract BaseKeyframeTrack InternalKeyframes { get; }
        public abstract void Resize(int newSize);
        public abstract void Stretch(int newSize);
        public abstract void Bake();
    }
    public abstract class PropertyAnimation<T> : BasePropertyAnimation where T : Keyframe
    {
        protected KeyframeTrack<T> _keyframes;

        public PropertyAnimation(int frameCount)
        {
            _frameCount = frameCount;
            _keyframes = new KeyframeTrack<T>(this);
        }

        [Category("Property Animation"), Default]
        protected override BaseKeyframeTrack InternalKeyframes { get { return _keyframes; } }
        [Category("Property Animation"), Browsable(true), Default]
        public KeyframeTrack<T> Keyframes { get { return _keyframes; } }

        public abstract void Append(PropertyAnimation<T> other);
    }
}

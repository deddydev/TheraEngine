using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEngine.Animation
{
    delegate Matrix4 Matrix4GetValue(float frameIndex);
    public class PropAnimMatrix4 : PropertyAnimation<Matrix4Keyframe>, IEnumerable<Matrix4Keyframe>
    {
        private Matrix4 _defaultValue = Matrix4.Identity;
        private Matrix4[] _baked;
        private Matrix4GetValue _getValue;
        
        [Serialize]
        public Matrix4 DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }
        
        public PropAnimMatrix4(int frameCount, bool looped, bool useKeyframes) 
            : base(frameCount, looped, useKeyframes) { }

        protected override void UseKeyframesChanged()
        {
            if (_useKeyframes)
                _getValue = GetValueKeyframed;
            else
                _getValue = GetValueBaked;
        }
        protected override object GetValue(float frame)
            => _getValue(frame);
        public Matrix4 GetValueBaked(float frameIndex)
            => _baked[(int)(frameIndex / Engine.TargetUpdateFreq * FramesPerSecond)];
        public Matrix4 GetValueKeyframed(float frameIndex)
            => _keyframes.KeyCount == 0 ? _defaultValue : _keyframes.First.Interpolate(frameIndex);

        /// <summary>
        /// Bakes the interpolated data for fastest access by the game.
        /// However, this method takes up more space and does not support time dilation (speeding up and slowing down with proper in-betweens)
        /// </summary>
        public override void Bake()
        {
            _baked = new Matrix4[FrameCount];
            for (int i = 0; i < FrameCount; ++i)
                _baked[i] = GetValueKeyframed(i);
        }
        public override void Resize(int newSize)
        {
            throw new NotImplementedException();
        }
        public override void Stretch(int newSize)
        {
            throw new NotImplementedException();
        }
        public override void Append(PropertyAnimation<Matrix4Keyframe> other)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Matrix4Keyframe> GetEnumerator() { return ((IEnumerable<Matrix4Keyframe>)_keyframes).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<Matrix4Keyframe>)_keyframes).GetEnumerator(); }
    }
    public class Matrix4Keyframe : Keyframe
    {
        public Matrix4Keyframe(float frameIndex, Matrix4 outValue) : base()
        {
            _frameIndex = frameIndex;
            _outValue = outValue;
        }

        protected delegate Matrix4 DelInterpolate(Matrix4Keyframe key1, Matrix4Keyframe key2, float time);
        
        protected Matrix4 _outValue;
        
        public Matrix4 OutValue
        {
            get => _outValue;
            set => _outValue = value;
        }
        public new Matrix4Keyframe Next
        {
            get => _next as Matrix4Keyframe;
            set => _next = value;
        }
        public new Matrix4Keyframe Prev
        {
            get => _prev as Matrix4Keyframe;
            set => _prev = value;
        }
        public Matrix4 Interpolate(float frameIndex)
        {
            if (_prev == this || _next == this)
                return _outValue;

            if (frameIndex < _frameIndex && _prev._frameIndex > _frameIndex)
                return Prev.Interpolate(frameIndex);

            if (frameIndex > _next._frameIndex && _next._frameIndex > _frameIndex)
                return Next.Interpolate(frameIndex);

            //float t = (frameIndex - _frameIndex) / (_next._frameIndex - _frameIndex);

            return _outValue;

            //return _interpolate(this, Next, t);
        }
    }
}

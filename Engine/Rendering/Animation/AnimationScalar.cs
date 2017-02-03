using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace CustomEngine.Rendering.Animation
{
    delegate float ScalarGetValue(float frameIndex);
    public class AnimationScalar : PropertyAnimation<ScalarKeyframe>, IEnumerable<ScalarKeyframe>
    {
        public override ResourceType ResourceType { get { return ResourceType.AnimationScalar; } }

        float[] _baked;
        ScalarGetValue _getValue;

        public AnimationScalar(int frameCount, bool looped, bool useKeyframes) 
            : base(frameCount, looped, useKeyframes) { }

        protected override object GetValue(float frame) { return _getValue(frame); }
        protected override void UseKeyframesChanged()
        {
            if (_useKeyframes)
                _getValue = GetValueKeyframed;
            else
                _getValue = GetValueBaked;
        }
        public float GetValueBaked(float frameIndex)
        {
            return _baked[(int)frameIndex];
        }
        public float GetValueKeyframed(float frameIndex)
        {
            ScalarKeyframe key = _keyframes.GetKeyBefore(frameIndex);
            if (key != null)
                return key.Interpolate(frameIndex);
            throw new IndexOutOfRangeException("Invalid frame index.");
        }
        /// <summary>
        /// Bakes the interpolated data for fastest access by the game.
        /// </summary>
        public override void Bake()
        {
            _baked = new float[FrameCount];
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
        public override void Append(PropertyAnimation<ScalarKeyframe> other)
        {
            throw new NotImplementedException();
        }
        public IEnumerator<ScalarKeyframe> GetEnumerator() { return ((IEnumerable<ScalarKeyframe>)_keyframes).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<ScalarKeyframe>)_keyframes).GetEnumerator(); }
    }
    public class ScalarKeyframe : Keyframe
    {
        public ScalarKeyframe(float frameIndex, float inValue, float outValue) : base()
        {
            _frameIndex = frameIndex;
            _inValue = inValue;
            _outValue = outValue;
        }

        protected float _inValue;
        protected float _inTangent;

        protected float _outValue;
        protected float _outTangent;

        public float InValue { get { return _inValue; } set { _inValue = value; } }
        public float OutValue { get { return _outValue; } set { _outValue = value; } }

        public float InTanget { get { return _inTangent; } set { _inTangent = value; } }
        public float OutTangent { get { return _outTangent; } set { _outTangent = value; } }

        public new ScalarKeyframe Next { get { return _next as ScalarKeyframe; } set { _next = value; } }
        public new ScalarKeyframe Prev { get { return _prev as ScalarKeyframe; } set { _prev = value; } }

        delegate float DelInterpolate(float t, float p0, float p1, float t0, float t1);
        private DelInterpolate _interpolate = CubicHermite;
        public float Interpolate(float frameIndex)
        {
            if (frameIndex < _frameIndex)
            {
                if (_prev == this)
                    return _inValue;

                return Prev.Interpolate(frameIndex);
            }

            if (_next == this)
                return _outValue;

            if (frameIndex > _next._frameIndex)
                return Next.Interpolate(frameIndex);

            float t = (frameIndex - _frameIndex) / (_next._frameIndex - _frameIndex);
            return _interpolate(t, _outValue, Next._inValue, _outTangent, Next._inTangent);
        }
        
        public static float CubicHermite(float time, float p0, float p1, float t0, float t1)
        {
            float time2 = time * time;
            float time3 = time2 * time;
            return 
                p0 * (2.0f * time3 - 3.0f * time2 + 1.0f) +
                t0 * (time3 - 2.0f * time2 + time) +
                p1 * (-2.0f * time3 + 3.0f * time2) +
                t1 * (time3 - time2);
        }

        public void AverageKeyframe()
        {
            AverageValues();
            AverageTangents();
        }
        public void AverageTangents()
        {
            _inTangent = _outTangent = (_inTangent + _outTangent) / 2.0f;
        }
        public void AverageValues()
        {
            _inValue = _outValue = (_inValue + _outValue) / 2.0f;
        }
        public void MakeOutLinear()
        {
            _outTangent = (Next.InValue - OutValue) / (Next._frameIndex - _frameIndex);
        }
        public void MakeInLinear()
        {
            _inTangent = (InValue - Prev.OutValue) / (_frameIndex - Prev._frameIndex);
        }
    }
}

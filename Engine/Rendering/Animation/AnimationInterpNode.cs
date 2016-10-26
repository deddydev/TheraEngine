using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace CustomEngine.Rendering.Animation
{
    delegate float InterpGetValue(float frameIndex);
    public class AnimationInterpNode : PropertyAnimation<InterpKeyframe>, IEnumerable<InterpKeyframe>
    {
        float[] _baked;
        InterpGetValue _getValue;

        public AnimationInterpNode(int frameCount) : base(frameCount) { }

        protected override object GetValue(float frame) { return _getValue(frame); }
        protected override void UseKeyframesChanged(bool oldUseKeyframes)
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
            InterpKeyframe key = _keyframes.GetKeyBefore(frameIndex);
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
        public override void Append(PropertyAnimation<InterpKeyframe> other)
        {
            throw new NotImplementedException();
        }
        public IEnumerator<InterpKeyframe> GetEnumerator()
        {
            return ((IEnumerable<InterpKeyframe>)_keyframes).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<InterpKeyframe>)_keyframes).GetEnumerator();
        }
    }
    public class InterpKeyframe : Keyframe
    {
        public InterpKeyframe(float frameIndex, float inValue, float outValue)
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

        public new InterpKeyframe Next { get { return _next as InterpKeyframe; } set { _next = value; } }
        public new InterpKeyframe Prev { get { return _prev as InterpKeyframe; } set { _prev = value; } }

        public float Interpolate(float frameIndex)
        {
            if (frameIndex < _frameIndex)
                return Prev.Interpolate(frameIndex);
            if (frameIndex > _next._frameIndex)
                return Next.Interpolate(frameIndex);

            float t = (frameIndex - _frameIndex) / (_next._frameIndex - _frameIndex);
            float t2 = t * t;
            float t3 = t2 * t;
            return (2.0f * t3 - 3.0f * t2 + 1.0f) * _outValue +
                (t3 - 2.0f * t2 + t) * _outTangent +
                (-2.0f * t3 + 3.0f * t2) * Next._inValue +
                (t3 - t2) * Next._inTangent;
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

using System;
using System.Collections;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Animation
{
    public delegate float FloatGetValue(float frameIndex);
    public class PropAnimFloat : PropertyAnimation<FloatKeyframe>, IEnumerable<FloatKeyframe>
    {
        private float _defaultValue = 0.0f;
        private float[] _baked;
        private FloatGetValue _getValue;

        public float DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        public PropAnimFloat(int frameCount, bool looped, bool useKeyframes) 
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
        public float GetValueBaked(float frameIndex)
            => _baked[(int)(frameIndex / Engine.TargetUpdateFreq * FramesPerSecond)];
        public float GetValueKeyframed(float frameIndex)
            => _keyframes.KeyCount == 0 ? _defaultValue : _keyframes.First.Interpolate(frameIndex);

        /// <summary>
        /// Bakes the interpolated data for fastest access by the game.
        /// </summary>
        public override void Bake()
        {
            float oneOverFPS = 1.0f / _keyframes.FramesPerSecond;
            int totalFrames = FrameCount;
            _baked = new float[totalFrames];
            for (int i = 0; i < totalFrames; ++i)
                _baked[i] = GetValueKeyframed(i * oneOverFPS);
        }
        public override void Resize(int newSize)
        {
            throw new NotImplementedException();
        }
        public override void Stretch(int newSize)
        {
            throw new NotImplementedException();
        }
        public override void Append(PropertyAnimation<FloatKeyframe> other)
        {
            throw new NotImplementedException();
        }
        public IEnumerator<FloatKeyframe> GetEnumerator() => ((IEnumerable<FloatKeyframe>)_keyframes).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<FloatKeyframe>)_keyframes).GetEnumerator();
    }
    public class FloatKeyframe : Keyframe
    {
        public FloatKeyframe(float frameIndex, float inoutValue, PlanarInterpType type) : base()
        {
            _frameIndex = frameIndex;
            _inValue = _outValue = inoutValue;
            InterpolationType = type;
        }
        public FloatKeyframe(float frameIndex, float inValue, float outValue, PlanarInterpType type) : base()
        {
            _frameIndex = frameIndex;
            _inValue = inValue;
            _outValue = outValue;
            InterpolationType = type;
        }

        delegate float DelInterpolate(FloatKeyframe key1, FloatKeyframe key2, float time);
        private DelInterpolate _interpolate = CubicHermite;
        protected PlanarInterpType _interpolationType;
        protected float _inValue;
        protected float _inTangent;
        protected float _outValue;
        protected float _outTangent;

        public float InValue
        {
            get => _inValue;
            set => _inValue = value;
        }
        public float OutValue
        {
            get => _outValue;
            set => _outValue = value;
        }
        public float InTangent
        {
            get => _inTangent;
            set => _inTangent = value;
        }
        public float OutTangent
        {
            get => _outTangent;
            set => _outTangent = value;
        }
        public new FloatKeyframe Next
        {
            get => _next as FloatKeyframe;
            set => _next = value;
        }
        public new FloatKeyframe Prev
        {
            get => _prev as FloatKeyframe;
            set => _prev = value;
        }
        public PlanarInterpType InterpolationType
        {
            get => _interpolationType;
            set
            {
                _interpolationType = value;
                switch (_interpolationType)
                {
                    case PlanarInterpType.Step:
                        _interpolate = Step;
                        break;
                    case PlanarInterpType.Linear:
                        _interpolate = Linear;
                        break;
                    case PlanarInterpType.CubicHermite:
                        _interpolate = CubicHermite;
                        break;
                    case PlanarInterpType.CubicBezier:
                        _interpolate = CubicBezier;
                        break;
                }
            }
        }
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
            return _interpolate(this, Next, t);
        }
        public static float Step(FloatKeyframe key1, FloatKeyframe key2, float time)
            => time < 1.0f ? key1.OutValue : key2.OutValue;
        public static float Linear(FloatKeyframe key1, FloatKeyframe key2, float time)
            => CustomMath.Lerp(key1.OutValue, key2.InValue, time);
        public static float CubicBezier(FloatKeyframe key1, FloatKeyframe key2, float time)
            => CustomMath.CubicBezier(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);
        public static float CubicHermite(FloatKeyframe key1, FloatKeyframe key2, float time)
            => CustomMath.CubicHermite(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);
        public void AverageKeyframe()
        {
            AverageValues();
            AverageTangents();
        }
        public void AverageTangents()
            => _inTangent = _outTangent = (_inTangent + _outTangent) / 2.0f;
        public void AverageValues()
            => _inValue = _outValue = (_inValue + _outValue) / 2.0f;
        public void MakeOutLinear()
            => _outTangent = (Next.InValue - OutValue) / (Next._frameIndex - _frameIndex);
        public void MakeInLinear()
            => _inTangent = (InValue - Prev.OutValue) / (_frameIndex - Prev._frameIndex);
    }
}

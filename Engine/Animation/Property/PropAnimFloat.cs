using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEngine.Animation
{
    public class PropAnimFloat : PropertyAnimation<FloatKeyframe>, IEnumerable<FloatKeyframe>
    {
        private float _defaultValue = 0.0f;
        private GetValue<float> _getValue;

        [Serialize(Condition = "!UseKeyframes")]
        private float[] _baked = null;

        [Serialize(Condition = "UseKeyframes")]
        public float DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }
        
        public PropAnimFloat(float lengthInSeconds, bool looped, bool useKeyframes)
            : base(lengthInSeconds, looped, useKeyframes) { }
        public PropAnimFloat(int frameCount, float FPS, bool looped, bool useKeyframes) 
            : base(frameCount, FPS, looped, useKeyframes) { }

        protected override void UseKeyframesChanged()
            => _getValue = _useKeyframes ? (GetValue<float>)GetValueKeyframed : GetValueBaked;
        protected override object GetValue(float second)
            => _getValue(second);
        public float GetValueBaked(float second)
            => _baked[(int)Math.Floor(second * BakedFramesPerSecond)];
        public float GetValueBaked(int frameIndex)
            => _baked[frameIndex];
        public float GetValueKeyframed(float second)
            => _keyframes.KeyCount == 0 ? _defaultValue : _keyframes.First.Interpolate(second);

        /// <summary>
        /// Bakes the interpolated data for fastest access by the game.
        /// </summary>
        public override void Bake(float framesPerSecond)
        {
            _bakedFPS = framesPerSecond;
            _bakedFrameCount = (int)Math.Ceiling(LengthInSeconds * framesPerSecond);
            _baked = new float[BakedFrameCount];
            for (int i = 0; i < BakedFrameCount; ++i)
                _baked[i] = GetValueKeyframed(i);
        }
        public IEnumerator<FloatKeyframe> GetEnumerator() => ((IEnumerable<FloatKeyframe>)_keyframes).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<FloatKeyframe>)_keyframes).GetEnumerator();
    }
    public class FloatKeyframe : Keyframe
    {
        public FloatKeyframe(int frameIndex, float FPS, float inValue, float outValue, float inTangent, float outTangent, PlanarInterpType type)
            : this(frameIndex / FPS, inValue, outValue, inTangent, outTangent, type) { }
        public FloatKeyframe(int frameIndex, float FPS, float inoutValue, float inoutTangent, PlanarInterpType type)
            : this(frameIndex / FPS, inoutValue, inoutValue, inoutTangent, inoutTangent, type) { }
        public FloatKeyframe(float second, float inoutValue, float inoutTangent, PlanarInterpType type)
            : this(second, inoutValue, inoutValue, inoutTangent, inoutTangent, type) { }
        public FloatKeyframe(float second, float inValue, float outValue, float inTangent, float outTangent, PlanarInterpType type) : base()
        {
            Second = second;
            _inValue = inValue;
            _outValue = outValue;
            _inTangent = inTangent;
            _outTangent = outTangent;
            InterpolationType = type;
        }

        delegate float DelInterpolate(FloatKeyframe key1, FloatKeyframe key2, float time);
        private DelInterpolate _interpolate = CubicHermite;
        protected PlanarInterpType _interpolationType;
        protected float _inValue;
        protected float _inTangent;
        protected float _outValue;
        protected float _outTangent;

        [Serialize(IsXmlAttribute = true)]
        public float InValue
        {
            get => _inValue;
            set => _inValue = value;
        }
        [Serialize(IsXmlAttribute = true)]
        public float OutValue
        {
            get => _outValue;
            set => _outValue = value;
        }
        [Serialize(IsXmlAttribute = true)]
        public float InTangent
        {
            get => _inTangent;
            set => _inTangent = value;
        }
        [Serialize(IsXmlAttribute = true)]
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
        [Serialize(IsXmlAttribute = true)]
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
        public float Interpolate(float desiredSecond)
        {
            if (desiredSecond < Second)
            {
                if (_prev == this)
                    return _inValue;

                return Prev.Interpolate(desiredSecond);
            }

            if (desiredSecond > _next.Second)
            {
                if (_next == this)
                    return _outValue;

                return Next.Interpolate(desiredSecond);
            }

            float span = _next.Second - Second;
            float diff = desiredSecond - Second;
            float time = diff / span;
            return _interpolate(this, Next, time);
        }

        public static float Step(FloatKeyframe key1, FloatKeyframe key2, float time)
            => time < 1.0f ? key1.OutValue : key2.OutValue;
        public static float Linear(FloatKeyframe key1, FloatKeyframe key2, float time)
            => CustomMath.Lerp(key1.OutValue, key2.InValue, time);
        public static float CubicBezier(FloatKeyframe key1, FloatKeyframe key2, float time)
            => CustomMath.CubicBezier(key1.OutValue, key1.OutValue + key1.OutTangent, key2.InValue + key2.InTangent, key2.InValue, time);
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
            => _outTangent = (Next.InValue - OutValue) / (Next.Second - Second);
        public void MakeInLinear()
            => _inTangent = (InValue - Prev.OutValue) / (Second - Prev.Second);

        public override string WriteToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5}", Second, InValue, OutValue, InTangent, OutTangent, InterpolationType);
        }

        public override void ReadFromString(string str)
        {
            string[] parts = str.Split(' ');
            Second = float.Parse(parts[0]);
            InValue = float.Parse(parts[1]);
            OutValue = float.Parse(parts[2]);
            InTangent = float.Parse(parts[3]);
            OutTangent = float.Parse(parts[4]);
            InterpolationType = parts[5].AsEnum<PlanarInterpType>();
        }
    }
}

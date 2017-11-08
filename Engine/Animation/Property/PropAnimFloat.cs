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

        [TSerialize(Condition = "!UseKeyframes")]
        private float[] _baked = null;

        [TSerialize(Condition = "UseKeyframes")]
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
        
        public override void Bake(float framesPerSecond)
        {
            _bakedFPS = framesPerSecond;
            _bakedFrameCount = (int)Math.Ceiling(LengthInSeconds * framesPerSecond);
            _baked = new float[BakedFrameCount];
            for (int i = 0; i < BakedFrameCount; ++i)
                _baked[i] = GetValueKeyframed(i);
        }

        public IEnumerator<FloatKeyframe> GetEnumerator()
            => ((IEnumerable<FloatKeyframe>)_keyframes).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<FloatKeyframe>)_keyframes).GetEnumerator();
    }
    public class FloatKeyframe : Keyframe
    {
        public FloatKeyframe(int frameIndex, float FPS, float inValue, float outValue, float inTangent, float outTangent, PlanarInterpType type)
            : this(frameIndex / FPS, inValue, outValue, inTangent, outTangent, type) { }
        public FloatKeyframe(int frameIndex, float FPS, float inoutValue, float inoutTangent, PlanarInterpType type)
            : this(frameIndex / FPS, inoutValue, inoutValue, inoutTangent, inoutTangent, type) { }
        public FloatKeyframe(float second, float inoutValue, float inoutTangent, PlanarInterpType type)
            : this(second, inoutValue, inoutValue, inoutTangent, inoutTangent, type) { }
        public FloatKeyframe(float second, float inoutValue, float inTangent, float outTangent, PlanarInterpType type)
            : this(second, inoutValue, inoutValue, inTangent, outTangent, type) { }
        public FloatKeyframe(float second, float inValue, float outValue, float inTangent, float outTangent, PlanarInterpType type) : base()
        {
            Second = second;
            InValue = inValue;
            OutValue = outValue;
            InTangent = inTangent;
            OutTangent = outTangent;
            InterpolationType = type;
        }

        private delegate float DelInterpolate(FloatKeyframe key1, FloatKeyframe key2, float time);
        private DelInterpolate _interpolate = CubicHermite;
        protected PlanarInterpType _interpolationType;

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public float InValue { get; set; }
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public float OutValue { get; set; }

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public float InTangent { get; set; }
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public float OutTangent { get; set; }

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

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
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
            //First, check if the desired second is between this key and the next key.
            if (desiredSecond < Second)
            {
                //If the previous key's second is greater than this second, this key must be the first key. 
                //Return the InValue as the desired second comes before this one.
                //Otherwise, move to the previous key to calculate the interpolated value.
                return _prev.Second < Second ? Prev.Interpolate(desiredSecond) : InValue;
            }
            else if (desiredSecond > _next.Second)
            {
                //If the next key's second is less than this second, this key must be the last key. 
                //Return the OutValue as the desired second comes after this one.
                //Otherwise, move to the previous key to calculate the interpolated value.
                return _next.Second > Second ? Next.Interpolate(desiredSecond) : OutValue;
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
            => InTangent = OutTangent = (InTangent + OutTangent) / 2.0f;
        public void AverageValues()
            => InValue = OutValue = (InValue + OutValue) / 2.0f;
        public void MakeOutLinear()
            => OutTangent = (Next.InValue - OutValue) / (Next.Second - Second);
        public void MakeInLinear()
            => InTangent = (InValue - Prev.OutValue) / (Second - Prev.Second);

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

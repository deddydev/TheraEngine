using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEngine.Animation
{
    public class PropAnimVec2 : PropertyAnimation<Vec2Keyframe>, IEnumerable<Vec2Keyframe>
    {
        private Vec2 _defaultValue = Vec2.Zero;
        private GetValue<Vec2> _getValue;

        [TSerialize(Condition = "!UseKeyframes")]
        private Vec2[] _baked = null;

        [TSerialize(Condition = "UseKeyframes")]
        public Vec2 DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        public PropAnimVec2() : base(0.0f, false, true) { }
        public PropAnimVec2(float lengthInSeconds, bool looped, bool useKeyframes)
            : base(lengthInSeconds, looped, useKeyframes) { }
        public PropAnimVec2(int frameCount, float FPS, bool looped, bool useKeyframes) 
            : base(frameCount, FPS, looped, useKeyframes) { }

        protected override void UseKeyframesChanged()
            => _getValue = _useKeyframes ? (GetValue<Vec2>)GetValueKeyframed : GetValueBaked;

        public Vec2 GetValue(float second)
            => _getValue(second);
        protected override object GetValueGeneric(float second)
            => _getValue(second);
        public Vec2 GetValueBaked(float second)
            => _baked[(int)Math.Floor(second * BakedFramesPerSecond)];
        public Vec2 GetValueBaked(int frameIndex)
            => _baked[frameIndex];
        public Vec2 GetValueKeyframed(float second)
            => _keyframes.Count == 0 ? _defaultValue : _keyframes.First.Interpolate(second);

        public override void Bake(float framesPerSecond)
        {
            _bakedFPS = framesPerSecond;
            _bakedFrameCount = (int)Math.Ceiling(LengthInSeconds * framesPerSecond);
            _baked = new Vec2[BakedFrameCount];
            for (int i = 0; i < BakedFrameCount; ++i)
                _baked[i] = GetValueKeyframed(i);
        }

        public IEnumerator<Vec2Keyframe> GetEnumerator()
            => ((IEnumerable<Vec2Keyframe>)_keyframes).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<Vec2Keyframe>)_keyframes).GetEnumerator();
    }
    public class Vec2Keyframe : Keyframe
    {
        public Vec2Keyframe(int frameIndex, float FPS, Vec2 inValue, Vec2 outValue, Vec2 inTangent, Vec2 outTangent, PlanarInterpType type)
            : this(frameIndex / FPS, inValue, outValue, inTangent, outTangent, type) { }
        public Vec2Keyframe(int frameIndex, float FPS, Vec2 inoutValue, Vec2 inoutTangent, PlanarInterpType type)
            : this(frameIndex / FPS, inoutValue, inoutValue, inoutTangent, inoutTangent, type) { }
        public Vec2Keyframe(float second, Vec2 inoutValue, Vec2 inoutTangent, PlanarInterpType type)
            : this(second, inoutValue, inoutValue, inoutTangent, inoutTangent, type) { }
        public Vec2Keyframe(float second, Vec2 inValue, Vec2 outValue, Vec2 inTangent, Vec2 outTangent, PlanarInterpType type) : base()
        {
            Second = second;
            InValue = inValue;
            OutValue = outValue;
            InTangent = inTangent;
            OutTangent = outTangent;
            InterpolationType = type;
        }

        protected PlanarInterpType _interpolationType;

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public Vec2 InValue { get; set; }
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public Vec2 OutValue { get; set; }

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public Vec2 InTangent { get; set; }
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public Vec2 OutTangent { get; set; }

        public new Vec2Keyframe Next
        {
            get => _next as Vec2Keyframe;
            set => _next = value;
        }
        public new Vec2Keyframe Prev
        {
            get => _prev as Vec2Keyframe;
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
                        _interpolate = Lerp;
                        break;
                    case PlanarInterpType.CubicHermite:
                        _interpolate = CubicHermite;
                        break;
                    case PlanarInterpType.CubicBezier:
                        _interpolate = Bezier;
                        break;
                }
            }
        }

        private delegate Vec2 DelInterpolate(Vec2Keyframe key1, Vec2Keyframe key2, float time);
        private DelInterpolate _interpolate = CubicHermite;
        public Vec2 Interpolate(float desiredSecond)
        {
            if (desiredSecond < Second)
            {
                if (_prev == this)
                    return InValue;

                return Prev.Interpolate(desiredSecond);
            }

            if (desiredSecond > _next.Second)
            {
                if (_next == this)
                    return OutValue;

                return Next.Interpolate(desiredSecond);
            }

            float span = _next.Second - Second;
            float diff = desiredSecond - Second;
            float time = diff / span;
            return _interpolate(this, Next, time);
        }
        public static Vec2 Step(Vec2Keyframe key1, Vec2Keyframe key2, float time)
            => time < 1.0f ? key1.OutValue : key2.OutValue;
        public static Vec2 Lerp(Vec2Keyframe key1, Vec2Keyframe key2, float time)
            => Vec2.Lerp(key1.OutValue, key2.InValue, time);
        public static Vec2 Bezier(Vec2Keyframe key1, Vec2Keyframe key2, float time)
            => Interp.CubicBezier(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);
        public static Vec2 CubicHermite(Vec2Keyframe key1, Vec2Keyframe key2, float time)
            => Interp.CubicHermite(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);

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
            return string.Format("{0} {1} {2} {3} {4} {5}", Second, InValue.WriteToString(), OutValue.WriteToString(), InTangent.WriteToString(), OutTangent.WriteToString(), InterpolationType);
        }

        public override void ReadFromString(string str)
        {
            string[] parts = str.Split(' ');
            Second = float.Parse(parts[0]);
            InValue = new Vec2(float.Parse(parts[1]), float.Parse(parts[2]));
            OutValue = new Vec2(float.Parse(parts[3]), float.Parse(parts[4]));
            InTangent = new Vec2(float.Parse(parts[5]), float.Parse(parts[6]));
            OutTangent = new Vec2(float.Parse(parts[7]), float.Parse(parts[8]));
            InterpolationType = parts[9].AsEnum<PlanarInterpType>();
        }
    }
}

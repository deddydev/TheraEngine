using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEngine.Animation
{
    public class PropAnimVec4 : PropAnimKeyframed<Vec4Keyframe>, IEnumerable<Vec4Keyframe>
    {
        private DelGetValue<Vec4> _getValue;

        [TSerialize(Condition = "Baked")]
        private Vec4[] _baked = null;
        /// <summary>
        /// The default value to return when no keyframes are set.
        /// </summary>
        [TSerialize(Condition = "!Baked")]
        public Vec4 DefaultValue { get; set; } = Vec4.Zero;

        public PropAnimVec4() : base(0.0f, false) { }
        public PropAnimVec4(float lengthInSeconds, bool looped, bool useKeyframes)
            : base(lengthInSeconds, looped, useKeyframes) { }
        public PropAnimVec4(int frameCount, float FPS, bool looped, bool useKeyframes) 
            : base(frameCount, FPS, looped, useKeyframes) { }

        protected override void BakedChanged()
            => _getValue = !Baked ? (DelGetValue<Vec4>)GetValueKeyframed : GetValueBaked;

        public Vec4 GetValue(float second)
            => _getValue(second);
        protected override object GetValueGeneric(float second)
            => _getValue(second);
        public Vec4 GetValueBaked(float second)
            => _baked[(int)Math.Floor(second * BakedFramesPerSecond)];
        public Vec4 GetValueBaked(int frameIndex)
            => _baked[frameIndex];
        public Vec4 GetValueKeyframed(float second)
            => _keyframes.Count == 0 ? DefaultValue : _keyframes.First.Interpolate(second);

        public override void Bake(float framesPerSecond)
        {
            _bakedFPS = framesPerSecond;
            _bakedFrameCount = (int)Math.Ceiling(LengthInSeconds * framesPerSecond);
            _baked = new Vec4[BakedFrameCount];
            for (int i = 0; i < BakedFrameCount; ++i)
                _baked[i] = GetValueKeyframed(i);
        }

        public IEnumerator<Vec4Keyframe> GetEnumerator()
            => ((IEnumerable<Vec4Keyframe>)_keyframes).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<Vec4Keyframe>)_keyframes).GetEnumerator();
    }
    public class Vec4Keyframe : Keyframe
    {
        public Vec4Keyframe(int frameIndex, float FPS, Vec4 inValue, Vec4 outValue, Vec4 inTangent, Vec4 outTangent, PlanarInterpType type)
            : this(frameIndex / FPS, inValue, outValue, inTangent, outTangent, type) { }
        public Vec4Keyframe(int frameIndex, float FPS, Vec4 inoutValue, Vec4 inoutTangent, PlanarInterpType type)
            : this(frameIndex / FPS, inoutValue, inoutValue, inoutTangent, inoutTangent, type) { }
        public Vec4Keyframe(float second, Vec4 inoutValue, Vec4 inoutTangent, PlanarInterpType type)
            : this(second, inoutValue, inoutValue, inoutTangent, inoutTangent, type) { }
        public Vec4Keyframe(float second, Vec4 inValue, Vec4 outValue, Vec4 inTangent, Vec4 outTangent, PlanarInterpType type) : base()
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
        public Vec4 InValue { get; set; }
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public Vec4 OutValue { get; set; }

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public Vec4 InTangent { get; set; }
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public Vec4 OutTangent { get; set; }

        public new Vec4Keyframe Next
        {
            get => _next as Vec4Keyframe;
            set => _next = value;
        }
        public new Vec4Keyframe Prev
        {
            get => _prev as Vec4Keyframe;
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

        private delegate Vec4 DelInterpolate(Vec4Keyframe key1, Vec4Keyframe key2, float time);
        private DelInterpolate _interpolate = CubicHermite;
        public Vec4 Interpolate(float desiredSecond)
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
        public static Vec4 Step(Vec4Keyframe key1, Vec4Keyframe key2, float time)
            => time < 1.0f ? key1.OutValue : key2.OutValue;
        public static Vec4 Lerp(Vec4Keyframe key1, Vec4Keyframe key2, float time)
            => Vec4.Lerp(key1.OutValue, key2.InValue, time);
        public static Vec4 Bezier(Vec4Keyframe key1, Vec4Keyframe key2, float time)
            => Interp.CubicBezier(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);
        public static Vec4 CubicHermite(Vec4Keyframe key1, Vec4Keyframe key2, float time)
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
            InValue = new Vec4(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]), float.Parse(parts[4]));
            OutValue = new Vec4(float.Parse(parts[5]), float.Parse(parts[6]), float.Parse(parts[7]), float.Parse(parts[8]));
            InTangent = new Vec4(float.Parse(parts[9]), float.Parse(parts[10]), float.Parse(parts[11]), float.Parse(parts[12]));
            OutTangent = new Vec4(float.Parse(parts[13]), float.Parse(parts[14]), float.Parse(parts[15]), float.Parse(parts[16]));
            InterpolationType = parts[17].AsEnum<PlanarInterpType>();
        }
    }
}

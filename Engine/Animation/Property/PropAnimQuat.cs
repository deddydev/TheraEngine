using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEngine.Animation
{
    public class PropAnimQuat : PropAnimKeyframed<QuatKeyframe>, IEnumerable<QuatKeyframe>
    {
        private DelGetValue<Quat> _getValue;

        [TSerialize(Condition = "Baked")]
        private Quat[] _baked = null;
        /// <summary>
        /// The default value to return when no keyframes are set.
        /// </summary>
        [Category(PropAnimCategory)]
        [TSerialize(Condition = "!Baked")]
        public Quat DefaultValue { get; set; } = Quat.Identity;

        public PropAnimQuat() : base(0.0f, false) { }
        public PropAnimQuat(float lengthInSeconds, bool looped, bool useKeyframes)
            : base(lengthInSeconds, looped, useKeyframes) { }
        public PropAnimQuat(int frameCount, float FPS, bool looped, bool useKeyframes) 
            : base(frameCount, FPS, looped, useKeyframes) { }

        protected override void BakedChanged()
            => _getValue = !IsBaked ? (DelGetValue<Quat>)GetValueKeyframed : GetValueBaked;

        public Quat GetValue(float second)
            => _getValue(second);
        protected override object GetValueGeneric(float second)
            => _getValue(second);
        public Quat GetValueBaked(float second)
            => _baked[(int)Math.Floor(second * BakedFramesPerSecond)];
        public Quat GetValueBaked(int frameIndex)
            => _baked[frameIndex];
        public Quat GetValueKeyframed(float second)
            => _keyframes.Count == 0 ? DefaultValue : _keyframes.First.Interpolate(second);
        
        public override void Bake(float framesPerSecond)
        {
            _bakedFPS = framesPerSecond;
            _bakedFrameCount = (int)Math.Ceiling(LengthInSeconds * framesPerSecond);
            _baked = new Quat[BakedFrameCount];
            for (int i = 0; i < BakedFrameCount; ++i)
                _baked[i] = GetValueKeyframed(i);
        }
        
        protected override object GetCurrentValueGeneric()
        {
            throw new NotImplementedException();
        }

        protected override void OnProgressed(float delta)
        {
            throw new NotImplementedException();
        }
    }
    public class QuatKeyframe : Keyframe, IRadialKeyframe
    {
        public QuatKeyframe() { }
        public QuatKeyframe(int frameIndex, float FPS, Quat inValue, Quat outValue, Quat inTangent, Quat outTangent, ERadialInterpType type)
            : this(frameIndex / FPS, inValue, outValue, inTangent, outTangent, type) { }
        public QuatKeyframe(int frameIndex, float FPS, Quat inoutValue, Quat inTangent, Quat outTangent, ERadialInterpType type)
            : this(frameIndex / FPS, inoutValue, inoutValue, inTangent, outTangent, type) { }
        public QuatKeyframe(float second, Quat inoutValue, Quat inTangent, Quat outTangent, ERadialInterpType type)
            : this(second, inoutValue, inoutValue, inTangent, outTangent, type) { }
        public QuatKeyframe(float second, Quat inValue, Quat outValue, Quat inTangent, Quat outTangent, ERadialInterpType type) : base()
        {
            Second = second;
            InValue = inValue;
            OutValue = outValue;
            InTangent = inTangent;
            OutTangent = outTangent;
            InterpolationType = type;
        }

        private delegate Quat DelInterpolate(QuatKeyframe key1, QuatKeyframe key2, float time);
        private DelInterpolate _interpolate = CubicBezier;
        protected ERadialInterpType _interpolationType;

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public Quat InValue { get; set; }
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public Quat OutValue { get; set; }

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public Quat InTangent { get; set; }
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public Quat OutTangent { get; set; }

        [Browsable(false)]
        public new QuatKeyframe Next
        {
            get => _next as QuatKeyframe;
            set => _next = value;
        }
        [Browsable(false)]
        public new QuatKeyframe Prev
        {
            get => _prev as QuatKeyframe;
            set => _prev = value;
        }

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public ERadialInterpType InterpolationType
        {
            get => _interpolationType;
            set
            {
                _interpolationType = value;
                switch (_interpolationType)
                {
                    case ERadialInterpType.Step:
                        _interpolate = Step;
                        break;
                    case ERadialInterpType.Linear:
                        _interpolate = Linear;
                        break;
                    case ERadialInterpType.CubicBezier:
                        _interpolate = CubicBezier;
                        break;
                }
            }
        }
        public Quat Interpolate(float desiredSecond)
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
        public static Quat Step(QuatKeyframe key1, QuatKeyframe key2, float time)
            => time < 1.0f ? key1.OutValue : key2.OutValue;
        public static Quat Linear(QuatKeyframe key1, QuatKeyframe key2, float time)
            => Quat.Slerp(key1.OutValue, key2.InValue, time);
        public static Quat CubicBezier(QuatKeyframe key1, QuatKeyframe key2, float time)
            => Quat.Scubic(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);

        public override string WriteToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5}", Second, InValue.WriteToString(), OutValue.WriteToString(), InTangent.WriteToString(), OutTangent.WriteToString(), InterpolationType);
        }

        public override void ReadFromString(string str)
        {
            string[] parts = str.Split(' ');
            Second = float.Parse(parts[0]);
            InValue = new Quat(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]), float.Parse(parts[4]));
            OutValue = new Quat(float.Parse(parts[5]), float.Parse(parts[6]), float.Parse(parts[7]), float.Parse(parts[8]));
            InTangent = new Quat(float.Parse(parts[9]), float.Parse(parts[10]), float.Parse(parts[11]), float.Parse(parts[12]));
            OutTangent = new Quat(float.Parse(parts[13]), float.Parse(parts[14]), float.Parse(parts[15]), float.Parse(parts[16]));
            InterpolationType = parts[17].AsEnum<ERadialInterpType>();
        }
    }
}

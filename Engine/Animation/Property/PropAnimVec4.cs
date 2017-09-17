using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEngine.Animation
{
    delegate Vec4 Vec4GetValue(float frameIndex);
    public class PropAnimVec4 : PropertyAnimation<Vec4Keyframe>, IEnumerable<Vec4Keyframe>
    {
        private Vec4 _defaultValue = Vec4.Zero;
        private Vec4GetValue _getValue;

        [Serialize(Condition = "!UseKeyframes")]
        private Vec4[] _baked;

        [Serialize(Condition = "UseKeyframes")]
        public Vec4 DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        public PropAnimVec4(int frameCount, bool looped, bool useKeyframes) 
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
        public Vec4 GetValueBaked(float frameIndex)
            => _baked[(int)(frameIndex / Engine.TargetUpdateFreq * BakedFramesPerSecond)];
        public Vec4 GetValueKeyframed(float frameIndex)
            => _keyframes.KeyCount == 0 ? _defaultValue : _keyframes.First.Interpolate(frameIndex);

        /// <summary>
        /// Bakes the interpolated data for fastest access by the game.
        /// However, this method takes up more space and does not support time dilation (speeding up and slowing down with proper in-betweens)
        /// </summary>
        public override void Bake()
        {
            _baked = new Vec4[BakedFrameCount];
            for (int i = 0; i < BakedFrameCount; ++i)
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
        public override void Append(PropertyAnimation<Vec4Keyframe> other)
        {
            throw new NotImplementedException();
        }
        public IEnumerator<Vec4Keyframe> GetEnumerator() { return ((IEnumerable<Vec4Keyframe>)_keyframes).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<Vec4Keyframe>)_keyframes).GetEnumerator(); }
    }
    public class Vec4Keyframe : Keyframe
    {
        public Vec4Keyframe(float frameIndex, Vec4 inValue, Vec4 outValue) : base()
        {
            Second = frameIndex;
            _inValue = inValue;
            _outValue = outValue;
        }

        protected Vec4 _inValue;
        protected Vec4 _inTangent;
        protected Vec4 _outValue;
        protected Vec4 _outTangent;
        protected PlanarInterpType _interpolationType;

        [Serialize(IsXmlAttribute = true)]
        public Vec4 InValue
        {
            get => _inValue;
            set => _inValue = value;
        }
        [Serialize(IsXmlAttribute = true)]
        public Vec4 OutValue
        {
            get => _outValue;
            set => _outValue = value;
        }
        [Serialize(IsXmlAttribute = true)]
        public Vec4 InTangent
        {
            get => _inTangent;
            set => _inTangent = value;
        }
        [Serialize(IsXmlAttribute = true)]
        public Vec4 OutTangent
        {
            get => _outTangent;
            set => _outTangent = value;
        }
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
        delegate Vec4 DelInterpolate(Vec4Keyframe key1, Vec4Keyframe key2, float time);
        private DelInterpolate _interpolate = CubicHermite;
        public Vec4 Interpolate(float frameIndex)
        {
            if (frameIndex < Second && _prev.Second > Second)
            {
                if (_prev == this)
                    return _inValue;

                return Prev.Interpolate(frameIndex);
            }

            if (_next == this)
                return _outValue;

            if (frameIndex > _next.Second && _next.Second > Second)
                return Next.Interpolate(frameIndex);

            float t = (frameIndex - Second) / (_next.Second - Second);
            return _interpolate(this, Next, t);
        }
        public static Vec4 Step(Vec4Keyframe key1, Vec4Keyframe key2, float time)
            => time < 1.0f ? key1.OutValue : key2.OutValue;
        public static Vec4 Lerp(Vec4Keyframe key1, Vec4Keyframe key2, float time)
            => Vec4.Lerp(key1.OutValue, key2.InValue, time);
        public static Vec4 Bezier(Vec4Keyframe key1, Vec4Keyframe key2, float time)
            => CustomMath.CubicBezier(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);
        public static Vec4 CubicHermite(Vec4Keyframe key1, Vec4Keyframe key2, float time)
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

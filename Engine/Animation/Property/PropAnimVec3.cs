using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEngine.Animation
{
    delegate Vec3 Vec3GetValue(float frameIndex);
    public class PropAnimVec3 : PropertyAnimation<Vec3Keyframe>, IEnumerable<Vec3Keyframe>
    {
        private Vec3 _defaultValue = Vec3.Zero;
        private Vec3GetValue _getValue;

        [Serialize(Condition = "!UseKeyframes")]
        private Vec3[] _baked;

        [Serialize(Condition = "UseKeyframes")]
        public Vec3 DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }
        
        public PropAnimVec3(int frameCount, bool looped, bool useKeyframes) 
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
        public Vec3 GetValueBaked(float frameIndex)
            => _baked[(int)(frameIndex / Engine.TargetUpdateFreq * BakedFramesPerSecond)];
        public Vec3 GetValueKeyframed(float frameIndex)
            => _keyframes.KeyCount == 0 ? _defaultValue : _keyframes.First.Interpolate(frameIndex);
        public Vec3 GetVelocityKeyframed(float frameIndex)
            => _keyframes.KeyCount == 0 ? Vec3.Zero : _keyframes.First.InterpolateVelocity(frameIndex);
        public Vec3 GetAccelerationKeyframed(float frameIndex)
            => _keyframes.KeyCount == 0 ? Vec3.Zero : _keyframes.First.InterpolateAcceleration(frameIndex);
        
        /// <summary>
        /// Bakes the interpolated data for fastest access by the game.
        /// However, this method takes up more space and does not support time dilation (speeding up and slowing down with proper in-betweens)
        /// </summary>
        public override void Bake()
        {
            _baked = new Vec3[BakedFrameCount];
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
        public override void Append(PropertyAnimation<Vec3Keyframe> other)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Vec3Keyframe> GetEnumerator() { return ((IEnumerable<Vec3Keyframe>)_keyframes).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<Vec3Keyframe>)_keyframes).GetEnumerator(); }
    }
    public class Vec3Keyframe : Keyframe
    {
        public Vec3Keyframe(float frameIndex, Vec3 value, PlanarInterpType type = PlanarInterpType.Linear)
            : this(frameIndex, value, value, type) { }

        public Vec3Keyframe(float frameIndex, Vec3 inValue, Vec3 outValue, PlanarInterpType type = PlanarInterpType.Linear)
            : this(frameIndex, inValue, outValue, Vec3.Zero, Vec3.Zero, type) { }

        public Vec3Keyframe(float frameIndex, Vec3 inValue, Vec3 outValue, Vec3 inTangent, Vec3 outTangent, PlanarInterpType type = PlanarInterpType.Linear) : base()
        {
            Second = frameIndex;
            _inValue = inValue;
            _outValue = outValue;
            _inTangent = inTangent;
            _outTangent = outTangent;
            InterpolationType = type;
        }

        protected delegate Vec3 DelInterpolate(Vec3Keyframe key1, Vec3Keyframe key2, float time);
        protected PlanarInterpType _interpolationType;
        protected DelInterpolate _interpolate = CubicHermite;
        protected DelInterpolate _interpolateVelocity = CubicHermiteVelocity;
        protected DelInterpolate _interpolateAcceleration = CubicHermiteAcceleration;

        protected Vec3 _inValue;
        protected Vec3 _inTangent;
        protected Vec3 _outValue;
        protected Vec3 _outTangent;

        [Serialize(IsXmlAttribute = true)]
        public Vec3 InValue
        {
            get => _inValue;
            set => _inValue = value;
        }

        [Serialize(IsXmlAttribute = true)]
        public Vec3 OutValue
        {
            get => _outValue;
            set => _outValue = value;
        }

        [Serialize(IsXmlAttribute = true)]
        public Vec3 InTangent
        {
            get => _inTangent;
            set => _inTangent = value;
        }

        [Serialize(IsXmlAttribute = true)]
        public Vec3 OutTangent
        {
            get => _outTangent;
            set => _outTangent = value;
        }

        public new Vec3Keyframe Next
        {
            get => _next as Vec3Keyframe;
            set => _next = value;
        }
        public new Vec3Keyframe Prev
        {
            get => _prev as Vec3Keyframe;
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
                        _interpolateVelocity = StepVelocity;
                        _interpolateAcceleration = StepAcceleration;
                        break;
                    case PlanarInterpType.Linear:
                        _interpolate = Lerp;
                        _interpolateVelocity = LerpVelocity;
                        _interpolateAcceleration = LerpAcceleration;
                        break;
                    case PlanarInterpType.CubicHermite:
                        _interpolate = CubicHermite;
                        _interpolateVelocity = CubicHermiteVelocity;
                        _interpolateAcceleration = CubicHermiteAcceleration;
                        break;
                    case PlanarInterpType.CubicBezier:
                        _interpolate = CubicBezier;
                        _interpolateVelocity = CubicBezierVelocity;
                        _interpolateAcceleration = CubicBezierAcceleration;
                        break;
                }
            }
        }
        public Vec3 Interpolate(float frameIndex)
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
        public Vec3 InterpolateVelocity(float frameIndex)
        {
            if (frameIndex < Second && _prev.Second > Second)
                return Prev.InterpolateVelocity(frameIndex);
            
            if (frameIndex > _next.Second && _next.Second > Second)
                return Next.InterpolateVelocity(frameIndex);

            float t = (frameIndex - Second) / (_next.Second - Second);
            return _interpolateVelocity(this, Next, t);
        }
        public Vec3 InterpolateAcceleration(float frameIndex)
        {
            if (frameIndex < Second && _prev.Second > Second)
                return Prev.InterpolateAcceleration(frameIndex);
            
            if (frameIndex > _next.Second && _next.Second > Second)
                return Next.InterpolateAcceleration(frameIndex);

            float t = (frameIndex - Second) / (_next.Second - Second);
            return _interpolateAcceleration(this, Next, t);
        }

        public static Vec3 Step(Vec3Keyframe key1, Vec3Keyframe key2, float time)
            => time < 1.0f ? key1.OutValue : key2.OutValue;
        public static Vec3 StepVelocity(Vec3Keyframe key1, Vec3Keyframe key2, float time)
            => Vec3.Zero;
        public static Vec3 StepAcceleration(Vec3Keyframe key1, Vec3Keyframe key2, float time)
            => Vec3.Zero;
        
        public static Vec3 Lerp(Vec3Keyframe key1, Vec3Keyframe key2, float time)
            => Vec3.Lerp(key1.OutValue, key2.InValue, time);
        public static Vec3 LerpVelocity(Vec3Keyframe key1, Vec3Keyframe key2, float time)
            => (key2.InValue - key1.OutValue) / time;
        public static Vec3 LerpAcceleration(Vec3Keyframe key1, Vec3Keyframe key2, float time)
            => Vec3.Zero;
        
        public static Vec3 CubicBezier(Vec3Keyframe key1, Vec3Keyframe key2, float time)
            => CustomMath.CubicBezier(key1.OutValue, key1.OutValue + key1.OutTangent, key2.InValue + key2.InTangent, key2.InValue, time);
        public static Vec3 CubicBezierVelocity(Vec3Keyframe key1, Vec3Keyframe key2, float time)
            => CustomMath.CubicBezierVelocity(key1.OutValue, key1.OutValue + key1.OutTangent, key2.InValue + key2.InTangent, key2.InValue, time);
        public static Vec3 CubicBezierAcceleration(Vec3Keyframe key1, Vec3Keyframe key2, float time)
            => CustomMath.CubicBezierAcceleration(key1.OutValue, key1.OutValue + key1.OutTangent, key2.InValue + key2.InTangent, key2.InValue, time);
        
        public static Vec3 CubicHermite(Vec3Keyframe key1, Vec3Keyframe key2, float time)
            => CustomMath.CubicHermite(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);
        public static Vec3 CubicHermiteVelocity(Vec3Keyframe key1, Vec3Keyframe key2, float time)
            => CustomMath.CubicHermiteVelocity(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);
        public static Vec3 CubicHermiteAcceleration(Vec3Keyframe key1, Vec3Keyframe key2, float time)
            => CustomMath.CubicHermiteAcceleration(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);

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
            InValue = new Vec3(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
            OutValue = new Vec3(float.Parse(parts[4]), float.Parse(parts[5]), float.Parse(parts[6]));
            InTangent = new Vec3(float.Parse(parts[7]), float.Parse(parts[8]), float.Parse(parts[9]));
            OutTangent = new Vec3(float.Parse(parts[10]), float.Parse(parts[11]), float.Parse(parts[12]));
            InterpolationType = parts[13].AsEnum<PlanarInterpType>();
        }
    }
}

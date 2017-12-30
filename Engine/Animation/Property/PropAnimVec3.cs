using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEngine.Animation
{
    public class PropAnimVec3 : PropAnimKeyframed<Vec3Keyframe>, IEnumerable<Vec3Keyframe>
    {
        private DelGetValue<Vec3> _getValue;

        [TSerialize(Condition = "Baked")]
        private Vec3[] _baked = null;
        /// <summary>
        /// The default value to return when no keyframes are set.
        /// </summary>
        [TSerialize(Condition = "!Baked")]
        public Vec3 DefaultValue { get; set; } = Vec3.Zero;

        public PropAnimVec3() : base(0.0f, false) { }
        public PropAnimVec3(float lengthInSeconds, bool looped, bool useKeyframes)
            : base(lengthInSeconds, looped, useKeyframes) { }
        public PropAnimVec3(int frameCount, float FPS, bool looped, bool useKeyframes) 
            : base(frameCount, FPS, looped, useKeyframes) { }

        protected override void BakedChanged()
            => _getValue = !Baked ? (DelGetValue<Vec3>)GetValueKeyframed : GetValueBaked;

        public Vec3 GetValue(float second)
            => _getValue(second);
        protected override object GetValueGeneric(float second)
            => _getValue(second);
        public Vec3 GetValueBaked(float second)
            => _baked[(int)Math.Floor(second * BakedFramesPerSecond)];
        public Vec3 GetValueBaked(int frameIndex)
            => _baked[frameIndex];
        public Vec3 GetValueKeyframed(float second)
            => _keyframes.Count == 0 ? DefaultValue : _keyframes.First.Interpolate(second);
        public Vec3 GetVelocityKeyframed(float second)
            => _keyframes.Count == 0 ? 0.0f : _keyframes.First.InterpolateVelocity(second);
        public Vec3 GetAccelerationKeyframed(float second)
            => _keyframes.Count == 0 ? 0.0f : _keyframes.First.InterpolateAcceleration(second);

        public override void Bake(float framesPerSecond)
        {
            _bakedFPS = framesPerSecond;
            _bakedFrameCount = (int)Math.Ceiling(LengthInSeconds * framesPerSecond);
            _baked = new Vec3[BakedFrameCount];
            for (int i = 0; i < BakedFrameCount; ++i)
                _baked[i] = GetValueKeyframed(i);
        }

        public IEnumerator<Vec3Keyframe> GetEnumerator()
            => ((IEnumerable<Vec3Keyframe>)_keyframes).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() 
            => ((IEnumerable<Vec3Keyframe>)_keyframes).GetEnumerator();
    }
    public class Vec3Keyframe : Keyframe
    {
        public Vec3Keyframe(int frameIndex, float FPS, Vec3 inValue, Vec3 outValue, Vec3 inTangent, Vec3 outTangent, PlanarInterpType type)
            : this(frameIndex / FPS, inValue, outValue, inTangent, outTangent, type) { }
        public Vec3Keyframe(int frameIndex, float FPS, Vec3 inoutValue, Vec3 inoutTangent, PlanarInterpType type)
            : this(frameIndex / FPS, inoutValue, inoutValue, inoutTangent, inoutTangent, type) { }
        public Vec3Keyframe(float second, Vec3 inoutValue, Vec3 inoutTangent, PlanarInterpType type)
            : this(second, inoutValue, inoutValue, inoutTangent, inoutTangent, type) { }
        public Vec3Keyframe(float second, Vec3 inValue, Vec3 outValue, Vec3 inTangent, Vec3 outTangent, PlanarInterpType type) : base()
        {
            Second = second;
            InValue = inValue;
            OutValue = outValue;
            InTangent = inTangent;
            OutTangent = outTangent;
            InterpolationType = type;
        }

        protected delegate Vec3 DelInterpolate(Vec3Keyframe key1, Vec3Keyframe key2, float time);
        protected PlanarInterpType _interpolationType;
        protected DelInterpolate _interpolate = CubicHermite;
        protected DelInterpolate _interpolateVelocity = CubicHermiteVelocity;
        protected DelInterpolate _interpolateAcceleration = CubicHermiteAcceleration;
        
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public Vec3 InValue { get; set; }
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public Vec3 OutValue { get; set; }

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public Vec3 InTangent { get; set; }
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public Vec3 OutTangent { get; set; }

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
        public Vec3 Interpolate(float desiredSecond)
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
        public Vec3 InterpolateVelocity(float desiredSecond)
        {
            //First, check if the desired second is between this key and the next key.
            if (desiredSecond < Second)
            {
                //If the previous key's second is greater than this second, this key must be the first key. 
                //Return the InValue as the desired second comes before this one.
                //Otherwise, move to the previous key to calculate the interpolated value.
                return _prev.Second < Second ? Prev.InterpolateVelocity(desiredSecond) : InValue;
            }
            else if (desiredSecond > _next.Second)
            {
                //If the next key's second is less than this second, this key must be the last key. 
                //Return the OutValue as the desired second comes after this one.
                //Otherwise, move to the previous key to calculate the interpolated value.
                return _next.Second > Second ? Next.InterpolateVelocity(desiredSecond) : OutValue;
            }

            float span = _next.Second - Second;
            float diff = desiredSecond - Second;
            float time = diff / span;
            return _interpolateVelocity(this, Next, time);
        }
        public Vec3 InterpolateAcceleration(float desiredSecond)
        {
            //First, check if the desired second is between this key and the next key.
            if (desiredSecond < Second)
            {
                //If the previous key's second is greater than this second, this key must be the first key. 
                //Return the InValue as the desired second comes before this one.
                //Otherwise, move to the previous key to calculate the interpolated value.
                return _prev.Second < Second ? Prev.InterpolateVelocity(desiredSecond) : InValue;
            }
            else if (desiredSecond > _next.Second)
            {
                //If the next key's second is less than this second, this key must be the last key. 
                //Return the OutValue as the desired second comes after this one.
                //Otherwise, move to the previous key to calculate the interpolated value.
                return _next.Second > Second ? Next.InterpolateVelocity(desiredSecond) : OutValue;
            }

            float span = _next.Second - Second;
            float diff = desiredSecond - Second;
            float time = diff / span;
            return _interpolateAcceleration(this, Next, time);
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
            => Interp.CubicBezier(key1.OutValue, key1.OutValue + key1.OutTangent, key2.InValue + key2.InTangent, key2.InValue, time);
        public static Vec3 CubicBezierVelocity(Vec3Keyframe key1, Vec3Keyframe key2, float time)
            => Interp.CubicBezierVelocity(key1.OutValue, key1.OutValue + key1.OutTangent, key2.InValue + key2.InTangent, key2.InValue, time);
        public static Vec3 CubicBezierAcceleration(Vec3Keyframe key1, Vec3Keyframe key2, float time)
            => Interp.CubicBezierAcceleration(key1.OutValue, key1.OutValue + key1.OutTangent, key2.InValue + key2.InTangent, key2.InValue, time);
        
        public static Vec3 CubicHermite(Vec3Keyframe key1, Vec3Keyframe key2, float time)
            => Interp.CubicHermite(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);
        public static Vec3 CubicHermiteVelocity(Vec3Keyframe key1, Vec3Keyframe key2, float time)
            => Interp.CubicHermiteVelocity(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);
        public static Vec3 CubicHermiteAcceleration(Vec3Keyframe key1, Vec3Keyframe key2, float time)
            => Interp.CubicHermiteAcceleration(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);

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
            InValue = new Vec3(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
            OutValue = new Vec3(float.Parse(parts[4]), float.Parse(parts[5]), float.Parse(parts[6]));
            InTangent = new Vec3(float.Parse(parts[7]), float.Parse(parts[8]), float.Parse(parts[9]));
            OutTangent = new Vec3(float.Parse(parts[10]), float.Parse(parts[11]), float.Parse(parts[12]));
            InterpolationType = parts[13].AsEnum<PlanarInterpType>();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Core.Maths;

namespace TheraEngine.Animation
{
    public class PropAnimInt : PropAnimKeyframed<IntKeyframe>, IEnumerable<IntKeyframe>
    {
        private DelGetValue<int> _getValue;

        [TSerialize(Condition = "Baked")]
        private int[] _baked = null;
        /// <summary>
        /// The default value to return when no keyframes are set.
        /// </summary>
        [Category(PropAnimCategory)]
        [TSerialize(Condition = "!Baked")]
        public int DefaultValue { get; set; } = 0;

        public PropAnimInt() : base(0.0f, false) { }
        public PropAnimInt(float lengthInSeconds, bool looped, bool useKeyframes)
            : base(lengthInSeconds, looped, useKeyframes) { }
        public PropAnimInt(int frameCount, float FPS, bool looped, bool useKeyframes) 
            : base(frameCount, FPS, looped, useKeyframes) { }

        protected override void BakedChanged()
            => _getValue = !IsBaked ? (DelGetValue<int>)GetValueKeyframed : GetValueBaked;
        
        public int GetValue(float second)
            => _getValue(second);
        protected override object GetValueGeneric(float second)
            => _getValue(second);
        public int GetValueBaked(float second)
            => _baked[(int)Math.Floor(second * BakedFramesPerSecond)];
        public int GetValueBaked(int frameIndex)
            => _baked[frameIndex];
        public int GetValueKeyframed(float second)
            => _keyframes.Count == 0 ? DefaultValue : _keyframes.First.Interpolate(second);
        
        public override void Bake(float framesPerSecond)
        {
            _bakedFPS = framesPerSecond;
            _bakedFrameCount = (int)Math.Ceiling(LengthInSeconds * framesPerSecond);
            _baked = new int[BakedFrameCount];
            for (int i = 0; i < BakedFrameCount; ++i)
                _baked[i] = GetValueKeyframed(i);
        }

        public void GetMinMax(out int min, out int max)
        {
            if (_keyframes.Count == 0)
            {
                min = DefaultValue;
                max = DefaultValue;
            }
            else
            {
                IntKeyframe kf = _keyframes.First;
                if (_keyframes.Count == 1)
                {
                    if (kf.Second.IsZero())
                    {
                        min = max = kf.OutValue;
                    }
                    else if (kf.Second.EqualTo(_keyframes.LengthInSeconds))
                    {
                        min = max = kf.InValue;
                    }
                    else
                    {
                        min = Math.Min(kf.InValue, kf.OutValue);
                        max = Math.Max(kf.InValue, kf.OutValue);
                    }
                }
                else
                {
                    min = int.MaxValue;
                    max = int.MinValue;
                    for (int i = 0; i < _keyframes.Count; ++i)
                    {
                        if (kf.Second.IsZero())
                        {
                            min = TMath.Min(min, kf.OutValue);
                            max = TMath.Max(min, kf.OutValue);
                        }
                        else if (kf.Second.EqualTo(_keyframes.LengthInSeconds))
                        {
                            min = TMath.Min(min, kf.InValue);
                            max = TMath.Max(min, kf.InValue);
                        }
                        else
                        {
                            min = TMath.Min(min, kf.InValue, kf.OutValue);
                            max = TMath.Max(max, kf.InValue, kf.OutValue);
                        }

                        //If not the last keyframe, evaluate the interpolation
                        //between this keyframe and the next to find spots where
                        //velocity reaches zero. This means that the position value
                        //is an extrema and should be considered for min/max.
                        if (i != _keyframes.Count - 1)
                        {
                            IntKeyframe next = kf.Next;

                            //Retrieve velocity interpolation equation coefficients
                            //so we can solve for the two time values where velocity is zero.
                            Interp.CubicHermiteVelocityCoefs(
                                kf.OutValue, kf.OutTangent, next.InTangent, next.InValue,
                                out float second, out float first, out float zero);

                            if (TMath.QuadraticRealRoots(second, first, zero,
                                out float time1, out float time2))
                            {
                                int val1 = kf.InterpolateNextNormalized(time1);
                                int val2 = kf.InterpolateNextNormalized(time2);
                                min = TMath.Min(min, val1, val2);
                                max = TMath.Max(max, val1, val2);
                            }

                            kf = next;
                        }
                    }
                }
            }
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
    public class IntKeyframe : Keyframe, IPlanarKeyframe<int>
    {
        public IntKeyframe() { }
        public IntKeyframe(int frameIndex, float FPS, int inValue, int outValue, float inTangent, float outTangent, EPlanarInterpType type)
            : this(frameIndex / FPS, inValue, outValue, inTangent, outTangent, type) { }
        public IntKeyframe(int frameIndex, float FPS, int inoutValue, float inoutTangent, EPlanarInterpType type)
            : this(frameIndex / FPS, inoutValue, inoutValue, inoutTangent, inoutTangent, type) { }
        public IntKeyframe(float second, int inoutValue, float inoutTangent, EPlanarInterpType type)
            : this(second, inoutValue, inoutValue, inoutTangent, inoutTangent, type) { }
        public IntKeyframe(float second, int inoutValue, float inTangent, float outTangent, EPlanarInterpType type)
            : this(second, inoutValue, inoutValue, inTangent, outTangent, type) { }
        public IntKeyframe(float second, int inValue, int outValue, float inTangent, float outTangent, EPlanarInterpType type) : base()
        {
            Second = second;
            InValue = inValue;
            OutValue = outValue;
            InTangent = inTangent;
            OutTangent = outTangent;
            InterpolationType = type;

            _interpolate = CubicHermite;
            _interpolateVelocity = CubicHermiteVelocity;
            _interpolateAcceleration = CubicHermiteAcceleration;
    }

        private delegate int DelInterpolate(IntKeyframe key1, IntKeyframe key2, float time);
        private DelInterpolate _interpolate;
        private DelInterpolate _interpolateVelocity;
        private DelInterpolate _interpolateAcceleration;
        protected EPlanarInterpType _interpolationType;
        
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public int InValue { get; set; }
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public int OutValue { get; set; }

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public float InTangent { get; set; }
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public float OutTangent { get; set; }

        [Browsable(false)]
        public new IntKeyframe Next
        {
            get => _next as IntKeyframe;
            set => _next = value;
        }
        [Browsable(false)]
        public new IntKeyframe Prev
        {
            get => _prev as IntKeyframe;
            set => _prev = value;
        }

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public EPlanarInterpType InterpolationType
        {
            get => _interpolationType;
            set
            {
                _interpolationType = value;
                switch (_interpolationType)
                {
                    case EPlanarInterpType.Step:
                        _interpolate = Step;
                        _interpolateVelocity = StepVelocity;
                        _interpolateAcceleration = StepAcceleration;
                        break;
                    case EPlanarInterpType.Linear:
                        _interpolate = Lerp;
                        _interpolateVelocity = LerpVelocity;
                        _interpolateAcceleration = LerpAcceleration;
                        break;
                    case EPlanarInterpType.CubicHermite:
                        _interpolate = CubicHermite;
                        _interpolateVelocity = CubicHermiteVelocity;
                        _interpolateAcceleration = CubicHermiteAcceleration;
                        break;
                    case EPlanarInterpType.CubicBezier:
                        _interpolate = CubicBezier;
                        _interpolateVelocity = CubicBezierVelocity;
                        _interpolateAcceleration = CubicBezierAcceleration;
                        break;
                }
            }
        }
        
        object IPlanarKeyframe.InValue { get => InValue; set => InValue = (int)value; }
        object IPlanarKeyframe.OutValue { get => OutValue; set => OutValue = (int)value; }
        object IPlanarKeyframe.InTangent { get => InTangent; set => InTangent = (int)value; }
        object IPlanarKeyframe.OutTangent { get => OutTangent; set => OutTangent = (int)value; }
        int IPlanarKeyframe<int>.InTangent { get => (int)InTangent; set => InTangent = value; }
        int IPlanarKeyframe<int>.OutTangent { get => (int)OutTangent; set => OutTangent = value; }

        /// <summary>
        /// Interpolates from this keyframe to the next using a normalized time value (0.0f - 1.0f)
        /// </summary>
        public int InterpolateNextNormalized(float time) => _interpolate(this, Next, time);
        /// <summary>
        /// Interpolates velocity from this keyframe to the next using a normalized time value (0.0f - 1.0f)
        /// </summary>
        public int InterpolateVelocityNextNormalized(float time) => _interpolateVelocity(this, Next, time);
        /// <summary>
        /// Interpolates acceleration from this keyframe to the next using a normalized time value (0.0f - 1.0f)
        /// </summary>
        public int InterpolateAccelerationNextNormalized(float time) => _interpolateAcceleration(this, Next, time);
        public int Interpolate(float desiredSecond)
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
        public int InterpolateVelocity(float desiredSecond)
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
        public int InterpolateAcceleration(float desiredSecond)
        {
            //First, check if the desired second is between this key and the next key.
            if (desiredSecond < Second)
            {
                //If the previous key's second is greater than this second, this key must be the first key. 
                //Return the InValue as the desired second comes before this one.
                //Otherwise, move to the previous key to calculate the interpolated value.
                return _prev.Second < Second ? Prev.InterpolateAcceleration(desiredSecond) : InValue;
            }
            else if (desiredSecond > _next.Second)
            {
                //If the next key's second is less than this second, this key must be the last key. 
                //Return the OutValue as the desired second comes after this one.
                //Otherwise, move to the previous key to calculate the interpolated value.
                return _next.Second > Second ? Next.InterpolateAcceleration(desiredSecond) : OutValue;
            }

            float span = _next.Second - Second;
            float diff = desiredSecond - Second;
            float time = diff / span;
            return _interpolateAcceleration(this, Next, time);
        }

        protected virtual int ConvertInterpolatedValue(float interpolatedValue) => (int)interpolatedValue;
        
        public static int Step(IntKeyframe key1, IntKeyframe key2, float time)
            => time < 1.0f ? key1.OutValue : key2.OutValue;
        public static int StepVelocity(IntKeyframe key1, IntKeyframe key2, float time)
            => 0;
        public static int StepAcceleration(IntKeyframe key1, IntKeyframe key2, float time)
            => 0;

        public int Lerp(IntKeyframe key1, IntKeyframe key2, float time)
            => ConvertInterpolatedValue(Interp.Lerp(key1.OutValue, key2.InValue, time));
        public int LerpVelocity(IntKeyframe key1, IntKeyframe key2, float time)
            => ConvertInterpolatedValue((key2.InValue - key1.OutValue) / time);
        public static int LerpAcceleration(IntKeyframe key1, IntKeyframe key2, float time)
            => 0;

        public int CubicBezier(IntKeyframe key1, IntKeyframe key2, float time)
            => ConvertInterpolatedValue(Interp.CubicBezier(key1.OutValue, key1.OutValue + key1.OutTangent, key2.InValue + key2.InTangent, key2.InValue, time));
        public int CubicBezierVelocity(IntKeyframe key1, IntKeyframe key2, float time)
            => ConvertInterpolatedValue(Interp.CubicBezierVelocity(key1.OutValue, key1.OutValue + key1.OutTangent, key2.InValue + key2.InTangent, key2.InValue, time));
        public int CubicBezierAcceleration(IntKeyframe key1, IntKeyframe key2, float time)
            => ConvertInterpolatedValue(Interp.CubicBezierAcceleration(key1.OutValue, key1.OutValue + key1.OutTangent, key2.InValue + key2.InTangent, key2.InValue, time));

        public int CubicHermite(IntKeyframe key1, IntKeyframe key2, float time)
            => ConvertInterpolatedValue(Interp.CubicHermite(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time));
        public int CubicHermiteVelocity(IntKeyframe key1, IntKeyframe key2, float time)
            => ConvertInterpolatedValue(Interp.CubicHermiteVelocity(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time));
        public int CubicHermiteAcceleration(IntKeyframe key1, IntKeyframe key2, float time)
            => ConvertInterpolatedValue(Interp.CubicHermiteAcceleration(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time));

        public void AverageKeyframe()
        {
            AverageValues();
            AverageTangents();
        }
        public void AverageTangents()
            => InTangent = OutTangent = (InTangent + OutTangent) / 2.0f;
        public void AverageValues()
            => InValue = OutValue = (InValue + OutValue) / 2;
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
            InValue = int.Parse(parts[1]);
            OutValue = int.Parse(parts[2]);
            InTangent = float.Parse(parts[3]);
            OutTangent = float.Parse(parts[4]);
            InterpolationType = parts[5].AsEnum<EPlanarInterpType>();
        }
        
        void IPlanarKeyframe.ParsePlanar(string inValue, string outValue, string inTangent, string outTangent)
        {
            InValue = int.Parse(inValue);
            OutValue = int.Parse(outValue);
            InTangent = float.Parse(inTangent);
            OutTangent = float.Parse(outTangent);
        }
        void IPlanarKeyframe.WritePlanar(out string inValue, out string outValue, out string inTangent, out string outTangent)
        {
            inValue = InValue.ToString();
            outValue = OutValue.ToString();
            inTangent = InTangent.ToString();
            outTangent = OutTangent.ToString();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEngine.Animation
{
    public class PropAnimFloat : PropAnimKeyframed<FloatKeyframe>, IEnumerable<FloatKeyframe>
    {
        private DelGetValue<float> _getValue;

        [TSerialize(Condition = "Baked")]
        private float[] _baked = null;
        /// <summary>
        /// The default value to return when no keyframes are set.
        /// </summary>
        [Category(PropAnimCategory)]
        [TSerialize(Condition = "!Baked")]
        public float DefaultValue { get; set; } = 0.0f;

        public PropAnimFloat() : base(0.0f, false) { }
        public PropAnimFloat(float lengthInSeconds, bool looped, bool isBaked)
            : base(lengthInSeconds, looped, isBaked) { }
        public PropAnimFloat(int frameCount, float FPS, bool looped, bool isBaked) 
            : base(frameCount, FPS, looped, isBaked) { }

        protected override void BakedChanged()
            => _getValue = !Baked ? (DelGetValue<float>)GetValueKeyframed : GetValueBaked;
        
        public float GetValue(float second)
            => _getValue(second);
        protected override object GetValueGeneric(float second)
            => _getValue(second);
        public float GetValueBaked(float second)
            => _baked[(int)Math.Floor(second * BakedFramesPerSecond)];
        public float GetValueBaked(int frameIndex)
            => _baked[frameIndex];
        public float GetValueKeyframed(float second)
            => _keyframes.Count == 0 ? DefaultValue : _keyframes.First.Interpolate(second);
        
        public override void Bake(float framesPerSecond)
        {
            _bakedFPS = framesPerSecond;
            _bakedFrameCount = (int)Math.Ceiling(LengthInSeconds * framesPerSecond);
            _baked = new float[BakedFrameCount];
            for (int i = 0; i < BakedFrameCount; ++i)
                _baked[i] = GetValueKeyframed(i);
        }

        public void GetMinMax(out float min, out float max)
        {
            if (_keyframes.Count == 0)
            {
                min = DefaultValue;
                max = DefaultValue;
            }
            else
            {
                FloatKeyframe kf = _keyframes.First;
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
                    min = float.MaxValue;
                    max = float.MinValue;
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
                            FloatKeyframe next = kf.Next;

                            //Retrieve velocity interpolation equation coefficients
                            //so we can solve for the two time values where velocity is zero.
                            Interp.CubicHermiteVelocityCoefs(
                                kf.OutValue, kf.OutTangent, next.InTangent, next.InValue,
                                out float second, out float first, out float zero);

                            if (TMath.QuadraticRealRoots(second, first, zero,
                                out float time1, out float time2))
                            {
                                float val1 = kf.InterpolateNextNormalized(time1);
                                float val2 = kf.InterpolateNextNormalized(time2);
                                min = TMath.Min(min, val1, val2);
                                max = TMath.Max(max, val1, val2);
                            }

                            kf = next;
                        }
                    }
                }
            }
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
        private DelInterpolate _interpolateVelocity = CubicHermiteVelocity;
        private DelInterpolate _interpolateAcceleration = CubicHermiteAcceleration;
        protected PlanarInterpType _interpolationType;
        
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public float InValue { get; set; }
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public float OutValue { get; set; }

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public float InTangent { get; set; }
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public float OutTangent { get; set; }

        [Browsable(false)]
        public new FloatKeyframe Next
        {
            get => _next as FloatKeyframe;
            set => _next = value;
        }
        [Browsable(false)]
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
        /// <summary>
        /// Interpolates from this keyframe to the next using a normalized time value (0.0f - 1.0f)
        /// </summary>
        public float InterpolateNextNormalized(float time) => _interpolate(this, Next, time);
        /// <summary>
        /// Interpolates velocity from this keyframe to the next using a normalized time value (0.0f - 1.0f)
        /// </summary>
        public float InterpolateVelocityNextNormalized(float time) => _interpolateVelocity(this, Next, time);
        /// <summary>
        /// Interpolates acceleration from this keyframe to the next using a normalized time value (0.0f - 1.0f)
        /// </summary>
        public float InterpolateAccelerationNextNormalized(float time) => _interpolateAcceleration(this, Next, time);
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
        public float InterpolateVelocity(float desiredSecond)
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
        public float InterpolateAcceleration(float desiredSecond)
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

        public static float Step(FloatKeyframe key1, FloatKeyframe key2, float time)
            => time < 1.0f ? key1.OutValue : key2.OutValue;
        public static float StepVelocity(FloatKeyframe key1, FloatKeyframe key2, float time)
            => 0.0f;
        public static float StepAcceleration(FloatKeyframe key1, FloatKeyframe key2, float time)
            => 0.0f;

        public static float Lerp(FloatKeyframe key1, FloatKeyframe key2, float time)
            => Interp.Lerp(key1.OutValue, key2.InValue, time);
        public static float LerpVelocity(FloatKeyframe key1, FloatKeyframe key2, float time)
            => (key2.InValue - key1.OutValue) / time;
        public static float LerpAcceleration(FloatKeyframe key1, FloatKeyframe key2, float time)
            => 0.0f;

        public static float CubicBezier(FloatKeyframe key1, FloatKeyframe key2, float time)
            => Interp.CubicBezier(key1.OutValue, key1.OutValue + key1.OutTangent, key2.InValue + key2.InTangent, key2.InValue, time);
        public static float CubicBezierVelocity(FloatKeyframe key1, FloatKeyframe key2, float time)
            => Interp.CubicBezierVelocity(key1.OutValue, key1.OutValue + key1.OutTangent, key2.InValue + key2.InTangent, key2.InValue, time);
        public static float CubicBezierAcceleration(FloatKeyframe key1, FloatKeyframe key2, float time)
            => Interp.CubicBezierAcceleration(key1.OutValue, key1.OutValue + key1.OutTangent, key2.InValue + key2.InTangent, key2.InValue, time);

        public static float CubicHermite(FloatKeyframe key1, FloatKeyframe key2, float time)
            => Interp.CubicHermite(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);
        public static float CubicHermiteVelocity(FloatKeyframe key1, FloatKeyframe key2, float time)
         => Interp.CubicHermiteVelocity(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);
        public static float CubicHermiteAcceleration(FloatKeyframe key1, FloatKeyframe key2, float time)
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

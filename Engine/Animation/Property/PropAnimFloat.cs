using System;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Reflection.Attributes;

namespace TheraEngine.Animation
{
    public class PropAnimFloat : PropAnimVector<float, FloatKeyframe>
    {
        public PropAnimFloat() : base() { }
        public PropAnimFloat(float lengthInSeconds, bool looped, bool useKeyframes)
            : base(lengthInSeconds, looped, useKeyframes) { }
        public PropAnimFloat(int frameCount, float FPS, bool looped, bool useKeyframes)
            : base(frameCount, FPS, looped, useKeyframes) { }

        protected override float LerpValues(float t1, float t2, float time) => Interp.Lerp(t1, t2, time);

        public void GetMinMax(out float min, out float max)
        {
            if (_keyframes.Count == 0)
            {
                min = DefaultValue;
                max = DefaultValue;
            }
            else
            {
                VectorKeyframe<float> kf = _keyframes.First;
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
                            VectorKeyframe<float> next = kf.Next;

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
    }
    public class FloatKeyframe : VectorKeyframe<float>
    {
        public FloatKeyframe()
            : this(0.0f, 0.0f, 0.0f, EPlanarInterpType.CubicBezier) { }
        public FloatKeyframe(int frameIndex, float FPS, float inValue, float outValue, float inTangent, float outTangent, EPlanarInterpType type)
            : this(frameIndex / FPS, inValue, outValue, inTangent, outTangent, type) { }
        public FloatKeyframe(int frameIndex, float FPS, float inoutValue, float inoutTangent, EPlanarInterpType type)
            : this(frameIndex / FPS, inoutValue, inoutValue, inoutTangent, inoutTangent, type) { }
        public FloatKeyframe(float second, float inoutValue, float inoutTangent, EPlanarInterpType type)
            : this(second, inoutValue, inoutValue, inoutTangent, inoutTangent, type) { }
        public FloatKeyframe(float second, float inoutValue, float inTangent, float outTangent, EPlanarInterpType type)
            : this(second, inoutValue, inoutValue, inTangent, outTangent, type) { }
        public FloatKeyframe(float second, float inValue, float outValue, float inTangent, float outTangent, EPlanarInterpType type)
            : base(second, inValue, outValue, inTangent, outTangent, type) { }
        
        public override float Lerp(VectorKeyframe<float> key1, VectorKeyframe<float> key2, float time)
            => Interp.Lerp(key1.OutValue, key2.InValue, time);
        public override float LerpVelocity(VectorKeyframe<float> key1, VectorKeyframe<float> key2, float time)
            => (key2.InValue - key1.OutValue) / time;

        public override float CubicBezier(VectorKeyframe<float> key1, VectorKeyframe<float> key2, float time)
            => Interp.CubicBezier(key1.OutValue, key1.OutValue + key1.OutTangent, key2.InValue + key2.InTangent, key2.InValue, time);
        public override float CubicBezierVelocity(VectorKeyframe<float> key1, VectorKeyframe<float> key2, float time)
            => Interp.CubicBezierVelocity(key1.OutValue, key1.OutValue + key1.OutTangent, key2.InValue + key2.InTangent, key2.InValue, time);
        public override float CubicBezierAcceleration(VectorKeyframe<float> key1, VectorKeyframe<float> key2, float time)
            => Interp.CubicBezierAcceleration(key1.OutValue, key1.OutValue + key1.OutTangent, key2.InValue + key2.InTangent, key2.InValue, time);

        public override float CubicHermite(VectorKeyframe<float> key1, VectorKeyframe<float> key2, float time)
            => Interp.CubicHermite(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);
        public override float CubicHermiteVelocity(VectorKeyframe<float> key1, VectorKeyframe<float> key2, float time)
            => Interp.CubicHermiteVelocity(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);
        public override float CubicHermiteAcceleration(VectorKeyframe<float> key1, VectorKeyframe<float> key2, float time)
            => Interp.CubicHermiteAcceleration(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);

        [GridCallable]
        public override void AverageTangents()
            => InTangent = OutTangent = (InTangent + OutTangent) / 2.0f;
        [GridCallable]
        public override void AverageValues()
            => InValue = OutValue = (InValue + OutValue) / 2.0f;
        [GridCallable]
        public override void MakeOutLinear()
            => OutTangent = (Next.InValue - OutValue) / (Next.Second - Second);
        [GridCallable]
        public override void MakeInLinear()
            => InTangent = (InValue - Prev.OutValue) / (Second - Prev.Second);

        public override string WriteToString()
            => string.Format("{0} {1} {2} {3} {4} {5}", Second, InValue.ToString(), OutValue.ToString(), InTangent.ToString(), OutTangent.ToString(), InterpolationType);

        public override void ReadFromString(string str)
        {
            string[] parts = str.Split(' ');
            Second = float.Parse(parts[0]);
            InValue = float.Parse(parts[1]);
            OutValue = float.Parse(parts[2]);
            InTangent = float.Parse(parts[3]);
            OutTangent = float.Parse(parts[4]);
            InterpolationType = parts[5].AsEnum<EPlanarInterpType>();
        }
        protected override void ParsePlanar(string inValue, string outValue, string inTangent, string outTangent)
        {
            InValue = float.Parse(inValue);
            OutValue = float.Parse(outValue);
            InTangent = float.Parse(inTangent);
            OutTangent = float.Parse(outTangent);
        }
        protected override void WritePlanar(out string inValue, out string outValue, out string inTangent, out string outTangent)
        {
            inValue = InValue.ToString();
            outValue = OutValue.ToString();
            inTangent = InTangent.ToString();
            outTangent = OutTangent.ToString();
        }
    }
}

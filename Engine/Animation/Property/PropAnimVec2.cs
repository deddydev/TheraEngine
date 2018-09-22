using System;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Reflection.Attributes;

namespace TheraEngine.Animation
{
    public class PropAnimVec2 : PropAnimVector<Vec2, Vec2Keyframe>
    {
        public PropAnimVec2() : base() { }
        public PropAnimVec2(float lengthInSeconds, bool looped, bool useKeyframes)
            : base(lengthInSeconds, looped, useKeyframes) { }
        public PropAnimVec2(int frameCount, float FPS, bool looped, bool useKeyframes)
            : base(frameCount, FPS, looped, useKeyframes) { }

        protected override Vec2 LerpValues(Vec2 t1, Vec2 t2, float time) => Vec2.Lerp(t1, t2, time);
        protected override float[] GetComponents(Vec2 value) => new float[] { value.X, value.Y };
        protected override Vec2 GetMaxValue() => new Vec2(float.MaxValue);
        protected override Vec2 GetMinValue() => new Vec2(float.MinValue);
    }
    public class Vec2Keyframe : VectorKeyframe<Vec2>
    {
        public override Vec2 Lerp(VectorKeyframe<Vec2> key1, VectorKeyframe<Vec2> key2, float time)
            => Vec2.Lerp(key1.OutValue, key2.InValue, time);
        public override Vec2 LerpVelocity(VectorKeyframe<Vec2> key1, VectorKeyframe<Vec2> key2, float time)
            => (key2.InValue - key1.OutValue) / time;

        public override Vec2 CubicBezier(VectorKeyframe<Vec2> key1, VectorKeyframe<Vec2> key2, float time)
            => Interp.CubicBezier(key1.OutValue, key1.OutValue + key1.OutTangent, key2.InValue + key2.InTangent, key2.InValue, time);
        public override Vec2 CubicBezierVelocity(VectorKeyframe<Vec2> key1, VectorKeyframe<Vec2> key2, float time)
            => Interp.CubicBezierVelocity(key1.OutValue, key1.OutValue + key1.OutTangent, key2.InValue + key2.InTangent, key2.InValue, time);
        public override Vec2 CubicBezierAcceleration(VectorKeyframe<Vec2> key1, VectorKeyframe<Vec2> key2, float time)
            => Interp.CubicBezierAcceleration(key1.OutValue, key1.OutValue + key1.OutTangent, key2.InValue + key2.InTangent, key2.InValue, time);

        public override Vec2 CubicHermite(VectorKeyframe<Vec2> key1, VectorKeyframe<Vec2> key2, float time)
            => Interp.CubicHermite(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);
        public override Vec2 CubicHermiteVelocity(VectorKeyframe<Vec2> key1, VectorKeyframe<Vec2> key2, float time)
            => Interp.CubicHermiteVelocity(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);
        public override Vec2 CubicHermiteAcceleration(VectorKeyframe<Vec2> key1, VectorKeyframe<Vec2> key2, float time)
            => Interp.CubicHermiteAcceleration(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);
        
        public override string WriteToString()
            => string.Format("{0} {1} {2} {3} {4} {5}", Second, InValue.WriteToString(), OutValue.WriteToString(), InTangent.WriteToString(), OutTangent.WriteToString(), InterpolationType);

        public override void ReadFromString(string str)
        {
            string[] parts = str.Split(' ');
            Second = float.Parse(parts[0]);
            InValue = new Vec2(float.Parse(parts[1]), float.Parse(parts[2]));
            OutValue = new Vec2(float.Parse(parts[3]), float.Parse(parts[4]));
            InTangent = new Vec2(float.Parse(parts[5]), float.Parse(parts[6]));
            OutTangent = new Vec2(float.Parse(parts[7]), float.Parse(parts[8]));
            InterpolationType = parts[9].AsEnum<EPlanarInterpType>();
        }
        protected override void ParsePlanar(string inValue, string outValue, string inTangent, string outTangent)
        {
            InValue = new Vec2(inValue);
            OutValue = new Vec2(outValue);
            InTangent = new Vec2(inTangent);
            OutTangent = new Vec2(outTangent);
        }
        protected override void WritePlanar(out string inValue, out string outValue, out string inTangent, out string outTangent)
        {
            inValue = InValue.ToString("", "", " ");
            outValue = OutValue.ToString("", "", " ");
            inTangent = InTangent.ToString("", "", " ");
            outTangent = OutTangent.ToString("", "", " ");
        }

        [GridCallable]
        public override void MakeOutLinear()
        {
            var next = Next;
            float span;
            if (next == null)
            {
                if (OwningTrack != null && OwningTrack.FirstKey != this)
                {
                    next = (VectorKeyframe<Vec2>)OwningTrack.FirstKey;
                    span = OwningTrack.LengthInSeconds - Second + next.Second;
                }
                else
                    return;
            }
            else
                span = next.Second - Second;
            OutTangent = (next.InValue - OutValue) / span;
        }
        [GridCallable]
        public override void MakeInLinear()
        {
            var prev = Prev;
            float span;
            if (prev == null)
            {
                if (OwningTrack != null && OwningTrack.LastKey != this)
                {
                    prev = (VectorKeyframe<Vec2>)OwningTrack.LastKey;
                    span = OwningTrack.LengthInSeconds - prev.Second + Second;
                }
                else
                    return;
            }
            else
                span = Second - prev.Second;
            InTangent = (InValue - prev.OutValue) / span;
        }

        [GridCallable]
        public override void UnifyTangents(EUnifyBias bias)
        {
            switch (bias)
            {
                case EUnifyBias.Average:
                    InTangent = OutTangent = (InTangent + OutTangent) / 2.0f;
                    break;
                case EUnifyBias.In:
                    OutTangent = InTangent;
                    break;
                case EUnifyBias.Out:
                    InTangent = OutTangent;
                    break;
            }
        }
        [GridCallable]
        public override void UnifyTangentDirections(EUnifyBias bias)
        {
            switch (bias)
            {
                case EUnifyBias.Average:
                    {
                        float inLength = InTangent.Length;
                        float outLength = OutTangent.Length;
                        Vec2 inTan = InTangent.Normalized();
                        Vec2 outTan = OutTangent.Normalized();
                        Vec2 avg = (-inTan + outTan) * 0.5f;
                        avg.Normalize();
                        InTangent = -avg * inLength;
                        OutTangent = avg * outLength;
                    }
                    break;
                case EUnifyBias.In:
                    {
                        float outLength = OutTangent.Length;
                        Vec2 inTan = InTangent.Normalized();
                        OutTangent = -inTan * outLength;
                    }
                    break;
                case EUnifyBias.Out:
                    {
                        float inLength = InTangent.Length;
                        Vec2 outTan = OutTangent.Normalized();
                        InTangent = -outTan * inLength;
                    }
                    break;
            }
        }
        [GridCallable]
        public override void UnifyTangentMagnitudes(EUnifyBias bias)
        {
            switch (bias)
            {
                case EUnifyBias.Average:
                    {
                        float inLength = InTangent.Length;
                        float outLength = OutTangent.Length;
                        float avgLength = (inLength + outLength) * 0.5f;
                        Vec2 inTan = InTangent.Normalized();
                        Vec2 outTan = OutTangent.Normalized();
                        InTangent = inTan * avgLength;
                        OutTangent = outTan * avgLength;
                    }
                    break;
                case EUnifyBias.In:
                    {
                        float inLength = InTangent.Length;
                        Vec2 outTan = OutTangent.Normalized();
                        OutTangent = -outTan * inLength;
                        break;
                    }
                case EUnifyBias.Out:
                    {
                        float outLength = OutTangent.Length;
                        Vec2 inTan = InTangent.Normalized();
                        InTangent = -inTan * outLength;
                        break;
                    }
            }
        }
        [GridCallable]
        public override void UnifyValues(EUnifyBias bias)
        {
            switch (bias)
            {
                case EUnifyBias.Average:
                    InValue = OutValue = (InValue + OutValue) * 0.5f;
                    break;
                case EUnifyBias.In:
                    OutValue = InValue;
                    break;
                case EUnifyBias.Out:
                    InValue = OutValue;
                    break;
            }
        }
    }
}

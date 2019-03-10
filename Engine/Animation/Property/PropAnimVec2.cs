using System;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Maths.Transforms;
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
        protected override float GetVelocityMagnitude()
        {
            Vec2 b = CurrentVelocity;
            float a = 1.0f;
            Vec3 start = Vec3.Zero;
            Vec3 end = new Vec3(a, b);
            return start.DistanceTo(end);
        }
    }
    public class Vec2Keyframe : VectorKeyframe<Vec2>
    {
        public Vec2Keyframe()
            : this(0.0f, 0.0f, 0.0f, EVectorInterpType.CubicBezier) { }
        public Vec2Keyframe(int frameIndex, float FPS, Vec2 inValue, Vec2 outValue, Vec2 inTangent, Vec2 outTangent, EVectorInterpType type)
            : this(frameIndex / FPS, inValue, outValue, inTangent, outTangent, type) { }
        public Vec2Keyframe(int frameIndex, float FPS, Vec2 inoutValue, Vec2 inoutTangent, EVectorInterpType type)
            : this(frameIndex / FPS, inoutValue, inoutValue, inoutTangent, inoutTangent, type) { }
        public Vec2Keyframe(float second, Vec2 inoutValue, Vec2 inoutTangent, EVectorInterpType type)
            : this(second, inoutValue, inoutValue, inoutTangent, inoutTangent, type) { }
        public Vec2Keyframe(float second, Vec2 inoutValue, Vec2 inTangent, Vec2 outTangent, EVectorInterpType type)
            : this(second, inoutValue, inoutValue, inTangent, outTangent, type) { }
        public Vec2Keyframe(float second, Vec2 inValue, Vec2 outValue, Vec2 inTangent, Vec2 outTangent, EVectorInterpType type)
            : base(second, inValue, outValue, inTangent, outTangent, type) { }

        public override Vec2 Lerp(VectorKeyframe<Vec2> next, float diff, float span)
          => Interp.Lerp(OutValue, next.InValue, span == 0 ? 0.0f : diff / span);
        public override Vec2 LerpVelocity(VectorKeyframe<Vec2> next, float diff, float span)
            => span == 0 ? 0.0f : (next.InValue - OutValue) / (diff / span);

        public override Vec2 CubicBezier(VectorKeyframe<Vec2> next, float diff, float span)
            => Interp.CubicBezier(OutValue, OutValue + OutTangent * span, next.InValue + next.InTangent * span, next.InValue, span == 0 ? 0.0f : diff / span);
        public override Vec2 CubicBezierVelocity(VectorKeyframe<Vec2> next, float diff, float span)
            => Interp.CubicBezierVelocity(OutValue, OutValue + OutTangent * span, next.InValue + next.InTangent * span, next.InValue, span == 0 ? 0.0f : diff / span);
        public override Vec2 CubicBezierAcceleration(VectorKeyframe<Vec2> next, float diff, float span)
            => Interp.CubicBezierAcceleration(OutValue, OutValue + OutTangent * span, next.InValue + next.InTangent * span, next.InValue, span == 0 ? 0.0f : diff / span);

        public override Vec2 CubicHermite(VectorKeyframe<Vec2> next, float diff, float span)
            => Interp.CubicHermite(OutValue, OutTangent * span, -next.InTangent * span, next.InValue, span == 0 ? 0.0f : diff / span);
        public override Vec2 CubicHermiteVelocity(VectorKeyframe<Vec2> next, float diff, float span)
            => Interp.CubicHermiteVelocity(OutValue, OutTangent * span, -next.InTangent * span, next.InValue, span == 0 ? 0.0f : diff / span);
        public override Vec2 CubicHermiteAcceleration(VectorKeyframe<Vec2> next, float diff, float span)
            => Interp.CubicHermiteAcceleration(OutValue, OutTangent * span, -next.InTangent * span, next.InValue, span == 0 ? 0.0f : diff / span);

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
            InterpolationType = parts[9].AsEnum<EVectorInterpType>();
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
                    InTangent = -(OutTangent = (-InTangent + OutTangent) * 0.5f);
                    break;
                case EUnifyBias.In:
                    OutTangent = -InTangent;
                    break;
                case EUnifyBias.Out:
                    InTangent = -OutTangent;
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
        public void GenerateTangents()
        {
            //var next = GetNextKeyframe(out float nextSpan);
            //var prev = GetPrevKeyframe(out float prevSpan);

            //if (Math.Abs(InValue - OutValue) < 0.0001f)
            //{
            //    float tangent = 0.0f;
            //    float weightCount = 0;
            //    if (prev != null && prevSpan > 0.0f)
            //    {
            //        tangent += (InValue - prev.OutValue) / prevSpan;
            //        weightCount++;
            //    }
            //    if (next != null && nextSpan > 0.0f)
            //    {
            //        tangent += (next.InValue - OutValue) / nextSpan;
            //        weightCount++;
            //    }

            //    if (weightCount > 0)
            //        tangent /= weightCount;

            //    OutTangent = tangent;
            //    InTangent = -tangent;
            //}
            //else
            //{
            //    if (prev != null && prevSpan > 0.0f)
            //    {
            //        InTangent = -(InValue - prev.OutValue) / prevSpan;
            //    }
            //    if (next != null && nextSpan > 0.0f)
            //    {
            //        OutTangent = (next.InValue - OutValue) / nextSpan;
            //    }
            //}
        }
        public void GenerateOutTangent()
        {
            var next = GetNextKeyframe(out float nextSpan);
            if (next != null && nextSpan > 0.0f)
            {
                OutTangent = (next.InValue - OutValue) / nextSpan;
            }

            //var next = GetNextKeyframe(out float span1);
            //var prev = GetPrevKeyframe(out float span2);
            //float valueDiff = (next?.InValue ?? InValue) - (prev?.OutValue ?? OutValue);
            //float secDiff = (next?.Second ?? Second) - (prev?.Second ?? Second);
            //if (secDiff != 0.0f)
            //    OutTangent = valueDiff / secDiff;
        }
        public void GenerateInTangent()
        {
            var prev = GetPrevKeyframe(out float prevSpan);
            if (prev != null && prevSpan > 0.0f)
            {
                InTangent = -(InValue - prev.OutValue) / prevSpan;
            }

            //var next = GetNextKeyframe(out float span1);
            //var prev = GetPrevKeyframe(out float span2);
            //float valueDiff = (next?.InValue ?? InValue) - (prev?.OutValue ?? OutValue);
            //float secDiff = (next?.Second ?? Second) - (prev?.Second ?? Second);
            //if (secDiff != 0.0f)
            //    InTangent = -valueDiff / secDiff;
        }
        public void GenerateAdjacentTangents(bool prev, bool next)
        {
            if (prev)
            {
                var prevkf = GetPrevKeyframe(out float span2) as Vec2Keyframe;
                prevkf?.GenerateTangents();
                GenerateInTangent();
            }
            if (next)
            {
                var nextKf = GetNextKeyframe(out float span1) as Vec2Keyframe;
                nextKf?.GenerateTangents();
                GenerateOutTangent();
            }
        }
    }
}

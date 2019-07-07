using Extensions;
using System;
using System.ComponentModel;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Reflection.Attributes;

namespace TheraEngine.Animation
{
    public class PropAnimVec4 : PropAnimVector<Vec4, Vec4Keyframe>
    {
        public PropAnimVec4() : base() { }
        public PropAnimVec4(float lengthInSeconds, bool looped, bool useKeyframes)
            : base(lengthInSeconds, looped, useKeyframes) { }
        public PropAnimVec4(int frameCount, float FPS, bool looped, bool useKeyframes)
            : base(frameCount, FPS, looped, useKeyframes) { }

        protected override Vec4 LerpValues(Vec4 t1, Vec4 t2, float time) => Vec4.Lerp(t1, t2, time);
        protected override float[] GetComponents(Vec4 value) => new float[] { value.X, value.Y, value.Z, value.W };
        protected override Vec4 GetMaxValue() => new Vec4(float.MaxValue);
        protected override Vec4 GetMinValue() => new Vec4(float.MinValue);
        protected override float GetVelocityMagnitude()
        {
            Vec4 b = CurrentVelocity;
            float a = 1.0f;
            Vec5 start = Vec5.Zero;
            Vec5 end = new Vec5(a, b.X, b.Y, b.Z, b.W);
            return start.DistanceTo(end);
        }
        private struct Vec5
        {
            public float X, Y, Z, U, V;

            public static readonly Vec5 Zero = new Vec5();

            public Vec5(float x, float y, float z, float u, float v)
            {
                X = x;
                Y = y;
                Z = z;
                U = u;
                V = v;
            }

            [Browsable(false)]
            public float LengthSquared => X * X + Y * Y + Z * Z + U * U + V * V;
            [Browsable(false)]
            public float Length => (float)Math.Sqrt(LengthSquared);
            [Browsable(false)]
            public float LengthFast => 1.0f / TMath.InverseSqrtFast(LengthSquared);

            public float DistanceTo(Vec5 point) => (point - this).Length;
            public float DistanceToFast(Vec5 point) => (point - this).LengthFast;
            public float DistanceToSquared(Vec5 point) => (point - this).LengthSquared;

            public static Vec5 operator -(Vec5 left, Vec5 right)
                => new Vec5(
                    left.X - right.X,
                    left.Y - right.Y,
                    left.Z - right.Z,
                    left.U - right.U,
                    left.V - right.V);
        }
    }
    public class Vec4Keyframe : VectorKeyframe<Vec4>
    {
        public override Vec4 Lerp(VectorKeyframe<Vec4> next, float diff, float span)
            => Interp.Lerp(OutValue, next.InValue, diff / span);
        public override Vec4 LerpVelocity(VectorKeyframe<Vec4> next, float diff, float span)
            => (next.InValue - OutValue) / (diff / span);

        public override Vec4 CubicBezier(VectorKeyframe<Vec4> next, float diff, float span)
            => Interp.CubicBezier(OutValue, OutValue + OutTangent * span, next.InValue + next.InTangent * span, next.InValue, diff / span);
        public override Vec4 CubicBezierVelocity(VectorKeyframe<Vec4> next, float diff, float span)
            => Interp.CubicBezierVelocity(OutValue, OutValue + OutTangent * span, next.InValue + next.InTangent * span, next.InValue, diff / span);
        public override Vec4 CubicBezierAcceleration(VectorKeyframe<Vec4> next, float diff, float span)
            => Interp.CubicBezierAcceleration(OutValue, OutValue + OutTangent * span, next.InValue + next.InTangent * span, next.InValue, diff / span);

        public override Vec4 CubicHermite(VectorKeyframe<Vec4> next, float diff, float span)
            => Interp.CubicHermite(OutValue, OutTangent * span, -next.InTangent * span, next.InValue, diff / span);
        public override Vec4 CubicHermiteVelocity(VectorKeyframe<Vec4> next, float diff, float span)
            => Interp.CubicHermiteVelocity(OutValue, OutTangent * span, -next.InTangent * span, next.InValue, diff / span);
        public override Vec4 CubicHermiteAcceleration(VectorKeyframe<Vec4> next, float diff, float span)
            => Interp.CubicHermiteAcceleration(OutValue, OutTangent * span, -next.InTangent * span, next.InValue, diff / span);

        public override string WriteToString() 
            => string.Format("{0} {1} {2} {3} {4} {5}", Second, InValue.WriteToString(), OutValue.WriteToString(), InTangent.WriteToString(), OutTangent.WriteToString(), InterpolationType);
        
        public override void ReadFromString(string str)
        {
            string[] parts = str.Split(' ');
            Second = float.Parse(parts[0]);
            InValue = new Vec4(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]), float.Parse(parts[4]));
            OutValue = new Vec4(float.Parse(parts[5]), float.Parse(parts[6]), float.Parse(parts[7]), float.Parse(parts[8]));
            InTangent = new Vec4(float.Parse(parts[9]), float.Parse(parts[10]), float.Parse(parts[11]), float.Parse(parts[12]));
            OutTangent = new Vec4(float.Parse(parts[13]), float.Parse(parts[14]), float.Parse(parts[15]), float.Parse(parts[16]));
            InterpolationType = parts[17].AsEnum<EVectorInterpType>();
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
                    next = (VectorKeyframe<Vec4>)OwningTrack.FirstKey;
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
                    prev = (VectorKeyframe<Vec4>)OwningTrack.LastKey;
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
                        Vec4 inTan = InTangent.Normalized();
                        Vec4 outTan = OutTangent.Normalized();
                        Vec4 avg = (-inTan + outTan) * 0.5f;
                        avg.Normalize();
                        InTangent = -avg * inLength;
                        OutTangent = avg * outLength;
                    }
                    break;
                case EUnifyBias.In:
                    {
                        float outLength = OutTangent.Length;
                        Vec4 inTan = InTangent.Normalized();
                        OutTangent = -inTan * outLength;
                    }
                    break;
                case EUnifyBias.Out:
                    {
                        float inLength = InTangent.Length;
                        Vec4 outTan = OutTangent.Normalized();
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
                        Vec4 inTan = InTangent.Normalized();
                        Vec4 outTan = OutTangent.Normalized();
                        InTangent = inTan * avgLength;
                        OutTangent = outTan * avgLength;
                    }
                    break;
                case EUnifyBias.In:
                    {
                        float inLength = InTangent.Length;
                        Vec4 outTan = OutTangent.Normalized();
                        OutTangent = -outTan * inLength;
                        break;
                    }
                case EUnifyBias.Out:
                    {
                        float outLength = OutTangent.Length;
                        Vec4 inTan = InTangent.Normalized();
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

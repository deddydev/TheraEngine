using Extensions;
using System;
using TheraEngine.Core.Maths;

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
        protected override float[] GetComponents(float value) => new float[] { value };
        protected override float GetMaxValue() => float.MaxValue;
        protected override float GetMinValue() => float.MinValue;
        protected override float GetVelocityMagnitude()
        {
            float b = CurrentVelocity;
            float a = 1.0f;
            Vec2 start = new Vec2(0.0f, 0.0f);
            Vec2 end = new Vec2(a, b);
            return start.DistanceTo(end);
        }
    }
    public class FloatKeyframe : VectorKeyframe<float>
    {
        public FloatKeyframe()
            : this(0.0f, 0.0f, 0.0f, EVectorInterpType.CubicBezier) { }
        public FloatKeyframe(int frameIndex, float FPS, float inValue, float outValue, float inTangent, float outTangent, EVectorInterpType type)
            : this(frameIndex / FPS, inValue, outValue, inTangent, outTangent, type) { }
        public FloatKeyframe(int frameIndex, float FPS, float inoutValue, float inoutTangent, EVectorInterpType type)
            : this(frameIndex / FPS, inoutValue, inoutValue, inoutTangent, inoutTangent, type) { }
        public FloatKeyframe(float second, float inoutValue, float inoutTangent, EVectorInterpType type)
            : this(second, inoutValue, inoutValue, inoutTangent, inoutTangent, type) { }
        public FloatKeyframe(float second, float inoutValue, float inTangent, float outTangent, EVectorInterpType type)
            : this(second, inoutValue, inoutValue, inTangent, outTangent, type) { }
        public FloatKeyframe(float second, float inValue, float outValue, float inTangent, float outTangent, EVectorInterpType type)
            : base(second, inValue, outValue, inTangent, outTangent, type) { }

        public override float Lerp(VectorKeyframe<float> next, float diff, float span)
            => Interp.Lerp(OutValue, next.InValue, span.IsZero() ? 0.0f : diff / span);
        public override float LerpVelocity(VectorKeyframe<float> next, float diff, float span)
            => span.IsZero() ? 0.0f : (next.InValue - OutValue) / (diff / span);

        public override float CubicBezier(VectorKeyframe<float> next, float diff, float span)
            => Interp.CubicBezier(OutValue, OutValue + OutTangent * span, next.InValue + next.InTangent * span, next.InValue, span.IsZero() ? 0.0f : diff / span);
        public override float CubicBezierVelocity(VectorKeyframe<float> next, float diff, float span)
            => span.IsZero() ? 0.0f : Interp.CubicBezierVelocity(OutValue, OutValue + OutTangent * span, next.InValue + next.InTangent * span, next.InValue, diff / span) / span;
        public override float CubicBezierAcceleration(VectorKeyframe<float> next, float diff, float span)
            => span.IsZero() ? 0.0f : Interp.CubicBezierAcceleration(OutValue, OutValue + OutTangent * span, next.InValue + next.InTangent * span, next.InValue, diff / span) / (span * span);
        
        public override float CubicHermite(VectorKeyframe<float> next, float diff, float span)
            => Interp.CubicHermite(OutValue, OutTangent * span, -next.InTangent * span, next.InValue, span.IsZero() ? 0.0f : diff / span);
        public override float CubicHermiteVelocity(VectorKeyframe<float> next, float diff, float span)
            => span.IsZero() ? 0.0f : Interp.CubicHermiteVelocity(OutValue, OutTangent * span, -next.InTangent * span, next.InValue, diff / span) / span;
        public override float CubicHermiteAcceleration(VectorKeyframe<float> next, float diff, float span)
            => span.IsZero() ? 0.0f : Interp.CubicHermiteAcceleration(OutValue, OutTangent * span, -next.InTangent * span, next.InValue, diff / span) / (span * span);
        
        public override string WriteToString()
            => string.Format("{0} {1} {2} {3} {4} {5}", Second, InValue.ToString(), OutValue.ToString(), InTangent.ToString(), OutTangent.ToString(), InterpolationType);

        public override string ToString()
        {
            switch (InterpolationType)
            {
                case EVectorInterpType.Step:
                    return string.Format("[F:{0} : {3}] V:({1} {2})", Second, InValue.ToString(), OutValue.ToString(), InterpolationType);
                case EVectorInterpType.Linear:
                    return string.Format("[F:{0} : {3}] V:({1} {2})", Second, InValue.ToString(), OutValue.ToString(), InterpolationType);
                case EVectorInterpType.CubicHermite:
                    return string.Format("[F:{0} : {5}] V:({1} {2}) T:({3} {4})", Second, InValue.ToString(), OutValue.ToString(), InTangent.ToString(), OutTangent.ToString(), InterpolationType);
                default:
                case EVectorInterpType.CubicBezier:
                    return string.Format("[F:{0} : {5}] V:({1} {2}) T:({3} {4})", Second, InValue.ToString(), OutValue.ToString(), InTangent.ToString(), OutTangent.ToString(), InterpolationType);
            }
        }

        public override void ReadFromString(string str)
        {
            string[] parts = str.Split(' ');
            Second = float.Parse(parts[0]);
            InValue = float.Parse(parts[1]);
            OutValue = float.Parse(parts[2]);
            InTangent = float.Parse(parts[3]);
            OutTangent = float.Parse(parts[4]);
            InterpolationType = parts[5].AsEnum<EVectorInterpType>();
        }
        
        public override void MakeOutLinear()
        {
            var next = Next;
            float span;
            if (next is null)
            {
                if (OwningTrack != null && OwningTrack.FirstKey != this)
                {
                    next = (VectorKeyframe<float>)OwningTrack.FirstKey;
                    span = OwningTrack.LengthInSeconds - Second + next.Second;
                }
                else
                    return;
            }
            else
                span = next.Second - Second;
            OutTangent = (next.InValue - OutValue) / span;
        }
        public override void MakeInLinear()
        {
            var prev = Prev;
            float span;
            if (prev is null)
            {
                if (OwningTrack != null && OwningTrack.LastKey != this)
                {
                    prev = (VectorKeyframe<float>)OwningTrack.LastKey;
                    span = OwningTrack.LengthInSeconds - prev.Second + Second;
                }
                else
                    return;
            }
            else
                span = Second - prev.Second;
            InTangent = -(InValue - prev.OutValue) / span;
        }

        public override void UnifyTangentDirections(EUnifyBias bias) => UnifyTangents(bias);
        public override void UnifyTangentMagnitudes(EUnifyBias bias) => UnifyTangents(bias);
        
        public override void UnifyTangents(EUnifyBias bias)
        {
            switch (bias)
            {
                case EUnifyBias.Average:
                    float avg = (-InTangent + OutTangent) * 0.5f;
                    OutTangent = avg;
                    InTangent = -avg;
                    break;
                case EUnifyBias.In:
                    OutTangent = -InTangent;
                    break;
                case EUnifyBias.Out:
                    InTangent = -OutTangent;
                    break;
            }
        }
        public override void UnifyValues(EUnifyBias bias)
        {
            switch (bias)
            {
                case EUnifyBias.Average:
                    InValue = OutValue = (InValue + OutValue) / 2.0f;
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
            var next = GetNextKeyframe(out float nextSpan);
            var prev = GetPrevKeyframe(out float prevSpan);

            if (Math.Abs(InValue - OutValue) < 0.0001f)
            {
                float tangent = 0.0f;
                float weightCount = 0;
                if (prev != null && prevSpan > 0.0f)
                {
                    tangent += (InValue - prev.OutValue) / prevSpan;
                    weightCount++;
                }
                if (next != null && nextSpan > 0.0f)
                {
                    tangent += (next.InValue - OutValue) / nextSpan;
                    weightCount++;
                }

                if (weightCount > 0)
                    tangent /= weightCount;

                OutTangent = tangent;
                InTangent = -tangent;
            }
            else
            {
                if (prev != null && prevSpan > 0.0f)
                {
                    InTangent = -(InValue - prev.OutValue) / prevSpan;
                }
                if (next != null && nextSpan > 0.0f)
                {
                    OutTangent = (next.InValue - OutValue) / nextSpan;
                }
            }

            //float valueDiff = (next?.InValue ?? InValue) - (prev?.OutValue ?? OutValue);
            //float secDiff = (next?.Second ?? Second) - (prev?.Second ?? Second);
            //if (secDiff != 0.0f)
            //    InTangent = -(OutTangent = valueDiff / secDiff);
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
                var prevkf = GetPrevKeyframe(out float span2) as FloatKeyframe;
                prevkf?.GenerateTangents();
                GenerateInTangent();
            }
            if (next)
            {
                var nextKf = GetNextKeyframe(out float span1) as FloatKeyframe;
                nextKf?.GenerateTangents();
                GenerateOutTangent();
            }
        }
    }
}

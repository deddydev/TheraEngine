using Extensions;
using System;
using System.Collections.Generic;
using TheraEngine.Core.Maths;

namespace TheraEngine.Animation
{
    public class PropAnimInt : PropAnimVector<int, IntKeyframe>, IEnumerable<IntKeyframe>
    {
        public PropAnimInt() : base() { }
        public PropAnimInt(float lengthInSeconds, bool looped, bool useKeyframes)
            : base(lengthInSeconds, looped, useKeyframes) { }
        public PropAnimInt(int frameCount, float FPS, bool looped, bool useKeyframes)
            : base(frameCount, FPS, looped, useKeyframes) { }

        //public bool UseConstantVelocity { get; set; } = true;
        //public float ConstantVelocitySpeed { get; set; } = 1.0f;
        
        protected override int LerpValues(int t1, int t2, float time) => (int)Interp.Lerp(t1, t2, time);
        protected override float[] GetComponents(int value) => new float[] { value };
        protected override int GetMaxValue() => int.MaxValue;
        protected override int GetMinValue() => int.MinValue;
        protected override float GetVelocityMagnitude()
        {
            float b = CurrentVelocity;
            float a = 1.0f;
            Vec2 start = new Vec2(0.0f, 0.0f);
            Vec2 end = new Vec2(a, b);
            return start.DistanceTo(end);
        }
    }
    public class IntKeyframe : VectorKeyframe<int>
    {
        public IntKeyframe()
            : this(0.0f, 0, 0, EVectorInterpType.CubicBezier) { }
        public IntKeyframe(int frameIndex, float FPS, int inValue, int outValue, int inTangent, int outTangent, EVectorInterpType type)
            : this(frameIndex / FPS, inValue, outValue, inTangent, outTangent, type) { }
        public IntKeyframe(int frameIndex, float FPS, int inoutValue, int inoutTangent, EVectorInterpType type)
            : this(frameIndex / FPS, inoutValue, inoutValue, inoutTangent, inoutTangent, type) { }
        public IntKeyframe(float second, int inoutValue, int inoutTangent, EVectorInterpType type)
            : this(second, inoutValue, inoutValue, inoutTangent, inoutTangent, type) { }
        public IntKeyframe(float second, int inoutValue, int inTangent, int outTangent, EVectorInterpType type)
            : this(second, inoutValue, inoutValue, inTangent, outTangent, type) { }
        public IntKeyframe(float second, int inValue, int outValue, int inTangent, int outTangent, EVectorInterpType type)
            : base(second, inValue, outValue, inTangent, outTangent, type) { }
        
        public override int Lerp(VectorKeyframe<int> next, float diff, float span)
            => (int)Interp.Lerp(OutValue, next.InValue, diff / span);
        public override int LerpVelocity(VectorKeyframe<int> next, float diff, float span)
            => (int)((next.InValue - OutValue) / (diff / span));

        public override int CubicBezier(VectorKeyframe<int> next, float diff, float span)
            => (int)Interp.CubicBezier(OutValue, OutValue + OutTangent * span, next.InValue + next.InTangent * span, next.InValue, diff / span);
        public override int CubicBezierVelocity(VectorKeyframe<int> next, float diff, float span)
            => (int)Interp.CubicBezierVelocity(OutValue, OutValue + OutTangent * span, next.InValue + next.InTangent * span, next.InValue, diff / span);
        public override int CubicBezierAcceleration(VectorKeyframe<int> next, float diff, float span)
            => (int)Interp.CubicBezierAcceleration(OutValue, OutValue + OutTangent * span, next.InValue + next.InTangent * span, next.InValue, diff / span);

        public override int CubicHermite(VectorKeyframe<int> next, float diff, float span)
            => (int)Interp.CubicHermite(OutValue, OutTangent * span, -next.InTangent * span, next.InValue, diff / span);
        public override int CubicHermiteVelocity(VectorKeyframe<int> next, float diff, float span)
            => (int)Interp.CubicHermiteVelocity(OutValue, OutTangent * span, -next.InTangent * span, next.InValue, diff / span);
        public override int CubicHermiteAcceleration(VectorKeyframe<int> next, float diff, float span)
            => (int)Interp.CubicHermiteAcceleration(OutValue, OutTangent * span, -next.InTangent * span, next.InValue, diff / span);

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
            InValue = int.Parse(parts[1]);
            OutValue = int.Parse(parts[2]);
            InTangent = int.Parse(parts[3]);
            OutTangent = int.Parse(parts[4]);
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
                    next = (VectorKeyframe<int>)OwningTrack.FirstKey;
                    span = OwningTrack.LengthInSeconds - Second + next.Second;
                }
                else
                    return;
            }
            else
                span = next.Second - Second;
            OutTangent = (int)((next.InValue - OutValue) / span);
        }
        public override void MakeInLinear()
        {
            var prev = Prev;
            float span;
            if (prev is null)
            {
                if (OwningTrack != null && OwningTrack.LastKey != this)
                {
                    prev = (VectorKeyframe<int>)OwningTrack.LastKey;
                    span = OwningTrack.LengthInSeconds - prev.Second + Second;
                }
                else
                    return;
            }
            else
                span = Second - prev.Second;
            InTangent = (int)(-(InValue - prev.OutValue) / span);
        }

        public override void UnifyTangentDirections(EUnifyBias bias) => UnifyTangents(bias);
        public override void UnifyTangentMagnitudes(EUnifyBias bias) => UnifyTangents(bias);

        public override void UnifyTangents(EUnifyBias bias)
        {
            switch (bias)
            {
                case EUnifyBias.Average:
                    float avg = (-InTangent + OutTangent) * 0.5f;
                    OutTangent = (int)avg;
                    InTangent = (int)-avg;
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
                    InValue = OutValue = (int)((InValue + OutValue) / 2.0f);
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

                OutTangent = (int)tangent;
                InTangent = (int)-tangent;
            }
            else
            {
                if (prev != null && prevSpan > 0.0f)
                {
                    InTangent = (int)(-(InValue - prev.OutValue) / prevSpan);
                }
                if (next != null && nextSpan > 0.0f)
                {
                    OutTangent = (int)((next.InValue - OutValue) / nextSpan);
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
                OutTangent = (int)((next.InValue - OutValue) / nextSpan);
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
                InTangent = (int)(-(InValue - prev.OutValue) / prevSpan);
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
                var prevkf = GetPrevKeyframe(out float span2) as IntKeyframe;
                prevkf?.GenerateOutTangent();
                GenerateInTangent();
            }
            if (next)
            {
                var nextKf = GetNextKeyframe(out float span1) as IntKeyframe;
                nextKf?.GenerateInTangent();
                GenerateOutTangent();
            }
        }
    }
}

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

        //public bool UseConstantVelocity { get; set; } = true;
        //public float ConstantVelocitySpeed { get; set; } = 1.0f;

        protected override float LerpValues(float t1, float t2, float time) => Interp.Lerp(t1, t2, time);
        protected override float[] GetComponents(float value) => new float[] { value };
        protected override float GetMaxValue() => float.MaxValue;
        protected override float GetMinValue() => float.MinValue;
        //public override void Progress(float delta)
        //{
        //    if (UseConstantVelocity)
        //    {
        //        float b = CurrentVelocity;
        //        float a = 1.0f;
        //        Vec2 start = new Vec2(0.0f, 0.0f);
        //        Vec2 end = new Vec2(a, b);
        //        float c = start.DistanceTo(end);
        //        float triangleSizeRatio = ConstantVelocitySpeed / c;
        //        Speed = triangleSizeRatio;
        //    }
        //    base.Progress(delta);
        //}
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
            => Interp.Lerp(OutValue, next.InValue, diff / span);
        public override float LerpVelocity(VectorKeyframe<float> next, float diff, float span)
            => (next.InValue - OutValue) / (diff / span);

        public override float CubicBezier(VectorKeyframe<float> next, float diff, float span)
            => Interp.CubicBezier(OutValue, OutValue + OutTangent * span, next.InValue + next.InTangent * span, next.InValue, diff / span);
        public override float CubicBezierVelocity(VectorKeyframe<float> next, float diff, float span)
            => Interp.CubicBezierVelocity(OutValue, OutValue + OutTangent * span, next.InValue + next.InTangent * span, next.InValue, diff / span);
        public override float CubicBezierAcceleration(VectorKeyframe<float> next, float diff, float span)
            => Interp.CubicBezierAcceleration(OutValue, OutValue + OutTangent * span, next.InValue + next.InTangent * span, next.InValue, diff / span);
        
        public override float CubicHermite(VectorKeyframe<float> next, float diff, float span)
            => Interp.CubicHermite(OutValue, OutTangent * span, -next.InTangent * span, next.InValue, diff / span);
        public override float CubicHermiteVelocity(VectorKeyframe<float> next, float diff, float span)
            => Interp.CubicHermiteVelocity(OutValue, OutTangent * span, -next.InTangent * span, next.InValue, diff / span);
        public override float CubicHermiteAcceleration(VectorKeyframe<float> next, float diff, float span)
            => Interp.CubicHermiteAcceleration(OutValue, OutTangent * span, -next.InTangent * span, next.InValue, diff / span);
        
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
            if (next == null)
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
            if (prev == null)
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
            float valueDiff = (Next?.InValue ?? InValue) - (Prev?.OutValue ?? OutValue);
            float secDiff = (Next?.Second ?? Second) - (Prev?.Second ?? Second);
            if (secDiff != 0.0f)
                InTangent = -(OutTangent = valueDiff / secDiff);
        }
    }
}

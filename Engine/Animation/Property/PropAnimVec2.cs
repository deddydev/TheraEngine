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
    }
}

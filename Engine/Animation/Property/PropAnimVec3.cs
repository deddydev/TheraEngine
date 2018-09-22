using System;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Reflection.Attributes;

namespace TheraEngine.Animation
{
    public class PropAnimVec3 : PropAnimVector<Vec3, Vec3Keyframe>
    {
        public PropAnimVec3() : base() { }
        public PropAnimVec3(float lengthInSeconds, bool looped, bool useKeyframes)
            : base(lengthInSeconds, looped, useKeyframes) { }
        public PropAnimVec3(int frameCount, float FPS, bool looped, bool useKeyframes)
            : base(frameCount, FPS, looped, useKeyframes) { }

        protected override Vec3 LerpValues(Vec3 t1, Vec3 t2, float time) => Vec3.Lerp(t1, t2, time);
    }
    public class Vec3Keyframe : VectorKeyframe<Vec3>
    {
        public override Vec3 Lerp(VectorKeyframe<Vec3> key1, VectorKeyframe<Vec3> key2, float time)
            => Vec3.Lerp(key1.OutValue, key2.InValue, time);
        public override Vec3 LerpVelocity(VectorKeyframe<Vec3> key1, VectorKeyframe<Vec3> key2, float time)
            => (key2.InValue - key1.OutValue) / time;
        
        public override Vec3 CubicBezier(VectorKeyframe<Vec3> key1, VectorKeyframe<Vec3> key2, float time)
            => Interp.CubicBezier(key1.OutValue, key1.OutValue + key1.OutTangent, key2.InValue + key2.InTangent, key2.InValue, time);
        public override Vec3 CubicBezierVelocity(VectorKeyframe<Vec3> key1, VectorKeyframe<Vec3> key2, float time)
            => Interp.CubicBezierVelocity(key1.OutValue, key1.OutValue + key1.OutTangent, key2.InValue + key2.InTangent, key2.InValue, time);
        public override Vec3 CubicBezierAcceleration(VectorKeyframe<Vec3> key1, VectorKeyframe<Vec3> key2, float time)
            => Interp.CubicBezierAcceleration(key1.OutValue, key1.OutValue + key1.OutTangent, key2.InValue + key2.InTangent, key2.InValue, time);
        
        public override Vec3 CubicHermite(VectorKeyframe<Vec3> key1, VectorKeyframe<Vec3> key2, float time)
            => Interp.CubicHermite(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);
        public override Vec3 CubicHermiteVelocity(VectorKeyframe<Vec3> key1, VectorKeyframe<Vec3> key2, float time)
            => Interp.CubicHermiteVelocity(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);
        public override Vec3 CubicHermiteAcceleration(VectorKeyframe<Vec3> key1, VectorKeyframe<Vec3> key2, float time)
            => Interp.CubicHermiteAcceleration(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);

        [GridCallable]
        public override void AverageTangentDirections()
        {
            float inLength = InTangent.Length;
            float outLength = OutTangent.Length;
            Vec3 inTan = InTangent.Normalized();
            Vec3 outTan = OutTangent.Normalized();
            Vec3 avg = (inTan + outTan) * 0.5f;
            avg.Normalize();
            InTangent = -avg * inLength;
            OutTangent = avg * outLength;
        }
        [GridCallable]
        public override void AverageTangentMagnitudes()
        {
            float inLength = InTangent.Length;
            float outLength = OutTangent.Length;
            float avgLength = (inLength + outLength) * 0.5f;
            Vec3 inTan = InTangent.Normalized();
            Vec3 outTan = OutTangent.Normalized();
            InTangent = inTan * avgLength;
            OutTangent = outTan * avgLength;
        }
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
            InValue = new Vec3(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
            OutValue = new Vec3(float.Parse(parts[4]), float.Parse(parts[5]), float.Parse(parts[6]));
            InTangent = new Vec3(float.Parse(parts[7]), float.Parse(parts[8]), float.Parse(parts[9]));
            OutTangent = new Vec3(float.Parse(parts[10]), float.Parse(parts[11]), float.Parse(parts[12]));
            InterpolationType = parts[13].AsEnum<EPlanarInterpType>();
        }
        protected override void ParsePlanar(string inValue, string outValue, string inTangent, string outTangent)
        {
            InValue = new Vec3(inValue);
            OutValue = new Vec3(outValue);
            InTangent = new Vec3(inTangent);
            OutTangent = new Vec3(outTangent);
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

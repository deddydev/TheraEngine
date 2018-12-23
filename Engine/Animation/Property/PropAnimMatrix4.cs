using System;
using System.Linq;

namespace TheraEngine.Animation
{
    public class PropAnimMatrix4 : PropAnimLerpable<Matrix4, Matrix4Keyframe>
    {
        public PropAnimMatrix4()
            : base() { }
        public PropAnimMatrix4(float lengthInSeconds, bool looped, bool useKeyframes)
            : base(lengthInSeconds, looped, useKeyframes) { }
        public PropAnimMatrix4(int frameCount, float FPS, bool looped, bool useKeyframes)
            : base(frameCount, FPS, looped, useKeyframes) { }

        protected override Matrix4 LerpValues(Matrix4 from, Matrix4 to, float time) => Matrix4.Lerp(from, to, time);
    }
    public class Matrix4Keyframe : LerpableKeyframe<Matrix4>
    {
        public Matrix4Keyframe()
            : this(0.0f, new Matrix4(0.0f), new Matrix4(0.0f)) { }
        public Matrix4Keyframe(int frameIndex, float framesPerSecond, Matrix4 inValue, Matrix4 outValue)
            : this(frameIndex / framesPerSecond, inValue, outValue) { }
        public Matrix4Keyframe(int frameIndex, float framesPerSecond, Matrix4 inoutValue)
            : this(frameIndex / framesPerSecond, inoutValue, inoutValue) { }
        public Matrix4Keyframe(float second, Matrix4 inoutValue)
            : this(second, inoutValue, inoutValue) { }
        public Matrix4Keyframe(float second, Matrix4 inValue, Matrix4 outValue)
            : base(second, inValue, outValue) { }
        
        public override Matrix4 Lerp(LerpableKeyframe<Matrix4> key1, LerpableKeyframe<Matrix4> key2, float time)
            => Matrix4.Lerp(key1.OutValue, key2.InValue, time);

        public override string WriteToString()
            => string.Format("{0} {1} {2}", Second, InValue.WriteToString(), OutValue.WriteToString());

        public override void ReadFromString(string str)
        {
            float[] values = str.Split(' ').Select(x => float.Parse(x)).ToArray();
            Matrix4 inValue = Matrix4.Identity;
            Matrix4 outValue = Matrix4.Identity;
            for (int i = 0; i < 16; ++i)
            {
                if (i + 1 < values.Length)
                    inValue[i] = values[i + 1];
                if (i + 17 < values.Length)
                    outValue[i] = values[i + 17];
            }
            Second = values.Length > 0 ? values[0] : 0.0f;
            InValue = inValue;
            OutValue = outValue;
        }
    }
}

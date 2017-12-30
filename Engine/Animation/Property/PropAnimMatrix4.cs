using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEngine.Animation
{
    public class PropAnimMatrix4 : PropAnimKeyframed<Matrix4Keyframe>, IEnumerable<Matrix4Keyframe>
    {
        private DelGetValue<Matrix4> _getValue;

        [TSerialize(Condition = "Baked")]
        private Matrix4[] _baked = null;
        /// <summary>
        /// The default value to return when no keyframes are set.
        /// </summary>
        [TSerialize(Condition = "!Baked")]
        public Matrix4 DefaultValue { get; set; } = Matrix4.Identity;

        public PropAnimMatrix4() : base(0.0f, false) { }
        public PropAnimMatrix4(float lengthInSeconds, bool looped, bool useKeyframes)
            : base(lengthInSeconds, looped, useKeyframes) { }
        public PropAnimMatrix4(int frameCount, float FPS, bool looped, bool useKeyframes) 
            : base(frameCount, FPS, looped, useKeyframes) { }

        protected override void BakedChanged()
            => _getValue = !Baked ? (DelGetValue<Matrix4>)GetValueKeyframed : GetValueBaked;

        public Matrix4 GetValue(float second)
            => _getValue(second);
        protected override object GetValueGeneric(float second)
            => _getValue(second);
        public Matrix4 GetValueBaked(float second)
            => _baked[(int)Math.Floor(second * BakedFramesPerSecond)];
        public Matrix4 GetValueBaked(int frameIndex)
            => _baked[frameIndex];
        public Matrix4 GetValueKeyframed(float second)
            => _keyframes.Count == 0 ? DefaultValue : _keyframes.First.Interpolate(second);
        
        public override void Bake(float framesPerSecond)
        {
            _bakedFPS = framesPerSecond;
            _bakedFrameCount = (int)Math.Ceiling(LengthInSeconds * framesPerSecond);
            _baked = new Matrix4[BakedFrameCount];
            for (int i = 0; i < BakedFrameCount; ++i)
                _baked[i] = GetValueKeyframed(i);
        }

        public IEnumerator<Matrix4Keyframe> GetEnumerator()
            => ((IEnumerable<Matrix4Keyframe>)_keyframes).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<Matrix4Keyframe>)_keyframes).GetEnumerator();
    }
    public class Matrix4Keyframe : Keyframe
    {
        public Matrix4Keyframe(float second, Matrix4 value) : base()
        {
            Second = second;
            Value = value;
        }

        protected delegate Matrix4 DelInterpolate(Matrix4Keyframe key1, Matrix4Keyframe key2, float time);
        
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public Matrix4 Value { get; set; }

        public new Matrix4Keyframe Next
        {
            get => _next as Matrix4Keyframe;
            set => _next = value;
        }
        public new Matrix4Keyframe Prev
        {
            get => _prev as Matrix4Keyframe;
            set => _prev = value;
        }

        public Matrix4 Interpolate(float frameIndex)
        {
            if (_prev == this || _next == this)
                return Value;

            if (frameIndex < Second && _prev.Second > Second)
                return Prev.Interpolate(frameIndex);

            if (frameIndex > _next.Second && _next.Second > Second)
                return Next.Interpolate(frameIndex);

            //float t = (frameIndex - _frameIndex) / (_next._frameIndex - _frameIndex);

            return Value;

            //return _interpolate(this, Next, t);
        }

        public override void ReadFromString(string str)
        {
            int spaceIndex = str.IndexOf(' ');
            Second = float.Parse(str.Substring(0, spaceIndex));
            Value = new Matrix4();
            Value.ReadFromString(str.Substring(spaceIndex + 1));
        }
        public override string WriteToString()
        {
            return string.Format("{0} {1}", Second, Value.WriteToString());
        }
    }
}

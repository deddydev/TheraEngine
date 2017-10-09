using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEngine.Animation
{
    public class PropAnimBool : PropertyAnimation<BoolKeyframe>, IEnumerable<BoolKeyframe>
    {
        private bool _defaultValue = false;
        private GetValue<bool> _getValue;

        [Serialize(Condition = "!UseKeyframes")]
        private bool[] _baked = null;

        [Serialize(Condition = "UseKeyframes")]
        public bool DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        public PropAnimBool(float lengthInSeconds, bool looped, bool useKeyframes)
            : base(lengthInSeconds, looped, useKeyframes) { }
        public PropAnimBool(int frameCount, float FPS, bool looped, bool useKeyframes) 
            : base(frameCount, FPS, looped, useKeyframes) { }

        protected override void UseKeyframesChanged()
            => _getValue = _useKeyframes ? (GetValue<bool>)GetValueKeyframed : GetValueBaked;
        protected override object GetValue(float second)
            => _getValue(second);
        public bool GetValueBaked(float second)
            => _baked[(int)Math.Floor(second * BakedFramesPerSecond)];
        public bool GetValueBaked(int frameIndex)
            => _baked[frameIndex];
        public bool GetValueKeyframed(float frameIndex)
        {
            BoolKeyframe key = _keyframes.GetKeyBefore(frameIndex);
            if (key != null)
                return key.Value;
            return _defaultValue;
        }

        public override void Bake(float framesPerSecond)
        {
            _bakedFPS = framesPerSecond;
            _bakedFrameCount = (int)Math.Ceiling(LengthInSeconds * framesPerSecond);
            _baked = new bool[BakedFrameCount];
            for (int i = 0; i < BakedFrameCount; ++i)
                _baked[i] = GetValueKeyframed(i);
        }

        public IEnumerator<BoolKeyframe> GetEnumerator()
            => ((IEnumerable<BoolKeyframe>)_keyframes).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<BoolKeyframe>)_keyframes).GetEnumerator();
    }
    public class BoolKeyframe : Keyframe
    {
        public BoolKeyframe(int frameIndex, float FPS, bool value)
            : this(frameIndex / FPS, value) { }
        public BoolKeyframe(float second, bool value) : base()
        {
            Second = second;
            Value = value;
        }
        
        [Serialize(IsXmlAttribute = true)]
        public bool Value { get; set; }

        public new BoolKeyframe Next
        {
            get => _next as BoolKeyframe;
            set => _next = value;
        }
        public new BoolKeyframe Prev
        {
            get => _prev as BoolKeyframe;
            set => _prev = value;
        }

        public override void ReadFromString(string str)
        {
            int spaceIndex = str.IndexOf(' ');
            Second = float.Parse(str.Substring(0, spaceIndex));
            Value = bool.Parse(str.Substring(spaceIndex + 1));
        }
        public override string WriteToString()
        {
            return string.Format("{0} {1}", Second, Value);
        }
    }
}

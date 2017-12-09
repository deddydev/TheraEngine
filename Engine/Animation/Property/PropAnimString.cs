using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEngine.Animation
{
    public class PropAnimString : PropertyAnimation<StringKeyframe>, IEnumerable<StringKeyframe>
    {
        private string _defaultValue = "";
        private GetValue<string> _getValue;

        [TSerialize(Condition = "!UseKeyframes")]
        private string[] _baked = null;

        [TSerialize(Condition = "UseKeyframes")]
        public string DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        public PropAnimString() : base(0.0f, false, true) { }
        public PropAnimString(float lengthInSeconds, bool looped, bool useKeyframes)
            : base(lengthInSeconds, looped, useKeyframes) { }
        public PropAnimString(int frameCount, float FPS, bool looped, bool useKeyframes) 
            : base(frameCount, FPS, looped, useKeyframes) { }

        protected override void UseKeyframesChanged()
            => _getValue = _useKeyframes ? (GetValue<string>)GetValueKeyframed : GetValueBaked;

        public string GetValue(float second)
            => _getValue(second);
        protected override object GetValueGeneric(float second)
            => _getValue(second);
        public string GetValueBaked(float second)
            => _baked[(int)Math.Floor(second * BakedFramesPerSecond)];
        public string GetValueBaked(int frameIndex)
            => _baked[frameIndex];
        public string GetValueKeyframed(float second)
        {
            StringKeyframe key = _keyframes.GetKeyBefore(second);
            if (key != null)
                return key.Value;
            return _defaultValue;
        }
        
        public override void Bake(float framesPerSecond)
        {
            _bakedFPS = framesPerSecond;
            _bakedFrameCount = (int)Math.Ceiling(LengthInSeconds * framesPerSecond);
            _baked = new string[BakedFrameCount];
            for (int i = 0; i < BakedFrameCount; ++i)
                _baked[i] = GetValueKeyframed(i);
        }

        public IEnumerator<StringKeyframe> GetEnumerator()
            => ((IEnumerable<StringKeyframe>)_keyframes).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<StringKeyframe>)_keyframes).GetEnumerator();
    }
    public class StringKeyframe : Keyframe
    {
        protected string _value;
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public string Value
        {
            get => _value;
            set => _value = value;
        }
        public new StringKeyframe Next
        {
            get => _next as StringKeyframe;
            set => _next = value;
        }
        public new StringKeyframe Prev
        {
            get => _prev as StringKeyframe;
            set => _prev = value;
        }

        public override void ReadFromString(string str)
        {
            int spaceIndex = str.IndexOf(' ');
            Second = float.Parse(str.Substring(0, spaceIndex));
            Value = str.Substring(spaceIndex + 1);
        }
        public override string WriteToString()
        {
            return string.Format("{0} {1}", Second, Value);
        }
    }
}

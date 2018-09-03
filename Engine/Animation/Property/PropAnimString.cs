using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Core.Reflection.Attributes;

namespace TheraEngine.Animation
{
    public class PropAnimString : PropAnimKeyframed<StringKeyframe>, IEnumerable<StringKeyframe>
    {
        private DelGetValue<string> _getValue;

        [TSerialize(Condition = "Baked")]
        private string[] _baked = null;
        /// <summary>
        /// The default value to return when no keyframes are set.
        /// </summary>
        [Category(PropAnimCategory)]
        [TSerialize(Condition = "!Baked")]
        public string DefaultValue { get; set; } = string.Empty;

        public PropAnimString() : base(0.0f, false) { }
        public PropAnimString(float lengthInSeconds, bool looped, bool useKeyframes)
            : base(lengthInSeconds, looped, useKeyframes) { }
        public PropAnimString(int frameCount, float FPS, bool looped, bool useKeyframes) 
            : base(frameCount, FPS, looped, useKeyframes) { }

        protected override void BakedChanged()
            => _getValue = !Baked ? (DelGetValue<string>)GetValueKeyframed : GetValueBaked;

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
            return DefaultValue;
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
        [TString(true, false, false, true)]
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public string Value { get; set; }

        [Browsable(false)]
        public new StringKeyframe Next
        {
            get => _next as StringKeyframe;
            set => _next = value;
        }
        [Browsable(false)]
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
        public override string WriteToString() => string.Format("{0} {1}", Second, Value);
    }
}

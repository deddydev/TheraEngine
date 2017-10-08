﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEngine.Animation
{
    delegate string StringGetValue(float frameIndex);
    public class PropAnimString : PropertyAnimation<StringKeyframe>, IEnumerable<StringKeyframe>
    {
        private string _defaultValue = "";
        private StringGetValue _getValue;

        [Serialize(Condition = "!UseKeyframes")]
        private string[] _baked;

        [Serialize(Condition = "UseKeyframes")]
        public string DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        public PropAnimString(float lengthInSeconds, bool looped, bool useKeyframes)
            : base(lengthInSeconds, looped, useKeyframes) { }
        public PropAnimString(int frameCount, float FPS, bool looped, bool useKeyframes) 
            : base(frameCount, FPS, looped, useKeyframes) { }

        protected override object GetValue(float frame)
            => _getValue(frame);
        protected override void UseKeyframesChanged()
        {
            if (_useKeyframes)
                _getValue = GetValueKeyframed;
            else
                _getValue = GetValueBaked;
        }
        public string GetValueBaked(float second)
            => _baked[(int)Math.Floor(second * BakedFramesPerSecond)];
        public string GetValueBaked(int frameIndex)
            => _baked[frameIndex];
        public string GetValueKeyframed(float frameIndex)
        {
            StringKeyframe key = _keyframes.GetKeyBefore(frameIndex);
            if (key != null)
                return key.Value;
            return _defaultValue;
        }
        /// <summary>
        /// Bakes the interpolated data for fastest access by the game.
        /// </summary>
        public override void Bake(float framesPerSecond)
        {
            _bakedFPS = framesPerSecond;
            _bakedFrameCount = (int)Math.Ceiling(LengthInSeconds * framesPerSecond);
            _baked = new string[BakedFrameCount];
            for (int i = 0; i < BakedFrameCount; ++i)
                _baked[i] = GetValueKeyframed(i);
        }
        public override void Resize(int newSize)
        {
            throw new NotImplementedException();
        }
        public override void Stretch(int newSize)
        {
            throw new NotImplementedException();
        }
        public override void Append(PropertyAnimation<StringKeyframe> other)
        {
            throw new NotImplementedException();
        }
        public IEnumerator<StringKeyframe> GetEnumerator() => ((IEnumerable<StringKeyframe>)_keyframes).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<StringKeyframe>)_keyframes).GetEnumerator();
    }
    public class StringKeyframe : Keyframe
    {
        protected string _value;
        [Serialize(IsXmlAttribute = true)]
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

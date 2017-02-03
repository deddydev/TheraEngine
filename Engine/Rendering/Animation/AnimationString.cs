using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace CustomEngine.Rendering.Animation
{
    delegate string StringGetValue(float frameIndex);
    public class AnimationString : PropertyAnimation<StringKeyframe>, IEnumerable<StringKeyframe>
    {
        public override ResourceType ResourceType { get { return ResourceType.AnimationString; } }

        string[] _baked;
        StringGetValue _getValue;

        public AnimationString(int frameCount, bool looped, bool useKeyframes) 
            : base(frameCount, looped, useKeyframes) { }

        protected override object GetValue(float frame)
        {
            return _getValue(frame);
        }
        protected override void UseKeyframesChanged()
        {
            if (_useKeyframes)
                _getValue = GetValueKeyframed;
            else
                _getValue = GetValueBaked;
        }
        public string GetValueBaked(float frameIndex) { return _baked[(int)frameIndex]; }
        public string GetValueKeyframed(float frameIndex)
        {
            StringKeyframe key = _keyframes.GetKeyBefore(frameIndex);
            if (key != null)
                return key.Value;
            throw new Exception("Invalid frame index.");
        }
        /// <summary>
        /// Bakes the interpolated data for fastest access by the game.
        /// </summary>
        public override void Bake()
        {
            _baked = new string[FrameCount];
            for (int i = 0; i < FrameCount; ++i)
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
        public IEnumerator<StringKeyframe> GetEnumerator()
        {
            return ((IEnumerable<StringKeyframe>)_keyframes).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<StringKeyframe>)_keyframes).GetEnumerator();
        }
    }
    public class StringKeyframe : Keyframe
    {
        protected string _value;

        public string Value { get { return _value; } set { _value = value; } }
        public new StringKeyframe Next { get { return _next as StringKeyframe; } set { _next = value; } }
        public new StringKeyframe Prev { get { return _prev as StringKeyframe; } set { _prev = value; } }
    }
}

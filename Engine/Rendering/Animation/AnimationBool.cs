using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using CustomEngine.Files;

namespace CustomEngine.Rendering.Animation
{
    delegate bool BoolGetValue(float frameIndex);
    public class AnimationBool : PropertyAnimation<BoolKeyframe>, IEnumerable<BoolKeyframe>
    {
        public override ResourceType ResourceType { get { return ResourceType.AnimationBool; } }
        
        bool[] _baked;
        BoolGetValue _getValue;

        public AnimationBool(int frameCount, bool looped, bool useKeyframes) 
            : base(frameCount, looped, useKeyframes) { }

        protected override object GetValue(float frame) { return _getValue(frame); }
        protected override void UseKeyframesChanged()
        {
            if (_useKeyframes)
                _getValue = GetValueKeyframed;
            else
                _getValue = GetValueBaked;
        }
        public bool GetValueBaked(float frameIndex) { return _baked[(int)frameIndex]; }
        public bool GetValueKeyframed(float frameIndex)
        {
            BoolKeyframe key = _keyframes.GetKeyBefore(frameIndex);
            if (key != null)
                return key.Value;
            throw new Exception("Invalid frame index.");
        }
        public override void Bake()
        {
            _baked = new bool[FrameCount];
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
        public override void Append(PropertyAnimation<BoolKeyframe> other)
        {
            throw new NotImplementedException();
        }
        public IEnumerator<BoolKeyframe> GetEnumerator()
        {
            return ((IEnumerable<BoolKeyframe>)_keyframes).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<BoolKeyframe>)_keyframes).GetEnumerator();
        }

        public override void Write(VoidPtr address, StringTable table)
        {
            throw new NotImplementedException();
        }

        public override void Read(VoidPtr address, VoidPtr strings)
        {
            throw new NotImplementedException();
        }

        public override void Write(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void Read(XMLReader reader)
        {
            throw new NotImplementedException();
        }
    }
    public class BoolKeyframe : Keyframe
    {
        protected bool _value;

        public bool Value { get { return _value; } set { _value = value; } }
        public new BoolKeyframe Next { get { return _next as BoolKeyframe; } set { _next = value; } }
        public new BoolKeyframe Prev { get { return _prev as BoolKeyframe; } set { _prev = value; } }

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using CustomEngine.Files;

namespace CustomEngine.Rendering.Animation
{
    delegate Vec4 Vec4GetValue(float frameIndex);
    public class AnimationVec4 : PropertyAnimation<Vec4Keyframe>, IEnumerable<Vec4Keyframe>
    {
        Vec4[] _baked;
        Vec4GetValue _getValue;
        
        public AnimationVec4(int frameCount, bool looped, bool useKeyframes) 
            : base(frameCount, looped, useKeyframes) { }

        protected override object GetValue(float frame) { return _getValue(frame); }
        protected override void UseKeyframesChanged()
        {
            if (_useKeyframes)
                _getValue = GetValueKeyframed;
            else
                _getValue = GetValueBaked;
        }
        public Vec4 GetValueBaked(float frameIndex)
        {
            return _baked[(int)frameIndex];
        }
        public Vec4 GetValueKeyframed(float frameIndex)
        {
            Vec4Keyframe key = _keyframes.GetKeyBefore(frameIndex);
            if (key != null)
                return key.Interpolate(frameIndex);
            throw new IndexOutOfRangeException("Invalid frame index.");
        }
        /// <summary>
        /// Bakes the interpolated data for fastest access by the game.
        /// </summary>
        public override void Bake()
        {
            _baked = new Vec4[FrameCount];
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
        public override void Append(PropertyAnimation<Vec4Keyframe> other)
        {
            throw new NotImplementedException();
        }
        public IEnumerator<Vec4Keyframe> GetEnumerator() { return ((IEnumerable<Vec4Keyframe>)_keyframes).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<Vec4Keyframe>)_keyframes).GetEnumerator(); }

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

        protected override int OnCalculateSize(StringTable table)
        {
            throw new NotImplementedException();
        }
    }
    public class Vec4Keyframe : Keyframe
    {
        public Vec4Keyframe(float frameIndex, Vec4 inValue, Vec4 outValue) : base()
        {
            _frameIndex = frameIndex;
            _inValue = inValue;
            _outValue = outValue;
        }
        
        protected Vec4 _inValue;
        protected Vec4 _inTangent;

        protected Vec4 _outValue;
        protected Vec4 _outTangent;

        public Vec4 InValue { get { return _inValue; } set { _inValue = value; } }
        public Vec4 OutValue { get { return _outValue; } set { _outValue = value; } }

        public Vec4 InTanget { get { return _inTangent; } set { _inTangent = value; } }
        public Vec4 OutTangent { get { return _outTangent; } set { _outTangent = value; } }
        
        public new Vec4Keyframe Next { get { return _next as Vec4Keyframe; } set { _next = value; } }
        public new Vec4Keyframe Prev { get { return _prev as Vec4Keyframe; } set { _prev = value; } }
        
        delegate Vec4 DelInterpolate(float t, Vec4 p0, Vec4 p1, Vec4 t0, Vec4 t1);
        private DelInterpolate _interpolate = CubicHermite;
        public Vec4 Interpolate(float frameIndex)
        {
            if (frameIndex < _frameIndex)
            {
                if (_prev == this)
                    return _inValue;

                return Prev.Interpolate(frameIndex);
            }

            if (_next == this)
                return _outValue;

            if (frameIndex > _next._frameIndex)
                return Next.Interpolate(frameIndex);

            float t = (frameIndex - _frameIndex) / (_next._frameIndex - _frameIndex);
            return _interpolate(t, _outValue, Next._inValue, _outTangent, Next._inTangent);
        }
        
        public static Vec4 CubicHermite(float time, Vec4 p0, Vec4 p1, Vec4 t0, Vec4 t1)
        {
            float time2 = time * time;
            float time3 = time2 * time;
            return 
                p0 * (2.0f * time3 - 3.0f * time2 + 1.0f) +
                t0 * (time3 - 2.0f * time2 + time) +
                p1 * (-2.0f * time3 + 3.0f * time2) +
                t1 * (time3 - time2);
        }

        public void AverageKeyframe()
        {
            AverageValues();
            AverageTangents();
        }
        public void AverageTangents()
        {
            _inTangent = _outTangent = (_inTangent + _outTangent) / 2.0f;
        }
        public void AverageValues()
        {
            _inValue = _outValue = (_inValue + _outValue) / 2.0f;
        }
        public void MakeOutLinear()
        {
            _outTangent = (Next.InValue - OutValue) / (Next._frameIndex - _frameIndex);
        }
        public void MakeInLinear()
        {
            _inTangent = (InValue - Prev.OutValue) / (_frameIndex - Prev._frameIndex);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using CustomEngine.Files;

namespace CustomEngine.Rendering.Animation
{
    delegate Vec3 Vec3GetValue(float frameIndex);
    public class AnimationVec3 : PropertyAnimation<Vec3Keyframe>, IEnumerable<Vec3Keyframe>
    {
        Vec3[] _baked;
        Vec3GetValue _getValue;
        
        public AnimationVec3(int frameCount, bool looped, bool useKeyframes) 
            : base(frameCount, looped, useKeyframes) { }

        protected override void UseKeyframesChanged()
        {
            if (_useKeyframes)
                _getValue = GetValueKeyframed;
            else
                _getValue = GetValueBaked;
        }
        protected override object GetValue(float frame)
            => _getValue(frame);
        public Vec3 GetValueBaked(float frameIndex)
            => _baked[(int)(frameIndex * _keyframes.FPS)];
        public Vec3 GetValueKeyframed(float frameIndex)
            => _keyframes.First.Interpolate(frameIndex);

        /// <summary>
        /// Bakes the interpolated data for fastest access by the game.
        /// However, this method takes up more space and does not support time dilation (speeding up and slowing down with proper in-betweens)
        /// </summary>
        public override void Bake()
        {
            _baked = new Vec3[FrameCount];
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
        public override void Append(PropertyAnimation<Vec3Keyframe> other)
        {
            throw new NotImplementedException();
        }
        public IEnumerator<Vec3Keyframe> GetEnumerator() { return ((IEnumerable<Vec3Keyframe>)_keyframes).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<Vec3Keyframe>)_keyframes).GetEnumerator(); }
    }
    public class Vec3Keyframe : Keyframe
    {
        public Vec3Keyframe(float frameIndex, Vec3 value, PlanarInterpType type = PlanarInterpType.Linear)
            : this(frameIndex, value, value, type) { }
        public Vec3Keyframe(float frameIndex, Vec3 inValue, Vec3 outValue, PlanarInterpType type = PlanarInterpType.Linear)
            : this(frameIndex, inValue, outValue, Vec3.Zero, Vec3.Zero, type) { }
        public Vec3Keyframe(float frameIndex, Vec3 inValue, Vec3 outValue, Vec3 inTangent, Vec3 outTangent, PlanarInterpType type = PlanarInterpType.Linear) : base()
        {
            _frameIndex = frameIndex;
            _inValue = inValue;
            _outValue = outValue;
            _inTangent = inTangent;
            _outTangent = outTangent;
        }

        protected Vec3 _inValue;
        protected Vec3 _inTangent;
        protected Vec3 _outValue;
        protected Vec3 _outTangent;
        protected PlanarInterpType _interpolationType;

        public Vec3 InValue
        {
            get => _inValue;
            set => _inValue = value;
        }
        public Vec3 OutValue
        {
            get => _outValue;
            set => _outValue = value;
        }
        public Vec3 InTangent
        {
            get => _inTangent;
            set => _inTangent = value;
        }
        public Vec3 OutTangent
        {
            get => _outTangent;
            set => _outTangent = value;
        }
        public new Vec3Keyframe Next
        {
            get => _next as Vec3Keyframe;
            set => _next = value;
        }
        public new Vec3Keyframe Prev
        {
            get => _prev as Vec3Keyframe;
            set => _prev = value;
        }
        public PlanarInterpType InterpolationType
        {
            get => _interpolationType;
            set
            {
                _interpolationType = value;
                switch (_interpolationType)
                {
                    case PlanarInterpType.Step:
                        _interpolate = Step;
                        break;
                    case PlanarInterpType.Linear:
                        _interpolate = Lerp;
                        break;
                    case PlanarInterpType.CubicHermite:
                        _interpolate = CubicHermite;
                        break;
                    case PlanarInterpType.CubicBezier:
                        _interpolate = Bezier;
                        break;
                }
            }
        }
        delegate Vec3 DelInterpolate(Vec3Keyframe key1, Vec3Keyframe key2, float time);
        private DelInterpolate _interpolate = CubicHermite;
        public Vec3 Interpolate(float frameIndex)
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
            return _interpolate(this, Next, t);
        }
        public static Vec3 Step(Vec3Keyframe key1, Vec3Keyframe key2, float time)
            => time < 1.0f ? key1.OutValue : key2.OutValue;
        public static Vec3 Lerp(Vec3Keyframe key1, Vec3Keyframe key2, float time)
            => Vec3.Lerp(key1.OutValue, key2.InValue, time);
        public static Vec3 Bezier(Vec3Keyframe key1, Vec3Keyframe key2, float time)
            => CustomMath.CubicBezier(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);
        public static Vec3 CubicHermite(Vec3Keyframe key1, Vec3Keyframe key2, float time)
            => CustomMath.CubicHermite(key1.OutValue, key1.OutTangent, key2.InTangent, key2.InValue, time);

        public void AverageKeyframe()
        {
            AverageValues();
            AverageTangents();
        }
        public void AverageTangents()
            => _inTangent = _outTangent = (_inTangent + _outTangent) / 2.0f;
        public void AverageValues()
            => _inValue = _outValue = (_inValue + _outValue) / 2.0f;
        public void MakeOutLinear()
            => _outTangent = (Next.InValue - OutValue) / (Next._frameIndex - _frameIndex);
        public void MakeInLinear()
            => _inTangent = (InValue - Prev.OutValue) / (_frameIndex - Prev._frameIndex);
    }
}

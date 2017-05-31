using System;
using System.Collections;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Animation
{
    public delegate Quat QuatGetValue(float frameIndex);
    public class PropAnimQuat : PropertyAnimation<QuatKeyframe>, IEnumerable<QuatKeyframe>
    {
        private Quat _defaultValue = Quat.Identity;
        private Quat[] _baked;
        private QuatGetValue _getValue;

        public Quat DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        public PropAnimQuat(int frameCount, bool looped, bool useKeyframes) 
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
        public Quat GetValueBaked(float frameIndex)
            => _baked[(int)(frameIndex / Engine.TargetUpdateFreq * FramesPerSecond)];
        public Quat GetValueKeyframed(float frameIndex)
            => _keyframes.KeyCount == 0 ? _defaultValue : _keyframes.First.Interpolate(frameIndex);

        /// <summary>
        /// Bakes the interpolated data for fastest access by the game.
        /// However, this method takes up more space and does not support time dilation (speeding up and slowing down with proper in-betweens)
        /// </summary>
        public override void Bake()
        {
            _baked = new Quat[FrameCount];
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
        public override void Append(PropertyAnimation<QuatKeyframe> other)
        {
            throw new NotImplementedException();
        }
        public IEnumerator<QuatKeyframe> GetEnumerator() { return ((IEnumerable<QuatKeyframe>)_keyframes).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<QuatKeyframe>)_keyframes).GetEnumerator(); }
    }
    public class QuatKeyframe : Keyframe
    {
        public QuatKeyframe(float frameIndex, Quat value, RadialInterpType type)
            : this(frameIndex, value, value, type) { }
        public QuatKeyframe(float frameIndex, Quat inValue, Quat outValue, RadialInterpType type) : base()
        {
            _frameIndex = frameIndex;
            _inValue = inValue;
            _outValue = outValue;
            InterpolationType = type;
        }

        delegate Quat DelInterpolate(QuatKeyframe key1, QuatKeyframe key2, float time);
        private DelInterpolate _interpolate = CubicBezier;
        protected RadialInterpType _interpolationType;
        protected Quat _inValue;
        protected Quat _inTangent;
        protected Quat _outValue;
        protected Quat _outTangent;

        public Quat InValue
        {
            get => _inValue;
            set => _inValue = value;
        }
        public Quat OutValue
        {
            get => _outValue;
            set => _outValue = value;
        }
        public Quat InTanget
        {
            get => _inTangent;
            set => _inTangent = value;
        }
        public Quat OutTangent
        {
            get => _outTangent;
            set => _outTangent = value;
        }
        public new QuatKeyframe Next
        {
            get => _next as QuatKeyframe;
            set => _next = value;
        }
        public new QuatKeyframe Prev
        {
            get => _prev as QuatKeyframe;
            set => _prev = value;
        }
        public RadialInterpType InterpolationType
        {
            get => _interpolationType;
            set
            {
                _interpolationType = value;
                switch (_interpolationType)
                {
                    case RadialInterpType.Step:
                        _interpolate = Step;
                        break;
                    case RadialInterpType.Linear:
                        _interpolate = Linear;
                        break;
                    case RadialInterpType.CubicBezier:
                        _interpolate = CubicBezier;
                        break;
                }
            }
        }
        public Quat Interpolate(float frameIndex)
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
        public static Quat Step(QuatKeyframe key1, QuatKeyframe key2, float time)
            => time < 1.0f ? key1.OutValue : key2.OutValue;
        public static Quat Linear(QuatKeyframe key1, QuatKeyframe key2, float time)
            => Quat.Slerp(key1.OutValue, key2.InValue, time);
        public static Quat CubicBezier(QuatKeyframe key1, QuatKeyframe key2, float time)
            => Quat.Scubic(key1.OutValue, key1.OutTangent, key2.InTanget, key2.InValue, time);
    }
}

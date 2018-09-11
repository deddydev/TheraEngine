using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Reflection.Attributes;

namespace TheraEngine.Animation
{
    public enum EVectorInterpValueType
    {
        Position,
        Velocity,
        Acceleration,
    }
    public abstract class PropAnimVector<T, T2> : PropAnimKeyframed<T2>
        where T : unmanaged
        where T2 : VectorKeyframe<T>, new()
    {
        private DelGetValue<T> _getValue;

        [TSerialize(Condition = "Baked")]
        private T[] _baked = null;
        /// <summary>
        /// The default value to return when no keyframes are set.
        /// </summary>
        [Category(PropAnimCategory)]
        [TSerialize(Condition = "!Baked")]
        public T DefaultValue { get; set; } = new T();

        public PropAnimVector() : base(0.0f, false) { }
        public PropAnimVector(float lengthInSeconds, bool looped, bool useKeyframes)
            : base(lengthInSeconds, looped, useKeyframes) { }
        public PropAnimVector(int frameCount, float FPS, bool looped, bool useKeyframes) 
            : base(frameCount, FPS, looped, useKeyframes) { }
        
        protected override void BakedChanged()
            => _getValue = !Baked ? (DelGetValue<T>)GetValueKeyframed : GetValueBaked;

        public T GetValue(float second)
            => _getValue(second);
        protected override object GetValueGeneric(float second)
            => _getValue(second);
        public T GetValueBaked(float second)
            => _baked[(int)Math.Floor(second * BakedFramesPerSecond)];
        public T GetValueBaked(int frameIndex)
            => _baked[frameIndex];
        public T GetValueKeyframed(float second)
            => _keyframes.Count == 0 ? DefaultValue : _keyframes.First.Interpolate(second, EVectorInterpValueType.Position);
        public T GetVelocityKeyframed(float second)
            => _keyframes.Count == 0 ? new T() : _keyframes.First.Interpolate(second, EVectorInterpValueType.Velocity);
        public T GetAccelerationKeyframed(float second)
            => _keyframes.Count == 0 ? new T() : _keyframes.First.Interpolate(second, EVectorInterpValueType.Acceleration);

        public override void Bake(float framesPerSecond)
        {
            _bakedFPS = framesPerSecond;
            _bakedFrameCount = (int)Math.Ceiling(LengthInSeconds * framesPerSecond);
            _baked = new T[BakedFrameCount];
            for (int i = 0; i < BakedFrameCount; ++i)
                _baked[i] = GetValueKeyframed(i);
        }
    }
    public abstract class VectorKeyframe<T> : Keyframe, IPlanarKeyframe<T> where T : unmanaged
    {
        public VectorKeyframe()
            : this(0.0f, new T(), new T(), EPlanarInterpType.CubicBezier) { }
        public VectorKeyframe(int frameIndex, float FPS, T inValue, T outValue, T inTangent, T outTangent, EPlanarInterpType type)
            : this(frameIndex / FPS, inValue, outValue, inTangent, outTangent, type) { }
        public VectorKeyframe(int frameIndex, float FPS, T inoutValue, T inoutTangent, EPlanarInterpType type)
            : this(frameIndex / FPS, inoutValue, inoutValue, inoutTangent, inoutTangent, type) { }
        public VectorKeyframe(float second, T inoutValue, T inoutTangent, EPlanarInterpType type)
            : this(second, inoutValue, inoutValue, inoutTangent, inoutTangent, type) { }
        public VectorKeyframe(float second, T inValue, T outValue, T inTangent, T outTangent, EPlanarInterpType type) : base()
        {
            Second = second;
            InValue = inValue;
            OutValue = outValue;
            InTangent = inTangent;
            OutTangent = outTangent;
            InterpolationType = type;
        }

        protected delegate T DelInterpolate(VectorKeyframe<T> key1, VectorKeyframe<T> key2, float time);
        protected EPlanarInterpType _interpolationType;
        protected DelInterpolate _interpolate;
        protected DelInterpolate _interpolateVelocity;
        protected DelInterpolate _interpolateAcceleration;

        object IPlanarKeyframe.InValue { get => InValue; set => InValue = (T)value; }
        object IPlanarKeyframe.OutValue { get => OutValue; set => OutValue = (T)value; }
        object IPlanarKeyframe.InTangent { get => InTangent; set => InTangent = (T)value; }
        object IPlanarKeyframe.OutTangent { get => OutTangent; set => OutTangent = (T)value; }

        [Category("Keyframe")]
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public T InValue
        {
            get => _inValue;
            set
            {
                _inValue = value;
                OwningTrack?.OnChanged();
            }
        }
        [Category("Keyframe")]
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public T OutValue
        {
            get => _outValue;
            set
            {
                _outValue = value;
                OwningTrack?.OnChanged();
            }
        }
        [Category("Keyframe")]
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public T InTangent
        {
            get => _inTangent;
            set
            {
                _inTangent = value;
                OwningTrack?.OnChanged();
            }
        }
        [Category("Keyframe")]
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public T OutTangent
        {
            get => _outTangent;
            set
            {
                _outTangent = value;
                OwningTrack?.OnChanged();
            }
        }

        private T _inValue, _outValue, _inTangent, _outTangent;

        [Browsable(false)]
        [Category("Keyframe")]
        public new VectorKeyframe<T> Next
        {
            get => _next as VectorKeyframe<T>;
            //set => _next = value;
        }
        [Browsable(false)]
        [Category("Keyframe")]
        public new VectorKeyframe<T> Prev
        {
            get => _prev as VectorKeyframe<T>;
            //set => _prev = value;
        }

        [Category("Keyframe")]
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public EPlanarInterpType InterpolationType
        {
            get => _interpolationType;
            set
            {
                _interpolationType = value;
                switch (_interpolationType)
                {
                    case EPlanarInterpType.Step:
                        _interpolate = Step;
                        _interpolateVelocity = StepVelocity;
                        _interpolateAcceleration = StepAcceleration;
                        break;
                    case EPlanarInterpType.Linear:
                        _interpolate = Lerp;
                        _interpolateVelocity = LerpVelocity;
                        _interpolateAcceleration = LerpAcceleration;
                        break;
                    case EPlanarInterpType.CubicHermite:
                        _interpolate = CubicHermite;
                        _interpolateVelocity = CubicHermiteVelocity;
                        _interpolateAcceleration = CubicHermiteAcceleration;
                        break;
                    case EPlanarInterpType.CubicBezier:
                        _interpolate = CubicBezier;
                        _interpolateVelocity = CubicBezierVelocity;
                        _interpolateAcceleration = CubicBezierAcceleration;
                        break;
                }
                OwningTrack?.OnChanged();
            }
        }

        /// <summary>
        /// Interpolates from this keyframe to the next using a normalized time value (0.0f - 1.0f)
        /// </summary>
        public T InterpolateNextNormalized(float time) => _interpolate(this, Next, time);
        /// <summary>
        /// Interpolates velocity from this keyframe to the next using a normalized time value (0.0f - 1.0f)
        /// </summary>
        public T InterpolateVelocityNextNormalized(float time) => _interpolateVelocity(this, Next, time);
        /// <summary>
        /// Interpolates acceleration from this keyframe to the next using a normalized time value (0.0f - 1.0f)
        /// </summary>
        public T InterpolateAccelerationNextNormalized(float time) => _interpolateAcceleration(this, Next, time);

        public T Interpolate(float desiredSecond, EVectorInterpValueType type)
        {
            //First, check if the desired second is between this key and the next key.
            if (desiredSecond < Second)
            {
                if (_prev == null)
                    return InValue;

                //If the previous key's second is greater than this second, this key must be the first key. 
                //Return the InValue as the desired second comes before this one.
                //Otherwise, move to the previous key to calculate the interpolated value.
                return _prev.Second < Second ? Prev.Interpolate(desiredSecond, type) : InValue;
            }
            else if (_next == null)
            {
                return OutValue;
            }
            else if (desiredSecond > _next.Second)
            {
                //If the next key's second is less than this second, this key must be the last key. 
                //Return the OutValue as the desired second comes after this one.
                //Otherwise, move to the previous key to calculate the interpolated value.
                return _next.Second > Second ? Next.Interpolate(desiredSecond, type) : OutValue;
            }

            float span = _next.Second - Second;
            float diff = desiredSecond - Second;
            float time = diff / span;

            switch (type)
            {
                default:
                case EVectorInterpValueType.Position:
                    return _interpolate(this, Next, time);
                case EVectorInterpValueType.Velocity:
                    return _interpolateVelocity(this, Next, time);
                case EVectorInterpValueType.Acceleration:
                    return _interpolateAcceleration(this, Next, time);
            }
        }

        public T Step(VectorKeyframe<T> key1, VectorKeyframe<T> key2, float time)
            => time < 1.0f ? key1.OutValue : key2.OutValue;
        public T StepVelocity(VectorKeyframe<T> key1, VectorKeyframe<T> key2, float time)
            => new T();
        public T StepAcceleration(VectorKeyframe<T> key1, VectorKeyframe<T> key2, float time)
            => new T();

        public abstract T Lerp(VectorKeyframe<T> key1, VectorKeyframe<T> key2, float time);
        public abstract T LerpVelocity(VectorKeyframe<T> key1, VectorKeyframe<T> key2, float time);
        public static T LerpAcceleration(VectorKeyframe<T> key1, VectorKeyframe<T> key2, float time)
            => new T();

        public abstract T CubicHermite(VectorKeyframe<T> key1, VectorKeyframe<T> key2, float time);
        public abstract T CubicHermiteVelocity(VectorKeyframe<T> key1, VectorKeyframe<T> key2, float time);
        public abstract T CubicHermiteAcceleration(VectorKeyframe<T> key1, VectorKeyframe<T> key2, float time);

        public abstract T CubicBezier(VectorKeyframe<T> key1, VectorKeyframe<T> key2, float time);
        public abstract T CubicBezierVelocity(VectorKeyframe<T> key1, VectorKeyframe<T> key2, float time);
        public abstract T CubicBezierAcceleration(VectorKeyframe<T> key1, VectorKeyframe<T> key2, float time);

        [GridCallable]
        public void AverageKeyframe()
        {
            AverageValues();
            AverageTangents();
        }
        [GridCallable]
        public abstract void AverageTangents();
        [GridCallable]
        public abstract void AverageValues();
        [GridCallable]
        public abstract void MakeOutLinear();
        [GridCallable]
        public abstract void MakeInLinear();
        
        void IPlanarKeyframe.ParsePlanar(string inValue, string outValue, string inTangent, string outTangent)
            => ParsePlanar(inValue, outValue, inTangent, outTangent);
        protected abstract void ParsePlanar(string inValue, string outValue, string inTangent, string outTangent);

        void IPlanarKeyframe.WritePlanar(out string inValue, out string outValue, out string inTangent, out string outTangent)
            => WritePlanar(out inValue, out outValue, out inTangent, out outTangent);
        protected abstract void WritePlanar(out string inValue, out string outValue, out string inTangent, out string outTangent);
    }
}

using System;
using System.ComponentModel;
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

        [TSerialize("BakedValues"/*, Condition = "Baked"*/)]
        private T[] _baked = null;
        /// <summary>
        /// The default value to return when no keyframes are set.
        /// </summary>
        [Category(PropAnimCategory)]
        [TSerialize(/*Condition = "!Baked"*/)]
        public T DefaultValue { get; set; } = new T();
        [Category(PropAnimCategory)]
        [TSerialize(/*Condition = "!Baked"*/)]
        public bool ConstrainKeyframeFPS { get; set; } = false;
        [Category(PropAnimCategory)]
        [TSerialize(/*Condition = "!Baked"*/)]
        public bool LerpConstrainedFPS { get; set; } = false;
        
        public PropAnimVector() : base(0.0f, false) { }
        public PropAnimVector(float lengthInSeconds, bool looped, bool useKeyframes)
            : base(lengthInSeconds, looped, useKeyframes) { }
        public PropAnimVector(int frameCount, float FPS, bool looped, bool useKeyframes) 
            : base(frameCount, FPS, looped, useKeyframes) { }

        public T CurrentPosition { get; private set; }
        public T CurrentVelocity { get; private set; }
        public T CurrentAcceleration { get; private set; }

        protected override object GetValueGeneric() => CurrentPosition;
        protected override object GetValueGeneric(float second) => _getValue(second);

        public T GetValue(float second) => _getValue(second);
        public T GetValueBaked(float second)
        {
            float frameTime = second * BakedFramesPerSecond;
            int frame = (int)frameTime;
            if (LerpConstrainedFPS)
            {
                if (frame == _baked.Length - 1)
                {
                    if (Looped && frame != 0)
                    {
                        T t1 = _baked[frame];
                        T t2 = _baked[0];

                        //TODO: interpolate values by creating tangents dynamically?
                        float lerpTime = frameTime - frame;
                        return LerpValues(t1, t2, lerpTime);
                    }
                    return _baked[frame];
                }
                else
                {
                    T t1 = _baked[frame];
                    T t2 = _baked[frame + 1];

                    //TODO: interpolate values by creating tangents dynamically?
                    float lerpTime = frameTime - frame;
                    return LerpValues(t1, t2, lerpTime);
                }
            }
            else
            {
                return _baked[frame];
            }
        }
        protected abstract T LerpValues(T t1, T t2, float time);

        protected override void BakedChanged()
        {
            if (Baked)
            {
                Bake(_bakedFPS);
                _getValue = GetValueBaked;
            }
            else
            {
                _baked = null;
                _getValue = GetValueKeyframed;
            }
        }
        public T GetValueBaked(int frameIndex)
            => _baked == null || _baked.Length == 0 ? new T() :
            _baked[frameIndex.Clamp(0, _baked.Length - 1)];

        public T GetValueKeyframed(float second)
            => Interpolate(second, EVectorInterpValueType.Position);
        public T GetVelocityKeyframed(float second)
            => Interpolate(second, EVectorInterpValueType.Velocity);
        public T GetAccelerationKeyframed(float second)
            => Interpolate(second, EVectorInterpValueType.Acceleration);
        private T Interpolate(float second, EVectorInterpValueType type)
        {
            if (_keyframes.Count == 0)
                return DefaultValue;

            if (ConstrainKeyframeFPS)
            {
                int frame = (int)(second * _bakedFPS);
                float floorSec = frame / _bakedFPS;
                float ceilSec = (frame + 1) / _bakedFPS;
                float time = second - floorSec;
                if (LerpConstrainedFPS)
                    return LerpKeyedValues(floorSec, ceilSec, time, type);
                second = floorSec;
            }
            
            return _keyframes.First.Interpolate(second, type);
        }
        private VectorKeyframe<T> _prevKeyframe = null;
        protected override void OnProgressed(float delta)
        {
            if (_prevKeyframe == null)
                _prevKeyframe = Keyframes.First;
            if (_keyframes.Count == 0)
            {
                CurrentPosition = DefaultValue;
                CurrentVelocity = new T();
                CurrentAcceleration = new T();
            }
            float second = _currentTime;
            if (ConstrainKeyframeFPS)
            {
                int frame = (int)(second * _bakedFPS);
                float floorSec = frame / _bakedFPS;
                float ceilSec = (frame + 1) / _bakedFPS;
                float time = (second - floorSec) * _bakedFPS;
                if (LerpConstrainedFPS)
                {
                    _prevKeyframe.Interpolate(floorSec,
                        out _prevKeyframe, 
                        out VectorKeyframe<T> nextKeyF,
                        out float normalizedTimeF,
                        out T floorPosition,
                        out T floorVelocity,
                        out T floorAcceleration);

                    _prevKeyframe.Interpolate(ceilSec,
                       out VectorKeyframe<T> prevKeyC,
                       out VectorKeyframe<T> nextKeyC,
                       out float normalizedTimeC,
                       out T ceilPosition,
                       out T ceilVelocity,
                       out T ceilAcceleration);

                    CurrentPosition = LerpValues(floorPosition, ceilPosition, time);
                    CurrentVelocity = LerpValues(floorVelocity, ceilVelocity, time);
                    CurrentAcceleration = LerpValues(floorAcceleration, ceilAcceleration, time);
                    return;
                }
                second = floorSec;
            }

            _prevKeyframe.Interpolate(second,
                out _prevKeyframe,
                out VectorKeyframe<T> nextKey,
                out float normalizedTime,
                out T pos,
                out T vel,
                out T acc);

            CurrentPosition = pos;
            CurrentVelocity = vel;
            CurrentAcceleration = acc;
        }
        private T LerpKeyedValues(float floorSec, float ceilSec, float time, EVectorInterpValueType type)
        {
            T floorValue = _keyframes.First.Interpolate(floorSec, type,
                out VectorKeyframe<T> prevKey, out VectorKeyframe<T> nextKey, out float normalizedTime);
            T ceilValue = prevKey.Interpolate(ceilSec, type);
            return LerpValues(floorValue, ceilValue, time);
        }
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
                if (_prev == null || _prev.Second >= Second)
                {
                    //This is the first key
                    if (OwningTrack.LastKey != this)
                    {
                        VectorKeyframe<T> last = (VectorKeyframe<T>)OwningTrack.LastKey;

                        float span2 = OwningTrack.LengthInSeconds - last.Second + Second;
                        float diff2 = OwningTrack.LengthInSeconds - last.Second + desiredSecond;
                        float normalizedTime = diff2 / span2;

                        switch (type)
                        {
                            default:
                            case EVectorInterpValueType.Position:
                                return _interpolate(last, this, normalizedTime);
                            case EVectorInterpValueType.Velocity:
                                return _interpolateVelocity(last, this, normalizedTime);
                            case EVectorInterpValueType.Acceleration:
                                return _interpolateAcceleration(last, this, normalizedTime);
                        }
                    }

                    if (type == EVectorInterpValueType.Position)
                        return InValue;
                    else
                        return new T();
                }

                //If the previous key's second is greater than this second, this key must be the first key. 
                //Return the InValue as the desired second comes before this one.
                //Otherwise, move to the previous key to calculate the interpolated value.
                return Prev.Interpolate(desiredSecond, type);
            }
            else if (_next == null || _next.Second <= Second)
            {
                //This is the last key
                if (OwningTrack.FirstKey != this)
                {
                    VectorKeyframe<T> first = (VectorKeyframe<T>)OwningTrack.FirstKey;

                    float span2 = OwningTrack.LengthInSeconds - Second + first.Second;
                    float diff2 = OwningTrack.LengthInSeconds - desiredSecond + first.Second;
                    float normalizedTime = diff2 / span2;

                    switch (type)
                    {
                        default:
                        case EVectorInterpValueType.Position:
                            return _interpolate(this, first, normalizedTime);
                        case EVectorInterpValueType.Velocity:
                            return _interpolateVelocity(this, first, normalizedTime);
                        case EVectorInterpValueType.Acceleration:
                            return _interpolateAcceleration(this, first, normalizedTime);
                    }
                }

                if (type == EVectorInterpValueType.Position)
                    return OutValue;
                else
                    return new T();
            }
            else if (desiredSecond > _next.Second)
            {
                //If the next key's second is less than this second, this key must be the last key. 
                //Return the OutValue as the desired second comes after this one.
                //Otherwise, move to the previous key to calculate the interpolated value.
                return Next.Interpolate(desiredSecond, type);
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
        public T Interpolate(
            float desiredSecond, 
            EVectorInterpValueType type,
            out VectorKeyframe<T> prevKey,
            out VectorKeyframe<T> nextKey,
            out float normalizedTime)
        {
            prevKey = this;
            nextKey = Next;

            //First, check if the desired second is between this key and the next key.
            if (desiredSecond < Second)
            {
                if (_prev == null || _prev.Second >= Second)
                {
                    //This is the first key
                    if (OwningTrack.LastKey != this)
                    {
                        VectorKeyframe<T> last = (VectorKeyframe<T>)OwningTrack.LastKey;

                        float span2 = OwningTrack.LengthInSeconds - last.Second + Second;
                        float diff2 = OwningTrack.LengthInSeconds - last.Second + desiredSecond;
                        normalizedTime = diff2 / span2;

                        switch (type)
                        {
                            default:
                            case EVectorInterpValueType.Position:
                                return _interpolate(last, this, normalizedTime);
                            case EVectorInterpValueType.Velocity:
                                return _interpolateVelocity(last, this, normalizedTime);
                            case EVectorInterpValueType.Acceleration:
                                return _interpolateAcceleration(last, this, normalizedTime);
                        }
                    }

                    normalizedTime = 0.0f;
                    if (type == EVectorInterpValueType.Position)
                        return InValue;
                    else
                        return new T();
                }

                //If the previous key's second is greater than this second, this key must be the first key. 
                //Return the InValue as the desired second comes before this one.
                //Otherwise, move to the previous key to calculate the interpolated value.
                return Prev.Interpolate(desiredSecond, type, out prevKey, out nextKey, out normalizedTime);
            }
            else if (_next == null || _next.Second <= Second)
            {
                //This is the last key
                if (OwningTrack.FirstKey != this)
                {
                    VectorKeyframe<T> first = (VectorKeyframe<T>)OwningTrack.FirstKey;

                    float span2 = OwningTrack.LengthInSeconds - Second + first.Second;
                    float diff2 = OwningTrack.LengthInSeconds - desiredSecond + first.Second;
                    normalizedTime = diff2 / span2;

                    switch (type)
                    {
                        default:
                        case EVectorInterpValueType.Position:
                            return _interpolate(this, first, normalizedTime);
                        case EVectorInterpValueType.Velocity:
                            return _interpolateVelocity(this, first, normalizedTime);
                        case EVectorInterpValueType.Acceleration:
                            return _interpolateAcceleration(this, first, normalizedTime);
                    }
                }

                normalizedTime = 0.0f;
                if (type == EVectorInterpValueType.Position)
                    return OutValue;
                else
                    return new T();
            }
            else if (desiredSecond > _next.Second)
            {
                //If the next key's second is less than this second, this key must be the last key. 
                //Return the OutValue as the desired second comes after this one.
                //Otherwise, move to the previous key to calculate the interpolated value.
                return Next.Interpolate(desiredSecond, type, out prevKey, out nextKey, out normalizedTime);
            }

            float span = _next.Second - Second;
            float diff = desiredSecond - Second;
            normalizedTime = diff / span;

            switch (type)
            {
                default:
                case EVectorInterpValueType.Position:
                    return _interpolate(this, Next, normalizedTime);
                case EVectorInterpValueType.Velocity:
                    return _interpolateVelocity(this, Next, normalizedTime);
                case EVectorInterpValueType.Acceleration:
                    return _interpolateAcceleration(this, Next, normalizedTime);
            }
        }
        public void Interpolate(
            float desiredSecond,
            out VectorKeyframe<T> prevKey,
            out VectorKeyframe<T> nextKey,
            out float normalizedTime,
            out T position,
            out T velocity,
            out T acceleration)
        {
            prevKey = this;
            nextKey = Next;

            //First, check if the desired second is between this key and the next key.
            if (desiredSecond < Second)
            {
                if (_prev == null || _prev.Second >= Second)
                {
                    //This is the first key
                    if (OwningTrack.LastKey != this)
                    {
                        VectorKeyframe<T> last = (VectorKeyframe<T>)OwningTrack.LastKey;
                        
                        float span2 = OwningTrack.LengthInSeconds - last.Second + Second;
                        float diff2 = OwningTrack.LengthInSeconds - last.Second + desiredSecond;
                        normalizedTime = diff2 / span2;

                        position = _interpolate(last, this, normalizedTime);
                        velocity = _interpolateVelocity(last, this, normalizedTime);
                        acceleration = _interpolateAcceleration(last, this, normalizedTime);

                        return;
                    }

                    normalizedTime = 0.0f;
                    position = InValue;
                    velocity = new T();
                    acceleration = new T();

                    return;
                }

                //If the previous key's second is greater than this second, this key must be the first key. 
                //Return the InValue as the desired second comes before this one.
                //Otherwise, move to the previous key to calculate the interpolated value.
                Prev.Interpolate(desiredSecond,
                    out prevKey,
                    out nextKey, 
                    out normalizedTime, 
                    out position,
                    out velocity,
                    out acceleration);

                return;
            }
            else if (_next == null || _next.Second <= Second)
            {
                //This is the last key
                if (OwningTrack.FirstKey != this)
                {
                    VectorKeyframe<T> first = (VectorKeyframe<T>)OwningTrack.FirstKey;

                    float span2 = OwningTrack.LengthInSeconds - Second + first.Second;
                    float diff2 = OwningTrack.LengthInSeconds - desiredSecond + first.Second;
                    normalizedTime = diff2 / span2;

                    position = _interpolate(this, first, normalizedTime);
                    velocity = _interpolateVelocity(this, first, normalizedTime);
                    acceleration = _interpolateAcceleration(this, first, normalizedTime);

                    return;
                }

                normalizedTime = 0.0f;
                position = OutValue;
                velocity = new T();
                acceleration = new T();

                return;
            }
            else if (desiredSecond > _next.Second)
            {
                //If the next key's second is less than this second, this key must be the last key. 
                //Return the OutValue as the desired second comes after this one.
                //Otherwise, move to the previous key to calculate the interpolated value.
                Next.Interpolate(desiredSecond, 
                    out prevKey, 
                    out nextKey,
                    out normalizedTime,
                    out position, 
                    out velocity, 
                    out acceleration);

                return;
            }

            float span = _next.Second - Second;
            float diff = desiredSecond - Second;
            normalizedTime = diff / span;
            
            position = _interpolate(this, Next, normalizedTime);
            velocity = _interpolateVelocity(this, Next, normalizedTime);
            acceleration = _interpolateAcceleration(this, Next, normalizedTime);
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

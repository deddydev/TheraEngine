using System;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Core.Maths;
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
        private T _defaultValue = new T();
        private bool _constrainKeyframedFPS = false;
        private bool _lerpConstrainedFPS = false;
        private VectorKeyframe<T> _prevKeyframe = null;

        public event Action<PropAnimVector<T, T2>> DefaultValueChanged;
        public event Action<PropAnimVector<T, T2>> ConstrainKeyframedFPSChanged;
        public event Action<PropAnimVector<T, T2>> LerpConstrainedFPSChanged;

        public event Action<PropAnimVector<T, T2>> CurrentPositionChanged;
        public event Action<PropAnimVector<T, T2>> CurrentVelocityChanged;
        public event Action<PropAnimVector<T, T2>> CurrentAccelerationChanged;

        protected void OnDefaultValueChanged() => DefaultValueChanged?.Invoke(this);
        protected void OnConstrainKeyframedFPSChanged() => ConstrainKeyframedFPSChanged?.Invoke(this);
        protected void OnLerpConstrainedFPSChanged() => LerpConstrainedFPSChanged?.Invoke(this);
        
        [TSerialize("BakedValues", Condition = "IsBaked")]
        private T[] _baked = null;

        /// <summary>
        /// The default value to return when no keyframes are set.
        /// </summary>
        [Description("The default value to return when no keyframes are set.")]
        [Category(PropAnimCategory)]
        [TSerialize]
        public T DefaultValue
        {
            get => _defaultValue;
            set
            {
                _defaultValue = value;
                OnDefaultValueChanged();
            }
        }

        [DisplayName("Constrain Keyframed FPS")]
        [Category(PropAnimCategory)]
        [TSerialize]
        public bool ConstrainKeyframedFPS
        {
            get => _constrainKeyframedFPS;
            set
            {
                _constrainKeyframedFPS = value;
                OnConstrainKeyframedFPSChanged();
            }
        }
        [DisplayName("Lerp Constrained FPS")]
        /// <summary>
        /// If true and the animation is baked or ConstrainKeyframedFPS is true, 
        /// lerps between two frames if the second lies between them.
        /// This essentially fakes a higher frames per second for data at a lower resolution.
        /// </summary>
        [Description(
            "If true and the animation is baked or ConstrainKeyframedFPS is true, " +
            "lerps between two frames if the second lies between them. " +
            "This essentially fakes a higher frames per second for data at a lower resolution.")]
        [Category(PropAnimCategory)]
        [TSerialize]
        public bool LerpConstrainedFPS
        {
            get => _lerpConstrainedFPS;
            set
            {
                _lerpConstrainedFPS = value;
                OnLerpConstrainedFPSChanged();
            }
        }

        public PropAnimVector() : base(0.0f, false) { }
        public PropAnimVector(float lengthInSeconds, bool looped, bool useKeyframes)
            : base(lengthInSeconds, looped, useKeyframes) { }
        public PropAnimVector(int frameCount, float FPS, bool looped, bool useKeyframes) 
            : base(frameCount, FPS, looped, useKeyframes) { }

        private T _currentPosition;
        private T _currentVelocity;
        private T _currentAcceleration;

        public T CurrentPosition
        {
            get => _currentPosition;
            private set
            {
                _currentPosition = value;
                CurrentPositionChanged?.Invoke(this);
            }
        }
        public T CurrentVelocity
        {
            get => _currentVelocity;
            private set
            {
                _currentVelocity = value;
                CurrentVelocityChanged?.Invoke(this);
            }
        }
        public T CurrentAcceleration
        {
            get => _currentAcceleration;
            private set
            {
                _currentAcceleration = value;
                CurrentAccelerationChanged?.Invoke(this);
            }
        }

        protected override object GetCurrentValueGeneric() => CurrentPosition;
        protected override object GetValueGeneric(float second) => _getValue(second);

        public T GetValue(float second) => _getValue(second);

        public T GetValueBakedBySecond(float second)
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

                        //Span is always 1 frame, so no need to divide to normalize
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

                    //Span is always 1 frame, so no need to divide to normalize
                    float lerpTime = frameTime - frame;

                    return LerpValues(t1, t2, lerpTime);
                }
            }
            else
            {
                return _baked[frame];
            }
        }
        public T GetValueBakedByFrame(int frame)
        {
            if (!_baked.IndexInArrayRange(frame))
                return new T();
            return _baked[frame.Clamp(0, _baked.Length - 1)];
        }
        protected abstract T LerpValues(T t1, T t2, float time);

        protected override void BakedChanged()
        {
            if (IsBaked)
            {
                Bake(_bakedFPS);
                _getValue = GetValueBakedBySecond;
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

            if (ConstrainKeyframedFPS)
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
        public override float CurrentTime
        {
            get => base.CurrentTime;
            set
            {
                float newTime = value.RemapToRange(0.0f, _lengthInSeconds);
                float oldTime = _currentTime;
                _currentTime = newTime;
                OnProgressed(newTime - oldTime);
                OnCurrentTimeChanged();
            }
        }
        protected override void OnProgressed(float delta)
        {
            //TODO: assign separate functions to be called by OnProgressed to avoid if statements and returns

            if (IsBaked)
            {
                CurrentPosition = GetValueBakedBySecond(_currentTime);
                CurrentVelocity = new T();
                CurrentAcceleration = new T();
                return;
            }

            if (_prevKeyframe == null)
                _prevKeyframe = Keyframes.First;
            if (_keyframes.Count == 0)
            {
                CurrentPosition = DefaultValue;
                CurrentVelocity = new T();
                CurrentAcceleration = new T();
                return;
            }
            float second = _currentTime;
            if (ConstrainKeyframedFPS)
            {
                int frame = (int)(second * _bakedFPS);
                float floorSec = frame / _bakedFPS;
                float ceilSec = (frame + 1) / _bakedFPS;

                //second - floorSec is the resulting delta from one frame to the next.
                //we want the delta to be between two frames with a specified number of frames in between, 
                //so we multiply by the FPS.
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
            float invFPS = 1.0f / _bakedFPS;
            for (int i = 0; i < BakedFrameCount; ++i)
                _baked[i] = GetValueKeyframed(i * invFPS);
        }
        public void GetMinMax(
            out (float Time, float Value)[] min,
            out (float Time, float Value)[] max)
        {
            float[] inComps = GetComponents(DefaultValue), outComps, inTanComps, outTanComps;
            int compCount = inComps.Length;
            if (_keyframes.Count == 0)
            {
                min = max = inComps.Select(x => (0.0f, x)).ToArray();
                return;
            }

            VectorKeyframe<T> kf = _keyframes.First;
            if (_keyframes.Count == 1)
            {
                if (kf.Second.IsZero())
                {
                    outComps = GetComponents(kf.OutValue);
                    min = max = outComps.Select(x => (kf.Second, x)).ToArray();
                }
                else if (kf.Second.EqualTo(_keyframes.LengthInSeconds))
                {
                    inComps = GetComponents(kf.InValue);
                    min = max = inComps.Select(x => (kf.Second, x)).ToArray();
                }
                else
                {
                    inComps = GetComponents(kf.InValue);
                    outComps = GetComponents(kf.OutValue);

                    min = new (float Time, float Value)[compCount];
                    max = new (float Time, float Value)[compCount];

                    for (int i = 0; i < compCount; ++i)
                    {
                        min[i] = (kf.Second, Math.Min(inComps[i], outComps[i]));
                        max[i] = (kf.Second, Math.Max(inComps[i], outComps[i]));
                    }
                }
            }
            else
            {
                min = new(float Time, float Value)[compCount];
                min = min.FillWith((0.0f, float.MaxValue));

                max = new(float Time, float Value)[compCount];
                max = max.FillWith((0.0f, float.MinValue));

                VectorKeyframe<T> next;
                for (int i = 0; i < _keyframes.Count; ++i, kf = next)
                {
                    next = kf.Next;
                    for (int x = 0; x < compCount; ++x)
                    {
                        float minVal = min[x].Value;
                        float maxVal = max[x].Value;

                        float oldMin = minVal;
                        float oldMax = maxVal;

                        inComps = GetComponents(kf.InValue);
                        outComps = GetComponents(kf.OutValue);

                        //Check if the keyframe already exceeds the current bounds
                        if (kf.Second.IsZero())
                        {
                            min[x].Value = Math.Min(minVal, outComps[x]);
                            max[x].Value = Math.Max(maxVal, outComps[x]);
                        }
                        else if (kf.Second.EqualTo(_keyframes.LengthInSeconds))
                        {
                            min[x].Value = Math.Min(minVal, inComps[x]);
                            max[x].Value = Math.Max(maxVal, inComps[x]);
                        }
                        else
                        {
                            min[x].Value = TMath.Min(minVal, inComps[x], outComps[x]);
                            max[x].Value = TMath.Max(maxVal, inComps[x], outComps[x]);
                        }

                        if (oldMin != minVal)
                            min[x].Time = kf.Second;
                        if (oldMax != maxVal)
                            max[x].Time = kf.Second;

                        //If not the last keyframe, evaluate the interpolation
                        //between this keyframe and the next to find spots where
                        //velocity reaches zero. This means that the position value
                        //is an extrema and should be considered for min/max.
                        if (i != _keyframes.Count - 1)
                        {
                            inTanComps = GetComponents(kf.InTangent);
                            outTanComps = GetComponents(kf.OutTangent);

                            //Retrieve velocity interpolation equation coefficients
                            //so we can solve for the two time values where velocity is zero.
                            Interp.CubicHermiteVelocityCoefs(
                                outComps[x], outTanComps[x], inTanComps[x], inComps[x],
                                out float second, out float first, out float zero);

                            if (TMath.QuadraticRealRoots(second, first, zero,
                                out float time1, out float time2))
                            {
                                T val1 = kf.InterpolateNextNormalized(time1);
                                T val2 = kf.InterpolateNextNormalized(time2);

                                oldMin = min.Value;
                                oldMax = max.Value;

                                min.Value = ComponentMin(min.Value, val1, val2);
                                max.Value = ComponentMax(max.Value, val1, val2);

                                float[] values = GetComponents();
                                if (!Equal(oldMin, min.Value))
                                {
                                    if (min.Value == val1)
                                        min.Time = time1;
                                    if (min.Value == val2)
                                        min.Time = time2;
                                }
                                if (!Equal(oldMax, max.Value))
                                {
                                    if (max.Value == val1)
                                        max.Time = time1;
                                    if (max.Value == val2)
                                        max.Time = time2;
                                }
                            }
                        }
                    }
                }
            }
        }

        internal abstract float[] GetComponents(T defaultValue);
        internal abstract T GetMaxValue();
        internal abstract T GetMinValue();
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

        private T _inValue, _outValue, _inTangent, _outTangent;
        private bool _syncInOutValues = true;
        private bool _syncInOutTangentDirections = true;
        private bool _syncInOutTangentMagnitudes = true;

#if EDITOR
        [Category("Keyframe")]
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public bool SyncInOutValues
        {
            get => _syncInOutValues;
            set
            {
                _syncInOutValues = value;
                if (_syncInOutValues)
                    AverageValues();
                OwningTrack?.OnChanged();
            }
        }
        [Category("Keyframe")]
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public bool SyncInOutTangentDirections
        {
            get => _syncInOutTangentDirections;
            set
            {
                _syncInOutTangentDirections = value;
                if (_syncInOutTangentDirections)
                    AverageValues();
                OwningTrack?.OnChanged();
            }
        }
        [Category("Keyframe")]
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public bool SyncInOutTangentMagnitudes
        {
            get => _syncInOutTangentMagnitudes;
            set
            {
                _syncInOutTangentMagnitudes = value;
                if (_syncInOutTangentMagnitudes)
                    AverageValues();
                OwningTrack?.OnChanged();
            }
        }
#endif
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
            float span, diff;
            VectorKeyframe<T> key1, key2;

            if (desiredSecond >= Second)
            {
                if (Next == null)
                {
                    //This is the last keyframe

                    if (OwningTrack.FirstKey != this)
                    {
                        VectorKeyframe<T> first = (VectorKeyframe<T>)OwningTrack.FirstKey;
                        span = OwningTrack.LengthInSeconds - Second + first.Second;
                        diff = desiredSecond - Second;
                        key1 = this;
                        key2 = first;
                    }
                    else
                    {
                        if (type == EVectorInterpValueType.Position)
                            return OutValue;
                        else
                            return new T();
                    }
                }
                else if (desiredSecond < Next.Second)
                {
                    //Within two keyframes, interpolate regularly
                    span = _next.Second - Second;
                    diff = desiredSecond - Second;
                    key1 = this;
                    key2 = Next;
                }
                else
                {
                    return Next.Interpolate(desiredSecond, type);
                }
            }
            else //desiredSecond < Second
            {
                if (Prev != null)
                    return Prev.Interpolate(desiredSecond, type);
                
                //This is the first keyframe

                if (OwningTrack.LastKey != this)
                {
                    VectorKeyframe<T> last = (VectorKeyframe<T>)OwningTrack.LastKey;
                    span = OwningTrack.LengthInSeconds - last.Second + Second;
                    diff = OwningTrack.LengthInSeconds - last.Second + desiredSecond;
                    key1 = last;
                    key2 = this;
                }
                else
                {
                    if (type == EVectorInterpValueType.Position)
                        return InValue;
                    else
                        return new T();
                }
            }

            float time = diff / span;
            switch (type)
            {
                default:
                case EVectorInterpValueType.Position:
                    return _interpolate(key1, key2, time);
                case EVectorInterpValueType.Velocity:
                    return _interpolateVelocity(key1, key2, time);
                case EVectorInterpValueType.Acceleration:
                    return _interpolateAcceleration(key1, key2, time);
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

            float span, diff;
            VectorKeyframe<T> key1, key2;

            if (desiredSecond >= Second)
            {
                if (Next == null)
                {
                    //This is the last keyframe

                    if (OwningTrack.FirstKey != this)
                    {
                        VectorKeyframe<T> first = (VectorKeyframe<T>)OwningTrack.FirstKey;
                        span = OwningTrack.LengthInSeconds - Second + first.Second;
                        diff = desiredSecond - Second;
                        key1 = this;
                        key2 = first;
                    }
                    else
                    {
                        normalizedTime = 0.0f;
                        if (type == EVectorInterpValueType.Position)
                            return OutValue;
                        else
                            return new T();
                    }
                }
                else if (desiredSecond < Next.Second)
                {
                    //Within two keyframes, interpolate regularly
                    span = _next.Second - Second;
                    diff = desiredSecond - Second;
                    key1 = this;
                    key2 = Next;
                }
                else
                {
                    return Next.Interpolate(desiredSecond, type, out prevKey, out nextKey, out normalizedTime);
                }
            }
            else //desiredSecond < Second
            {
                if (Prev != null)
                    return Prev.Interpolate(desiredSecond, type, out prevKey, out nextKey, out normalizedTime);

                //This is the first keyframe

                if (OwningTrack.LastKey != this)
                {
                    VectorKeyframe<T> last = (VectorKeyframe<T>)OwningTrack.LastKey;
                    span = OwningTrack.LengthInSeconds - last.Second + Second;
                    diff = OwningTrack.LengthInSeconds - last.Second + desiredSecond;
                    key1 = last;
                    key2 = this;
                }
                else
                {
                    normalizedTime = 0.0f;
                    if (type == EVectorInterpValueType.Position)
                        return InValue;
                    else
                        return new T();
                }
            }

            normalizedTime = diff / span;
            switch (type)
            {
                default:
                case EVectorInterpValueType.Position:
                    return _interpolate(key1, key2, normalizedTime);
                case EVectorInterpValueType.Velocity:
                    return _interpolateVelocity(key1, key2, normalizedTime);
                case EVectorInterpValueType.Acceleration:
                    return _interpolateAcceleration(key1, key2, normalizedTime);
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

            float span, diff;
            VectorKeyframe<T> key1, key2;

            if (desiredSecond >= Second)
            {
                if (Next == null)
                {
                    //This is the last keyframe

                    if (OwningTrack.FirstKey != this)
                    {
                        VectorKeyframe<T> first = (VectorKeyframe<T>)OwningTrack.FirstKey;
                        span = OwningTrack.LengthInSeconds - Second + first.Second;
                        diff = desiredSecond - Second;
                        key1 = this;
                        key2 = first;
                    }
                    else
                    {
                        normalizedTime = 0.0f;
                        position = OutValue;
                        velocity = new T();
                        acceleration = new T();
                        return;
                    }
                }
                else if (desiredSecond < Next.Second)
                {
                    //Within two keyframes, interpolate regularly
                    span = _next.Second - Second;
                    diff = desiredSecond - Second;
                    key1 = this;
                    key2 = Next;
                }
                else
                {
                    Next.Interpolate(desiredSecond,
                        out prevKey,
                        out nextKey,
                        out normalizedTime,
                        out position,
                        out velocity,
                        out acceleration);

                    return;
                }
            }
            else //desiredSecond < Second
            {
                if (Prev != null)
                {
                    Prev.Interpolate(desiredSecond,
                        out prevKey,
                        out nextKey,
                        out normalizedTime,
                        out position,
                        out velocity,
                        out acceleration);

                    return;
                }

                //This is the first keyframe

                if (OwningTrack.LastKey != this)
                {
                    VectorKeyframe<T> last = (VectorKeyframe<T>)OwningTrack.LastKey;
                    span = OwningTrack.LengthInSeconds - last.Second + Second;
                    diff = OwningTrack.LengthInSeconds - last.Second + desiredSecond;
                    key1 = last;
                    key2 = this;
                }
                else
                {
                    normalizedTime = 0.0f;
                    position = InValue;
                    velocity = new T();
                    acceleration = new T();
                    return;
                }
            }

            normalizedTime = diff / span;
            position = _interpolate(key1, key2, normalizedTime);
            velocity = _interpolateVelocity(key1, key2, normalizedTime);
            acceleration = _interpolateAcceleration(key1, key2, normalizedTime);
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
            AverageTangentDirections();
            AverageTangentMagnitudes();
        }
        [GridCallable]
        public abstract void AverageTangentDirections();
        [GridCallable]
        public abstract void AverageTangentMagnitudes();
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

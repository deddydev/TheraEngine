using System;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Core.Attributes;
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
    public abstract class PropAnimVector<TValue, TValueKey> : PropAnimKeyframed<TValueKey>
        where TValue : unmanaged
        where TValueKey : VectorKeyframe<TValue>, new()
    {
        protected const string VectorAnimCategory = "Vector Animation";

        public PropAnimVector()
            : base(0.0f, false) { }
        public PropAnimVector(float lengthInSeconds, bool looped, bool useKeyframes)
            : base(lengthInSeconds, looped, useKeyframes) { }
        public PropAnimVector(int frameCount, float framesPerSecond, bool looped, bool useKeyframes)
            : base(frameCount, framesPerSecond, looped, useKeyframes) { }

        public event Action<PropAnimVector<TValue, TValueKey>> DefaultValueChanged;
        public event Action<PropAnimVector<TValue, TValueKey>> ConstrainKeyframedFPSChanged;
        public event Action<PropAnimVector<TValue, TValueKey>> LerpConstrainedFPSChanged;

        public event Action<PropAnimVector<TValue, TValueKey>> CurrentPositionChanged;
        public event Action<PropAnimVector<TValue, TValueKey>> CurrentVelocityChanged;
        public event Action<PropAnimVector<TValue, TValueKey>> CurrentAccelerationChanged;

        private DelGetValue<TValue> _getValue;
        private TValue _defaultValue = new TValue();
        private bool _constrainKeyframedFPS = false;
        private bool _lerpConstrainedFPS = false;
        private VectorKeyframe<TValue> _prevKeyframe = null;

        private TValue _currentPosition;
        private TValue _currentVelocity;
        private TValue _currentAcceleration;

        [TSerialize("BakedValues", Condition = "IsBaked")]
        private TValue[] _baked = null;

        /// <summary>
        /// If true, speed calculated relative to the current tangent rather than multiplied directly with the current velocity (change in position).
        /// </summary>
        [Description("If true, speed calculated relative to the current tangent rather than multiplied directly with the current velocity (change in position).")]
        [TSerialize]
        [Category(VectorAnimCategory)]
        public bool UseTangentRelativeSpeed { get; set; } = false;
        /// <summary>
        /// The default value to return when no keyframes are set.
        /// </summary>
        [TNumericPrefixSuffix(null, " m")]
        [Description("The default value to return when no keyframes are set.")]
        [Category(VectorAnimCategory)]
        [TSerialize]
        public TValue DefaultValue
        {
            get => _defaultValue;
            set
            {
                _defaultValue = value;
                OnDefaultValueChanged();
            }
        }
        [DisplayName("Constrain Keyframed FPS")]
        [Category(VectorAnimCategory)]
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
        [Category(VectorAnimCategory)]
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
        /// <summary>
        /// The value at the current time.
        /// </summary>
        [Category(VectorAnimCategory)]
        [TNumericPrefixSuffix(null, " m")]
        public TValue CurrentPosition
        {
            get => _currentPosition;
            private set
            {
                _currentPosition = value;
                CurrentPositionChanged?.Invoke(this);
            }
        }
        /// <summary>
        /// The velocity at the current time.
        /// </summary>
        [Category(VectorAnimCategory)]
        [TNumericPrefixSuffix(null, " m/sec")]
        public TValue CurrentVelocity
        {
            get => _currentVelocity;
            private set
            {
                _currentVelocity = value;
                CurrentVelocityChanged?.Invoke(this);
            }
        }
        /// <summary>
        /// The acceleration at the current time.
        /// </summary>
        [Category(VectorAnimCategory)]
        [TNumericPrefixSuffix(null, " m/sec^2")]
        public TValue CurrentAcceleration
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

        public TValue GetValue(float second) => _getValue(second);
        public TValue GetValueBakedBySecond(float second)
        {
            float frameTime = second.RemapToRange(0, LengthInSeconds) * BakedFramesPerSecond;
            int frame = (int)frameTime;
            if (LerpConstrainedFPS)
            {
                if (frame == _baked.Length - 1)
                {
                    if (Looped && frame != 0)
                    {
                        TValue t1 = _baked[frame];
                        TValue t2 = _baked[0];

                        //TODO: interpolate values by creating tangents dynamically?

                        //Span is always 1 frame, so no need to divide to normalize
                        float lerpTime = frameTime - frame;

                        return LerpValues(t1, t2, lerpTime);
                    }
                    return _baked[frame];
                }
                else
                {
                    TValue t1 = _baked[frame];
                    TValue t2 = _baked[frame + 1];

                    //TODO: interpolate values by creating tangents dynamically?

                    //Span is always 1 frame, so no need to divide to normalize
                    float lerpTime = frameTime - frame;

                    return LerpValues(t1, t2, lerpTime);
                }
            }
            else if (_baked.IndexInArrayRange(frame))
                return _baked[frame];
            else
                return DefaultValue;
        }
        /// <summary>
        /// Returns a value from the baked array by frame index.
        /// </summary>
        /// <param name="frame">The frame to get a value for.</param>
        /// <returns>The value at the specified frame.</returns>
        public TValue GetValueBakedByFrame(int frame)
        {
            if (!_baked.IndexInArrayRange(frame))
                return new TValue();
            return _baked[frame.Clamp(0, _baked.Length - 1)];
        }
        /// <summary>
        /// Returns a linearly interpolated value between two values.
        /// </summary>
        /// <param name="from">The starting value.</param>
        /// <param name="to">The target value.</param>
        /// <param name="time">Normalized time between the two values (0.0f - 1.0f).</param>
        /// <returns>A linearly interpolated value between two values.</returns>
        protected abstract TValue LerpValues(TValue from, TValue to, float time);

        protected void OnDefaultValueChanged()
            => DefaultValueChanged?.Invoke(this);

        protected void OnConstrainKeyframedFPSChanged()
            => ConstrainKeyframedFPSChanged?.Invoke(this);

        protected void OnLerpConstrainedFPSChanged()
            => LerpConstrainedFPSChanged?.Invoke(this);

        protected override void BakedChanged()
            => _getValue = !IsBaked ? (DelGetValue<TValue>)GetValueKeyframed : GetValueBakedBySecond;

        public TValue GetValueBaked(int frameIndex)
            => _baked == null || _baked.Length == 0 ? new TValue() :
            _baked[frameIndex.Clamp(0, _baked.Length - 1)];

        public TValue GetValueKeyframed(float second)
            => Interpolate(second, EVectorInterpValueType.Position);
        public TValue GetVelocityKeyframed(float second)
            => Interpolate(second, EVectorInterpValueType.Velocity);
        public TValue GetAccelerationKeyframed(float second)
            => Interpolate(second, EVectorInterpValueType.Acceleration);
        private TValue Interpolate(float second, EVectorInterpValueType type)
        {
            if (_keyframes.Count == 0)
                return DefaultValue;

            if (ConstrainKeyframedFPS)
            {
                int frame = (int)(second * _bakedFPS);
                float floorSec = _bakedFPS != 0.0f ? (frame / _bakedFPS) : 0.0f;
                float ceilSec = _bakedFPS != 0.0f ? ((frame + 1) / _bakedFPS) : 0.0f;
                float time = second - floorSec;

                if (LerpConstrainedFPS)
                    return LerpKeyedValues(floorSec, ceilSec, time, type);

                second = floorSec;
            }
            
            return _keyframes.First?.Interpolate(second, type) ?? DefaultValue;
        }
        [Category(AnimCategory)]
        [TNumericPrefixSuffix(null, " sec")]
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

        public override void Progress(float delta)
        {
            if (UseTangentRelativeSpeed)
                delta /= GetVelocityMagnitude();
            
            base.Progress(delta);
        }

        protected abstract float GetVelocityMagnitude();
        protected override void OnProgressed(float delta)
        {
            //TODO: assign separate functions to be called by OnProgressed to avoid if statements and returns

            if (IsBaked)
            {
                CurrentPosition = GetValueBakedBySecond(_currentTime);
                CurrentVelocity = new TValue();
                CurrentAcceleration = new TValue();
                return;
            }

            if (_prevKeyframe == null)
                _prevKeyframe = Keyframes.GetKeyBefore(_currentTime, true, true);

            if (_keyframes.Count == 0)
            {
                CurrentPosition = DefaultValue;
                CurrentVelocity = new TValue();
                CurrentAcceleration = new TValue();
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
                        out VectorKeyframe<TValue> nextKeyF,
                        out float normalizedTimeF,
                        out TValue floorPosition,
                        out TValue floorVelocity,
                        out TValue floorAcceleration);

                    _prevKeyframe.Interpolate(ceilSec,
                       out VectorKeyframe<TValue> prevKeyC,
                       out VectorKeyframe<TValue> nextKeyC,
                       out float normalizedTimeC,
                       out TValue ceilPosition,
                       out TValue ceilVelocity,
                       out TValue ceilAcceleration);

                    CurrentPosition = LerpValues(floorPosition, ceilPosition, time);
                    CurrentVelocity = LerpValues(floorVelocity, ceilVelocity, time);
                    CurrentAcceleration = LerpValues(floorAcceleration, ceilAcceleration, time);
                    return;
                }
                second = floorSec;
            }

            _prevKeyframe.Interpolate(second,
                out _prevKeyframe,
                out VectorKeyframe<TValue> nextKey,
                out float normalizedTime,
                out TValue pos,
                out TValue vel,
                out TValue acc);

            CurrentPosition = pos;
            CurrentVelocity = vel;
            CurrentAcceleration = acc;
        }
        private TValue LerpKeyedValues(float floorSec, float ceilSec, float time, EVectorInterpValueType type)
        {
            TValue floorValue = _keyframes.First.Interpolate(floorSec, type,
                out VectorKeyframe<TValue> prevKey, out VectorKeyframe<TValue> nextKey, out float normalizedTime);
            TValue ceilValue = prevKey.Interpolate(ceilSec, type);
            return LerpValues(floorValue, ceilValue, time);
        }
        public override void Bake(float framesPerSecond)
        {
            _bakedFPS = framesPerSecond;
            _bakedFrameCount = (int)Math.Ceiling(LengthInSeconds * framesPerSecond);
            _baked = new TValue[BakedFrameCount];
            float invFPS = 1.0f / _bakedFPS;
            for (int i = 0; i < BakedFrameCount; ++i)
                _baked[i] = GetValueKeyframed(i * invFPS);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void GetMinMax(bool velocity,
            out (float Time, float Value)[] min,
            out (float Time, float Value)[] max)
        {
            float[] inComps = GetComponents(velocity ? new TValue() : DefaultValue), outComps, inTanComps = null, outTanComps = null, valComps;
            int compCount = inComps.Length;

            //No keyframes? Return default value
            if (_keyframes.Count == 0)
            {
                min = max = inComps.Select(x => (0.0f, x)).ToArray();
                return;
            }

            VectorKeyframe<TValue> kf = _keyframes.First;

            //Only one keyframe?
            if (_keyframes.Count == 1)
            {
                //If the second is zero, the in value is irrelevant.
                if (kf.Second.IsZero())
                {
                    outComps = GetComponents(velocity ? kf.OutTangent : kf.OutValue);
                    min = max = outComps.Select(x => (kf.Second, x)).ToArray();
                }
                //Otherwise, if the second is equal to the total length,
                //the out value is irrelevant.
                else if (kf.Second.EqualTo(_keyframes.LengthInSeconds))
                {
                    inComps = GetComponents(velocity ? kf.InTangent : kf.InValue);
                    min = max = inComps.Select(x => (kf.Second, x)).ToArray();
                }
                else
                {
                    //Keyframe is somewhere in between the start and end,
                    //look at both the in and out values
                    inComps = GetComponents(velocity ? kf.InTangent : kf.InValue);
                    outComps = GetComponents(velocity ? kf.OutTangent : kf.OutValue);

                    min = new (float Time, float Value)[compCount];
                    max = new (float Time, float Value)[compCount];

                    for (int i = 0; i < compCount; ++i)
                    {
                        min[i] = (kf.Second, Math.Min(inComps[i], outComps[i]));
                        max[i] = (kf.Second, Math.Max(inComps[i], outComps[i]));
                    }
                }
            }
            else //There are two or more keyframes, need to evaluate interpolation for extrema using velocity
            {
                min = new(float Time, float Value)[compCount];
                min = min.FillWith((0.0f, float.MaxValue));

                max = new(float Time, float Value)[compCount];
                max = max.FillWith((0.0f, float.MinValue));

                VectorKeyframe<TValue> next;
                float minVal, maxVal, oldMin, oldMax;

                //Evaluate all keyframes
                for (int i = 0; i < _keyframes.Count; ++i, kf = next)
                {
                    //Retrieve the next keyframe; will be the first keyframe if this is the last
                    next = kf.Next ?? 
                        (kf.OwningTrack.FirstKey != kf ? 
                        (VectorKeyframe<TValue>)kf.OwningTrack.FirstKey :
                        null);

                    if (next == null)
                        break;

                    inComps = GetComponents(velocity ? next.InTangent : next.InValue);
                    outComps = GetComponents(velocity ? kf.OutTangent : kf.OutValue);

                    bool cubic = kf.InterpolationType > EVectorInterpType.Linear;
                    if (cubic)
                    {
                        inTanComps = GetComponents(next.InTangent);
                        outTanComps = GetComponents(kf.OutTangent);
                    }

                    //Evaluate interpolation
                    for (int x = 0; x < compCount; ++x)
                    {
                        minVal = min[x].Value;
                        maxVal = max[x].Value;

                        oldMin = minVal;
                        oldMax = maxVal;

                        //Check if the keyframe already exceeds the current bounds
                        //If the second is zero, the in value is irrelevant.
                        if (kf.Second.IsZero())
                        {
                            minVal = Math.Min(minVal, outComps[x]);
                            maxVal = Math.Max(maxVal, outComps[x]);
                        }
                        //Otherwise, if the second is equal to the total length,
                        //the out value is irrelevant.
                        else if (kf.Second.EqualTo(_keyframes.LengthInSeconds))
                        {
                            minVal = Math.Min(minVal, inComps[x]);
                            maxVal = Math.Max(maxVal, inComps[x]);
                        }
                        else
                        {
                            //Keyframe is somewhere in between the start and end,
                            //look at both the in and out values
                            minVal = TMath.Min(minVal, inComps[x], outComps[x]);
                            maxVal = TMath.Max(maxVal, inComps[x], outComps[x]);
                        }

                        //Make sure to update the second of the current min and max
                        if (oldMin != minVal)
                            min[x].Time = kf.Second;
                        if (oldMax != maxVal)
                            max[x].Time = kf.Second;

                        if (!cubic)
                            continue;

                        //If not the last keyframe, evaluate the interpolation
                        //between this keyframe and the next to find the exact second(s) where
                        //velocity reaches zero. This means that the position value at that second
                        //is an extrema and should be considered for min/max.
                        
                        //Retrieve velocity interpolation equation coefficients
                        //so we can solve for the time value where acceleration is zero.
                        float second = 0.0f, first = 0.0f, zero = 0.0f;
                        if (velocity)
                        {
                            inComps = GetComponents(next.InValue);
                            outComps = GetComponents(kf.OutValue);

                            if (kf.InterpolationType == EVectorInterpType.CubicHermite)
                                Interp.CubicHermiteAccelerationCoefs(
                                    outComps[x],
                                    outTanComps[x],
                                    inTanComps[x],
                                    inComps[x],
                                    out first, out zero);
                            else
                                Interp.CubicBezierAccelerationCoefs(
                                    outComps[x],
                                    outComps[x] + outTanComps[x],
                                    inComps[x] + inTanComps[x],
                                    inComps[x],
                                    out first, out zero);
                            
                            if (first != 0.0f)
                            {
                                float time = -zero / first;

                                oldMin = minVal;
                                oldMax = maxVal;
                                
                                //We only want times that are within 0 - 1
                                bool timeValid = time >= 0.0f && time <= 1.0f;
                                if (timeValid)
                                {
                                    //Retrieve velocity value using time found where acceleration = 0
                                    TValue val = kf.InterpolateVelocityNormalized(next, time);

                                    //Find real second within the animation using normalized time value
                                    float interpSec = 0.0f;
                                    if (kf.Next == null)
                                    {
                                        //This is the last keyframe,
                                        //So evaluate past the end to the first keyframe
                                        float span = LengthInSeconds - kf.Second + next.Second;
                                        interpSec = (kf.Second + span * time).RemapToRange(0.0f, LengthInSeconds);
                                    }
                                    else //Just lerp from this second to the next, easy
                                        interpSec = Interp.Lerp(kf.Second, next.Second, time);

                                    //Retrieve the components from the value and update min/max and second as usual
                                    valComps = GetComponents(val);

                                    minVal = TMath.Min(minVal, valComps[x]);
                                    maxVal = TMath.Max(maxVal, valComps[x]);

                                    if (oldMin != minVal && minVal == valComps[x])
                                        min[x].Time = interpSec;
                                    if (oldMax != maxVal && maxVal == valComps[x])
                                        max[x].Time = interpSec;
                                }
                            }
                        }
                        else
                        {
                            if (kf.InterpolationType == EVectorInterpType.CubicHermite)
                                Interp.CubicHermiteVelocityCoefs(
                                    outComps[x],
                                    outTanComps[x],
                                    inTanComps[x],
                                    inComps[x],
                                    out second, out first, out zero);
                            else
                                Interp.CubicBezierVelocityCoefs(
                                    outComps[x],
                                    outComps[x] + outTanComps[x],
                                    inComps[x] + inTanComps[x],
                                    inComps[x],
                                    out second, out first, out zero);

                            //Find the roots (zeroes) of the interpolation binomial using the coefficients
                            if (TMath.QuadraticRealRoots(second, first, zero, out float time1, out float time2))
                            {
                                oldMin = minVal;
                                oldMax = maxVal;

                                //The quadratic equation will return two times
                                float[] times = new float[] { time1, time2 };
                                foreach (float time in times)
                                {
                                    //We only want times that are within 0 - 1
                                    bool timeValid = time >= 0.0f && time <= 1.0f;
                                    if (timeValid)
                                    {
                                        //Retrieve position value using time found where velocity = 0
                                        TValue val = kf.InterpolatePositionNormalized(next, time);

                                        //Find real second within the animation using normalized time value
                                        float interpSec = 0.0f;
                                        if (kf.Next == null)
                                        {
                                            //This is the last keyframe,
                                            //So evaluate past the end to the first keyframe
                                            float span = LengthInSeconds - kf.Second + next.Second;
                                            interpSec = (kf.Second + span * time).RemapToRange(0.0f, LengthInSeconds);
                                        }
                                        else //Just lerp from this second to the next, easy
                                            interpSec = Interp.Lerp(kf.Second, next.Second, time);

                                        //Retrieve the components from the value and update min/max and second as usual
                                        valComps = GetComponents(val);

                                        minVal = TMath.Min(minVal, valComps[x]);
                                        maxVal = TMath.Max(maxVal, valComps[x]);

                                        if (oldMin != minVal && minVal == valComps[x])
                                            min[x].Time = interpSec;
                                        if (oldMax != maxVal && maxVal == valComps[x])
                                            max[x].Time = interpSec;
                                    }
                                }
                            }
                        }

                        min[x].Value = minVal;
                        max[x].Value = maxVal;
                    }
                }
            }
        }
        protected abstract float[] GetComponents(TValue value);
        protected abstract TValue GetMaxValue();
        protected abstract TValue GetMinValue();
    }
    public enum EUnifyBias
    {
        In,
        Out,
        Average,
    }
    public abstract class VectorKeyframe<T> : Keyframe, IPlanarKeyframe<T> where T : unmanaged
    {
        //T IPlanarKeyframe<T>.InValue { get => InValue; set => InValue = value; }
        //T IPlanarKeyframe<T>.OutValue { get => OutValue; set => OutValue = value; }
        //T IPlanarKeyframe<T>.InTangent { get => InTangent; set => InTangent = value; }
        //T IPlanarKeyframe<T>.OutTangent { get => OutTangent; set => OutTangent = value; }
        object IPlanarKeyframe.InValue { get => InValue; set => InValue = (T)value; }
        object IPlanarKeyframe.OutValue { get => OutValue; set => OutValue = (T)value; }
        object IPlanarKeyframe.InTangent { get => InTangent; set => InTangent = (T)value; }
        object IPlanarKeyframe.OutTangent { get => OutTangent; set => OutTangent = (T)value; }
        //EVectorInterpType IPlanarKeyframe.InterpolationType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public VectorKeyframe()
            : this(0.0f, new T(), new T(), EVectorInterpType.CubicBezier) { }
        public VectorKeyframe(int frameIndex, float FPS, T inValue, T outValue, T inTangent, T outTangent, EVectorInterpType type)
            : this(frameIndex / FPS, inValue, outValue, inTangent, outTangent, type) { }
        public VectorKeyframe(int frameIndex, float FPS, T inoutValue, T inoutTangent, EVectorInterpType type)
            : this(frameIndex / FPS, inoutValue, inoutValue, inoutTangent, inoutTangent, type) { }
        public VectorKeyframe(float second, T inoutValue, T inoutTangent, EVectorInterpType type)
            : this(second, inoutValue, inoutValue, inoutTangent, inoutTangent, type) { }
        public VectorKeyframe(float second, T inValue, T outValue, T inTangent, T outTangent, EVectorInterpType type) : base()
        {
            Second = second;
            InValue = inValue;
            OutValue = outValue;
            InTangent = inTangent;
            OutTangent = outTangent;
            InterpolationType = type;
        }

        protected delegate T DelInterpolate(VectorKeyframe<T> next, float timeOffset, float timeSpan);
        protected EVectorInterpType _interpolationType;
        protected DelInterpolate _interpolate;
        protected DelInterpolate _interpolateVelocity;
        protected DelInterpolate _interpolateAcceleration;

        [Browsable(false)]
        public override Type ValueType => typeof(T);

        private T _inValue, _outValue, _inTangent, _outTangent;
        private bool _syncInOutValues = true;
        private bool _syncInOutTangentDirections = true;
        private bool _syncInOutTangentMagnitudes = true;
        private bool _synchronizing = false;

        [Category("Editor Traits")]
        [TSerialize(NodeType = ENodeType.Attribute)]
        public bool SyncInOutValues
        {
            get => _syncInOutValues;
            set
            {
                _syncInOutValues = value;
                if (_syncInOutValues && !_synchronizing)
                {
                    _synchronizing = true;
                    UnifyValues(EUnifyBias.Average);
                    _synchronizing = false;
                }
                OwningTrack?.OnChanged();
            }
        }
        [Category("Editor Traits")]
        [TSerialize(NodeType = ENodeType.Attribute)]
        public bool SyncInOutTangentDirections
        {
            get => _syncInOutTangentDirections;
            set
            {
                _syncInOutTangentDirections = value;
                if (_syncInOutTangentDirections && !_synchronizing)
                {
                    _synchronizing = true;
                    UnifyTangentDirections(EUnifyBias.Average);
                    _synchronizing = false;
                }
                OwningTrack?.OnChanged();
            }
        }
        [Category("Editor Traits")]
        [TSerialize(NodeType = ENodeType.Attribute)]
        public bool SyncInOutTangentMagnitudes
        {
            get => _syncInOutTangentMagnitudes;
            set
            {
                _syncInOutTangentMagnitudes = value;
                if (_syncInOutTangentMagnitudes && !_synchronizing)
                {
                    _synchronizing = true;
                    UnifyTangentMagnitudes(EUnifyBias.Average);
                    _synchronizing = false;
                }
                OwningTrack?.OnChanged();
            }
        }
        [Category("Keyframe")]
        [TSerialize(NodeType = ENodeType.Attribute)]
        public T InValue
        {
            get => _inValue;
            set
            {
                _inValue = value;
                if (_syncInOutValues && !_synchronizing)
                {
                    _synchronizing = true;
                    UnifyValues(EUnifyBias.In);
                    _synchronizing = false;
                }
                OwningTrack?.OnChanged();
            }
        }
        [Category("Keyframe")]
        [TSerialize(NodeType = ENodeType.Attribute)]
        public T OutValue
        {
            get => _outValue;
            set
            {
                _outValue = value;
                if (_syncInOutValues && !_synchronizing)
                {
                    _synchronizing = true;
                    UnifyValues(EUnifyBias.Out);
                    _synchronizing = false;
                }
                OwningTrack?.OnChanged();
            }
        }
        [Category("Keyframe")]
        [TSerialize(NodeType = ENodeType.Attribute)]
        public T InTangent
        {
            get => _inTangent;
            set
            {
                _inTangent = value;
                if (!_synchronizing)
                {
                    _synchronizing = true;
                    if (SyncInOutTangentDirections && SyncInOutTangentMagnitudes)
                        UnifyTangents(EUnifyBias.In);
                    else if (SyncInOutTangentMagnitudes)
                        UnifyTangentMagnitudes(EUnifyBias.In);
                    else if (SyncInOutTangentDirections)
                        UnifyTangentDirections(EUnifyBias.In);
                    _synchronizing = false;
                }
                OwningTrack?.OnChanged();
            }
        }
        [Category("Keyframe")]
        [TSerialize(NodeType = ENodeType.Attribute)]
        public T OutTangent
        {
            get => _outTangent;
            set
            {
                _outTangent = value;
                if (!_synchronizing)
                {
                    _synchronizing = true;
                    if (SyncInOutTangentDirections && SyncInOutTangentMagnitudes)
                        UnifyTangents(EUnifyBias.Out);
                    else if (SyncInOutTangentMagnitudes)
                        UnifyTangentMagnitudes(EUnifyBias.Out);
                    else if (SyncInOutTangentDirections)
                        UnifyTangentDirections(EUnifyBias.Out);
                    _synchronizing = false;
                }
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
        [TSerialize(NodeType = ENodeType.Attribute)]
        public EVectorInterpType InterpolationType
        {
            get => _interpolationType;
            set
            {
                _interpolationType = value;
                switch (_interpolationType)
                {
                    case EVectorInterpType.Step:
                        _interpolate = Step;
                        _interpolateVelocity = StepVelocity;
                        _interpolateAcceleration = StepAcceleration;
                        break;
                    case EVectorInterpType.Linear:
                        _interpolate = Lerp;
                        _interpolateVelocity = LerpVelocity;
                        _interpolateAcceleration = LerpAcceleration;
                        break;
                    case EVectorInterpType.CubicHermite:
                        _interpolate = CubicHermite;
                        _interpolateVelocity = CubicHermiteVelocity;
                        _interpolateAcceleration = CubicHermiteAcceleration;
                        break;
                    case EVectorInterpType.CubicBezier:
                        _interpolate = CubicBezier;
                        _interpolateVelocity = CubicBezierVelocity;
                        _interpolateAcceleration = CubicBezierAcceleration;
                        break;
                }
                OwningTrack?.OnChanged();
            }
        }

        public VectorKeyframe<T> GetNextKeyframe(out float span)
        {
            if (IsLast || Next.Second > OwningTrack.LengthInSeconds)
            {
                if (OwningTrack.FirstKey != this)
                {
                    VectorKeyframe<T> next = (VectorKeyframe<T>)OwningTrack.FirstKey;
                    span = OwningTrack.LengthInSeconds - Second + next.Second;
                    return next;
                }
                else
                {
                    span = 0.0f;
                    return null;
                }
            }
            else
            {
                span = _next.Second - Second;
                return Next;
            }
        }
        public VectorKeyframe<T> GetPrevKeyframe(out float span)
        {
            if (IsFirst || Prev.Second < 0.0f)
            {
                if (OwningTrack.LastKey != this)
                {
                    VectorKeyframe<T> prev = (VectorKeyframe<T>)OwningTrack.LastKey;
                    span = OwningTrack.LengthInSeconds - prev.Second + Second;
                    return prev;
                }
                else
                {
                    span = 0.0f;
                    return null;
                }
            }
            else
            {
                span = Second - _prev.Second;
                return Prev;
            }
        }

        /// <summary>
        /// Interpolates from this keyframe to the next using a normalized time value (0.0f - 1.0f)
        /// </summary>
        public T InterpolatePositionNextNormalized(float time)
        {
            var next = GetNextKeyframe(out float span);
            if (next == null)
                return OutValue;
            return _interpolate(next, span * time, span);
        }
        /// <summary>
        /// Interpolates velocity from this keyframe to the next using a normalized time value (0.0f - 1.0f)
        /// </summary>
        public T InterpolateVelocityNextNormalized(float time)
        {
            var next = GetNextKeyframe(out float span);
            if (next == null)
                return OutTangent;
            return _interpolateVelocity(next, span * time, span);
        }
        /// <summary>
        /// Interpolates acceleration from this keyframe to the next using a normalized time value (0.0f - 1.0f)
        /// </summary>
        public T InterpolateAccelerationNextNormalized(float time)
        {
            var next = GetNextKeyframe(out float span);
            if (next == null)
                return default;
            return _interpolateAcceleration(next, span * time, span);
        }

        /// <summary>
        /// Interpolates from this keyframe to the next using a normalized time value (0.0f - 1.0f)
        /// </summary>
        public T InterpolatePositionNormalized(VectorKeyframe<T> next, float time)
        {
            if (next == null)
                return OutValue;

            float span;
            if (next.Second < Second)
                span = OwningTrack.LengthInSeconds - Second + next.Second;
            else
                span = next.Second - Second;
            return _interpolate(next, span * time, span);
        }
        /// <summary>
        /// Interpolates velocity from this keyframe to the next using a normalized time value (0.0f - 1.0f)
        /// </summary>
        public T InterpolateVelocityNormalized(VectorKeyframe<T> next, float time)
        {
            if (next == null)
                return OutTangent;

            float span;
            if (next.Second < Second)
                span = OwningTrack.LengthInSeconds - Second + next.Second;
            else
                span = next.Second - Second;
            return _interpolateVelocity(next, span * time, span);
        }
        /// <summary>
        /// Interpolates acceleration from this keyframe to the next using a normalized time value (0.0f - 1.0f)
        /// </summary>
        public T InterpolateAccelerationNormalized(VectorKeyframe<T> next, float time)
        {
            if (next == null)
                return default;

            float span;
            if (next.Second < Second)
                span = OwningTrack.LengthInSeconds - Second + next.Second;
            else
                span = next.Second - Second;
            return _interpolateAcceleration(next, span * time, span);
        }

        public T Interpolate(float desiredSecond, EVectorInterpValueType type)
        {
            float span = 1.0f, diff = 0.0f;
            VectorKeyframe<T> key1, key2;

            if (desiredSecond >= Second)
            {
                if (IsLast || Next.Second > OwningTrack.LengthInSeconds)
                {
                    if (OwningTrack.FirstKey != this)
                    {
                        VectorKeyframe<T> first = (VectorKeyframe<T>)OwningTrack.FirstKey;
                        span = OwningTrack.LengthInSeconds - Second + first.Second;
                        diff = desiredSecond - Second;
                        key1 = this;
                        key2 = first;
                    }
                    else
                        return type == EVectorInterpValueType.Position ? OutValue : new T();
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
                    return Next.Interpolate(desiredSecond, type);
            }
            else //desiredSecond < Second
            {
                if (!IsFirst)
                    return Prev.Interpolate(desiredSecond, type);

                float length = OwningTrack.LengthInSeconds;
                VectorKeyframe<T> last = (VectorKeyframe<T>)OwningTrack.GetKeyBeforeGeneric(length);
                //VectorKeyframe<T> last = (VectorKeyframe<T>)OwningTrack.LastKey;

                if (last != this && last != null)
                {
                    span = length - last.Second + Second;
                    diff = length - last.Second + desiredSecond;
                    key1 = last;
                    key2 = this;
                }
                else
                    return type == EVectorInterpValueType.Position ? InValue : new T();
            }

            switch (type)
            {
                default:
                case EVectorInterpValueType.Position:
                    return key1._interpolate(key2, diff, span);
                case EVectorInterpValueType.Velocity:
                    return key1._interpolateVelocity(key2, diff, span);
                case EVectorInterpValueType.Acceleration:
                    return key1._interpolateAcceleration(key2, diff, span);
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
                if (IsLast || Next.Second > OwningTrack.LengthInSeconds)
                {
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
                        return type == EVectorInterpValueType.Position ? OutValue : new T();
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
                if (!IsFirst)
                    return Prev.Interpolate(desiredSecond, type, out prevKey, out nextKey, out normalizedTime);

                float length = OwningTrack.LengthInSeconds;
                VectorKeyframe<T> last = (VectorKeyframe<T>)OwningTrack.GetKeyBeforeGeneric(length);
                //VectorKeyframe<T> last = (VectorKeyframe<T>)OwningTrack.LastKey;

                if (last != this && last != null)
                {
                    span = length - last.Second + Second;
                    diff = length - last.Second + desiredSecond;
                    key1 = last;
                    key2 = this;
                }
                else
                {
                    normalizedTime = 0.0f;
                    return type == EVectorInterpValueType.Position ? InValue : new T();
                }
            }

            normalizedTime = diff / span;
            switch (type)
            {
                default:
                case EVectorInterpValueType.Position:
                    return key1._interpolate(key2, diff, span);
                case EVectorInterpValueType.Velocity:
                    return key1._interpolateVelocity(key2, diff, span);
                case EVectorInterpValueType.Acceleration:
                    return key1._interpolateAcceleration(key2, diff, span);
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
                if (IsLast || Next.Second > OwningTrack.LengthInSeconds)
                {
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
                if (!IsFirst)
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
                else
                {
                    float length = OwningTrack.LengthInSeconds;
                    VectorKeyframe<T> last = (VectorKeyframe<T>)OwningTrack.GetKeyBeforeGeneric(length);
                    //VectorKeyframe<T> last = (VectorKeyframe<T>)OwningTrack.LastKey;

                    if (last != this && last != null)
                    {
                        span = length - last.Second + Second;
                        diff = length - last.Second + desiredSecond;
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
            }

            normalizedTime = diff / span;
            position = key1._interpolate(key2, diff, span);
            velocity = key1._interpolateVelocity(key2, diff, span);
            acceleration = key1._interpolateAcceleration(key2, diff, span);
        }

        public T Step(VectorKeyframe<T> next, float diff, float span) => (diff / span) < 1.0f ? OutValue : next.OutValue;
        public T StepVelocity(VectorKeyframe<T> next, float diff, float span) => new T();
        public T StepAcceleration(VectorKeyframe<T> next, float diff, float span) => new T();

        public abstract T Lerp(VectorKeyframe<T> next, float diff, float span);
        public abstract T LerpVelocity(VectorKeyframe<T> next, float diff, float span);
        public T LerpAcceleration(VectorKeyframe<T> next, float diff, float span) => new T();

        public abstract T CubicHermite(VectorKeyframe<T> next, float diff, float span);
        public abstract T CubicHermiteVelocity(VectorKeyframe<T> next, float diff, float span);
        public abstract T CubicHermiteAcceleration(VectorKeyframe<T> next, float diff, float span);

        public abstract T CubicBezier(VectorKeyframe<T> next, float diff, float span);
        public abstract T CubicBezierVelocity(VectorKeyframe<T> next, float diff, float span);
        public abstract T CubicBezierAcceleration(VectorKeyframe<T> next, float diff, float span);

        [GridCallable]
        public void AverageKeyframe(
            EUnifyBias valueBias, 
            EUnifyBias tangentBias,
            bool tangentDirections, 
            bool tangentMagnitudes)
        {
            UnifyValues(valueBias);
            if (tangentDirections)
            {
                if (tangentMagnitudes)
                    UnifyTangents(tangentBias);
                else
                    UnifyTangentDirections(tangentBias);
            }
            else
                UnifyTangentMagnitudes(tangentBias);
        }
        [GridCallable]
        public abstract void UnifyTangents(EUnifyBias bias);
        [GridCallable]
        public abstract void UnifyTangentDirections(EUnifyBias bias);
        [GridCallable]
        public abstract void UnifyTangentMagnitudes(EUnifyBias bias);
        [GridCallable]
        public abstract void UnifyValues(EUnifyBias bias);
        [GridCallable]
        public abstract void MakeOutLinear();
        [GridCallable]
        public abstract void MakeInLinear();
        
        public void UnifyKeyframe(EUnifyBias bias)
        {
            UnifyTangents(bias);
            UnifyValues(bias);
        }
    }
}

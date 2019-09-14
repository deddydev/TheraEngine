using Extensions;
using System;
using System.ComponentModel;

namespace TheraEngine.Animation
{
    public abstract class PropAnimLerpable<TValue, TValueKey> : PropAnimKeyframed<TValueKey>
        where TValue : unmanaged
        where TValueKey : LerpableKeyframe<TValue>, new()
    {
        public PropAnimLerpable()
            : this(0.0f, false) { }
        public PropAnimLerpable(float lengthInSeconds, bool looped, bool isBaked = false)
            : base(lengthInSeconds, looped, isBaked) { }
        public PropAnimLerpable(int frameCount, float framesPerSecond, bool looped, bool isBaked = false)
            : base(frameCount, framesPerSecond, looped, isBaked) { }

        public event Action<PropAnimLerpable<TValue, TValueKey>> DefaultValueChanged;
        public event Action<PropAnimLerpable<TValue, TValueKey>> ConstrainKeyframedFPSChanged;
        public event Action<PropAnimLerpable<TValue, TValueKey>> LerpConstrainedFPSChanged;
        public event Action<PropAnimLerpable<TValue, TValueKey>> CurrentValueChanged;

        private DelGetValue<TValue> _getValue;
        private TValue _defaultValue = new TValue();
        private bool _constrainKeyframedFPS = false;
        private bool _lerpConstrainedFPS = false;
        private LerpableKeyframe<TValue> _prevKeyframe = null;
        private TValue _currentPosition;

        [TSerialize("BakedValues", Condition = "IsBaked")]
        private TValue[] _baked = null;

        /// <summary>
        /// The default value to return when no keyframes are set.
        /// </summary>
        [Description("The default value to return when no keyframes are set.")]
        [Category(PropAnimCategory)]
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
        /// <summary>
        /// The value at the current time.
        /// </summary>
        public TValue CurrentPosition
        {
            get => _currentPosition;
            private set
            {
                _currentPosition = value;
                CurrentValueChanged?.Invoke(this);
            }
        }

        protected override object GetCurrentValueGeneric() => CurrentPosition;
        protected override object GetValueGeneric(float second) => _getValue(second);

        public TValue GetValue(float second) => _getValue(second);
        public TValue GetValueBakedBySecond(float second)
        {
            float frameTime = second * BakedFramesPerSecond;
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
            else
            {
                return _baked[frame];
            }
        }
        /// <summary>
        /// Returns a value from the baked array by frame index.
        /// </summary>
        /// <param name="frame">The frame to get a value for.</param>
        /// <returns>The value at the specified frame.</returns>
        public TValue GetValueBakedByFrame(int frame)
        {
            if (!_baked.IndexInRange(frame))
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
            => _baked is null || _baked.Length == 0 ? new TValue() :
            _baked[frameIndex.Clamp(0, _baked.Length - 1)];
        
        public TValue GetValueKeyframed(float second)
        {
            if (Keyframes.Count == 0)
                return DefaultValue;

            if (ConstrainKeyframedFPS)
            {
                int frame = (int)(second * _bakedFPS);
                float floorSec = frame / _bakedFPS;
                float ceilSec = (frame + 1) / _bakedFPS;
                float time = second - floorSec;

                if (LerpConstrainedFPS)
                    return LerpKeyedValues(floorSec, ceilSec, time);

                second = floorSec;
            }

            return Keyframes.First.Interpolate(second);
        }
        [Category(AnimCategory)]
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
                return;
            }

            if (_prevKeyframe is null)
                _prevKeyframe = Keyframes.First;
            if (Keyframes.Count == 0)
            {
                CurrentPosition = DefaultValue;
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
                        out LerpableKeyframe<TValue> nextKeyF,
                        out float normalizedTimeF,
                        out TValue floorPosition);

                    _prevKeyframe.Interpolate(ceilSec,
                       out LerpableKeyframe<TValue> prevKeyC,
                       out LerpableKeyframe<TValue> nextKeyC,
                       out float normalizedTimeC,
                       out TValue ceilPosition);

                    CurrentPosition = LerpValues(floorPosition, ceilPosition, time);
                    return;
                }
                second = floorSec;
            }

            _prevKeyframe.Interpolate(second,
                out _prevKeyframe,
                out LerpableKeyframe<TValue> nextKey,
                out float normalizedTime,
                out TValue pos);

            CurrentPosition = pos;
        }
        private TValue LerpKeyedValues(float floorSec, float ceilSec, float time)
        {
            TValue floorValue = Keyframes.First.Interpolate(floorSec,
                out LerpableKeyframe<TValue> prevKey, out LerpableKeyframe<TValue> nextKey, out float normalizedTime);
            TValue ceilValue = prevKey.Interpolate(ceilSec);
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
    }
    public abstract class LerpableKeyframe<T> : Keyframe where T : unmanaged
    {
        public LerpableKeyframe()
            : this(0.0f, new T(), new T()) { }
        public LerpableKeyframe(int frameIndex, float FPS, T inValue, T outValue)
            : this(frameIndex / FPS, inValue, outValue) { }
        public LerpableKeyframe(int frameIndex, float FPS, T inoutValue)
            : this(frameIndex / FPS, inoutValue, inoutValue) { }
        public LerpableKeyframe(float second, T inoutValue)
            : this(second, inoutValue, inoutValue) { }
        public LerpableKeyframe(float second, T inValue, T outValue) : base()
        {
            Second = second;
            InValue = inValue;
            OutValue = outValue;
        }

        protected delegate T DelInterpolate(LerpableKeyframe<T> key1, LerpableKeyframe<T> key2, float time);
        protected DelInterpolate _interpolate;

        [Browsable(false)]
        public override Type ValueType => typeof(T);

        private T _inValue, _outValue;
        
        [Category("Keyframe")]
        [TSerialize(NodeType = ENodeType.Attribute)]
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
        [TSerialize(NodeType = ENodeType.Attribute)]
        public T OutValue
        {
            get => _outValue;
            set
            {
                _outValue = value;
                OwningTrack?.OnChanged();
            }
        }

        [Browsable(false)]
        [Category("Keyframe")]
        public new LerpableKeyframe<T> Next
        {
            get => _next as LerpableKeyframe<T>;
            //set => _next = value;
        }
        [Browsable(false)]
        [Category("Keyframe")]
        public new LerpableKeyframe<T> Prev
        {
            get => _prev as LerpableKeyframe<T>;
            //set => _prev = value;
        }
        
        /// <summary>
        /// Interpolates from this keyframe to the next using a normalized time value (0.0f - 1.0f)
        /// </summary>
        public T InterpolateNextNormalized(float time) => _interpolate(this, Next, time);

        /// <summary>
        /// Interpolates from this keyframe to the next using a normalized time value (0.0f - 1.0f)
        /// </summary>
        public T InterpolateNormalized(LerpableKeyframe<T> next, float time) => _interpolate(this, next, time);

        public T Interpolate(float desiredSecond)
        {
            float span, diff;
            LerpableKeyframe<T> key1, key2;

            if (desiredSecond >= Second)
            {
                if (Next is null)
                {
                    //This is the last keyframe

                    if (OwningTrack.FirstKey != this)
                    {
                        LerpableKeyframe<T> first = (LerpableKeyframe<T>)OwningTrack.FirstKey;
                        span = OwningTrack.LengthInSeconds - Second + first.Second;
                        diff = desiredSecond - Second;
                        key1 = this;
                        key2 = first;
                    }
                    else
                    {
                        return OutValue;
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
                    return Next.Interpolate(desiredSecond);
                }
            }
            else //desiredSecond < Second
            {
                if (Prev != null)
                    return Prev.Interpolate(desiredSecond);

                //This is the first keyframe

                if (OwningTrack.LastKey != this)
                {
                    LerpableKeyframe<T> last = (LerpableKeyframe<T>)OwningTrack.LastKey;
                    span = OwningTrack.LengthInSeconds - last.Second + Second;
                    diff = OwningTrack.LengthInSeconds - last.Second + desiredSecond;
                    key1 = last;
                    key2 = this;
                }
                else
                {
                    return InValue;
                }
            }

            float time = diff / span;
            return _interpolate(key1, key2, time);
        }
        public T Interpolate(
            float desiredSecond,
            out LerpableKeyframe<T> prevKey,
            out LerpableKeyframe<T> nextKey,
            out float normalizedTime)
        {
            prevKey = this;
            nextKey = Next;

            float span, diff;
            LerpableKeyframe<T> key1, key2;

            if (desiredSecond >= Second)
            {
                if (Next is null)
                {
                    //This is the last keyframe

                    if (OwningTrack.FirstKey != this)
                    {
                        LerpableKeyframe<T> first = (LerpableKeyframe<T>)OwningTrack.FirstKey;
                        span = OwningTrack.LengthInSeconds - Second + first.Second;
                        diff = desiredSecond - Second;
                        key1 = this;
                        key2 = first;
                    }
                    else
                    {
                        normalizedTime = 0.0f;
                        return OutValue;
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
                    return Next.Interpolate(desiredSecond, out prevKey, out nextKey, out normalizedTime);
                }
            }
            else //desiredSecond < Second
            {
                if (Prev != null)
                    return Prev.Interpolate(desiredSecond, out prevKey, out nextKey, out normalizedTime);

                //This is the first keyframe

                if (OwningTrack.LastKey != this)
                {
                    LerpableKeyframe<T> last = (LerpableKeyframe<T>)OwningTrack.LastKey;
                    span = OwningTrack.LengthInSeconds - last.Second + Second;
                    diff = OwningTrack.LengthInSeconds - last.Second + desiredSecond;
                    key1 = last;
                    key2 = this;
                }
                else
                {
                    normalizedTime = 0.0f;
                    return InValue;
                }
            }

            normalizedTime = diff / span;
            return _interpolate(key1, key2, normalizedTime);
        }
        public void Interpolate(
            float desiredSecond,
            out LerpableKeyframe<T> prevKey,
            out LerpableKeyframe<T> nextKey,
            out float normalizedTime,
            out T position)
        {
            prevKey = this;
            nextKey = Next;

            float span, diff;
            LerpableKeyframe<T> key1, key2;

            if (desiredSecond >= Second)
            {
                if (Next is null)
                {
                    //This is the last keyframe

                    if (OwningTrack.FirstKey != this)
                    {
                        LerpableKeyframe<T> first = (LerpableKeyframe<T>)OwningTrack.FirstKey;
                        span = OwningTrack.LengthInSeconds - Second + first.Second;
                        diff = desiredSecond - Second;
                        key1 = this;
                        key2 = first;
                    }
                    else
                    {
                        normalizedTime = 0.0f;
                        position = OutValue;
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
                        out position);

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
                        out position);

                    return;
                }

                //This is the first keyframe

                if (OwningTrack.LastKey != this)
                {
                    LerpableKeyframe<T> last = (LerpableKeyframe<T>)OwningTrack.LastKey;
                    span = OwningTrack.LengthInSeconds - last.Second + Second;
                    diff = OwningTrack.LengthInSeconds - last.Second + desiredSecond;
                    key1 = last;
                    key2 = this;
                }
                else
                {
                    normalizedTime = 0.0f;
                    position = InValue;
                    return;
                }
            }

            normalizedTime = diff / span;
            position = _interpolate(key1, key2, normalizedTime);
        }

        public T Step(LerpableKeyframe<T> key1, LerpableKeyframe<T> key2, float time)
            => time < 1.0f ? key1.OutValue : key2.OutValue;

        public abstract T Lerp(LerpableKeyframe<T> key1, LerpableKeyframe<T> key2, float time);
    }
}
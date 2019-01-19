using System;
using System.ComponentModel;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using TheraEngine.Core.Files;
using TheraEngine.Input.Devices;
using TheraEngine.Core.Attributes;

namespace TheraEngine.Animation
{
    /// <summary>
    /// Dictates the current state of an animation.
    /// </summary>
    public enum EAnimationState
    {
        /// <summary>
        /// Stopped means that the animation is not playing and is set to its initial start position.
        /// </summary>
        Stopped,
        /// <summary>
        /// Paused means that the animation is not currently playing
        /// but is stopped at some arbitrary point in the animation, ready to start from that point again.
        /// </summary>
        Paused,
        /// <summary>
        /// Playing means that the animation is currently progressing forward.
        /// </summary>
        Playing,
    }
    [TFileExt("anim")]
    [TFileDef("Animation")]
    public abstract class BaseAnimation : TFileObject
    {
        protected const string AnimCategory = "Animation";

        public event Action<BaseAnimation> AnimationStarted;
        public event Action<BaseAnimation> AnimationEnded;
        public event Action<BaseAnimation> AnimationPaused;
        public event Action<BaseAnimation> CurrentFrameChanged;
        public event Action<BaseAnimation> SpeedChanged;
        public event Action<BaseAnimation> LoopChanged;
        public event Action<BaseAnimation> LengthChanged;
        public event Action<BaseAnimation> TickSelfChanged;

        protected void OnAnimationStarted() => AnimationStarted?.Invoke(this);
        protected void OnAnimationEnded() => AnimationEnded?.Invoke(this);
        protected void OnAnimationPaused() => AnimationPaused?.Invoke(this);
        protected void OnCurrentTimeChanged() => CurrentFrameChanged?.Invoke(this);
        protected void OnSpeedChanged() => SpeedChanged?.Invoke(this);
        protected void OnLoopChanged() => LoopChanged?.Invoke(this);
        protected void OnLengthChanged() => LengthChanged?.Invoke(this);
        protected void OnTickSelfChanged() => TickSelfChanged?.Invoke(this);

        [TSerialize("TickGroup")]
        protected ETickGroup _group = ETickGroup.PostPhysics;
        [TSerialize("TickOrder")]
        protected ETickOrder _order = ETickOrder.Animation;
        [TSerialize("TickPausedBehavior")]
        protected EInputPauseType _pausedBehavior = EInputPauseType.TickAlways;
        [TSerialize("LengthInSeconds")]
        protected float _lengthInSeconds = 0.0f;
        [TSerialize("Speed")]
        protected float _speed = 1.0f;
        [TSerialize("CurrentTime")]
        protected float _currentTime = 0.0f;
        [TSerialize("Looped")]
        protected bool _looped = false;
        [TSerialize("State")]
        protected EAnimationState _state = EAnimationState.Stopped;
        //[TSerialize("TickSelf")]
        protected bool _tickSelf = true;

        public BaseAnimation(float lengthInSeconds, bool looped)
        {
            _lengthInSeconds = lengthInSeconds;
            Looped = looped;
        }

        [Category(AnimCategory)]
        public ETickGroup Group
        {
            get => _group;
            set
            {
                if (_group == value)
                    return;
                if (_state == EAnimationState.Playing)
                {
                    UnregisterTick(_group, _order, Progress, _pausedBehavior);
                    RegisterTick(value, _order, Progress, _pausedBehavior);
                }
                _group = value;
            }
        }
        [Category(AnimCategory)]
        public ETickOrder Order
        {
            get => _order;
            set
            {
                if (_order == value)
                    return;
                if (_state == EAnimationState.Playing)
                {
                    UnregisterTick(_group, _order, Progress, _pausedBehavior);
                    RegisterTick(_group, value, Progress, _pausedBehavior);
                }
                _order = value;
            }
        }
        [Category(AnimCategory)]
        public EInputPauseType PausedBehavior
        {
            get => _pausedBehavior;
            set
            {
                if (_pausedBehavior == value)
                    return;
                if (_state == EAnimationState.Playing)
                {
                    UnregisterTick(_group, _order, Progress, _pausedBehavior);
                    RegisterTick(_group, _order, Progress, value);
                }
                _pausedBehavior = value;
            }
        }
        /// <summary>
        /// If true, the animation will progress itself when playing.
        /// If false, <see cref="Progress(float)"/> must be called to progress the animation.
        /// </summary>
        [Browsable(false)]
        [Category(AnimCategory)]
        [TSerialize(NodeType = ENodeType.Attribute)]
        public bool TickSelf
        {
            get => _tickSelf;
            set
            {
                if (_tickSelf == value)
                    return;
                _tickSelf = value;

                if (_state == EAnimationState.Playing)
                    RegisterTick(!_tickSelf);

                OnTickSelfChanged();
            }
        }

        public void SetFrameCount(int numFrames, float framesPerSecond, bool stretchAnimation)
            => SetLength(numFrames / framesPerSecond, stretchAnimation);
        public virtual void SetLength(float seconds, bool stretchAnimation, bool notifyChanged = true)
        {
            if (seconds < 0.0f)
                return;
            _lengthInSeconds = seconds;
            if (notifyChanged)
                OnLengthChanged();
        }

        [DisplayName("Length")]
        [TNumericPrefixSuffix(null, " sec")]
        [Category(AnimCategory)]
        public float LengthInSeconds
        {
            get => _lengthInSeconds;
            set => SetLength(value, false);
        }

        /// <summary>
        /// How fast the animation plays back.
        /// A speed of 2.0f would shorten the animation to play in half the time, where 0.5f would be lengthen the animation to play two times slower.
        /// CAN be negative to play the animation in reverse.
        /// </summary>
        [Category(AnimCategory)]
        public float Speed
        {
            get => _speed;
            set
            {
                _speed = value;
                OnSpeedChanged();
            }
        }
        [Category(AnimCategory)]
        public bool Looped
        {
            get => _looped;
            set
            {
                _looped = value;
                OnLoopChanged();
            }
        }
        /// <summary>
        /// Sets the current time within the animation.
        /// Do not use to progress forward or backward every frame, instead use Progress().
        /// </summary>
        [TNumericPrefixSuffix(null, " sec")]
        [Category(AnimCategory)]
        public virtual float CurrentTime
        {
            get => _currentTime;
            set
            {
                _currentTime = value.RemapToRange(0.0f, _lengthInSeconds);
                OnCurrentTimeChanged();
            }
        }

        [Category(AnimCategory)]
        public EAnimationState State
        {
            get => _state;
            set
            {
                if (value == _state)
                    return;
                switch (value)
                {
                    case EAnimationState.Playing:
                        Start();
                        break;
                    case EAnimationState.Paused:
                        Pause();
                        break;
                    case EAnimationState.Stopped:
                        Stop();
                        break;
                }
            }
        }

        [TPostDeserialize]
        internal virtual void PostDeserialize()
        {
            if (_state == EAnimationState.Playing)
            {
                _state = EAnimationState.Paused;
                Start();
            }
        }
        
        protected virtual void PreStarted() { }
        protected virtual void PostStarted() { }
        public virtual void Start()
        {
            if (_state == EAnimationState.Playing)
                return;
            PreStarted();
            if (_state == EAnimationState.Stopped)
                _currentTime = 0.0f;
            _state = EAnimationState.Playing;
            OnAnimationStarted();
            if (TickSelf)
                RegisterTick(false);
            PostStarted();
        }
        protected virtual void PreStopped() { }
        protected virtual void PostStopped() { }
        public virtual void Stop()
        {
            if (_state == EAnimationState.Stopped)
                return;
            PreStopped();
            _state = EAnimationState.Stopped;
            OnAnimationEnded();
            if (TickSelf)
                RegisterTick(true);
            PostStopped();
        }
        protected virtual void PrePaused() { }
        protected virtual void PostPaused() { }
        public virtual void Pause()
        {
            if (_state != EAnimationState.Playing)
                return;
            PrePaused();
            _state = EAnimationState.Paused;
            OnAnimationPaused();
            if (TickSelf)
                RegisterTick(true);
            PostPaused();
        }
        protected virtual void RegisterTick(bool unregister = false)
        {
            if (unregister)
                UnregisterTick(_group, _order, Progress, _pausedBehavior);
            else
                RegisterTick(_group, _order, Progress, _pausedBehavior);
        }
        /// <summary>
        /// Progresses this animation forward (or backward) by the specified change in seconds.
        /// </summary>
        /// <param name="delta">The change in seconds to add to the current time. Negative values are allowed.</param>
        public void Progress(float delta)
        {
            //Modify delta with speed
            delta *= Speed;

            //Increment the current time with the delta value
            _currentTime += delta;

            //Is the new current time out of range of the animation?
            bool greater = _currentTime >= _lengthInSeconds;
            bool less = _currentTime <= 0.0f;
            if (greater || less)
            {
                //If playing but not looped, end the animation
                if (_state == EAnimationState.Playing && !_looped)
                {
                    //Correct delta and current time for over-progression past the start or end point
                    if (greater)
                    {
                        delta -= _currentTime - _lengthInSeconds;
                        _currentTime = _lengthInSeconds;
                    }
                    else if (less)
                    {
                        delta -= _currentTime;
                        _currentTime = 0.0f;
                    }
                    //Progress whatever delta is remaining and then stop
                    OnProgressed(delta);
                    OnCurrentTimeChanged();
                    Stop();
                    return;
                }
                else
                {
                    //Remap current time into proper range and correct delta
                    float remappedCurrentTime = _currentTime.RemapToRange(0.0f, _lengthInSeconds);
                    delta = remappedCurrentTime - _currentTime;
                    _currentTime = remappedCurrentTime;
                }
            }

            OnProgressed(delta);
            OnCurrentTimeChanged();
        }
        /// <summary>
        /// Called when <see cref="Progress(float)"/> has been called and <see cref="CurrentTime"/> has been updated.
        /// </summary>
        /// <param name="delta"></param>
        protected abstract void OnProgressed(float delta);
    }
}

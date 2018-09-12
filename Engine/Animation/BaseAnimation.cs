using TheraEngine.Files;
using System;
using System.ComponentModel;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using TheraEngine.Core.Reflection.Attributes;

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
    [FileExt("anim")]
    [FileDef("Animation")]
    public abstract class BaseAnimation : TFileObject
    {
        public event Action AnimationStarted;
        public event Action AnimationEnded;
        public event Action AnimationPaused;
        public event Action CurrentFrameChanged;
        public event Action SpeedChanged;
        public event Action LoopChanged;
        public event Action LengthChanged;
        public event Action FPSChanged;

        protected void OnAnimationStarted() => AnimationStarted?.Invoke();
        protected void OnAnimationEnded() => AnimationEnded?.Invoke();
        protected void OnAnimationPaused() => AnimationPaused?.Invoke();
        protected void OnCurrentFrameChanged() => CurrentFrameChanged?.Invoke();
        protected void OnSpeedChanged() => SpeedChanged?.Invoke();
        protected void OnLoopChanged() => LoopChanged?.Invoke();
        protected void OnLengthChanged() => LengthChanged?.Invoke();
        protected void OnFPSChanged() => FPSChanged?.Invoke();

        [TSerialize("BakedFrameCount")]
        protected int _bakedFrameCount = 0;
        [TSerialize("BakedFPS")]
        protected float _bakedFPS = 0.0f;

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
        [TSerialize("IsBaked")]
        protected bool _isBaked = false;

        public BaseAnimation(float lengthInSeconds, bool looped, bool isBaked = false)
        {
            _bakedFPS = 60.0f;
            _lengthInSeconds = lengthInSeconds;
            SetBakedFramecount();
            Looped = looped;
            Baked = isBaked;
        }
        public BaseAnimation(int frameCount, float framesPerSecond, bool looped, bool isBaked = false)
        {
            _bakedFrameCount = frameCount;
            _bakedFPS = framesPerSecond.ClampMin(0.0f);

            if (_bakedFPS == 0.0f)
                _lengthInSeconds = 0;
            else
                _lengthInSeconds = _bakedFrameCount / _bakedFPS;

            Looped = looped;
            Baked = isBaked;
        }

        protected void SetBakedFramecount()
        {
            _bakedFrameCount = (int)Math.Ceiling(_lengthInSeconds * _bakedFPS);
        }

        /// <summary>
        /// Determines which method to use, baked or keyframed.
        /// Keyframed takes up less memory and calculates in-between frames on the fly, which allows for time dilation.
        /// Baked takes up more memory but requires no calculations. However, the animation cannot be sped up at all, nor slowed down without artifacts.
        /// </summary>
        [Category("Animation"), TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public bool Baked
        {
            get => _isBaked;
            set
            {
                _isBaked = value;
                BakedChanged();
            }
        }

        protected abstract void BakedChanged();

        public void SetFrameCount(int numFrames, float framesPerSecond, bool stretchAnimation)
            => SetLength(numFrames / framesPerSecond, stretchAnimation);
        public virtual void SetLength(float seconds, bool stretchAnimation)
        {
            if (seconds < 0.0f)
                return;
            _lengthInSeconds = seconds;
            SetBakedFramecount();
            LengthChanged?.Invoke();
        }

        [Category("Animation")]
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
        [Category("Animation")]
        public float Speed
        {
            get => _speed;
            set
            {
                _speed = value;
                SpeedChanged?.Invoke();
            }
        }
        /// <summary>
        /// How many frames of this animation should pass in a second.
        /// For example, if the animation is 30fps, and the game is running at 60fps,
        /// Only one frame of this animation will show for every two game frames (the animation won't be sped up).
        /// </summary>
        [Category("Animation")]
        public float BakedFramesPerSecond
        {
            get => _bakedFPS;
            set
            {
                _bakedFPS = value;
                SetBakedFramecount();
                OnFPSChanged();
            }
        }

        /// <summary>
        /// How many frames this animation contains.
        /// </summary>
        [Category("Animation")]
        public int BakedFrameCount
        {
            get => _bakedFrameCount;
            set
            {
                _bakedFrameCount = value;

                if (_bakedFPS <= 0.0f)
                    LengthInSeconds = 0.0f;
                else
                    LengthInSeconds = _bakedFrameCount / _bakedFPS;
            }
        }

        [Category("Animation")]
        public bool Looped
        {
            get => _looped;
            set
            {
                _looped = value;
                LoopChanged?.Invoke();
            }
        }
        /// <summary>
        /// Sets the current time within the animation.
        /// Do not use to progress forward or backward every frame, instead use Progress().
        /// </summary>
        [Category("Animation")]
        public float CurrentTime
        {
            get => _currentTime;
            set
            {
                _currentTime = value;
                if (_currentTime > _lengthInSeconds || _currentTime < 0.0f)
                {
                    if (_state == EAnimationState.Playing && !_looped)
                        Stop();
                    else
                        _currentTime = _currentTime.RemapToRange(0.0f, _lengthInSeconds);
                }
                OnCurrentFrameChanged();
            }
        }

        [Category("Animation")]
        public EAnimationState State
        {
            get => _state;
            set
            {
                if (value == _state)
                    return;
                //_state = value;
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

        [PostDeserialize]
        private void PostDeserialize()
        {
            if (_state == EAnimationState.Playing)
            {
                _state = EAnimationState.Paused;
                Start();
            }
        }

        #region Playback
        protected virtual void PreStarted() { }
        protected virtual void PostStarted() { }
        public virtual void Start()
        {
            if (_state == EAnimationState.Playing)
                return;
            PreStarted();
            _state = EAnimationState.Playing;
            AnimationStarted?.Invoke();
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Animation, Progress, Input.Devices.EInputPauseType.TickAlways);
            PostStarted();
        }
        protected virtual void PreStopped() { }
        protected virtual void PostStopped() { }
        public virtual void Stop()
        {
            if (_state == EAnimationState.Stopped)
                return;
            PreStopped();
            _currentTime = 0.0f;
            _state = EAnimationState.Stopped;
            OnAnimationEnded();
            UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Animation, Progress, Input.Devices.EInputPauseType.TickAlways);
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
            UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Animation, Progress, Input.Devices.EInputPauseType.TickAlways);
            PostPaused();
        }
        #endregion

        /// <summary>
        /// Progresses this animation forward by the specified change in seconds.
        /// </summary>
        /// <param name="delta"></param>
        public void Progress(float delta)
        {
            //TODO: don't use CurrentTime,
            //determine if delta * speed is positive or negative,
            //optimize previously accessed keyframe accordingly
            CurrentTime += delta * _speed;
        }

        /// <summary>
        /// Bakes the interpolated data for fastest access by the game.
        /// However, this method takes up more space and does not support time dilation (speeding up and slowing down with proper in-betweens)
        /// </summary>
        public abstract void Bake(float framesPerSecond);
    }
}

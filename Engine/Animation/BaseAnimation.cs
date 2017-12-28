using TheraEngine.Files;
using System;
using System.ComponentModel;
using TheraEngine.Core.Reflection.Attributes.Serialization;

namespace TheraEngine.Animation
{
    /// <summary>
    /// Dictates the current state of an animation.
    /// </summary>
    public enum AnimationState
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
    public abstract class BaseAnimation : FileObject
    {
        public event Action AnimationStarted;
        public event Action AnimationEnded;
        public event Action CurrentFrameChanged;
        public event Action SpeedChanged;
        public event Action LoopChanged;
        public event Action LengthChanged;

        [TSerialize("BakedFrameCount", Condition = "_isBaked")]
        protected int _bakedFrameCount = 0;
        [TSerialize("BakedFPS", Condition = "_isBaked")]
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
        protected AnimationState _state = AnimationState.Stopped;
        [TSerialize("IsBaked")]
        protected bool _isBaked = false;

        public BaseAnimation(float lengthInSeconds, bool looped, bool isBaked = false)
        {
            _bakedFrameCount = (int)Math.Ceiling(lengthInSeconds * 60.0f);
            _bakedFPS = 60.0f;
            _lengthInSeconds = lengthInSeconds;
            Looped = looped;
            Baked = isBaked;
        }
        public BaseAnimation(int frameCount, float FPS, bool looped, bool isBaked = false)
        {
            _bakedFrameCount = frameCount;
            _bakedFPS = FPS;
            _lengthInSeconds = frameCount / FPS;
            Looped = looped;
            Baked = isBaked;
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
            _lengthInSeconds = seconds;
            LengthChanged?.Invoke();
        }

        [Category("Animation")]
        public float LengthInSeconds => _lengthInSeconds;
        
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
        public float BakedFramesPerSecond => _bakedFPS;
        
        /// <summary>
        /// How many frames this animation contains.
        /// </summary>
        [Category("Animation")]
        public int BakedFrameCount => _bakedFrameCount;
        
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
        [Category("Animation")]
        public float CurrentTime
        {
            get => _currentTime;
            set
            {
                _currentTime = value;
                if (_currentTime > _lengthInSeconds || _currentTime < 0.0f)
                {
                    if (_state == AnimationState.Playing && !_looped)
                        Stop();
                    else
                        _currentTime = _currentTime.RemapToRange(0.0f, _lengthInSeconds);
                }
                OnCurrentFrameChanged();
            }
        }

        [Category("Animation")]
        public AnimationState State  => _state;

        [PostDeserialize]
        private void PostDeserialize()
        {
            if (_state == AnimationState.Playing)
            {
                _state = AnimationState.Stopped;
                Start();
            }
        }
        protected virtual void PreStarted() { }
        protected virtual void PostStarted() { }
        public void Start()
        {
            if (_state == AnimationState.Playing)
                return;
            PreStarted();
            _state = AnimationState.Playing;
            AnimationStarted?.Invoke();
            CurrentTime = 0.0f;
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Animation, Progress, Input.Devices.InputPauseType.TickOnlyWhenUnpaused);
            PostStarted();
        }
        protected virtual void PreStopped() { }
        protected virtual void PostStopped() { }
        public void Stop()
        {
            if (_state == AnimationState.Stopped)
                return;
            PreStopped();
            _currentTime = 0.0f;
            _state = AnimationState.Stopped;
            AnimationEnded?.Invoke();
            UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Animation, Progress, Input.Devices.InputPauseType.TickOnlyWhenUnpaused);
            PostStopped();
        }
        protected virtual void PrePaused() { }
        protected virtual void PostPaused() { }
        public void Pause()
        {
            if (_state != AnimationState.Playing)
                return;
            PreStopped();
            _state = AnimationState.Paused;
            AnimationEnded?.Invoke();
            UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Animation, Progress, Input.Devices.InputPauseType.TickOnlyWhenUnpaused);
            PostStopped();
        }
        public void Progress(float delta) => CurrentTime += delta * _speed;
        protected virtual void OnCurrentFrameChanged() => CurrentFrameChanged?.Invoke();

        /// <summary>
        /// Bakes the interpolated data for fastest access by the game.
        /// However, this method takes up more space and does not support time dilation (speeding up and slowing down with proper in-betweens)
        /// </summary>
        public abstract void Bake(float framesPerSecond);
    }
}

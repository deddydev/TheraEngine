using TheraEngine.Files;
using System;
using System.ComponentModel;

namespace TheraEngine.Animation
{
    public abstract class BaseAnimation : FileObject
    {
        public event Action AnimationStarted;
        public event Action AnimationEnded;
        public event Action CurrentFrameChanged;
        public event Action SpeedChanged;
        public event Action LoopChanged;
        public event Action LengthChanged;

        [Serialize("BakedFrameCount", Condition = "_isBaked")]
        protected int _bakedFrameCount = 0;
        [Serialize("BakedFPS", Condition = "_isBaked")]
        protected float _bakedFPS = 0.0f;

        [Serialize("LengthInSeconds")]
        protected float _lengthInSeconds = 0.0f;
        protected float _speed = 1.0f;
        protected float _currentTime = 0.0f;
        protected bool _looped = false;
        protected bool _isPlaying = false;
        protected bool _isBaked = false;
        protected bool _useKeyframes = true;

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
        [Category("Animation"), Serialize]
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
        [Category("Animation"), Serialize]
        public int BakedFrameCount => _bakedFrameCount;
        
        [Category("Animation"), Serialize]
        public bool Looped
        {
            get => _looped;
            set
            {
                _looped = value;
                LoopChanged?.Invoke();
            }
        }
        [Category("Animation"), Serialize]
        public float CurrentTime
        {
            get => _currentTime;
            set
            {
                _currentTime = value;
                if (_isPlaying && (_currentTime > _lengthInSeconds || _currentTime < 0.0f))
                {
                    if (_looped)
                        _currentTime = _currentTime.RemapToRange(0.0f, _lengthInSeconds);
                    else
                        Stop();
                }
                OnCurrentFrameChanged();
            }
        }
        [Category("Animation"), Serialize]
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (value)
                    Start();
                else
                    Stop();
            }
        }
        [PostDeserialize]
        private void PostDeserialize()
        {
            if (_isPlaying)
            {
                AnimationStarted?.Invoke();
                RegisterTick(ETickGroup.PostPhysics, ETickOrder.BoneAnimation, Progress);
            }
        }
        public void Start()
        {
            if (_isPlaying)
                return;
            _isPlaying = true;
            AnimationStarted?.Invoke();
            CurrentTime = 0.0f;
            RegisterTick(ETickGroup.PostPhysics, ETickOrder.BoneAnimation, Progress);
        }
        public void Stop()
        {
            if (!_isPlaying)
                return;
            _isPlaying = false;
            AnimationEnded?.Invoke();
            UnregisterTick(ETickGroup.PostPhysics, ETickOrder.BoneAnimation, Progress);
        }
        public void Progress(float delta) => CurrentTime += delta * _speed;
        protected virtual void OnCurrentFrameChanged() => CurrentFrameChanged?.Invoke();
    }
}

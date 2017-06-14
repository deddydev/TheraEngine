using TheraEngine.Files;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.Animation
{
    public abstract class BaseAnimation : FileObject
    {
        public event Action AnimationStarted;
        public event Action AnimationEnded;
        public event Action CurrentFrameChanged;
        public event Action FramesPerSecondChanged;
        public event Action SpeedChanged;
        public event Action LoopChanged;
        public event Action FrameCountChanged;
        
        protected int _frameCount;
        protected float _fps = 60.0f;
        protected float _speed = 1.0f;
        protected float _currentFrame = 0.0f;
        protected bool _looped = false;
        protected bool _isPlaying = false;
        protected bool _useKeyframes = true;

        /// <summary>
        /// Sets this animation to use a new number of frames.
        /// </summary>
        /// <param name="frameCount">The new number of frames (independent of speed or frames per second)</param>
        /// <param name="stretchAnimation">If true, will compress or expand all keyframes to match the new length.</param>
        public virtual void SetFrameCount(int frameCount, bool stretchAnimation)
        {
            _frameCount = frameCount;
            FrameCountChanged?.Invoke();
        }

        [Category("Animation")]
        public float LengthInSeconds
        {
            get => FrameCount / FramesPerSecond / Speed;
            set => SetFrameCount((int)(value * Speed * FramesPerSecond), true);
        }
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
        [Category("Animation"), Serialize]
        public float FramesPerSecond
        {
            get => _fps;
            set
            {
                _fps = value;
                FramesPerSecondChanged?.Invoke();
            }
        }
        /// <summary>
        /// How many frames this animation contains.
        /// </summary>
        [Category("Animation"), Serialize]
        public int FrameCount
        {
            get => _frameCount;
        }
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
        public float CurrentFrame
        {
            get => _currentFrame;
            set
            {
                _currentFrame = value;
                if (_currentFrame > _frameCount - 1 || _currentFrame < 0.0f)
                {
                    if (_isPlaying)
                    {
                        if (_looped)
                            _currentFrame = _currentFrame.RemapToRange(0.0f, _frameCount - 1);
                        else
                            Stop();
                    }
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
                RegisterTick(ETickGroup.PrePhysics, ETickOrder.Animation, Progress);
            }
        }
        public void Start()
        {
            if (_isPlaying)
                return;
            _isPlaying = true;
            AnimationStarted?.Invoke();
            CurrentFrame = 0.0f;
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Animation, Progress);
        }
        public void Stop()
        {
            if (!_isPlaying)
                return;
            _isPlaying = false;
            AnimationEnded?.Invoke();
            UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Animation, Progress);
        }
        public void Progress(float delta) => CurrentFrame += delta * _fps * _speed;
        protected virtual void OnCurrentFrameChanged() => CurrentFrameChanged?.Invoke();
    }
}

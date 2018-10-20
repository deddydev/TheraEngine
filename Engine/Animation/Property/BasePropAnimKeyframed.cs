using System;
using System.ComponentModel;

namespace TheraEngine.Animation
{
    [FileExt("kpanm")]
    [FileDef("Keyframed Property Animation")]
    public abstract class BasePropAnimKeyframed : BasePropAnimBakeable
    {
        public BasePropAnimKeyframed(float lengthInSeconds, bool looped, bool isBaked = false)
            : base(lengthInSeconds, looped, isBaked) { }
        public BasePropAnimKeyframed(int frameCount, float framesPerSecond, bool looped, bool isBaked = false)
            : base(frameCount, framesPerSecond, looped, isBaked) { }
        
        [Category(PropAnimCategory)]
        protected abstract BaseKeyframeTrack InternalKeyframes { get; }

        public override void SetLength(float lengthInSeconds, bool stretchAnimation)
        {
            if (lengthInSeconds < 0.0f)
                return;
            InternalKeyframes.SetLength(lengthInSeconds, stretchAnimation);
            base.SetLength(lengthInSeconds, stretchAnimation);
        }
    }
    public abstract class BasePropAnimBakeable : BasePropAnim
    {
        public event Action<BasePropAnimBakeable> BakedFPSChanged;
        public event Action<BasePropAnimBakeable> BakedFrameCountChanged;
        public event Action<BasePropAnimBakeable> IsBakedChanged;

        protected void OnBakedFPSChanged() => BakedFPSChanged?.Invoke(this);
        protected void OnBakedFrameCountChanged() => BakedFrameCountChanged?.Invoke(this);
        protected void OnBakedChanged() => IsBakedChanged?.Invoke(this);
        
        public BasePropAnimBakeable(float lengthInSeconds, bool looped, bool isBaked = false)
            : base(lengthInSeconds, looped)
        {
            _bakedFPS = 60.0f;
            SetBakedFramecount();
            IsBaked = isBaked;
        }
        public BasePropAnimBakeable(int frameCount, float framesPerSecond, bool looped, bool isBaked = false)
            : base(framesPerSecond <= 0.0f ? 0.0f : frameCount / framesPerSecond, looped)
        {
            _bakedFrameCount = frameCount;
            _bakedFPS = framesPerSecond.ClampMin(0.0f);
            IsBaked = isBaked;
        }

        [TSerialize("BakedFrameCount", Order = 1)]
        protected int _bakedFrameCount = 0;
        [TSerialize("BakedFPS", Order = 0)]
        protected float _bakedFPS = 0.0f;
        [TSerialize("IsBaked")]
        protected bool _isBaked = false;
        
        /// <summary>
        /// Determines which method to use, baked or keyframed.
        /// Keyframed takes up less memory and calculates in-between frames on the fly, which allows for time dilation.
        /// Baked takes up more memory but requires no calculations. However, the animation cannot be sped up at all, nor slowed down without artifacts.
        /// </summary>
        [Category("Animation"), TSerialize(NodeType = ENodeType.SetParentAttribute, Order = 2)]
        public bool IsBaked
        {
            get => _isBaked;
            set
            {
                _isBaked = value;
                BakedChanged();
                OnBakedChanged();
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
                _bakedFPS = value.ClampMin(0.0f);
                SetBakedFramecount();
                OnBakedFPSChanged();
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
                SetLength(_bakedFPS <= 0.0f ? 0.0f : _bakedFrameCount / _bakedFPS, false);
                OnBakedFrameCountChanged();
            }
        }

        protected void SetBakedFramecount()
            => _bakedFrameCount = (int)Math.Ceiling(_lengthInSeconds * _bakedFPS);

        public override void SetLength(float lengthInSeconds, bool stretchAnimation)
        {
            if (lengthInSeconds < 0.0f)
                return;
            _lengthInSeconds = lengthInSeconds;
            SetBakedFramecount();
            base.SetLength(lengthInSeconds, stretchAnimation);
        }

        /// <summary>
        /// Bakes the interpolated data for fastest access by the game.
        /// However, this method takes up more space and does not support time dilation (speeding up and slowing down with proper in-betweens)
        /// </summary>
        public abstract void Bake(float framesPerSecond);
        protected abstract void BakedChanged();
    }
}
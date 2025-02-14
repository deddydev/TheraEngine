﻿using Extensions;
using System;
using System.ComponentModel;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Attributes;

namespace TheraEngine.Animation
{
    [TFileExt("kpanm")]
    [TFileDef("Keyframed Property Animation")]
    public abstract class BasePropAnimKeyframed : BasePropAnimBakeable
    {
        public BasePropAnimKeyframed(float lengthInSeconds, bool looped, bool useKeyframes = false)
            : base(lengthInSeconds, looped, useKeyframes) { }
        public BasePropAnimKeyframed(int frameCount, float framesPerSecond, bool looped, bool useKeyframes = false)
            : base(frameCount, framesPerSecond, looped, useKeyframes) { }
        
        protected abstract BaseKeyframeTrack InternalKeyframes { get; }

        public override void SetLength(float lengthInSeconds, bool stretchAnimation, bool notifyChanged = true)
        {
            if (lengthInSeconds < 0.0f)
                return;
            InternalKeyframes.SetLength(lengthInSeconds, stretchAnimation, notifyChanged, notifyChanged);
            base.SetLength(lengthInSeconds, stretchAnimation, notifyChanged);
        }
    }
    [TFileExt("bpanm")]
    [TFileDef("Bakeable Property Animation")]
    public abstract class BasePropAnimBakeable : BasePropAnim
    {
        public const string BakeablePropAnimCategory = "Bakeable Property Animation";

        public event Action<BasePropAnimBakeable> BakedFPSChanged;
        public event Action<BasePropAnimBakeable> BakedFrameCountChanged;
        public event Action<BasePropAnimBakeable> IsBakedChanged;

        protected void OnBakedFPSChanged() => BakedFPSChanged?.Invoke(this);
        protected void OnBakedFrameCountChanged() => BakedFrameCountChanged?.Invoke(this);
        protected void OnBakedChanged() => IsBakedChanged?.Invoke(this);
        
        public BasePropAnimBakeable(float lengthInSeconds, bool looped, bool useKeyframes = true)
            : base(lengthInSeconds, looped)
        {
            _bakedFPS = 60.0f;
            SetBakedFrameCount();
            IsBaked = !useKeyframes;
        }
        public BasePropAnimBakeable(int frameCount, float framesPerSecond, bool looped, bool useKeyframes = true)
            : base(framesPerSecond <= 0.0f ? 0.0f : frameCount / framesPerSecond, looped)
        {
            _bakedFrameCount = frameCount;
            _bakedFPS = framesPerSecond.ClampMin(0.0f);
            IsBaked = !useKeyframes;
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
        [Category(BakeablePropAnimCategory)]
        [TSerialize(NodeType = ENodeType.Attribute, Order = 2)]
        public bool IsBaked
        {
            get => _isBaked;
            set
            {
                if (value)
                    Bake(BakedFramesPerSecond);
                else
                    Bake(0.0f);

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
        [TNumericPrefixSuffix(null, " hz")]
        [Category(BakeablePropAnimCategory)]
        public float BakedFramesPerSecond
        {
            get => _bakedFPS;
            set
            {
                _bakedFPS = value.ClampMin(0.0f);
                SetBakedFrameCount();
                if (IsBaked)
                    Bake(BakedFramesPerSecond);
                OnBakedFPSChanged();
            }
        }
        /// <summary>
        /// How many frames this animation contains.
        /// </summary>
        [TNumericPrefixSuffix(null, " frames")]
        [Category(BakeablePropAnimCategory)]
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

        /// <summary>
        /// Sets _bakedFrameCount using _lengthInSeconds and _bakedFPS.
        /// </summary>
        protected void SetBakedFrameCount()
            => _bakedFrameCount = (int)Math.Ceiling(_lengthInSeconds * _bakedFPS);

        public override void SetLength(float lengthInSeconds, bool stretchAnimation, bool notifyChanged = true)
        {
            if (lengthInSeconds < 0.0f)
                return;
            _lengthInSeconds = lengthInSeconds;
            SetBakedFrameCount();
            base.SetLength(lengthInSeconds, stretchAnimation, notifyChanged);
        }

        /// <summary>
        /// Bakes the interpolated data for fastest access by the game.
        /// However, this method takes up more space and does not support time dilation (speeding up and slowing down with proper in-betweens)
        /// </summary>
        public abstract void Bake(float framesPerSecond);
        protected abstract void BakedChanged();
    }
}
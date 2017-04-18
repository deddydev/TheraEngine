using CustomEngine.Files;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Animation
{
    public class ModelAnimation : BaseAnimation
    {
        public Dictionary<string, BoneAnimation> _boneAnimations;

        protected internal override void Tick(float delta)
        {
            Progress(delta);
        }
    }
    public class BoneAnimation
    {
        /// <summary>
         /// Determines which method to use, baked or keyframed.
         /// Keyframed takes up less memory and calculates in-between frames on the fly, which allows for time dilation.
         /// Baked takes up more memory but requires no calculations. However, the animation cannot be sped up at all, nor slowed down without artifacts.
         /// </summary>
        [Category("Bone Animation"), Serialize]
        public bool UseKeyframes
        {
            get => _useKeyframes;
            set { _useKeyframes = value; UseKeyframesChanged(); }
        }

        protected virtual void UseKeyframesChanged() { }

        public string _name;
        private bool _useKeyframes = true;
        public KeyframeTrack<Vec3Keyframe> _translation;
        public KeyframeTrack<QuatKeyframe> _rotation;
        public KeyframeTrack<Vec3Keyframe> _scale;

        public void SetValue(Matrix4 transform, float frameIndex, PlanarInterpType type)
        {
            FrameState state = FrameState.DeriveTRS(transform);
            _translation.Add(new Vec3Keyframe(frameIndex, state.Translation, type));
            _rotation.Add(new QuatKeyframe(frameIndex, state.Quaternion, type));
            _scale.Add(new Vec3Keyframe(frameIndex, state.Scale, type));
        }
    }
}

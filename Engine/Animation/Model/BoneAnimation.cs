﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Components.Logic.Animation;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Animation
{
    public class BoneAnimation
    {
        public BoneAnimation() { }
        public BoneAnimation(SkeletalAnimation parent, string name)
        {
            _name = name;
            Parent = parent;
        }

        /// <summary>
        /// Determines which method to use, baked or keyframed.
        /// Keyframed takes up less memory and calculates in-between frames on the fly, which allows for time dilation.
        /// Baked takes up more memory but requires no calculations. However, the animation cannot be sped up at all, nor slowed down without artifacts.
        /// </summary>
        [Category("Bone Animation"), TSerialize]
        public bool UseKeyframes
        {
            get => _useKeyframes;
            set
            {
                _useKeyframes = value;
                UseKeyframesChanged();
            }
        }

        protected virtual void UseKeyframesChanged() { }

        public void SetLength(float seconds, bool stretchAnimation, bool notifyChanged = true)
            => _tracks.SetLength(seconds, stretchAnimation, notifyChanged);

        internal SkeletalAnimation Parent { get; set; }

        [Category("Bone Animation"), TSerialize("Name")]
        public string _name;
        private bool _useKeyframes = true;

        [TSerialize(nameof(TransformKeyCollection))]
        private TransformKeyCollection _tracks = new TransformKeyCollection();

        [Category("Bone Animation")]
        public PropAnimFloat TranslationX => _tracks.TranslationX;
        [Category("Bone Animation")]
        public PropAnimFloat TranslationY => _tracks.TranslationY;
        [Category("Bone Animation")]
        public PropAnimFloat TranslationZ => _tracks.TranslationZ;

        [Category("Bone Animation")]
        public PropAnimFloat RotationX => _tracks.RotationX;
        [Category("Bone Animation")]
        public PropAnimFloat RotationY => _tracks.RotationY;
        [Category("Bone Animation")]
        public PropAnimFloat RotationZ => _tracks.RotationZ;

        [Category("Bone Animation")]
        public PropAnimFloat ScaleX => _tracks.ScaleX;
        [Category("Bone Animation")]
        public PropAnimFloat ScaleY => _tracks.ScaleY;
        [Category("Bone Animation")]
        public PropAnimFloat ScaleZ => _tracks.ScaleZ;

        public void Progress(float delta)
            => _tracks.Progress(delta);
        
        public BoneFrame GetFrame()
            => new BoneFrame(_name, _tracks.GetValues(), _tracks.EulerOrder);
        public BoneFrame GetFrame(float second)
            => new BoneFrame(_name, _tracks.GetValues(second), _tracks.EulerOrder);
        
        //public void SetValue(Matrix4 transform, float frameIndex, PlanarInterpType planar, RadialInterpType radial)
        //{
        //    FrameState state = FrameState.DeriveTRS(transform);
        //    _translation.Add(new Vec3Keyframe(frameIndex, state.Translation, planar));
        //    _rotation.Add(new QuatKeyframe(frameIndex, state.Quaternion, radial));
        //    _scale.Add(new Vec3Keyframe(frameIndex, state.Scale, planar));
        //}
        private HashSet<string> _boneNotFoundCache = new HashSet<string>();
        public void UpdateSkeleton(Skeleton skeleton)
        {
            Bone bone = skeleton[_name];
            if (bone != null)
                UpdateState(bone.FrameState, bone.BindState);
            else if (!_boneNotFoundCache.Contains(_name))
            {
                _boneNotFoundCache.Add(_name);
                Engine.PrintLine($"Bone '{_name}' not found in skeleton '{skeleton.ToString()}'.");
            }
        }
        public void UpdateState(Transform frameState, Transform bindState)
        {
            GetTransform(bindState, out Vec3 translation, out Rotator rotation, out Vec3 scale);
            frameState.SetAll(translation, rotation, scale);
        }
        public void UpdateState(Transform frameState, Transform bindState, float second)
        {
            GetTransform(bindState, second, out Vec3 translation, out Rotator rotation, out Vec3 scale);
            frameState.SetAll(translation, rotation, scale);
        }

        /// <summary>
        /// Retrieves the parts of the transform for this bone at the current frame second.
        /// </summary>
        public unsafe void GetTransform(Transform bindState, out Vec3 translation, out Rotator rotation, out Vec3 scale)
            => _tracks.GetTransform(bindState, out translation, out rotation, out scale);
        /// <summary>
        /// Retrieves the parts of the transform for this bone at the requested frame second.
        /// </summary>
        public unsafe void GetTransform(Transform bindState, float second, out Vec3 translation, out Rotator rotation, out Vec3 scale)
            => _tracks.GetTransform(bindState, second, out translation, out rotation, out scale);
        public void UpdateStateBlended(Transform frameState, Transform bindState, BoneAnimation otherBoneAnim, float otherWeight, EAnimBlendType blendType)
            => UpdateStateBlended(frameState, bindState, otherBoneAnim, Parent.CurrentTime, otherBoneAnim.Parent.CurrentTime, otherWeight, blendType);
        public void UpdateStateBlended(
            Transform frameState,
            Transform bindState,
            BoneAnimation otherBoneAnim,
            float thisSecond,
            float otherSecond,
            float otherWeight,
            EAnimBlendType blendType)
        {
            GetTransform(bindState, thisSecond, out Vec3 t1, out Rotator r1, out Vec3 s1);
            otherBoneAnim.GetTransform(bindState, otherSecond, out Vec3 t2, out Rotator r2, out Vec3 s2);

            Vec3 t = Vec3.Lerp(t1, t2, otherWeight);
            Quat r = Quat.Slerp(r1.ToQuaternion(), r2.ToQuaternion(), otherWeight);
            Vec3 s = Vec3.Lerp(s1, s2, otherWeight);
            frameState.SetAll(t, r, s);
        }
        public void UpdateSkeletonBlended(
            Skeleton skeleton,
            BoneAnimation otherBoneAnim,
            float otherWeight,
            EAnimBlendType blendType)
        {
            Bone bone = skeleton[_name];
            if (bone != null)
                UpdateStateBlended(bone.FrameState, bone.BindState, otherBoneAnim, otherWeight, blendType);
        }
        public BoneFrame BlendedWith(float second, BoneFrame other, float otherWeight)
            => GetFrame(second).BlendedWith(other, otherWeight);

    }
}

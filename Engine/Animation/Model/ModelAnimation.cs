using TheraEngine.Rendering.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Files;

namespace TheraEngine.Animation
{
    [FileClass("MANIM", "Model Animation", 
        ImportableExtensions = new[] { "DAE" })]
    public class ModelAnimation : BaseAnimation
    {
        [ThirdPartyLoader("DAE")]
        public static FileObject LoadDAE(string path)
        {
            ModelImportOptions o = new ModelImportOptions()
            {
                ImportAnimations = true,
                ImportModels = false
            };
            return Collada.Import(path, o)?.ModelAnimations[0];
        }

        [Serialize("BoneAnimations")]
        public Dictionary<string, BoneAnimation> _boneAnimations = new Dictionary<string, BoneAnimation>();

        public override void SetLength(float seconds, bool stretchAnimation)
        {
            foreach (BoneAnimation b in _boneAnimations.Values)
                b.SetLength(seconds, stretchAnimation);
            base.SetLength(seconds, stretchAnimation);
        }

        protected internal void Tick(float delta)
            => Progress(delta);
        public BoneAnimation CreateBoneAnimation(string boneName)
        {
            BoneAnimation bone = new BoneAnimation(this, boneName);
            AddBoneAnimation(bone);
            return bone;
        }
        public void AddBoneAnimation(BoneAnimation anim)
        {
            anim.Parent = this;
            if (_boneAnimations.ContainsKey(anim._name))
                _boneAnimations[anim._name] = anim;
            else
                _boneAnimations.Add(anim._name, anim);
        }
        public void RemoveBoneAnimation(string boneName)
        {
            if (_boneAnimations.ContainsKey(boneName))
                _boneAnimations.Remove(boneName);
        }
        public void UpdateSkeleton(Skeleton skeleton)
        {
            foreach (BoneAnimation bone in _boneAnimations.Values)
                bone.UpdateSkeleton(skeleton);
        }
        public IEnumerable<string> GetAllNames(ModelAnimationFrame other)
        {
            return other.GetAllNames(this);
        }
        public IEnumerable<string> GetAllNames(ModelAnimation other)
        {
            string[] theseNames = new string[_boneAnimations.Keys.Count];
            _boneAnimations.Keys.CopyTo(theseNames, 0);
            string[] thoseNames = new string[other._boneAnimations.Keys.Count];
            other._boneAnimations.Keys.CopyTo(thoseNames, 0);
            return theseNames.Intersect(thoseNames);
        }
        public void UpdateSkeletonBlended(
            Skeleton skeleton,
            ModelAnimation other,
            float otherWeight,
            AnimBlendType blendType)
        {
            foreach (string name in GetAllNames(other))
            {
                if (_boneAnimations.ContainsKey(name))
                {
                    if (other._boneAnimations.ContainsKey(name))
                        _boneAnimations[name].UpdateSkeletonBlended(skeleton, other._boneAnimations[name], otherWeight, blendType);
                    else
                        _boneAnimations[name].UpdateSkeletonBlended(skeleton, null, otherWeight, blendType);
                }
                else
                {
                    if (other._boneAnimations.ContainsKey(name))
                    {
                        other._boneAnimations[name].UpdateSkeletonBlended(skeleton, null, 1.0f - otherWeight, blendType);
                    }
                }
            }
        }

        public ModelAnimationFrame GetFrame()
        {
            ModelAnimationFrame frame = new ModelAnimationFrame();
            foreach (BoneAnimation bone in _boneAnimations.Values)
                frame.AddBoneFrame(bone.GetFrame());
            return frame;
        }

        public void UpdateSkeletonBlendedMulti(Skeleton skeleton, ModelAnimation[] other, float[] otherWeight)
        {
            //string[] theseNames = new string[_boneAnimations.Keys.Count];
            //_boneAnimations.Keys.CopyTo(theseNames, 0);
            //string[] thoseNames = new string[other._boneAnimations.Keys.Count];
            //other._boneAnimations.Keys.CopyTo(thoseNames, 0);
            //IEnumerable<string> names = theseNames.Intersect(thoseNames);
            //foreach (string name in names)
            //{
            //    if (_boneAnimations.ContainsKey(name))
            //    {
            //        if (other._boneAnimations.ContainsKey(name))
            //        {

            //        }
            //    }
            //    else
            //    {
            //        if (other._boneAnimations.ContainsKey(name))
            //        {

            //        }
            //    }
            //}
        }
    }
    public class BoneAnimation
    {
        public BoneAnimation(ModelAnimation parent, string name)
        {
            _name = name;
            Parent = parent;
            _translation = new KeyframeTrack<Vec3Keyframe>();
            _rotation = new KeyframeTrack<QuatKeyframe>();
            _scale = new KeyframeTrack<Vec3Keyframe>();
        }

        public BoneFrame BlendedWith(float frameIndex, BoneFrame other, float otherWeight)
        {
            return GetFrame(frameIndex).BlendedWith(other, otherWeight);
        }

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

        public void SetLength(float seconds, bool stretchAnimation)
        {
            _translation.SetLength(seconds, stretchAnimation);
            _rotation.SetLength(seconds, stretchAnimation);
            _scale.SetLength(seconds, stretchAnimation);
        }

        internal ModelAnimation Parent { get; set; }
        public RotationOrder EulerOrder { get; set; }

        [Category("Bone Animation"), Serialize("Name")]
        public string _name;
        private bool _useKeyframes = true;

        //public KeyframeTrack<Matrix4Keyframe> _matrix;

        KeyframeTrack<FloatKeyframe>[] _tracks = new KeyframeTrack<FloatKeyframe>[]
        {
            new KeyframeTrack<FloatKeyframe>(), //tx
            new KeyframeTrack<FloatKeyframe>(), //ty
            new KeyframeTrack<FloatKeyframe>(), //tz
            new KeyframeTrack<FloatKeyframe>(), //ry
            new KeyframeTrack<FloatKeyframe>(), //rp
            new KeyframeTrack<FloatKeyframe>(), //rr
            new KeyframeTrack<FloatKeyframe>(), //sx
            new KeyframeTrack<FloatKeyframe>(), //sy
            new KeyframeTrack<FloatKeyframe>(), //sz
        };

        [Category("Bone Animation"), Serialize("TranslationXKeys")]
        public KeyframeTrack<FloatKeyframe> _translationX;
        [Category("Bone Animation"), Serialize("TranslationYKeys")]
        public KeyframeTrack<FloatKeyframe> _translationY;
        [Category("Bone Animation"), Serialize("TranslationZKeys")]
        public KeyframeTrack<FloatKeyframe> _translationZ;

        //[Category("Bone Animation"), Serialize("RotationKeys")]
        //public KeyframeTrack<QuatKeyframe> _rotation;

        [Category("Bone Animation"), Serialize("RotationYawKeys")]
        public KeyframeTrack<QuatKeyframe> _rotationYaw;
        [Category("Bone Animation"), Serialize("RotationPitchKeys")]
        public KeyframeTrack<QuatKeyframe> _rotationPitch;
        [Category("Bone Animation"), Serialize("RotationRollKeys")]
        public KeyframeTrack<QuatKeyframe> _rotationRoll;
        
        [Category("Bone Animation"), Serialize("ScaleXKeys")]
        public KeyframeTrack<FloatKeyframe> _scaleX;
        [Category("Bone Animation"), Serialize("ScaleYKeys")]
        public KeyframeTrack<FloatKeyframe> _scaleY;
        [Category("Bone Animation"), Serialize("ScaleZKeys")]
        public KeyframeTrack<FloatKeyframe> _scaleZ;

        public BoneFrame GetFrame()
            => GetFrame(Parent.CurrentTime);
        public BoneFrame GetFrame(float frameIndex)
        {
            float? tx = null, ty = null, tz = null;
            if (_translationX.First != null)
                tx = _translationX.First.Interpolate(frameIndex);
            if (_translationX.First != null)
                tx = _translationX.First.Interpolate(frameIndex);
            if (_translationX.First != null)
                tx = _translationX.First.Interpolate(frameIndex);

            Quat? r = null;
            if (_rotation.First != null)
                r = _rotation.First.Interpolate(frameIndex);

            Vec3? s = null;
            if (_scale.First != null)
                s = _scale.First.Interpolate(frameIndex);

            return new BoneFrame(
                _name,
                t.GetValueOrDefault(Vec3.Zero),     t.HasValue ? 1.0f : 0.0f,
                r.GetValueOrDefault(Quat.Identity), r.HasValue ? 1.0f : 0.0f,
                s.GetValueOrDefault(Vec3.One),      s.HasValue ? 1.0f : 0.0f);
        }
        //public void SetValue(Matrix4 transform, float frameIndex, PlanarInterpType planar, RadialInterpType radial)
        //{
        //    FrameState state = FrameState.DeriveTRS(transform);
        //    _translation.Add(new Vec3Keyframe(frameIndex, state.Translation, planar));
        //    _rotation.Add(new QuatKeyframe(frameIndex, state.Quaternion, radial));
        //    _scale.Add(new Vec3Keyframe(frameIndex, state.Scale, planar));
        //}
        public void UpdateSkeleton(Skeleton skeleton)
        {
            Bone bone = skeleton[_name];
            if (bone != null)
                UpdateState(bone.FrameState, bone.BindState);
        }
        public void UpdateState(FrameState frameState, FrameState bindState)
            => UpdateState(frameState, bindState, Parent.CurrentTime);
        public void UpdateState(FrameState frameState, FrameState bindState, float frameIndex)
        {
            Vec3 t = _translation.First == null ?
                bindState.Translation.Raw :
                _translation.First.Interpolate(frameIndex);
            Quat r = _rotation.First == null ?
                bindState.Quaternion :
                _rotation.First.Interpolate(frameIndex);
            Vec3 s = _scale.First == null ?
                bindState.Scale.Raw :
                _scale.First.Interpolate(frameIndex);
            frameState.SetAll(t, r, s);
        }
        public void UpdateStateBlended(FrameState frameState, FrameState bindState, BoneAnimation otherBoneAnim, float otherWeight, AnimBlendType blendType)
            => UpdateStateBlended(frameState, bindState, otherBoneAnim, Parent.CurrentTime, otherBoneAnim.Parent.CurrentTime, otherWeight, blendType);
        public void UpdateStateBlended(
            FrameState frameState,
            FrameState bindState, 
            BoneAnimation otherBoneAnim,
            float thisFrameIndex,
            float otherFrameIndex,
            float otherWeight,
            AnimBlendType blendType)
        {
            Vec3 t1 = _translation.First == null ?
                bindState.Translation.Raw :
                _translation.First.Interpolate(thisFrameIndex);
            Vec3 t2 = otherBoneAnim._translation.First == null ?
                bindState.Translation.Raw :
                otherBoneAnim._translation.First.Interpolate(otherFrameIndex);
            Vec3 t = Vec3.Lerp(t1, t2, otherWeight);
            
            Quat r1 = _rotation.First == null ?
                bindState.Quaternion :
                _rotation.First.Interpolate(thisFrameIndex);
            Quat r2 = otherBoneAnim._rotation.First == null ?
                bindState.Quaternion :
                 otherBoneAnim._rotation.First.Interpolate(otherFrameIndex);
            Quat r = Quat.Slerp(r1, r2, otherWeight);

            Vec3 s1 = _scale.First == null ?
                bindState.Scale.Raw :
                _scale.First.Interpolate(thisFrameIndex);
            Vec3 s2 = otherBoneAnim._scale.First == null ?
                bindState.Scale.Raw :
                otherBoneAnim._scale.First.Interpolate(otherFrameIndex);
            Vec3 s = Vec3.Lerp(s1, s2, otherWeight);

            frameState.SetAll(t, r, s);
        }
        public void UpdateSkeletonBlended(
            Skeleton skeleton,
            BoneAnimation otherBoneAnim,
            float otherWeight,
            AnimBlendType blendType)
        {
            Bone bone = skeleton[_name];
            if (bone != null)
                UpdateStateBlended(bone.FrameState, bone.BindState, otherBoneAnim, otherWeight, blendType);
        }
    }
}

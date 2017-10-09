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
            foreach (var track in _tracks)
                track.SetLength(seconds, stretchAnimation);
        }

        internal ModelAnimation Parent { get; set; }
        public RotationOrder EulerOrder { get; set; }

        [Category("Bone Animation"), Serialize("Name")]
        public string _name;
        private bool _useKeyframes = true;
        
        [Serialize("TransformKeys")]
        private KeyframeTrack<FloatKeyframe>[] _tracks = new KeyframeTrack<FloatKeyframe>[]
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

        [Category("Bone Animation")]
        public KeyframeTrack<FloatKeyframe> TranslationX => _tracks[0];
        [Category("Bone Animation")]
        public KeyframeTrack<FloatKeyframe> TranslationY => _tracks[1];
        [Category("Bone Animation")]
        public KeyframeTrack<FloatKeyframe> TranslationZ => _tracks[2];
        
        [Category("Bone Animation")]
        public KeyframeTrack<FloatKeyframe> RotationYaw => _tracks[3];
        [Category("Bone Animation")]
        public KeyframeTrack<FloatKeyframe> RotationPitch => _tracks[4];
        [Category("Bone Animation")]
        public KeyframeTrack<FloatKeyframe> RotationRoll => _tracks[5];

        [Category("Bone Animation")]
        public KeyframeTrack<FloatKeyframe> ScaleX => _tracks[6];
        [Category("Bone Animation")]
        public KeyframeTrack<FloatKeyframe> ScaleY => _tracks[7];
        [Category("Bone Animation")]
        public KeyframeTrack<FloatKeyframe> ScaleZ => _tracks[8];

        public BoneFrame GetFrame()
            => GetFrame(Parent.CurrentTime);
        public BoneFrame GetFrame(float second)
        {
            float?[] values = new float?[9];
            for (int i = 0; i < 9; ++i)
                values[i] = _tracks[i].First == null ? null : (float?)_tracks[i].First.Interpolate(second);

            return new BoneFrame(_name, values);
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
        public void UpdateState(FrameState frameState, FrameState bindState, float second)
        {
            Vec3 t = _translation.First == null ?
                bindState.Translation.Raw :
                _translation.First.Interpolate(second);
            Quat r = _rotation.First == null ?
                bindState.Quaternion :
                _rotation.First.Interpolate(second);
            Vec3 s = _scale.First == null ?
                bindState.Scale.Raw :
                _scale.First.Interpolate(second);
            frameState.SetAll(t, r, s);
        }
        public void UpdateStateBlended(FrameState frameState, FrameState bindState, BoneAnimation otherBoneAnim, float otherWeight, AnimBlendType blendType)
            => UpdateStateBlended(frameState, bindState, otherBoneAnim, Parent.CurrentTime, otherBoneAnim.Parent.CurrentTime, otherWeight, blendType);
        public void UpdateStateBlended(
            FrameState frameState,
            FrameState bindState, 
            BoneAnimation otherBoneAnim,
            float thisSecond,
            float otherSecond,
            float otherWeight,
            AnimBlendType blendType)
        {
            Vec3 t1 = _translation.First == null ?
                bindState.Translation.Raw :
                _translation.First.Interpolate(thisSecond);
            Vec3 t2 = otherBoneAnim._translation.First == null ?
                bindState.Translation.Raw :
                otherBoneAnim._translation.First.Interpolate(otherSecond);
            Vec3 t = Vec3.Lerp(t1, t2, otherWeight);
            
            Quat r1 = _rotation.First == null ?
                bindState.Quaternion :
                _rotation.First.Interpolate(thisSecond);
            Quat r2 = otherBoneAnim._rotation.First == null ?
                bindState.Quaternion :
                 otherBoneAnim._rotation.First.Interpolate(otherSecond);
            Quat r = Quat.Slerp(r1, r2, otherWeight);

            Vec3 s1 = _scale.First == null ?
                bindState.Scale.Raw :
                _scale.First.Interpolate(thisSecond);
            Vec3 s2 = otherBoneAnim._scale.First == null ?
                bindState.Scale.Raw :
                otherBoneAnim._scale.First.Interpolate(otherSecond);
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

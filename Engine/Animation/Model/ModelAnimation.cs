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
                IgnoreFlags =
                Core.Files.IgnoreFlags.Extra | 
                Core.Files.IgnoreFlags.Geometry |
                Core.Files.IgnoreFlags.Controllers |
                Core.Files.IgnoreFlags.Cameras | 
                Core.Files.IgnoreFlags.Lights
            };
            return Collada.Import(path, o)?.Models[0].Animation;
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

        public BoneAnimation FindOrCreateBoneAnimation(string boneName, out bool wasFound)
        {
            if (wasFound = _boneAnimations.ContainsKey(boneName))
                return _boneAnimations[boneName];
            
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
        public RotationOrder EulerOrder = RotationOrder.RYP;

        [Category("Bone Animation"), Serialize("Name")]
        public string _name;
        private bool _useKeyframes = true;
        
        [Serialize("TransformKeys")]
        private KeyframeTrack<FloatKeyframe>[] _tracks = new KeyframeTrack<FloatKeyframe>[]
        {
            new KeyframeTrack<FloatKeyframe>(), //tx
            new KeyframeTrack<FloatKeyframe>(), //ty
            new KeyframeTrack<FloatKeyframe>(), //tz
            new KeyframeTrack<FloatKeyframe>(), //rx
            new KeyframeTrack<FloatKeyframe>(), //ry
            new KeyframeTrack<FloatKeyframe>(), //rz
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
        public KeyframeTrack<FloatKeyframe> RotationX => _tracks[3];
        [Category("Bone Animation")]
        public KeyframeTrack<FloatKeyframe> RotationY => _tracks[4];
        [Category("Bone Animation")]
        public KeyframeTrack<FloatKeyframe> RotationZ => _tracks[5];

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

            return new BoneFrame(_name, values, EulerOrder);
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
        public void UpdateState(Transform frameState, Transform bindState)
            => UpdateState(frameState, bindState, Parent.CurrentTime);
        public unsafe void UpdateState(Transform frameState, Transform bindState, float second)
        {
            GetTransform(bindState, second, out Vec3 translation, out Rotator rotation, out Vec3 scale);
            frameState.SetAll(translation, rotation, scale);
        }

        /// <summary>
        /// Retrieves the parts of the transform for this bone at the requested frame second.
        /// </summary>
        public unsafe void GetTransform(Transform bindState, float second, out Vec3 translation, out Rotator rotation, out Vec3 scale)
        {
            Vec3 t, r, s;
            Vec3
                bt = bindState.Translation.Raw,
                br = bindState.Rotation.RawPitchYawRoll,
                bs = bindState.Scale.Raw;
            float* pt = (float*)&t;
            float* pr = (float*)&r;
            float* ps = (float*)&s;
            float* pbt = (float*)&bt;
            float* pbr = (float*)&br;
            float* pbs = (float*)&bs;
            for (int i = 0; i < 3; ++i)
            {
                var track = _tracks[i];
                *pt++ = track.First == null ? pbt[i] : track.First.Interpolate(second);
            }
            for (int i = 3; i < 6; ++i)
            {
                var track = _tracks[i];
                *pr++ = track.First == null ? pbr[i] : track.First.Interpolate(second);
            }
            for (int i = 6; i < 9; ++i)
            {
                var track = _tracks[i];
                *ps++ = track.First == null ? pbs[i] : track.First.Interpolate(second);
            }

            translation = t;
            rotation = new Rotator(r, EulerOrder);
            scale = s;
        }
        public void UpdateStateBlended(Transform frameState, Transform bindState, BoneAnimation otherBoneAnim, float otherWeight, AnimBlendType blendType)
            => UpdateStateBlended(frameState, bindState, otherBoneAnim, Parent.CurrentTime, otherBoneAnim.Parent.CurrentTime, otherWeight, blendType);
        public void UpdateStateBlended(
            Transform frameState,
            Transform bindState, 
            BoneAnimation otherBoneAnim,
            float thisSecond,
            float otherSecond,
            float otherWeight,
            AnimBlendType blendType)
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
            AnimBlendType blendType)
        {
            Bone bone = skeleton[_name];
            if (bone != null)
                UpdateStateBlended(bone.FrameState, bone.BindState, otherBoneAnim, otherWeight, blendType);
        }
        public BoneFrame BlendedWith(float second, BoneFrame other, float otherWeight)
            => GetFrame(second).BlendedWith(other, otherWeight);

    }
}

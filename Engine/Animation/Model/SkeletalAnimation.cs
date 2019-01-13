using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Components.Logic.Animation;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Animation
{
    [TFileDef("Skeletal Animation")]
    [TFileExt("skelanim", new string[] { "dae" }, null)]
    public class SkeletalAnimation : BaseAnimation
    {
        [ThirdPartyLoader("dae", true)]
        public static async Task<SkeletalAnimation> LoadDAEAsync(
            string path, IProgress<float> progress, CancellationToken cancel)
        {
            ColladaImportOptions o = new ColladaImportOptions()
            {
                IgnoreFlags =
                Collada.EIgnoreFlags.Extra |
                Collada.EIgnoreFlags.Geometry |
                Collada.EIgnoreFlags.Controllers |
                Collada.EIgnoreFlags.Cameras |
                Collada.EIgnoreFlags.Lights
            };
            return (await Collada.ImportAsync(path, o, progress, cancel))?.Models[0].Animation;
        }
        
        public SkeletalAnimation() : base(0.0f, false) { }
        public SkeletalAnimation(float lengthInSeconds, bool looped)
            : base(lengthInSeconds, looped) { }
        public SkeletalAnimation(int frameCount, float FPS, bool looped)
            : base(frameCount / FPS, looped) { }

        [TSerialize("BoneAnimations")]
        private Dictionary<string, BoneAnimation> _boneAnimations = new Dictionary<string, BoneAnimation>();

        [TPostDeserialize]
        internal override void PostDeserialize()
        {
            foreach (BoneAnimation b in _boneAnimations.Values)
                b.Parent = this;
            base.PostDeserialize();
        }

        public Dictionary<string, BoneAnimation> BoneAnimations { get => _boneAnimations; set => _boneAnimations = value; }

        public override void SetLength(float seconds, bool stretchAnimation, bool notifyChanged = true)
        {
            foreach (BoneAnimation b in _boneAnimations.Values)
                b.SetLength(seconds, stretchAnimation, notifyChanged);
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
        public IEnumerable<string> GetAllNames(SkeletalAnimationPose other)
        {
            return other.GetCommonNames(this);
        }
        public IEnumerable<string> GetAllNames(SkeletalAnimation other)
        {
            string[] theseNames = new string[_boneAnimations.Keys.Count];
            _boneAnimations.Keys.CopyTo(theseNames, 0);
            string[] thoseNames = new string[other._boneAnimations.Keys.Count];
            other._boneAnimations.Keys.CopyTo(thoseNames, 0);
            return theseNames.Intersect(thoseNames);
        }
        public void UpdateSkeletonBlended(
            Skeleton skeleton,
            SkeletalAnimation other,
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

        public SkeletalAnimationPose GetFrame()
        {
            SkeletalAnimationPose frame = new SkeletalAnimationPose();
            foreach (BoneAnimation bone in _boneAnimations.Values)
                frame.AddBoneFrame(bone.GetFrame());
            return frame;
        }

        public void UpdateSkeletonBlendedMulti(Skeleton skeleton, SkeletalAnimation[] other, float[] otherWeight)
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
        
        protected override void OnProgressed(float delta)
        {
            BoneAnimations.ForEach(x => x.Value.Progress(delta));
        }
    }
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
        {
            _tracks.Progress(delta);
        }

        public BoneFrame GetFrame()
        {
            return new BoneFrame(_name, _tracks.GetValues(), _tracks.EulerOrder);
        }
        public BoneFrame GetFrame(float second)
        {
            return new BoneFrame(_name, _tracks.GetValues(second), _tracks.EulerOrder);
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

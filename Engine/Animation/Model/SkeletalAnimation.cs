using TheraEngine.Rendering.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Components.Logic.Animation;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using System.Threading.Tasks;
using TheraEngine.Core.Files;

namespace TheraEngine.Animation
{
    [File3rdParty(new string[] { "dae" }, null)]
    [FileExt("skelanim")]
    [FileDef("Skeletal Animation")]
    public class SkeletalAnimation : BaseAnimation
    {
        [ThirdPartyLoader("dae", true)]
        public static async Task<TFileObject> LoadDAEAsync(string path)
        {
            ModelImportOptions o = new ModelImportOptions()
            {
                //IgnoreFlags =
                //Collada.EIgnoreFlags.Extra |
                //Collada.EIgnoreFlags.Geometry |
                //Collada.EIgnoreFlags.Controllers |
                //Collada.EIgnoreFlags.Cameras |
                //Collada.EIgnoreFlags.Lights
            };
            return (await Collada.ImportAsync(path, o))?.Models[0].Animation;
        }
        
        public SkeletalAnimation() : base(0.0f, false) { }
        public SkeletalAnimation(float lengthInSeconds, bool looped)
            : base(lengthInSeconds, looped) { }
        public SkeletalAnimation(int frameCount, float FPS, bool looped)
            : base(frameCount / FPS, looped) { }

        [TSerialize("BoneAnimations")]
        private Dictionary<string, BoneAnimation> _boneAnimations = new Dictionary<string, BoneAnimation>();

        //[PostDeserialize]
        internal override void PostDeserialize()
        {
            foreach (BoneAnimation b in _boneAnimations.Values)
                b.Parent = this;
            base.PostDeserialize();
        }

        public Dictionary<string, BoneAnimation> BoneAnimations { get => _boneAnimations; set => _boneAnimations = value; }

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
        public IEnumerable<string> GetAllNames(SkeletalAnimationFrame other)
        {
            return other.GetAllNames(this);
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

        public SkeletalAnimationFrame GetFrame()
        {
            SkeletalAnimationFrame frame = new SkeletalAnimationFrame();
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
            BoneAnimations.ForEach(x => x.Value.Progress(CurrentTime));
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

        public void SetLength(float seconds, bool stretchAnimation)
            => _tracks.SetLength(seconds, stretchAnimation);
        
        internal SkeletalAnimation Parent { get; set; }

        [Category("Bone Animation"), TSerialize("Name")]
        public string _name;
        private bool _useKeyframes = true;

        [TSerialize(nameof(TransformKeyCollection))]
        private TransformKeyCollection _tracks = new TransformKeyCollection();

        [Category("Bone Animation")]
        public KeyframeTrack<FloatKeyframe> TranslationX => _tracks.TranslationX;
        [Category("Bone Animation")]
        public KeyframeTrack<FloatKeyframe> TranslationY => _tracks.TranslationY;
        [Category("Bone Animation")]
        public KeyframeTrack<FloatKeyframe> TranslationZ => _tracks.TranslationZ;

        [Category("Bone Animation")]
        public KeyframeTrack<FloatKeyframe> RotationX => _tracks.RotationX;
        [Category("Bone Animation")]
        public KeyframeTrack<FloatKeyframe> RotationY => _tracks.RotationY;
        [Category("Bone Animation")]
        public KeyframeTrack<FloatKeyframe> RotationZ => _tracks.RotationZ;

        [Category("Bone Animation")]
        public KeyframeTrack<FloatKeyframe> ScaleX => _tracks.ScaleX;
        [Category("Bone Animation")]
        public KeyframeTrack<FloatKeyframe> ScaleY => _tracks.ScaleY;
        [Category("Bone Animation")]
        public KeyframeTrack<FloatKeyframe> ScaleZ => _tracks.ScaleZ;

        private FloatKeyframe[] _lastKeyframes = new FloatKeyframe[9];
        private float?[] _currentValues = new float?[9];

        public float? CurrentTranslationX => _currentValues[0];
        public float? CurrentTranslationY => _currentValues[1];
        public float? CurrentTranslationZ => _currentValues[2];
        public float? CurrentRotationX => _currentValues[3];
        public float? CurrentRotationY => _currentValues[4];
        public float? CurrentRotationZ => _currentValues[5];
        public float? CurrentScaleX => _currentValues[6];
        public float? CurrentScaleY => _currentValues[7];
        public float? CurrentScaleZ => _currentValues[8];

        public void Progress(float second)
        {
            KeyframeTrack<FloatKeyframe> track;
            for (int i = 0; i < 9; ++i)
            {
                track = _tracks[i];
                FloatKeyframe lastKf = _lastKeyframes[i] ?? track.First;
                _currentValues[i] = lastKf == null ? null : (float?)lastKf.Interpolate(second, EVectorInterpValueType.Position);
            }
        }

        public BoneFrame GetFrame() => new BoneFrame(_name, _currentValues, _tracks.EulerOrder);
        public BoneFrame GetFrame(float second)
        {
            float?[] values = new float?[9];
            KeyframeTrack<FloatKeyframe> track;
            for (int i = 0; i < 9; ++i)
            {
                track = _tracks[i];
                values[i] = track.First == null ? null : (float?)track.First.Interpolate(second, EVectorInterpValueType.Position);
            }
            return new BoneFrame(_name, values, _tracks.EulerOrder);
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

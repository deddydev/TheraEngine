using TheraEngine.Rendering.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Maths;

namespace TheraEngine.Animation
{
    public class SkeletalAnimationPose
    {
        public Dictionary<string, BoneFrame> _boneFrames = new Dictionary<string, BoneFrame>();

        public void AddBoneFrame(BoneFrame anim)
        {
            if (_boneFrames.ContainsKey(anim._name))
                _boneFrames[anim._name] = anim;
            else
                _boneFrames.Add(anim._name, anim);
        }
        public void RemoveBoneFrame(string boneName)
        {
            if (_boneFrames.ContainsKey(boneName))
                _boneFrames.Remove(boneName);
        }
        public void UpdateSkeleton(Skeleton skeleton)
        {
            foreach (BoneFrame b in _boneFrames.Values)
                b.UpdateSkeleton(skeleton);
        }
        /// <summary>
        /// Returns all bone names that exist in this and the other.
        /// </summary>
        public IEnumerable<string> BoneNamesUnion(SkeletalAnimationPose other)
        {
            string[] theseNames = new string[_boneFrames.Keys.Count];
            _boneFrames.Keys.CopyTo(theseNames, 0);
            string[] thoseNames = new string[other._boneFrames.Keys.Count];
            other._boneFrames.Keys.CopyTo(thoseNames, 0);
            return theseNames.Intersect(thoseNames);
        }
        /// <summary>
        /// Returns all bone names that exist in this and the other.
        /// </summary>
        public IEnumerable<string> BoneNamesUnion(SkeletalAnimation other)
        {
            string[] theseNames = new string[_boneFrames.Keys.Count];
            _boneFrames.Keys.CopyTo(theseNames, 0);
            string[] thoseNames = new string[other.BoneAnimations.Keys.Count];
            other.BoneAnimations.Keys.CopyTo(thoseNames, 0);
            return theseNames.Intersect(thoseNames);
        }
        public SkeletalAnimationPose BlendedWith(SkeletalAnimationPose other, float otherWeight)
        {
            SkeletalAnimationPose blendedFrame = new SkeletalAnimationPose();
            var union = BoneNamesUnion(other);
            foreach (string name in union)
            {
                if (_boneFrames.ContainsKey(name))
                {
                    if (other._boneFrames.ContainsKey(name))
                        blendedFrame.AddBoneFrame(_boneFrames[name].BlendedWith(other._boneFrames[name], otherWeight));
                    else
                        blendedFrame.AddBoneFrame(_boneFrames[name].BlendedWith(null, otherWeight));
                }
                else
                {
                    if (other._boneFrames.ContainsKey(name))
                        blendedFrame.AddBoneFrame(other._boneFrames[name].BlendedWith(null, 1.0f - otherWeight));
                }
            }
            return blendedFrame;
        }
        public SkeletalAnimationPose BlendedWith(SkeletalAnimation other, float frameIndex, float otherWeight)
        {
            SkeletalAnimationPose blendedFrame = new SkeletalAnimationPose();
            foreach (string name in BoneNamesUnion(other))
            {
                if (_boneFrames.ContainsKey(name))
                {
                    if (other.BoneAnimations.ContainsKey(name))
                        blendedFrame.AddBoneFrame(_boneFrames[name].BlendedWith(other.BoneAnimations[name], frameIndex, otherWeight));
                    else
                        blendedFrame.AddBoneFrame(_boneFrames[name].BlendedWith(null, otherWeight));
                }
                else
                {
                    if (other.BoneAnimations.ContainsKey(name))
                        blendedFrame.AddBoneFrame(other.BoneAnimations[name].BlendedWith(frameIndex, null, 1.0f - otherWeight));
                }
            }
            return blendedFrame;
        }

        public void BlendWith(SkeletalAnimationPose other, float otherWeight)
        {
            if (other is null)
                return;
            foreach (string name in BoneNamesUnion(other))
            {
                if (_boneFrames.ContainsKey(name))
                {
                    if (other._boneFrames.ContainsKey(name))
                        _boneFrames[name].BlendWith(other._boneFrames[name], otherWeight);
                    else
                        _boneFrames[name].BlendWith(null, otherWeight);
                }
                else
                {
                    if (other._boneFrames.ContainsKey(name))
                        AddBoneFrame(other._boneFrames[name].BlendedWith(null, 1.0f - otherWeight));

                    //else, neither has a bone with this name, ignore it
                }
            }
        }
        public void BlendWith(SkeletalAnimation other, float frameIndex, float otherWeight)
        {
            foreach (string name in BoneNamesUnion(other))
            {
                if (_boneFrames.ContainsKey(name))
                {
                    if (other.BoneAnimations.ContainsKey(name))
                        _boneFrames[name].BlendedWith(other.BoneAnimations[name], frameIndex, otherWeight);
                    else
                        _boneFrames[name].BlendedWith(null, otherWeight);
                }
                else
                {
                    if (other.BoneAnimations.ContainsKey(name))
                        other.BoneAnimations[name].BlendedWith(frameIndex, null, 1.0f - otherWeight);
                }
            }
        }
    }
    public class FrameVec3ValueWeight
    {
        public FrameVec3ValueWeight() { }
        public FrameVec3ValueWeight(Vec3 value, float weight)
        {
            Value = value;
            Weight = weight;
        }

        public Vec3 Value { get; set; }
        public float Weight { get; set; }
    }
    public class FrameQuatValueWeight
    {
        public FrameQuatValueWeight() { }
        public FrameQuatValueWeight(Quat value, float weight)
        {
            Value = value;
            Weight = weight;
        }

        public Quat Value { get; set; }
        public float Weight { get; set; }
    }
    public class BoneFrame
    {
        public string _name;
        private FrameVec3ValueWeight _translation;
        private FrameVec3ValueWeight _scale;
        private FrameQuatValueWeight _rotation;

        public FrameVec3ValueWeight Translation 
        {
            get => _translation;
            set => _translation = value;
        }
        public FrameVec3ValueWeight Scale
        {
            get => _scale;
            set => _scale = value;
        }
        public FrameQuatValueWeight Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        public Vec3 GetTranslation(Vec3 bindTranslation) => Vec3.Lerp(bindTranslation, _translation.Value, _translation.Weight);
        public Quat GetRotation(Quat bindRotation) => Quat.Slerp(bindRotation, _rotation.Value, _rotation.Weight);
        public Vec3 GetScale(Vec3 bindScale) => Vec3.Lerp(bindScale, _scale.Value, _scale.Weight);

        public Vec3 GetUnweightedTranslation() => _translation.Value;
        public Quat GetUnweightedRotation() => _rotation.Value;
        public Vec3 GetUnweightedScale() => _scale.Value;

        public BoneFrame(string name) => _name = name;
        public BoneFrame(string name, Vec3 translation, Quat rotation, Vec3 scale) : this(name)
        {
            Translation = new FrameVec3ValueWeight(translation, 1.0f);
            Rotation = new FrameQuatValueWeight(rotation, 1.0f);
            Scale = new FrameVec3ValueWeight(scale, 1.0f);
        }
        public BoneFrame(string name, (Vec3 translation, Quat rotation, Vec3 scale) parts)
            : this(name, parts.translation, parts.rotation, parts.scale) { }
        public BoneFrame(string name, TransformKeyCollection keys) 
            : this(name, keys.GetTransformParts()) { }
        public BoneFrame(string name, TransformKeyCollection keys, float second) 
            : this(name, keys.GetTransformParts(second)) { }

        //public BoneFrame(string name, Vec3 translation, Rotator rotation, Vec3 scale, params float[] weights)
        //{
        //    _name = name;

        //    _values = new FrameValueWeight[9];

        //    _values[0].Value = translation.X;
        //    _values[0].Weight = weights[0];
        //    _values[1].Value = translation.Y;
        //    _values[1].Weight = weights[1];
        //    _values[2].Value = translation.Z;
        //    _values[2].Weight = weights[2];

        //    _values[3].Value = rotation.Pitch;
        //    _values[3].Weight = weights[3];
        //    _values[4].Value = rotation.Yaw;
        //    _values[4].Weight = weights[4];
        //    _values[5].Value = rotation.Roll;
        //    _values[5].Weight = weights[5];

        //    _values[6].Value = scale.X;
        //    _values[6].Weight = weights[6];
        //    _values[7].Value = scale.Y;
        //    _values[7].Weight = weights[7];
        //    _values[8].Value = scale.Z;
        //    _values[8].Weight = weights[8];
        //}

        public void UpdateSkeleton(ISkeleton skeleton)
        {
            IBone bone = skeleton[_name];
            if (bone != null)
                UpdateState(bone.FrameState, bone.BindState);
        }
        public void UpdateState(ITransform frameState, ITransform bindState)
        {
            Vec3 t = GetTranslation(bindState.Translation.Raw);
            Quat r = GetRotation(bindState.Rotation.Raw);
            Vec3 s = GetScale(bindState.Scale.Raw);
            frameState.SetAll(t, r, s);
        }
        public void UpdateSkeletonBlended(ISkeleton skeleton, BoneFrame otherBoneFrame, float otherWeight)
        {
            IBone bone = skeleton[_name];
            if (bone != null)
                UpdateStateBlended(bone.FrameState, bone.BindState, otherBoneFrame, otherWeight);
        }
        public void UpdateStateBlended(
            ITransform frameState,
            ITransform bindState,
            BoneFrame otherBoneFrame,
            float otherWeight)
        {
            Vec3 t;
            Vec3 s;
            Quat r;

            if (otherBoneFrame is null)
            {
                otherWeight = 1.0f - otherWeight;

                _translation.Weight *= otherWeight;
                _rotation.Weight *= otherWeight;
                _scale.Weight *= otherWeight;

                t = GetTranslation(bindState.Translation.Raw);
                r = GetRotation(bindState.Rotation.Raw);
                s = GetScale(bindState.Scale);

                frameState.SetAll(t, r, s);
            }
            else
            {
                Vec3 t1 = GetTranslation(bindState.Translation.Raw);
                Vec3 t2 = otherBoneFrame.GetTranslation(bindState.Translation.Raw);
                t = Vec3.Lerp(t1, t2, otherWeight);

                Quat r1 = GetRotation(bindState.Rotation.Raw);
                Quat r2 = otherBoneFrame.GetRotation(bindState.Rotation.Raw);
                r = Quat.Slerp(r1, r2, otherWeight);

                Vec3 s1 = GetScale(bindState.Scale);
                Vec3 s2 = otherBoneFrame.GetScale(bindState.Scale);
                s = Vec3.Lerp(s1, s2, otherWeight);

                frameState.SetAll(t, r, s);
            }
        }
        public BoneFrame BlendedWith(BoneFrame otherBoneFrame, float otherWeight)
        {
            BoneFrame frame = new BoneFrame(_name);
            frame.BlendWith(otherBoneFrame, otherWeight);
            return frame;
        }

        public BoneFrame BlendedWith(BoneAnimation other, float frameIndex, float otherWeight)
            => BlendedWith(other.GetFrame(frameIndex), otherWeight);

        public void BlendWith(BoneFrame otherBoneFrame, float otherWeight)
        {
            Translation.Value = Interp.Lerp(Translation.Value, otherBoneFrame.Translation.Value, otherWeight);
            Translation.Weight = Interp.Lerp(Translation.Weight, otherBoneFrame.Translation.Weight, otherWeight);

            Rotation.Value = Quat.Slerp(Rotation.Value, otherBoneFrame.Rotation.Value, otherWeight);
            Rotation.Weight = Interp.Lerp(Rotation.Weight, otherBoneFrame.Rotation.Weight, otherWeight);

            Scale.Value = Interp.Lerp(Scale.Value, otherBoneFrame.Scale.Value, otherWeight);
            Scale.Weight = Interp.Lerp(Scale.Weight, otherBoneFrame.Scale.Weight, otherWeight);
        }
        public void BlendWith(BoneAnimation other, float frameIndex, float otherWeight)
        {
            BlendWith(other.GetFrame(frameIndex), otherWeight);
        }
    }
}

using TheraEngine.Rendering.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheraEngine.Animation
{
    public class ModelAnimationFrame
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
        public IEnumerable<string> GetAllNames(ModelAnimationFrame other)
        {
            string[] theseNames = new string[_boneFrames.Keys.Count];
            _boneFrames.Keys.CopyTo(theseNames, 0);
            string[] thoseNames = new string[other._boneFrames.Keys.Count];
            other._boneFrames.Keys.CopyTo(thoseNames, 0);
            return theseNames.Intersect(thoseNames);
        }
        public IEnumerable<string> GetAllNames(ModelAnimation other)
        {
            string[] theseNames = new string[_boneFrames.Keys.Count];
            _boneFrames.Keys.CopyTo(theseNames, 0);
            string[] thoseNames = new string[other._boneAnimations.Keys.Count];
            other._boneAnimations.Keys.CopyTo(thoseNames, 0);
            return theseNames.Intersect(thoseNames);
        }
        public ModelAnimationFrame BlendedWith(ModelAnimationFrame other, float otherWeight)
        {
            ModelAnimationFrame blendedFrame = new ModelAnimationFrame();
            foreach (string name in GetAllNames(other))
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
        public ModelAnimationFrame BlendedWith(ModelAnimation other, float frameIndex, float otherWeight)
        {
            ModelAnimationFrame blendedFrame = new ModelAnimationFrame();
            foreach (string name in GetAllNames(other))
            {
                if (_boneFrames.ContainsKey(name))
                {
                    if (other._boneAnimations.ContainsKey(name))
                        blendedFrame.AddBoneFrame(_boneFrames[name].BlendedWith(other._boneAnimations[name], frameIndex, otherWeight));
                    else
                        blendedFrame.AddBoneFrame(_boneFrames[name].BlendedWith(null, otherWeight));
                }
                else
                {
                    if (other._boneAnimations.ContainsKey(name))
                        blendedFrame.AddBoneFrame(other._boneAnimations[name].BlendedWith(frameIndex, null, 1.0f - otherWeight));
                }
            }
            return blendedFrame;
        }

        public void BlendWith(ModelAnimationFrame other, float otherWeight)
        {
            foreach (string name in GetAllNames(other))
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
        public void BlendWith(ModelAnimation other, float frameIndex, float otherWeight)
        {
            foreach (string name in GetAllNames(other))
            {
                if (_boneFrames.ContainsKey(name))
                {
                    if (other._boneAnimations.ContainsKey(name))
                        _boneFrames[name].BlendedWith(other._boneAnimations[name], frameIndex, otherWeight);
                    else
                        _boneFrames[name].BlendedWith(null, otherWeight);
                }
                else
                {
                    if (other._boneAnimations.ContainsKey(name))
                        other._boneAnimations[name].BlendedWith(frameIndex, null, 1.0f - otherWeight);
                }
            }
        }
    }
    public class FrameValueWeight
    {
        public float Value { get; set; }
        public float Weight { get; set; }
    }
    public class BoneFrame
    {
        public string _name;
        public FrameValueWeight[] _values = new FrameValueWeight[9]
        {
            new FrameValueWeight(), //tx
            new FrameValueWeight(), //ty
            new FrameValueWeight(), //tz
            new FrameValueWeight(), //ry
            new FrameValueWeight(), //rp
            new FrameValueWeight(), //rr
            new FrameValueWeight(), //sx
            new FrameValueWeight(), //sy
            new FrameValueWeight(), //sz
        };

        public Vec3 GetTranslation(Vec3 bindTranslation) => new Vec3(
            CustomMath.Lerp(bindTranslation.X, _values[0].Value, _values[0].Weight),
            CustomMath.Lerp(bindTranslation.Y, _values[1].Value, _values[1].Weight),
            CustomMath.Lerp(bindTranslation.Z, _values[2].Value, _values[2].Weight));
        public Rotator GetRotation(Rotator bindRotation) => new Rotator(
            CustomMath.Lerp(bindRotation.Pitch, _values[4].Value, _values[4].Weight),
            CustomMath.Lerp(bindRotation.Yaw, _values[3].Value, _values[3].Weight),
            CustomMath.Lerp(bindRotation.Roll, _values[5].Value, _values[5].Weight),
            RotationOrder.YPR);
        public Vec3 GetScale(Vec3 bindScale) => new Vec3(
            CustomMath.Lerp(bindScale.X, _values[6].Value, _values[6].Weight),
            CustomMath.Lerp(bindScale.Y, _values[7].Value, _values[7].Weight),
            CustomMath.Lerp(bindScale.Z, _values[8].Value, _values[8].Weight));

        public BoneFrame(string name, float?[] values)
        {
            _name = name;
            for (int i = 0; i < 9; ++i)
            {
                float? value = values[i];
                var weight = _values[i];
                if (value == null)
                {
                    weight.Value = i >= 6 ? 1.0f : 0.0f;
                    weight.Weight = 0.0f;
                }
                else
                {
                    weight.Value = value.Value;
                    weight.Weight = 1.0f;
                }
            }
        }

        public void UpdateSkeleton(Skeleton skeleton)
        {
            Bone bone = skeleton[_name];
            if (bone != null)
                UpdateState(bone.FrameState, bone.BindState);
        }
        public void UpdateState(FrameState frameState, FrameState bindState)
        {
            Vec3 t = GetTranslation(bindState.Translation);
            Rotator r = GetRotation(bindState.Rotation);
            Vec3 s = GetScale(bindState.Scale);
            frameState.SetAll(t, r, s);
        }
        public void UpdateSkeletonBlended(Skeleton skeleton, BoneFrame otherBoneFrame, float otherWeight)
        {
            Bone bone = skeleton[_name];
            if (bone != null)
                UpdateStateBlended(bone.FrameState, bone.BindState, otherBoneFrame, otherWeight);
        }
        public void UpdateStateBlended(
            FrameState frameState,
            FrameState bindState,
            BoneFrame otherBoneFrame,
            float otherWeight)
        {
            Vec3 t;
            Quat r;
            Vec3 s;

            if (otherBoneFrame == null)
            {
                t = _translation;
                r = _rotation;
                s = _scale;
                otherWeight = 1.0f - otherWeight;
                _translationWeight *= otherWeight;
                _rotationWeight *= otherWeight;
                _scaleWeight *= otherWeight;
            }
            else
            {
                Vec3 t1 = GetTranslation(bindState.Translation);
                Vec3 t2 = otherBoneFrame.GetTranslation(bindState.Translation);
                t = Vec3.Lerp(t1, t2, otherWeight);

                Quat r1 = GetRotation(bindState.Quaternion);
                Quat r2 = otherBoneFrame.GetRotation(bindState.Quaternion);
                r = Quat.Slerp(r1, r2, otherWeight);

                Vec3 s1 = GetScale(bindState.Scale);
                Vec3 s2 = otherBoneFrame.GetScale(bindState.Scale);
                s = Vec3.Lerp(s1, s2, otherWeight);
            }

            frameState.SetAll(t, r, s);
        }
        public BoneFrame BlendedWith(BoneFrame otherBoneFrame, float otherWeight)
        {
            Vec3 t1 = _translation;
            Vec3 t2 = otherBoneFrame._translation;
            Vec3 t = Vec3.Lerp(t1, t2, otherWeight);

            Quat r1 = _rotation;
            Quat r2 = otherBoneFrame._rotation;
            Quat r = Quat.Slerp(r1, r2, otherWeight);

            Vec3 s1 = _scale;
            Vec3 s2 = otherBoneFrame._scale;
            Vec3 s = Vec3.Lerp(s1, s2, otherWeight);

            return new BoneFrame(
                _name,
                t, _translationWeight * otherBoneFrame._translationWeight,
                r, _rotationWeight * otherBoneFrame._rotationWeight,
                s, _scaleWeight * otherBoneFrame._scaleWeight);
        }
        public BoneFrame BlendedWith(BoneAnimation other, float frameIndex, float otherWeight)
        {
            return BlendedWith(other.GetFrame(frameIndex), otherWeight);
        }

        public void BlendWith(BoneFrame otherBoneFrame, float otherWeight)
        {
            Vec3 t1 = _translation;
            Vec3 t2 = otherBoneFrame._translation;
            Vec3 t = Vec3.Lerp(t1, t2, otherWeight);

            Quat r1 = _rotation;
            Quat r2 = otherBoneFrame._rotation;
            Quat r = Quat.Slerp(r1, r2, otherWeight);

            Vec3 s1 = _scale;
            Vec3 s2 = otherBoneFrame._scale;
            Vec3 s = Vec3.Lerp(s1, s2, otherWeight);

            _translation = t;
            _translationWeight = _translationWeight * otherBoneFrame._translationWeight;
            _rotation = r;
            _rotationWeight = _rotationWeight * otherBoneFrame._rotationWeight;
            _scale = s;
            _scaleWeight = _scaleWeight * otherBoneFrame._scaleWeight;
        }
        public void BlendWith(BoneAnimation other, float frameIndex, float otherWeight)
        {
            BlendWith(other.GetFrame(frameIndex), otherWeight);
        }
    }
}

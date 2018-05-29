using TheraEngine.Rendering.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Core.Maths.Transforms;

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
        public IEnumerable<string> GetAllNames(SkeletalAnimation other)
        {
            string[] theseNames = new string[_boneFrames.Keys.Count];
            _boneFrames.Keys.CopyTo(theseNames, 0);
            string[] thoseNames = new string[other.BoneAnimations.Keys.Count];
            other.BoneAnimations.Keys.CopyTo(thoseNames, 0);
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
        public ModelAnimationFrame BlendedWith(SkeletalAnimation other, float frameIndex, float otherWeight)
        {
            ModelAnimationFrame blendedFrame = new ModelAnimationFrame();
            foreach (string name in GetAllNames(other))
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
        public void BlendWith(SkeletalAnimation other, float frameIndex, float otherWeight)
        {
            foreach (string name in GetAllNames(other))
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
    public class FrameValueWeight
    {
        public FrameValueWeight() { }
        public FrameValueWeight(float value, float weight)
        {
            Value = value;
            Weight = weight;
        }

        public float Value { get; set; }
        public float Weight { get; set; }
    }
    public class BoneFrame
    {
        public string _name;
        public RotationOrder _eulerOrder = RotationOrder.RYP;
        public FrameValueWeight[] _values;
        //t, r, s;
        //t, s => x, y, z;
        //r => p, y, r

        public Vec3 GetTranslation(Vec3 bindTranslation) => new Vec3(
            Interp.Lerp(bindTranslation.X, _values[0].Value, _values[0].Weight),
            Interp.Lerp(bindTranslation.Y, _values[1].Value, _values[1].Weight),
            Interp.Lerp(bindTranslation.Z, _values[2].Value, _values[2].Weight));
        public Rotator GetRotation(Rotator bindRotation) => new Rotator(
            Interp.Lerp(bindRotation.Pitch, _values[3].Value, _values[4].Weight),
            Interp.Lerp(bindRotation.Yaw, _values[4].Value, _values[3].Weight),
            Interp.Lerp(bindRotation.Roll, _values[5].Value, _values[5].Weight),
            _eulerOrder);
        public Quat GetQuat(Rotator bindRotation) => Quat.FromEulerAngles(
            Interp.Lerp(bindRotation.Pitch, _values[3].Value, _values[4].Weight),
            Interp.Lerp(bindRotation.Yaw, _values[4].Value, _values[3].Weight),
            Interp.Lerp(bindRotation.Roll, _values[5].Value, _values[5].Weight),
            _eulerOrder);
        public Vec3 GetScale(Vec3 bindScale) => new Vec3(
            Interp.Lerp(bindScale.X, _values[6].Value, _values[6].Weight),
            Interp.Lerp(bindScale.Y, _values[7].Value, _values[7].Weight),
            Interp.Lerp(bindScale.Z, _values[8].Value, _values[8].Weight));
        
        public Vec3 GetUnweightedTranslation()
            => new Vec3(_values[0].Value, _values[1].Value, _values[2].Value);
        public Vec3 GetUnweightedRotationPYR()
            => new Vec3(_values[3].Value, _values[4].Value, _values[5].Value);
        public Vec3 GetUnweightedScale() 
            => new Vec3(_values[6].Value, _values[7].Value, _values[8].Value);

        public BoneFrame(string name, float?[] values, RotationOrder eulerOrder)
        {
            _name = name;
            _eulerOrder = eulerOrder;
            _values = new FrameValueWeight[9];
            for (int i = 0; i < 9; ++i)
            {
                float? value = values[i];
                _values[i] = value == null ? new FrameValueWeight(i >= 6 ? 1.0f : 0.0f, 0.0f) : new FrameValueWeight(value.Value, 1.0f);
            }
        }
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

        public BoneFrame(string name, FrameValueWeight[] values, RotationOrder eulerOrder)
        {
            _name = name;
            _eulerOrder = eulerOrder;
            _values = values;
        }

        public void UpdateSkeleton(Skeleton skeleton)
        {
            Bone bone = skeleton[_name];
            if (bone != null)
                UpdateState(bone.FrameState, bone.BindState);
        }
        public void UpdateState(Transform frameState, Transform bindState)
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
            Transform frameState,
            Transform bindState,
            BoneFrame otherBoneFrame,
            float otherWeight)
        {
            Vec3 t;
            Vec3 s;

            if (otherBoneFrame == null)
            {
                otherWeight = 1.0f - otherWeight;
                for (int i = 0; i < _values.Length; ++i)
                    _values[i].Weight *= otherWeight;

                t = GetTranslation(bindState.Translation);
                Rotator r = GetRotation(bindState.Rotation);
                s = GetScale(bindState.Scale);

                frameState.SetAll(t, r, s);
            }
            else
            {
                Vec3 t1 = GetTranslation(bindState.Translation);
                Vec3 t2 = otherBoneFrame.GetTranslation(bindState.Translation);
                t = Vec3.Lerp(t1, t2, otherWeight);

                Quat r1 = GetQuat(bindState.Rotation);
                Quat r2 = otherBoneFrame.GetQuat(bindState.Rotation);
                Quat r = Quat.Slerp(r1, r2, otherWeight);

                Vec3 s1 = GetScale(bindState.Scale);
                Vec3 s2 = otherBoneFrame.GetScale(bindState.Scale);
                s = Vec3.Lerp(s1, s2, otherWeight);

                frameState.SetAll(t, r, s);
            }
        }
        public BoneFrame BlendedWith(BoneFrame otherBoneFrame, float otherWeight)
        {
            FrameValueWeight[] values = new FrameValueWeight[9];
            for (int i = 0; i < 9; ++i)
            {
                var value = _values[i];
                var otherValue = otherBoneFrame._values[i];
                values[i] = new FrameValueWeight(
                    Interp.Lerp(value.Value, otherValue.Value, otherWeight),
                    Interp.Lerp(value.Weight, otherValue.Weight, otherWeight));
            }
            return new BoneFrame(_name, values, _eulerOrder);
        }

        public BoneFrame BlendedWith(BoneAnimation other, float frameIndex, float otherWeight)
            => BlendedWith(other.GetFrame(frameIndex), otherWeight);

        public void BlendWith(BoneFrame otherBoneFrame, float otherWeight)
        {
            for (int i = 0; i < 9; ++i)
            {
                var value = _values[i];
                var otherValue = otherBoneFrame._values[i];
                value.Value = Interp.Lerp(value.Value, otherValue.Value, otherWeight);
                value.Weight = Interp.Lerp(value.Weight, otherValue.Weight, otherWeight);
            }
        }
        public void BlendWith(BoneAnimation other, float frameIndex, float otherWeight)
        {
            BlendWith(other.GetFrame(frameIndex), otherWeight);
        }
    }
}

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
        public RotationOrder _eulerOrder = RotationOrder.YPR;
        public FrameValueWeight[] _values;
        //t, r, s;
        //t, s => x, y, z;
        //r => p, y, r

        public Vec3 GetTranslation(Vec3 bindTranslation) => new Vec3(
            CustomMath.Lerp(bindTranslation.X, _values[0].Value, _values[0].Weight),
            CustomMath.Lerp(bindTranslation.Y, _values[1].Value, _values[1].Weight),
            CustomMath.Lerp(bindTranslation.Z, _values[2].Value, _values[2].Weight));
        public Rotator GetRotation(Rotator bindRotation) => new Rotator(
            CustomMath.Lerp(bindRotation.Pitch, _values[4].Value, _values[4].Weight),
            CustomMath.Lerp(bindRotation.Yaw, _values[3].Value, _values[3].Weight),
            CustomMath.Lerp(bindRotation.Roll, _values[5].Value, _values[5].Weight),
            _eulerOrder);
        public Vec3 GetScale(Vec3 bindScale) => new Vec3(
            CustomMath.Lerp(bindScale.X, _values[6].Value, _values[6].Weight),
            CustomMath.Lerp(bindScale.Y, _values[7].Value, _values[7].Weight),
            CustomMath.Lerp(bindScale.Z, _values[8].Value, _values[8].Weight));
        
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
                if (value == null)
                    _values[i] = new FrameValueWeight(i >= 6 ? 1.0f : 0.0f, 0.0f);
                else
                    _values[i] = new FrameValueWeight(value.Value, 1.0f);
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
        public void UpdateState(LocalRotTransform frameState, LocalRotTransform bindState)
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
            LocalRotTransform frameState,
            LocalRotTransform bindState,
            BoneFrame otherBoneFrame,
            float otherWeight)
        {
            Vec3 t;
            Rotator r;
            Vec3 s;

            if (otherBoneFrame == null)
            {
                otherWeight = 1.0f - otherWeight;
                for (int i = 0; i < _values.Length; ++i)
                    _values[i].Weight *= otherWeight;

                t = GetTranslation(bindState.Translation);
                r = GetRotation(bindState.Rotation);
                s = GetScale(bindState.Scale);
            }
            else
            {
                Vec3 t1 = GetTranslation(bindState.Translation);
                Vec3 t2 = otherBoneFrame.GetTranslation(bindState.Translation);
                t = Vec3.Lerp(t1, t2, otherWeight);

                Rotator r1 = GetRotation(bindState.Rotation);
                Rotator r2 = otherBoneFrame.GetRotation(bindState.Rotation);
                r = Rotator.Lerp(r1, r2, otherWeight);

                Vec3 s1 = GetScale(bindState.Scale);
                Vec3 s2 = otherBoneFrame.GetScale(bindState.Scale);
                s = Vec3.Lerp(s1, s2, otherWeight);
            }

            frameState.SetAll(t, r, s);
        }
        public BoneFrame BlendedWith(BoneFrame otherBoneFrame, float otherWeight)
        {
            FrameValueWeight[] values = new FrameValueWeight[9];
            for (int i = 0; i < 9; ++i)
            {
                var value = _values[i];
                var otherValue = otherBoneFrame._values[i];
                values[i] = new FrameValueWeight(
                    CustomMath.Lerp(value.Value, otherValue.Value, otherWeight),
                    CustomMath.Lerp(value.Weight, otherValue.Weight, otherWeight));
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
                value.Value = CustomMath.Lerp(value.Value, otherValue.Value, otherWeight);
                value.Weight = CustomMath.Lerp(value.Weight, otherValue.Weight, otherWeight);
            }
        }
        public void BlendWith(BoneAnimation other, float frameIndex, float otherWeight)
        {
            BlendWith(other.GetFrame(frameIndex), otherWeight);
        }
    }
}

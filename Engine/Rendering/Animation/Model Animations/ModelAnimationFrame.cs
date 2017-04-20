using CustomEngine.Rendering.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Animation
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
    }
    public class BoneFrame
    {
        public string _name;
        public Vec3 _translation;
        public float _translationWeight = 1.0f;
        public Quat _rotation;
        public float _rotationWeight = 1.0f;
        public Vec3 _scale;
        public float _scaleWeight = 1.0f;

        public Vec3 GetTranslation(Vec3 bindTranslation)
            => Vec3.Lerp(bindTranslation, _translation, _translationWeight);
        public Quat GetRotation(Quat bindRotation)
            => Quat.Slerp(bindRotation, _rotation, _rotationWeight);
        public Vec3 GetScale(Vec3 bindScale)
            => Vec3.Lerp(bindScale, _scale, _scaleWeight);

        public BoneFrame(
            string name, 
            Vec3 t, float tw,
            Quat r, float rw,
            Vec3 s, float sw)
        {
            _name = name;
            _translation = t;
            _translationWeight = tw;
            _rotation = r;
            _rotationWeight = rw;
            _scale = s;
            _scaleWeight = sw;
        }

        public void UpdateSkeleton(Skeleton skeleton)
        {
            Bone bone = skeleton[_name];
            if (bone != null)
                UpdateState(bone.FrameState, bone.BindState);
        }
        public void UpdateState(FrameState frameState, FrameState bindState)
        {
            Vec3 t = Vec3.Lerp(bindState.Translation, _translation, _translationWeight);
            Quat r = Quat.Slerp(bindState.Quaternion, _rotation, _rotationWeight);
            Vec3 s = Vec3.Lerp(bindState.Scale, _scale, _scaleWeight);
            frameState.SetAll(t, r, s);
        }
        public void UpdateSkeletonBlended(Skeleton skeleton, BoneFrame otherBoneFrame, float otherWeight)
        {
            Bone bone = skeleton[_name];
            if (bone != null)
                UpdateStateBlended(bone.FrameState, bone.BindState, otherBoneFrame, otherWeight);
        }
        public void UpdateStateBlended(FrameState frameState, FrameState bindState, BoneFrame otherBoneFrame, float otherWeight)
        {
            Vec3 t1 = _translation ?? bindState.Translation;
            Vec3 t2 = otherBoneFrame._translation ?? bindState.Translation;
            Vec3 t = Vec3.Lerp(t1, t2, otherWeight);

            Quat r1 = _rotation ?? bindState.Quaternion;
            Quat r2 = otherBoneFrame._rotation ?? bindState.Quaternion;
            Quat r = Quat.Slerp(r1, r2, otherWeight);

            Vec3 s1 = _scale ?? bindState.Scale;
            Vec3 s2 = otherBoneFrame._scale ?? bindState.Scale;
            Vec3 s = Vec3.Lerp(s1, s2, otherWeight);

            frameState.SetAll(t, r, s);
        }
        public BoneFrame BlendedWith(BoneFrame other, float otherWeight)
        {
            Vec3 t1 = _translation ?? bindState.Translation;
            Vec3 t2 = otherBoneFrame._translation ?? bindState.Translation;
            Vec3 t = Vec3.Lerp(t1, t2, otherWeight);

            Quat r1 = _rotation ?? bindState.Quaternion;
            Quat r2 = otherBoneFrame._rotation ?? bindState.Quaternion;
            Quat r = Quat.Slerp(r1, r2, otherWeight);

            Vec3 s1 = _scale ?? bindState.Scale;
            Vec3 s2 = otherBoneFrame._scale ?? bindState.Scale;
            Vec3 s = Vec3.Lerp(s1, s2, otherWeight);
        }
        public BoneFrame BlendedWith(BoneAnimation other, float frameIndex, float otherWeight)
        {

        }
    }
}

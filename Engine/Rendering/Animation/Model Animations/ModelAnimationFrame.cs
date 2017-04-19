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

        public void UpdateSkeleton(Skeleton skeleton)
        {
            foreach (BoneFrame b in _boneFrames.Values)
                b.UpdateSkeleton(skeleton);
        }
    }
    public class BoneFrame
    {
        public string _name;
        public Vec3? _translation;
        public Quat? _rotation;
        public Vec3? _scale;

        public BoneFrame(string name, Vec3? t, Quat? r, Vec3? s)
        {
            _name = name;
            _translation = t;
            _rotation = r;
            _scale = s;
        }

        public void UpdateSkeleton(Skeleton skeleton)
        {
            Bone bone = skeleton[_name];
            if (bone != null)
                UpdateState(bone.FrameState, bone.BindState);
        }
        public void UpdateState(FrameState frameState, FrameState bindState)
        {
            Vec3 t = _translation ?? bindState.Translation;
            Quat r = _rotation ?? bindState.Quaternion;
            Vec3 s = _scale ?? bindState.Scale;
            frameState.SetAll(t, r, s);
        }
        public void UpdateStateBlended(
            FrameState frameState,
            FrameState bindState,
            BoneFrame otherBoneFrame,
            float otherWeight,
            AnimBlendType blendType)
        {
            Vec3 t1 = _translation.First == null ?
                bindState.Translation :
                _translation.First.Interpolate(thisFrameIndex);
            Vec3 t2 = otherBoneAnim._translation.First == null ?
                bindState.Translation :
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
                bindState.Scale :
                _scale.First.Interpolate(thisFrameIndex);
            Vec3 s2 = otherBoneAnim._scale.First == null ?
                bindState.Scale :
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

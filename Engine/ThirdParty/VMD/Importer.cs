using System;
using System.Text;
using TheraEngine.Animation;
using TheraEngine.Core.Memory;
using TheraEngine.Rendering.Models;

namespace TheraEngine.ThirdParty.VMD
{
    public class VMDImporter
    {
        private SkeletalModel _model;
        private Skeleton _skeleton;
        
        public VMDImporter()
        {

        }
        
        public unsafe (
            SkeletalAnimation SkeletonAnimation,
            AnimationTree MorphAnimation,
            AnimationTree CameraAnimation,
            AnimationTree LightAnimation)
            Import(string path)
        {
            SkeletalAnimation skelAnim = null;
            AnimationTree morphAnim = null;
            AnimationTree camAnim = null;
            AnimationTree lightAnim = null;

            FileMap map = FileMap.FromFile(path);

            VoidPtr baseAddr = map.Address;
            byte[] bytes = new byte[30];
            for (int i = 0; i < 30; ++i)
                bytes[i] = *(byte*)baseAddr[i, 1];
            char[] chars = new char[30];
            Encoding.ASCII.GetDecoder().GetChars(bytes, 0, 30, chars, 0, true);
            string magic = new string(chars);
            if (!magic.Equals(Header2.MagicString))
                return (skelAnim, morphAnim, camAnim, lightAnim);
            
            Header2* vmd = (Header2*)baseAddr;

            skelAnim = ParseBones(vmd->BoneKeyframes);
            if (skelAnim != null)
                skelAnim.Name = vmd->ModelName;

            morphAnim = ParseMorphs(vmd->MorphKeyframes);
            if (morphAnim != null)
                morphAnim.Name = vmd->ModelName;

            camAnim = ParseCameraKeyframes(vmd->CameraKeyframes);
            if (camAnim != null)
                camAnim.Name = vmd->ModelName;

            lightAnim = ParseLightKeyframes(vmd->LightKeyframes);
            if (lightAnim != null)
                lightAnim.Name = vmd->ModelName;

            return (skelAnim, morphAnim, camAnim, lightAnim);
        }

        private unsafe SkeletalAnimation ParseBones(KeyframeList<BoneKeyframe>* kfs)
        {
            uint count = kfs->KeyframeCount;
            if (count == 0u)
                return null;

            var interpType = EVectorInterpType.CubicBezier;
            float hz = 1.0f / 60.0f;
            SkeletalAnimation anim = new SkeletalAnimation();
            for (uint i = 0; i < count; ++i)
            {
                var kf = kfs->Keyframes[i];
                var relPos = kf.RelativeTranslation;
                var rot = kf.Rotation;
                var index = kf.FrameIndex;
                float sec = index * hz;
                byte* interp = kf.Interpolation;
                string boneName = kf.BoneName;
                BoneAnimation boneAnim = anim.FindOrCreateBoneAnimation(boneName, out bool found);

                boneAnim.TranslationX.Keyframes.Add(new FloatKeyframe(sec, relPos.X, 0.0f, interpType));
                boneAnim.TranslationY.Keyframes.Add(new FloatKeyframe(sec, relPos.Y, 0.0f, interpType));
                boneAnim.TranslationZ.Keyframes.Add(new FloatKeyframe(sec, relPos.Z, 0.0f, interpType));

                boneAnim.RotationX.Keyframes.Add(new FloatKeyframe(sec, relPos.X, 0.0f, interpType));
                boneAnim.RotationY.Keyframes.Add(new FloatKeyframe(sec, relPos.Y, 0.0f, interpType));
                boneAnim.RotationZ.Keyframes.Add(new FloatKeyframe(sec, relPos.Z, 0.0f, interpType));
            }

            return anim;
        }
        private unsafe AnimationTree ParseMorphs(KeyframeList<MorphKeyframe>* kfs)
        {
            uint count = kfs->KeyframeCount;
            if (count == 0u)
                return null;

            AnimationTree anim = new AnimationTree();
            for (uint i = 0; i < count; ++i)
            {

            }

            return anim;
        }
        private unsafe AnimationTree ParseCameraKeyframes(KeyframeList<CameraKeyframe>* kfs)
        {
            uint count = kfs->KeyframeCount;
            if (count == 0u)
                return null;

            AnimationTree anim = new AnimationTree();
            for (uint i = 0; i < count; ++i)
            {

            }

            return anim;
        }
        private unsafe AnimationTree ParseLightKeyframes(KeyframeList<LightKeyframe>* kfs)
        {
            uint count = kfs->KeyframeCount;
            if (count == 0u)
                return null;

            AnimationTree anim = new AnimationTree();
            for (uint i = 0; i < count; ++i)
            {

            }

            return anim;
        }
    }
}

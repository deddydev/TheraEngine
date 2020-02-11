using System;
using System.Text;
using System.Threading;
using TheraEngine.Animation;
using TheraEngine.Core.Memory;
using TheraEngine.Rendering.Models;

namespace TheraEngine.ThirdParty.VMD
{
    public class VMDImporter
    {
        private SkeletalModel _model;
        private Skeleton _skeleton;
        private IProgress<float> _progress;
        private CancellationToken _cancel;

        public VMDImporter()
        {

        }
        public VMDImporter(IProgress<float> progress, CancellationToken cancel)
        {
            _progress = progress;
            _cancel = cancel;
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
            string magic = new string((sbyte*)baseAddr).Trim();
            if (!magic.Equals(Header2.MagicString))
                return (skelAnim, morphAnim, camAnim, lightAnim);
            
            Header2* vmd = (Header2*)baseAddr;
            var bones = vmd->BoneKeyframes;
            var morphs = vmd->MorphKeyframes;
            var cams = vmd->CameraKeyframes;
            var lights = vmd->LightKeyframes;

            uint total = bones->KeyframeCount + morphs->KeyframeCount + cams->KeyframeCount + lights->KeyframeCount;

            skelAnim = ParseBones(bones);
            if (skelAnim != null)
                skelAnim.Name = vmd->ModelName;

            morphAnim = ParseMorphs(morphs);
            if (morphAnim != null)
                morphAnim.Name = vmd->ModelName;

            camAnim = ParseCameraKeyframes(cams);
            if (camAnim != null)
                camAnim.Name = vmd->ModelName;

            lightAnim = ParseLightKeyframes(lights);
            if (lightAnim != null)
                lightAnim.Name = vmd->ModelName;

            return (skelAnim, morphAnim, camAnim, lightAnim);
        }

        private unsafe SkeletalAnimation ParseBones(KeyframeList<BoneKeyframe>* kfs)
        {
            uint count = kfs->KeyframeCount;
            if (count == 0u)
                return null;

            var vecInterpType = EVectorInterpType.CubicBezier;
            var rotInterpType = ERadialInterpType.CubicBezier;

            //TODO: 60 or 30 fps?
            float hz = 1.0f / 30.0f;

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

                boneAnim.TranslationX.Keyframes.Add(new FloatKeyframe(sec, relPos.X, 0.0f, vecInterpType));
                boneAnim.TranslationY.Keyframes.Add(new FloatKeyframe(sec, relPos.Y, 0.0f, vecInterpType));
                boneAnim.TranslationZ.Keyframes.Add(new FloatKeyframe(sec, relPos.Z, 0.0f, vecInterpType));

                boneAnim.Rotation.Keyframes.Add(new QuatKeyframe(sec, rot, rot, rotInterpType));
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

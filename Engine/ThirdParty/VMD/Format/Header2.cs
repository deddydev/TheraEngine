using System.Runtime.InteropServices;
using System.Text;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Memory;
using Extensions;

namespace TheraEngine.ThirdParty.VMD
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Header2
    {
        public const int Size = 50;
        public static readonly string MagicString = "Vocaloid Motion Data 0002";

        //Vocaloid Motion Data 0002 + 5 null bytes
        private fixed sbyte _magic[30];
        private fixed sbyte _modelName[20];

        public KeyframeList<BoneKeyframe>* BoneKeyframes => (KeyframeList<BoneKeyframe>*)(
            Address + Size);
        public KeyframeList<MorphKeyframe>* MorphKeyframes => (KeyframeList<MorphKeyframe>*)(
            Address + Size + 
            BoneKeyframes->GetSize());
        public KeyframeList<CameraKeyframe>* CameraKeyframes => (KeyframeList<CameraKeyframe>*)(
            Address + Size +
            BoneKeyframes->GetSize() + 
            MorphKeyframes->GetSize());
        public KeyframeList<LightKeyframe>* LightKeyframes => (KeyframeList<LightKeyframe>*)(
            Address + Size +
            BoneKeyframes->GetSize() +
            MorphKeyframes->GetSize() +
            CameraKeyframes->GetSize());

        public string Magic
        {
            get
            {
                fixed (sbyte* ptr = _magic)
                    return new string(ptr);
            }
            set
            {
                fixed (sbyte* ptr = _magic)
                    value.Write(ptr, 30, true);
            }
        }
        public string ModelName
        {
            get
            {
                fixed (sbyte* ptr = _modelName)
                    return new string(ptr);
            }
            set
            {
                fixed (sbyte* ptr = _modelName)
                    value.Write(ptr, 20, true);
            }
        }

        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Keyframe
    {
        private fixed sbyte _boneName[15];
        private uint _frameIndex;
        private Vec3 _position;
        private Vec4 _rotation;

        public string BoneName
        {
            get
            {
                fixed (sbyte* ptr = _boneName)
                    return new string(ptr);
            }
            set
            {
                fixed (sbyte* ptr = _boneName)
                    value.Write(ptr, 15, true);
            }
        }

        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
}

using System.Runtime.InteropServices;
using TheraEngine.Core.Memory;
using Extensions;

namespace TheraEngine.ThirdParty.VMD
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Header1
    {
        public const int Size = 40;
        public static readonly string MagicString = "Vocaloid Motion Data";

        private fixed sbyte _magic[30];
        private fixed sbyte _modelName[10];

        public KeyframeList<BoneKeyframe>* BoneKeyframes => (KeyframeList<BoneKeyframe>*)(
            Address + Size);
        public KeyframeList<MorphKeyframe>* MorphKeyframes => (KeyframeList<MorphKeyframe>*)(
            Address + Size + BoneKeyframes->GetSize());
        public KeyframeList<CameraKeyframe>* CameraKeyframes => (KeyframeList<CameraKeyframe>*)(
            Address + Size + BoneKeyframes->GetSize() + MorphKeyframes->GetSize());

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
                    value.Write(ptr, 10, true);
            }
        }

        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
}

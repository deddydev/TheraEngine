using System.Runtime.InteropServices;
using TheraEngine.Core.Memory;
using Extensions;

namespace TheraEngine.ThirdParty.VMD
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MorphKeyframe
    {
        public const int Size = 23;

        private fixed sbyte _morphName[15];
        private uint _frameIndex;
        private float _weight;
        
        public string MorphName
        {
            get
            {
                fixed (sbyte* ptr = _morphName)
                    return new string(ptr);
            }
            set
            {
                fixed (sbyte* ptr = _morphName)
                    value.Write(ptr, 15, true);
            }
        }
        public uint FrameIndex
        {
            get => _frameIndex;
            set => _frameIndex = value;
        }
        public float Weight
        {
            get => _weight;
            set => _weight = value;
        }

        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
}

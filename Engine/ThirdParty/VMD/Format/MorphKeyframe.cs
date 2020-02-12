using System.Runtime.InteropServices;
using TheraEngine.Core.Memory;
using Extensions;
using System.Text;

namespace TheraEngine.ThirdParty.VMD
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MorphKeyframe
    {
        public const int Size = 23;

        private fixed byte _morphName[15];
        private uint _frameIndex;
        private float _weight;
        
        public string MorphName
        {
            get
            {
                fixed (byte* ptr = _morphName)
                    return Encoding.GetEncoding(932).GetString(ptr, 15).Trim('\0');
            }
            set
            {
                //fixed (sbyte* ptr = _morphName)
                //    value.Write(ptr, 15, true);
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

using System.Runtime.InteropServices;
using System.Text;
using TheraEngine.Core.Memory;

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
                byte[] bytes = new byte[15];
                for (int i = 0; i < 15; ++i)
                    bytes[i] = _morphName[i];
                char[] chars = new char[15];
                Encoding.ASCII.GetDecoder().GetChars(bytes, 0, 30, chars, 0, true);
                return new string(chars);
            }
            set
            {
                byte[] bytes = new byte[15];
                Encoding.ASCII.GetEncoder().GetBytes(value.ToCharArray(), 0, value.Length, bytes, 0, true);
                for (int i = 0; i < 15; ++i)
                    _morphName[i] = bytes[i];
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

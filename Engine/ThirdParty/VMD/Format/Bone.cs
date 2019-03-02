using System;
using System.Runtime.InteropServices;
using System.Text;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Memory;

namespace TheraEngine.ThirdParty.VMD
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct BoneKeyframe
    {
        public const int Size = 111;

        private fixed byte _boneName[15];
        private uint _frameIndex;
        private Vec3 _boneRelativePosition;
        private Quat _boneRotation;
        private fixed byte _interpolation[64];

        public string BoneName
        {
            get
            {
                byte[] bytes = new byte[15];
                for (int i = 0; i < 15; ++i)
                    bytes[i] = _boneName[i];
                char[] chars = new char[15];
                Encoding.ASCII.GetDecoder().GetChars(bytes, 0, 30, chars, 0, true);
                return new string(chars);
            }
            set
            {
                byte[] bytes = new byte[15];
                Encoding.ASCII.GetEncoder().GetBytes(value.ToCharArray(), 0, value.Length, bytes, 0, true);
                for (int i = 0; i < 15; ++i)
                    _boneName[i] = bytes[i];
            }
        }
        public uint FrameIndex
        {
            get => _frameIndex;
            set => _frameIndex = value;
        }
        public Vec3 BoneRelativePosition
        {
            get => _boneRelativePosition;
            set => _boneRelativePosition = value;
        }
        public Quat BoneRotation
        {
            get => _boneRotation;
            set => _boneRotation = value;
        }

        public unsafe byte* Interpolation { get { fixed (byte* ptr = _interpolation) return ptr; } }
        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
}

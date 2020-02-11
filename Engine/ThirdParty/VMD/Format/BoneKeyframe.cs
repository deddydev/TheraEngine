using System;
using System.Runtime.InteropServices;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Memory;
using Extensions;

namespace TheraEngine.ThirdParty.VMD
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct BoneKeyframe
    {
        public const int Size = 111;

        private fixed sbyte _boneName[15];
        private uint _frameIndex;
        private Vec3 _relativeTranslation;
        private Quat _rotation;
        private fixed byte _interpolation[64];

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
        public uint FrameIndex
        {
            get => _frameIndex;
            set => _frameIndex = value;
        }
        public Vec3 RelativeTranslation
        {
            get => _relativeTranslation;
            set => _relativeTranslation = value;
        }
        public Quat Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        public unsafe byte* Interpolation { get { fixed (byte* ptr = _interpolation) return ptr; } }
        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
}

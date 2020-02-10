using System.Runtime.InteropServices;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Memory;

namespace TheraEngine.ThirdParty.VMD
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CameraKeyframe
    {
        public const int Size = 23;
        
        private uint _frameIndex;
        private float _zoomDistance;
        private Vec3 _position;
        private Vec3 _rotation;
        private fixed byte _interpolation[24];
        private uint _fov;
        private byte _isPerspective;

        public uint FrameIndex
        {
            get => _frameIndex;
            set => _frameIndex = value;
        }
        public float Length
        {
            get => _zoomDistance;
            set => _zoomDistance = value;
        }
        public Vec3 Position
        {
            get => _position;
            set => _position = value;
        }
        public Vec3 Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }
        public uint FOV
        {
            get => _fov;
            set => _fov = value;
        }
        public bool IsPerspective
        {
            get => _isPerspective != 0;
            set => _isPerspective = (byte)(value ? 1 : 0);
        }

        public unsafe byte* Interpolation { get { fixed (byte* ptr = _interpolation) return ptr; } }
        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
}

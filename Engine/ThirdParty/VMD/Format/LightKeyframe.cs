using System.Runtime.InteropServices;
using System.Text;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Memory;

namespace TheraEngine.ThirdParty.VMD
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct LightKeyframe
    {
        public const int Size = 28;

        private uint _frameIndex;
        private Vec3 _color;
        private Vec3 _direction;
        
        public uint FrameIndex
        {
            get => _frameIndex;
            set => _frameIndex = value;
        }
        public Vec3 Color
        {
            get => _color;
            set => _color = value;
        }
        public Vec3 Direction
        {
            get => _direction;
            set => _direction = value;
        }

        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
}

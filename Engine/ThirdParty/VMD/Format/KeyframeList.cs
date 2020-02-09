using System.Runtime.InteropServices;
using TheraEngine.Core.Memory;

namespace TheraEngine.ThirdParty.VMD
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct KeyframeList<T> where T : unmanaged
    {
        private uint _keyframeCount;
        public uint KeyframeCount
        {
            get => _keyframeCount;
            set => _keyframeCount = value;
        }

        public long GetSize() => KeyframeCount * sizeof(T);

        public T* Keyframes => (T*)(Address + 4);

        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
}

using System;
using System.Runtime.InteropServices;

namespace TheraEngine.Worlds.Actors
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct ActorStateHeader
    {
        public VoidPtr Address { get { fixed (void* ptr = &this) return (VoidPtr)ptr; } }
    }
}

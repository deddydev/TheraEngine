using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Worlds.Actors
{
    public class ActorState
    {

    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct ActorStateHeader
    {
        public VoidPtr Address { get { fixed (void* ptr = &this) return (VoidPtr)ptr; } }
    }
}

using System.ComponentModel;
using System.Runtime.InteropServices;
using TheraEngine.Core.Memory;

namespace {0}
@
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct {1} {2}
    @   
        public static readonly int Size = sizeof({1});
        
        
        
        [Browsable(false)]
        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
    #
#

using System;
using System.Runtime.InteropServices;
using System.Text;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Memory;

namespace TheraEngine.ThirdParty.PMX
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct TextureArrayHeader
    {
        //Textures "toon01.bmp" through to "toon10.bmp" are reserved.

        public int _count;
        public StringValue* Textures => (StringValue*)(Address + 4);

        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }

        public int GetSize()
        {
            int size = 4;
            StringValue* value = Textures;
            for (int i = 0; i < _count; ++i, ++value)
                size += value->TotalLength;
            return size;
        }
    }
}

using System;
using System.Runtime.InteropServices;
using System.Text;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Memory;

namespace TheraEngine.ThirdParty.PMX
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct TriangleArrayHeader
    {
        //TODO: is the vertex count _count * 3?
        //Or is this literally the vertex count, not face count
        public int _count;

        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
        public uint GetVertexIndex(int index, byte vertexIndexSize)
        {
            switch (vertexIndexSize)
            {
                case 1:
                    return *(byte*)(Address + index);
                case 2:
                    return *(ushort*)(Address + index * 2);
                default:
                case 4:
                    return *(uint*)(Address + index * 4);
            }
        }
        public UVec3 GetTriangleVertexIndices(int index, byte vertexIndexSize)
        {
            int vtx1 = index * 3;
            int vtx2 = vtx1 + 1;
            int vtx3 = vtx2 + 1;
            switch (vertexIndexSize)
            {
                case 1:
                    return new UVec3(
                        *(byte*)(Address + vtx1),
                        *(byte*)(Address + vtx2),
                        *(byte*)(Address + vtx3));
                case 2:
                    return new UVec3(
                        *(ushort*)(Address + vtx1 * 2),
                        *(ushort*)(Address + vtx2 * 2),
                        *(ushort*)(Address + vtx3 * 2));
                default:
                case 4:
                    return new UVec3(
                        *(uint*)(Address + vtx1 * 4),
                        *(uint*)(Address + vtx2 * 4),
                        *(uint*)(Address + vtx3 * 4));
            }
        }
        public int GetSize(byte vertexIndexSize) => 4 + vertexIndexSize * _count * 3;
    }
}

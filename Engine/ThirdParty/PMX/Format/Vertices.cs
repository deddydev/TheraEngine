using System;
using System.Runtime.InteropServices;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Memory;

namespace TheraEngine.ThirdParty.PMX
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct VertexArrayHeader
    {
        public int _count;
        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
        public VertexHeader* Vertices => (VertexHeader*)(Address + 4);

        public int GetSize(byte extraVec4Count, byte boneIndexSize)
        {
            int size = 4;
            VertexHeader* vtx = Vertices;
            for (int i = 0; i < _count; ++i, ++vtx)
                size += vtx->GetSize(extraVec4Count, boneIndexSize);
            return size;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct VertexHeader
    {
        public Vec3 _position;
        public Vec3 _normal;
        public Vec2 _uv;
        
        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
        public Vec4* ExtraVec4Data => (Vec4*)(Address + Vec3.Size * 2 + Vec2.Size);
        public WeightHeader* WeightData(byte extraVec4Count) => (WeightHeader*)(Address + Vec3.Size * 2 + Vec2.Size + Vec4.Size * extraVec4Count);
        public float EdgeScale(byte extraVec4Count, byte boneIndexSize)
        {
            WeightHeader* weightHeader = WeightData(extraVec4Count);
            return *(float*)((VoidPtr)weightHeader + weightHeader->GetSize(boneIndexSize));
        }
        public int GetSize(byte extraVec4Count, byte boneIndexSize)
        {
            int size = Vec3.Size * 2 + Vec2.Size + Vec4.Size * extraVec4Count;
            WeightHeader* weightHeader = (WeightHeader*)(Address + size);
            size += weightHeader->GetSize(boneIndexSize) + 4; // + 4 for edge scale float
            return size;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct WeightHeader
        {
            public enum EWeightType : byte
            {
                BDef1 = 0,
                BDef2 = 1,
                BDef4 = 2,
                SDef = 3,
                QDef = 4,
            }

            public EWeightType _type;
            public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
            public BDef1* BoneDef1 => (BDef1*)(Address + 1);
            public BDef2* BoneDef2 => (BDef2*)(Address + 1);
            public BDef4QDef* BoneDef4QuatDef => (BDef4QDef*)(Address + 1);
            public SDef* SphericalDef => (SDef*)(Address + 1);

            public int GetSize(byte boneIndexSize)
            {
                switch (_type)
                {
                    default:
                    case EWeightType.BDef1:
                        return 1 + BoneDef1->GetSize(boneIndexSize);
                    case EWeightType.BDef2:
                        return 1 + BoneDef2->GetSize(boneIndexSize);
                    case EWeightType.BDef4:
                    case EWeightType.QDef:
                        return 1 + BoneDef4QuatDef->GetSize(boneIndexSize);
                    case EWeightType.SDef:
                        return 1 + SphericalDef->GetSize(boneIndexSize);
                }
            }
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public unsafe struct BDef1
            {
                public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
                public int GetBoneIndex(byte boneIndexSize)
                {
                    return boneIndexSize switch
                    {
                        2 => *(short*)Address,
                        4 => *(int*)Address,
                        _ => *(sbyte*)Address,
                    };
                }
                public int GetSize(byte boneIndexSize) => boneIndexSize;
            }
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public unsafe struct BDef2
            {
                public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
                public int GetBoneIndex1(byte boneIndexSize)
                {
                    return boneIndexSize switch
                    {
                        2 => *(short*)Address,
                        4 => *(int*)Address,
                        _ => *(sbyte*)Address,
                    };
                }
                public int GetBoneIndex2(byte boneIndexSize)
                {
                    return boneIndexSize switch
                    {
                        2 => *(short*)(Address + boneIndexSize),
                        4 => *(int*)(Address + boneIndexSize),
                        _ => *(sbyte*)(Address + boneIndexSize),
                    };
                }
                public float Bone1Weight(byte boneIndexSize) => *(float*)(Address + boneIndexSize * 2);
                public float Bone2Weight(byte boneIndexSize) => 1.0f - Bone1Weight(boneIndexSize);
                public int GetSize(byte boneIndexSize) => boneIndexSize * 2 + 4;
            }
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public unsafe struct BDef4QDef
            {
                public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
                public int GetBoneIndex1(byte boneIndexSize)
                {
                    return boneIndexSize switch
                    {
                        2 => *(short*)Address,
                        4 => *(int*)Address,
                        _ => *(sbyte*)Address,
                    };
                }
                public int GetBoneIndex2(byte boneIndexSize)
                {
                    return boneIndexSize switch
                    {
                        2 => *(short*)(Address + boneIndexSize),
                        4 => *(int*)(Address + boneIndexSize),
                        _ => *(sbyte*)(Address + boneIndexSize),
                    };
                }
                public int GetBoneIndex3(byte boneIndexSize)
                {
                    return boneIndexSize switch
                    {
                        2 => *(short*)(Address + boneIndexSize * 2),
                        4 => *(int*)(Address + boneIndexSize * 2),
                        _ => *(sbyte*)(Address + boneIndexSize * 2),
                    };
                }
                public int GetBoneIndex4(byte boneIndexSize)
                {
                    return boneIndexSize switch
                    {
                        2 => *(short*)(Address + boneIndexSize * 3),
                        4 => *(int*)(Address + boneIndexSize * 3),
                        _ => *(sbyte*)(Address + boneIndexSize * 3),
                    };
                }
                public Vec4 GetWeights(byte boneIndexSize) => *(Vec4*)(Address + boneIndexSize * 4);
                public int GetSize(byte boneIndexSize) => boneIndexSize * 4 + 16;
            }
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public unsafe struct SDef
            {
                public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
                public int GetBoneIndex1(byte boneIndexSize)
                {
                    return boneIndexSize switch
                    {
                        2 => *(short*)Address,
                        4 => *(int*)Address,
                        _ => *(sbyte*)Address,
                    };
                }
                public int GetBoneIndex2(byte boneIndexSize)
                {
                    return boneIndexSize switch
                    {
                        2 => *(short*)(Address + boneIndexSize),
                        4 => *(int*)(Address + boneIndexSize),
                        _ => *(sbyte*)(Address + boneIndexSize),
                    };
                }
                public float Bone1Weight(byte boneIndexSize) => *(float*)(Address + boneIndexSize * 2);
                public float Bone2Weight(byte boneIndexSize) => 1.0f - Bone1Weight(boneIndexSize);
                public Vec3 GetC(byte boneIndexSize) => *(Vec3*)(Address + boneIndexSize * 2 + 4);
                public Vec3 GetR0(byte boneIndexSize) => *(Vec3*)(Address + boneIndexSize * 2 + 16);
                public Vec3 GetR1(byte boneIndexSize) => *(Vec3*)(Address + boneIndexSize * 2 + 28);
                public int GetSize(byte boneIndexSize) => boneIndexSize * 2 + 40;
            }
        }
    }
}

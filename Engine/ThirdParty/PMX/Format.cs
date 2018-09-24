using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Memory;

namespace TheraEngine.ThirdParty.PMX
{
    public enum EStringEncoding
    {
        UTF16LE = 0,
        UTF8 = 1,
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct StringValue
    {
        public int _length;

        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
        public byte* StringAddress => (byte*)(Address + 4);
        public int TotalLength => 4 + _length;
        public string GetString(EStringEncoding encoding)
        {
            Encoding f;
            if (encoding == EStringEncoding.UTF8)
                f = Encoding.UTF8;
            else
                f = Encoding.Unicode;
            return f.GetString(StringAddress, _length);
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Header
    {
        public fixed byte _magic[4]; //PMX 
        public float _version; //2.0, 2.1
        public byte _globalsCount; //8
        
        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
        public byte* Globals => (byte*)(Address + 9);

        public EStringEncoding StringEncoding => (EStringEncoding)(Globals[0]);
        public byte ExtraVec4Count => Globals[1]; //0-4
        public byte VertexIndexSize => Globals[2];
        public byte TextureIndexSize => Globals[3];
        public byte MaterialIndexSize => Globals[4];
        public byte BoneIndexSize => Globals[5];
        public byte MorphIndexSize => Globals[6];
        public byte RigidBodyIndexSize => Globals[7];

        public StringValue* ModelNameJPStringValue => (StringValue*)(Address + 9 + _globalsCount);
        public StringValue* ModelNameENStringValue => (StringValue*)(Address + 9 + _globalsCount + ModelNameJPStringValue->TotalLength);
        public StringValue* ModelCommentsJPStringValue => (StringValue*)(Address + 9 + _globalsCount + ModelNameJPStringValue->TotalLength + ModelNameENStringValue->TotalLength);
        public StringValue* ModelCommentsENStringValue => (StringValue*)(Address + 9 + _globalsCount + ModelNameJPStringValue->TotalLength + ModelNameENStringValue->TotalLength + ModelCommentsJPStringValue->TotalLength);

        public string ModelNameJP => ModelNameJPStringValue->GetString(StringEncoding);
        public string ModelNameEN => ModelNameENStringValue->GetString(StringEncoding);
        public string ModelCommentsJP => ModelCommentsJPStringValue->GetString(StringEncoding);
        public string ModelCommentsEN => ModelCommentsENStringValue->GetString(StringEncoding);
    }
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
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct WeightHeader
    {
        public enum EWeightType : byte
        {
            BDef1 = 0,
            BDef2 = 1,
            BDef4 = 2,
            SDef  = 3,
            QDef  = 4,
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
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct BDef1
    {
        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
        public int GetBoneIndex(byte boneIndexSize)
        {
            switch (boneIndexSize)
            {
                default:
                case 1:
                    return *(sbyte*)Address;
                case 2:
                    return *(short*)Address;
                case 4:
                    return *(int*)Address;
            }
        }
        public int GetSize(byte boneIndexSize) => boneIndexSize;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct BDef2
    {
        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
        public int GetBoneIndex1(byte boneIndexSize)
        {
            switch (boneIndexSize)
            {
                default:
                case 1:
                    return *(sbyte*)Address;
                case 2:
                    return *(short*)Address;
                case 4:
                    return *(int*)Address;
            }
        }
        public int GetBoneIndex2(byte boneIndexSize)
        {
            switch (boneIndexSize)
            {
                default:
                case 1:
                    return *(sbyte*)(Address + boneIndexSize);
                case 2:
                    return *(short*)(Address + boneIndexSize);
                case 4:
                    return *(int*)(Address + boneIndexSize);
            }
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
            switch (boneIndexSize)
            {
                default:
                case 1:
                    return *(sbyte*)Address;
                case 2:
                    return *(short*)Address;
                case 4:
                    return *(int*)Address;
            }
        }
        public int GetBoneIndex2(byte boneIndexSize)
        {
            switch (boneIndexSize)
            {
                default:
                case 1:
                    return *(sbyte*)(Address + boneIndexSize);
                case 2:
                    return *(short*)(Address + boneIndexSize);
                case 4:
                    return *(int*)(Address + boneIndexSize);
            }
        }
        public int GetBoneIndex3(byte boneIndexSize)
        {
            switch (boneIndexSize)
            {
                default:
                case 1:
                    return *(sbyte*)(Address + boneIndexSize * 2);
                case 2:
                    return *(short*)(Address + boneIndexSize * 2);
                case 4:
                    return *(int*)(Address + boneIndexSize * 2);
            }
        }
        public int GetBoneIndex4(byte boneIndexSize)
        {
            switch (boneIndexSize)
            {
                default:
                case 1:
                    return *(sbyte*)(Address + boneIndexSize * 3);
                case 2:
                    return *(short*)(Address + boneIndexSize * 3);
                case 4:
                    return *(int*)(Address + boneIndexSize * 3);
            }
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
            switch (boneIndexSize)
            {
                default:
                case 1:
                    return *(sbyte*)Address;
                case 2:
                    return *(short*)Address;
                case 4:
                    return *(int*)Address;
            }
        }
        public int GetBoneIndex2(byte boneIndexSize)
        {
            switch (boneIndexSize)
            {
                default:
                case 1:
                    return *(sbyte*)(Address + boneIndexSize);
                case 2:
                    return *(short*)(Address + boneIndexSize);
                case 4:
                    return *(int*)(Address + boneIndexSize);
            }
        }
        public float Bone1Weight(byte boneIndexSize) => *(float*)(Address + boneIndexSize * 2);
        public float Bone2Weight(byte boneIndexSize) => 1.0f - Bone1Weight(boneIndexSize);
        public Vec3 GetC(byte boneIndexSize) => *(Vec3*)(Address + boneIndexSize * 2 + 4);
        public Vec3 GetR0(byte boneIndexSize) => *(Vec3*)(Address + boneIndexSize * 2 + 16);
        public Vec3 GetR1(byte boneIndexSize) => *(Vec3*)(Address + boneIndexSize * 2 + 28);
        public int GetSize(byte boneIndexSize) => boneIndexSize * 2 + 40;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct FaceArrayHeader
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
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct TextureHeader
    {
        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }

        public int GetSize()
        {
            int size = 0;
            return size;
        }
    }
    ////Textures "toon01.bmp" through to "toon10.bmp" are reserved.
    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    //public unsafe struct TextureArrayHeader
    //{
    //    public int _count;
    //    public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    //    public TextureHeader* Textures => (TextureHeader*)(Address + 4);

    //    public int GetSize(byte extraVec4Count, byte boneIndexSize)
    //    {
    //        int size = 4;
    //        TextureHeader* tex = Textures;
    //        for (int i = 0; i < _count; ++i, ++tex)
    //            size += tex->GetSize();
    //        return size;
    //    }
    //}
    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    //public unsafe struct TextureHeader
    //{
    //    public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }

    //    public int GetSize()
    //    {
    //        int size = 0;
    //        return size;
    //    }
    //}
}

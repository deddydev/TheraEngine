using System.Runtime.InteropServices;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Memory;

namespace TheraEngine.ThirdParty.PMX
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MaterialArrayHeader
    {
        public int _count;
        public MaterialHeader* Materials => (MaterialHeader*)(Address + 4);
        
        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }

        public int GetSize(byte textureIndexSize)
        {
            int size = 4;
            MaterialHeader* value = Materials;
            for (int i = 0; i < _count; ++i, ++value)
                size += value->GetSize(textureIndexSize);
            return size;
        }
        
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct MaterialHeader
        {
            public StringValue* NameJP => (StringValue*)Address;
            public StringValue* NameEN => (StringValue*)(Address + NameJP->TotalLength);
            public MaterialPart1* Part1 => (MaterialPart1*)((VoidPtr)NameEN + NameEN->TotalLength);
            public MaterialPart2* GetPart2(byte textureIndexSize) => (MaterialPart2*)((VoidPtr)Part1 + Part1->GetSize(textureIndexSize));

            public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }

            public int GetSize(byte textureIndexSize)
            {
                int size = 0;
                size += NameJP->TotalLength;
                size += NameEN->TotalLength;
                size += Part1->GetSize(textureIndexSize);
                size += GetPart2(textureIndexSize)->GetSize(textureIndexSize);
                return size;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public unsafe struct MaterialPart1
            {
                public const int Size = 0x41;

                public Vec4 _diffuseColor;
                public Vec3 _specularColor;
                public float _specularStrength;
                public Vec3 _ambientColor;
                public byte _drawingFlags;
                public Vec4 _edgeColor;
                public float _edgeScale;

                public bool DisableBackfaceCulling
                {
                    get => (_drawingFlags & 0x1) != 0;
                    set
                    {
                        if (value)
                            _drawingFlags |= 0x1;
                        else
                            _drawingFlags &= (0xFF - 0x1);
                    }
                }
                public bool HasGroundShadow
                {
                    get => (_drawingFlags & 0x2) != 0;
                    set
                    {
                        if (value)
                            _drawingFlags |= 0x2;
                        else
                            _drawingFlags &= (0xFF - 0x2);
                    }
                }
                public bool DrawsShadows
                {
                    get => (_drawingFlags & 0x4) != 0;
                    set
                    {
                        if (value)
                            _drawingFlags |= 0x4;
                        else
                            _drawingFlags &= (0xFF - 0x4);
                    }
                }
                public bool RecievesShadows
                {
                    get => (_drawingFlags & 0x8) != 0;
                    set
                    {
                        if (value)
                            _drawingFlags |= 0x8;
                        else
                            _drawingFlags &= (0xFF - 0x8);
                    }
                }
                public bool HasEdge
                {
                    get => (_drawingFlags & 0x10) != 0;
                    set
                    {
                        if (value)
                            _drawingFlags |= 0x10;
                        else
                            _drawingFlags &= (0xFF - 0x10);
                    }
                }
                public bool HasVertexColor
                {
                    get => (_drawingFlags & 0x20) != 0;
                    set
                    {
                        if (value)
                            _drawingFlags |= 0x20;
                        else
                            _drawingFlags &= (0xFF - 0x20);
                    }
                }
                public bool DrawPoints
                {
                    get => (_drawingFlags & 0x40) != 0;
                    set
                    {
                        if (value)
                            _drawingFlags |= 0x40;
                        else
                            _drawingFlags &= (0xFF - 0x40);
                    }
                }
                public bool DrawLines
                {
                    get => (_drawingFlags & 0x80) != 0;
                    set
                    {
                        if (value)
                            _drawingFlags |= 0x80;
                        else
                            _drawingFlags &= (0xFF - 0x80);
                    }
                }
                public int GetTexIndex(byte textureIndexSize)
                {
                    VoidPtr offset = Address + Size;
                    switch (textureIndexSize)
                    {
                        default:
                        case 1: return *(byte*)offset;
                        case 2: return *(short*)offset;
                        case 4: return *(int*)offset;
                    }
                }
                public int GetTexEnvIndex(byte textureIndexSize)
                {
                    VoidPtr offset = Address + Size + textureIndexSize;
                    switch (textureIndexSize)
                    {
                        default:
                        case 1: return *(byte*)offset;
                        case 2: return *(short*)offset;
                        case 4: return *(int*)offset;
                    }
                }
                public int GetSize(byte textureIndexSize)
                {
                    return Size + textureIndexSize * 2;
                }
                public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
            }
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public unsafe struct MaterialPart2
            {
                public const int Size = 2;
                public enum EToonTexType : byte
                {
                    External = 0,
                    Internal = 1,
                }
                public enum EEnvBlendMode : byte
                {
                    Disabled = 0,
                    Multiply = 1,
                    Additive = 2,
                    ExtraVec4 = 3
                }
                public EEnvBlendMode _envBlendMode;
                public EToonTexType _toonTexType;

                public int GetToonTexIndex(byte textureIndexSize)
                {
                    VoidPtr offset = Address + Size;
                    switch (textureIndexSize)
                    {
                        default:
                        case 1: return *(byte*)offset;
                        case 2: return *(short*)offset;
                        case 4: return *(int*)offset;
                    }
                }
                public string GetMetaData(byte textureIndexSize, EStringEncoding encoding)
                    => GetMetaDataPtr(textureIndexSize)->GetString(encoding);
                public StringValue* GetMetaDataPtr(byte textureIndexSize)
                    => (StringValue*)(Address + Size + textureIndexSize);
                public int TriCount(byte textureIndexSize)
                {
                    StringValue* meta = GetMetaDataPtr(textureIndexSize);
                    return *(int*)((VoidPtr)meta + meta->TotalLength);
                }
                public int GetSize(byte textureIndexSize)
                {
                    StringValue* meta = GetMetaDataPtr(textureIndexSize);
                    return (VoidPtr)meta + meta->TotalLength + 4 - Address;
                }
                public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
            }
        }
    }
}

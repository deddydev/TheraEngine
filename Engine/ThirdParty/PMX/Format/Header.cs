using System.Runtime.InteropServices;
using TheraEngine.Core.Memory;

namespace TheraEngine.ThirdParty.PMX
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Header
    {
        public fixed byte _magic[4]; //PMX 
        public float _version; //2.0, 2.1

        public byte _globalsCount; //8
        public byte _textType; //0/1, 0 = utf16, 1 = utf8
        public byte _addUVCount; //0-4

        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
        public byte* Globals => (byte*)(Address + 11);

        public EStringEncoding StringEncoding 
        {
            get => (EStringEncoding)_textType;
            set => _textType = (byte)value;
        }

        public byte ExtraVec4Count { get => Globals[1]; set => Globals[1] = value; } //0-4
        public byte VertexIndexSize { get => Globals[2]; set => Globals[2] = value; }
        public byte TextureIndexSize { get => Globals[3]; set => Globals[3] = value; }
        public byte MaterialIndexSize { get => Globals[4]; set => Globals[4] = value; }
        public byte BoneIndexSize { get => Globals[5]; set => Globals[5] = value; }
        public byte MorphIndexSize { get => Globals[6]; set => Globals[6] = value; }
        public byte RigidBodyIndexSize { get => Globals[7]; set => Globals[7] = value; }

        public StringValue* ModelNameJPStringValue => (StringValue*)(Address + 9 + _globalsCount);
        public StringValue* ModelNameENStringValue => (StringValue*)((VoidPtr)ModelNameJPStringValue + ModelNameJPStringValue->TotalLength);
        public StringValue* ModelCommentsJPStringValue => (StringValue*)((VoidPtr)ModelNameENStringValue + ModelNameENStringValue->TotalLength);
        public StringValue* ModelCommentsENStringValue => (StringValue*)((VoidPtr)ModelCommentsJPStringValue + ModelCommentsJPStringValue->TotalLength);

        public string ModelNameJP => ModelNameJPStringValue->GetString(StringEncoding);
        public string ModelNameEN => ModelNameENStringValue->GetString(StringEncoding);
        public string ModelCommentsJP => ModelCommentsJPStringValue->GetString(StringEncoding);
        public string ModelCommentsEN => ModelCommentsENStringValue->GetString(StringEncoding);

        public VertexArrayHeader* VertexHeader => (VertexArrayHeader*)((VoidPtr)ModelCommentsENStringValue + ModelCommentsENStringValue->TotalLength);
        public TriangleArrayHeader* TrianglesHeader => (TriangleArrayHeader*)((VoidPtr)VertexHeader + VertexHeader->GetSize(ExtraVec4Count, BoneIndexSize));
        public TextureArrayHeader* TexturesHeader => (TextureArrayHeader*)((VoidPtr)TrianglesHeader + TrianglesHeader->GetSize(VertexIndexSize));
        public MaterialArrayHeader* MaterialsHeader => (MaterialArrayHeader*)((VoidPtr)TexturesHeader + TexturesHeader->GetSize());
    }
}

using System.ComponentModel;

namespace TheraEngine.Rendering.Models
{
    public class VertexShaderDesc
    {
        public static readonly int MaxMorphs = 0;
        public static readonly int MaxColors = 2;
        public static readonly int MaxTexCoords = 8;
        public static readonly int MaxOtherBuffers = 10;
        public static readonly int TotalBufferCount = (MaxMorphs + 1) * 6 + MaxColors + MaxTexCoords + MaxOtherBuffers;

        [TSerialize("MorphCount", IsXmlAttribute = true)]
        public int _morphCount = 0;
        [TSerialize("TexCoordCount", IsXmlAttribute = true)]
        public int _texcoordCount = 0;
        [TSerialize("ColorCount", IsXmlAttribute = true)]
        public int _colorCount = 0;
        [TSerialize("BoneCount", IsXmlAttribute = true)]
        public int _boneCount = 0;
        [TSerialize("HasNormals", IsXmlAttribute = true)]
        public bool _hasNormals = false;
        [TSerialize("HasBinormals", IsXmlAttribute = true)]
        public bool _hasBinormals = false;
        [TSerialize("HasTangents", IsXmlAttribute = true)]
        public bool _hasTangents = false;

        //Note: if there's only one bone, we can just multiply the model matrix by the bone's frame matrix. No need for weighting.
        public bool IsWeighted => _boneCount > 1;
        public bool IsSingleBound => _boneCount == 1;
        public bool HasSkinning => _boneCount > 0;

        public bool HasNormals => _hasNormals;
        public bool HasBinormals => _hasBinormals;
        public bool HasTangents => _hasTangents;
        public bool HasTexCoords => _texcoordCount > 0;
        public bool HasColors => _colorCount > 0;

        public VertexShaderDesc() { }

        public static VertexShaderDesc PosColor(int colorCount = 1)
        {
            return new VertexShaderDesc() { _colorCount = colorCount };
        }
        public static VertexShaderDesc PosTex(int texCoordCount = 1)
        {
            return new VertexShaderDesc() { _texcoordCount = texCoordCount };
        }
        public static VertexShaderDesc PosNormTex(int texCoordCount = 1)
        {
            return new VertexShaderDesc() { _texcoordCount = texCoordCount, _hasNormals = true };
        }
        public static VertexShaderDesc JustPositions()
        {
            return new VertexShaderDesc();
        }
    }
}

using System;
using System.ComponentModel;

namespace TheraEngine.Rendering.Models
{
    public class VertexShaderDesc : TObjectSlim
    {
        public static readonly int MaxMorphs = 0;
        public static readonly int MaxColors = 2;
        public static readonly int MaxTexCoords = 8;
        public static readonly int MaxOtherBuffers = 10;
        public static readonly int TotalBufferCount = (MaxMorphs + 1) * 6 + MaxColors + MaxTexCoords + MaxOtherBuffers;

        public event Action Changed;

        [TSerialize(nameof(MorphCount), IsAttribute = true)]
        private int _morphCount = 0;
        [TSerialize(nameof(TexcoordCount), IsAttribute = true)]
        private int _texcoordCount = 0;
        [TSerialize(nameof(ColorCount), IsAttribute = true)]
        private int _colorCount = 0;
        [TSerialize(nameof(BoneCount), IsAttribute = true)]
        private int _boneCount = 0;
        [TSerialize(nameof(HasNormals), IsAttribute = true)]
        private bool _hasNormals = false;
        [TSerialize(nameof(HasBinormals), IsAttribute = true)]
        private bool _hasBinormals = false;
        [TSerialize(nameof(HasTangents), IsAttribute = true)]
        private bool _hasTangents = false;

        //Note: if there's only one bone, we can just multiply the model matrix by the bone's frame matrix. No need for weighting.
        public bool IsWeighted => BoneCount > 1;
        public bool IsSingleBound => BoneCount == 1;
        public bool HasSkinning => BoneCount > 0;
        public bool HasTexCoords => TexcoordCount > 0;
        public bool HasColors => ColorCount > 0;

        private ETransformFlags _billboardMode = ETransformFlags.None;
        public ETransformFlags BillboardMode
        {
            get => _billboardMode;
            set
            {
                _billboardMode = value;
                Changed?.Invoke();
            }
        }

        public int MorphCount
        {
            get => _morphCount;
            set => _morphCount = value;
        }
        public int TexcoordCount 
        {
            get => _texcoordCount;
            set => _texcoordCount = value;
        }
        public int ColorCount
        {
            get => _colorCount;
            set => _colorCount = value;
        }
        public int BoneCount
        {
            get => _boneCount;
            set => _boneCount = value;
        }
        public bool HasNormals
        {
            get => _hasNormals;
            set => _hasNormals = value; 
        }
        public bool HasBinormals
        {
            get => _hasBinormals;
            set => _hasBinormals = value;
        }
        public bool HasTangents 
        {
            get => _hasTangents;
            set => _hasTangents = value;
        }

        public VertexShaderDesc() { }

        public static VertexShaderDesc PosColor(int colorCount = 1)
        {
            return new VertexShaderDesc() { ColorCount = colorCount };
        }
        public static VertexShaderDesc PosTex(int texCoordCount = 1)
        {
            return new VertexShaderDesc() { TexcoordCount = texCoordCount };
        }
        public static VertexShaderDesc PosNormTex(int texCoordCount = 1)
        {
            return new VertexShaderDesc() { TexcoordCount = texCoordCount, HasNormals = true };
        }
        public static VertexShaderDesc PosNorm()
        {
            return new VertexShaderDesc() { HasNormals = true };
        }
        public static VertexShaderDesc JustPositions()
        {
            return new VertexShaderDesc();
        }
    }
}

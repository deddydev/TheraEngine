using System.ComponentModel;
using System.Threading.Tasks;
using TheraEngine.Files;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Rendering.Models.Materials
{
    public enum ETextureType
    {
        Tex1D,
        Tex2D,
        Tex3D,
        TexCube,
        Tex2DRect,
        Tex1DArray,
        Tex2DArray,
        TexCubeArray,
        TexBuffer,
        Tex2DMultisample,
        Tex2DMultisampleArray,
    }
    public abstract class BaseTexRef : TFileObject, IFrameBufferAttachement
    {
        [TSerialize]
        public EFramebufferAttachment? FrameBufferAttachment { get; set; }

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public int Index { get; set; }

        public T GetRenderTextureGeneric<T>(bool loadSynchronously) where T : BaseRenderTexture
            => GetRenderTextureGeneric(loadSynchronously) as T;
        /// <summary>
        /// Converts this texture reference into a texture made for rendering.
        /// </summary>
        public abstract BaseRenderTexture GetRenderTextureGeneric(bool loadSynchronously);
        [Browsable(false)]
        public BaseRenderTexture RenderTextureGeneric => GetRenderTextureGeneric(true);

        [TSerialize(nameof(SamplerName), IsXmlAttribute = true)]
        private string _samplerName = null;
        public string SamplerName
        {
            get => _samplerName ?? "Texture" + Index;
            set => _samplerName = value;
        }

        /// <summary>
        /// Converts this texture reference into a texture made for rendering.
        /// </summary>
        public abstract Task<BaseRenderTexture> GetTextureGenericAsync();
        public abstract void AttachToFBO(EFramebufferTarget target, int mipLevel = 0);
        public abstract void DetachFromFBO(EFramebufferTarget target, int mipLevel = 0);
        public abstract void AttachToFBO(EFramebufferTarget target, EFramebufferAttachment attachment, int mipLevel = 0);
        public abstract void DetachFromFBO(EFramebufferTarget target, EFramebufferAttachment attachment, int mipLevel = 0);

        public static BaseTexRef CreateTexRef(ETextureType type)
        {
            switch (type)
            {
                case ETextureType.Tex1D:
                    return null;
                case ETextureType.Tex2D:
                    return new TexRef2D();
                case ETextureType.Tex3D:
                    return new TexRef3D();
                case ETextureType.TexCube:
                    return new TexRefCube();
                case ETextureType.Tex2DRect:
                    return null;
                case ETextureType.Tex1DArray:
                    return null;
                case ETextureType.Tex2DArray:
                    return null;
                case ETextureType.TexCubeArray:
                    return null;
                case ETextureType.TexBuffer:
                    return null;
                case ETextureType.Tex2DMultisample:
                    return null;
                case ETextureType.Tex2DMultisampleArray:
                    return null;
                default:
                    return null;

            }
        }
    }
}

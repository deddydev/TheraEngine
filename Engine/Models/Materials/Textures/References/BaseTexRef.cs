using System.ComponentModel;
using System.Threading.Tasks;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Files;
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
        [DisplayName("Texture Name")]
        [Category("Texture Reference")]
        public override string Name { get => base.Name; set => base.Name = value; }

        [Category("Texture Reference")]
        [TSerialize]
        public EFramebufferAttachment? FrameBufferAttachment { get; set; }
        
        public T GetRenderTextureGeneric<T>(bool loadSynchronously) where T : BaseRenderTexture
            => GetRenderTextureGeneric(loadSynchronously) as T;
        /// <summary>
        /// Converts this texture reference into a texture made for rendering.
        /// </summary>
        public abstract BaseRenderTexture GetRenderTextureGeneric(bool loadSynchronously);
        [Browsable(false)]
        public BaseRenderTexture RenderTextureGeneric => GetRenderTextureGeneric(true);

        /// <summary>
        /// This is the name the texture will use to bind to in the shader.
        /// If <see langword="null"/>, empty or whitespace, uses Texture# as the sampler name, where # is the texture's index in the material.
        /// </summary>
        [TSerialize(IsAttribute = true)]
        [Category("Texture Reference")]
        public string SamplerName { get; set; } = null;

        /// <summary>
        /// Converts this texture reference into a texture made for rendering.
        /// </summary>
        public abstract Task<BaseRenderTexture> GetRenderTextureGenericAsync();
        public abstract void AttachToFBO(EFramebufferTarget target, int mipLevel = 0);
        public abstract void DetachFromFBO(EFramebufferTarget target, int mipLevel = 0);
        public abstract void AttachToFBO(EFramebufferTarget target, EFramebufferAttachment attachment, int mipLevel = 0);
        public abstract void DetachFromFBO(EFramebufferTarget target, EFramebufferAttachment attachment, int mipLevel = 0);

        public static BaseTexRef CreateTexRef(ETextureType type)
        {
            return type switch
            {
                ETextureType.Tex1D => null, //return new TexRef1D();
                ETextureType.Tex2D => new TexRef2D(),
                ETextureType.Tex3D => new TexRef3D(),
                ETextureType.TexCube => new TexRefCube(),
                ETextureType.Tex2DRect => new TexRef2D() { Rectangle = true },
                ETextureType.Tex1DArray => null,
                ETextureType.Tex2DArray => null,
                ETextureType.TexCubeArray => null,
                ETextureType.TexBuffer => null,
                ETextureType.Tex2DMultisample => new TexRef2D() { MultiSample = true },
                ETextureType.Tex2DMultisampleArray => null,
                _ => null,
            };
        }

        /// <summary>
        /// Returns the sampler name for this texture to bind into the shader.
        /// </summary>
        /// <param name="textureIndex">The index of the texture. Only used if the override parameter and the SamplerName property are null or invalid.</param>
        /// <param name="samplerNameOverride">The binding name to force bind to, if desired.</param>
        /// <returns></returns>
        public string ResolveSamplerName(int textureIndex, string samplerNameOverride = null)
            => samplerNameOverride ?? SamplerName ?? ($"Texture{textureIndex.ToString()}");

        /// <summary>
        /// Passes this texture sampler into the fragment shader of the given program by name.
        /// </summary>
        public void SampleIn(RenderProgram program, int textureUnit)
            => program.Sampler(SamplerName, RenderTextureGeneric, textureUnit);
    }
}

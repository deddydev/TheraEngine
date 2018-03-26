using TheraEngine.Files;
using System.ComponentModel;
using System.Threading.Tasks;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Rendering.Models.Materials
{
    public abstract class BaseTexRef : TFileObject
    {
        private int _index;
        private EFramebufferAttachment? _frameBufferAttachment;

        public TMaterial Material { get; internal set; }

        [TSerialize]
        public EFramebufferAttachment? FrameBufferAttachment
        {
            get => _frameBufferAttachment;
            set => _frameBufferAttachment = value;
        }
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public int Index
        {
            get => _index;
            set => _index = value;
        }

        public T GetTextureGeneric<T>(bool loadSynchronously = false) where T : BaseRenderTexture
            => GetTextureGeneric(loadSynchronously) as T;
        /// <summary>
        /// Converts this texture reference into a texture made for rendering.
        /// </summary>
        public abstract BaseRenderTexture GetTextureGeneric(bool loadSynchronously = false);
        /// <summary>
        /// Converts this texture reference into a texture made for rendering.
        /// </summary>
        public abstract Task<BaseRenderTexture> GetTextureGenericAsync();
        internal abstract void AttachToFBO();
        public abstract void AttachToFBO(EFramebufferAttachment attachment, int mipLevel = 0);
    }
}

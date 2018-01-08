using TheraEngine.Files;
using TheraEngine.Rendering.Textures;
using System.Drawing.Imaging;
using System.Drawing;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Rendering.Models.Materials
{
    public abstract class BaseTexRef : FileObject
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

        /// <summary>
        /// Converts this texture reference into a texture made for rendering.
        /// </summary>
        public abstract BaseRenderTexture GetTextureGeneric(bool loadSynchronously = false);
        /// <summary>
        /// Converts this texture reference into a texture made for rendering.
        /// </summary>
        public abstract Task<BaseRenderTexture> GetTextureGenericAsync();
    }
}

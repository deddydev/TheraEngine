using System;

namespace TheraEngine.Rendering.Models.Materials.Textures
{
    public class RenderTexCube : BaseRenderTexture
    {
        public RenderTexCube() : this(null) { }
        public RenderTexCube(int bindingId) : base(bindingId) => Mipmaps = null;
        public RenderTexCube(params RenderCubeMipmap[] mipmaps) : base() => Mipmaps = mipmaps;
        public RenderTexCube(
            EPixelInternalFormat internalFormat,
            EPixelFormat pixelFormat,
            EPixelType pixelType,
            params RenderCubeMipmap[] mipmaps)
            : this(mipmaps)
        {
            InternalFormat = internalFormat;
            PixelFormat = pixelFormat;
            PixelType = pixelType;
        }
        public RenderTexCube(int bindingId, params RenderCubeMipmap[] mipmaps) 
            : base(bindingId) => Mipmaps = mipmaps;
        /// <summary>
        /// Initializes the texture as an unallocated texture to be filled by a framebuffer.
        /// </summary>
        public RenderTexCube(
            int cubeExtent,
            EPixelInternalFormat internalFormat,
            EPixelFormat pixelFormat,
            EPixelType pixelType) 
            : this(null)
        {
            InternalFormat = internalFormat;
            PixelFormat = pixelFormat;
            PixelType = pixelType;
            _mipmaps = null;
        }
        
        private RenderCubeMipmap[] _mipmaps;

        public override ETexTarget TextureTarget => ETexTarget.TextureCubeMap;

        public RenderCubeMipmap[] Mipmaps
        {
            get => _mipmaps;
            set => _mipmaps = value;
        }
        
        public static RenderTexCube[] GenTextures(int count)
            => Engine.Renderer.CreateObjects<RenderTexCube>(EObjectType.Texture, count);
        
        public override void PushData()
        {
            if (BaseRenderPanel.NeedsInvoke(PushData, BaseRenderPanel.PanelType.Rendering))
                return;
            
            Bind();
            OnPrePushData(out bool shouldPush, out bool allowPostPushCallback);

            if (_mipmaps == null || _mipmaps.Length == 0)
                Engine.Renderer.PushTextureData(TextureTarget, 0, InternalFormat, 1, 1, PixelFormat, PixelType, IntPtr.Zero);
            else
                for (int i = 0; i < _mipmaps.Length; ++i)
                    _mipmaps[i].PushData(i);
            
            Engine.Renderer.TexParameter(TextureTarget, ETexParamName.TextureBaseLevel, 0);
            Engine.Renderer.TexParameter(TextureTarget, ETexParamName.TextureMaxLevel, _mipmaps == null ? 0 : _mipmaps.Length - 1);
            Engine.Renderer.TexParameter(TextureTarget, ETexParamName.TextureMinLod, 0);
            Engine.Renderer.TexParameter(TextureTarget, ETexParamName.TextureMaxLod, _mipmaps == null ? 0 : _mipmaps.Length - 1);

            OnPostPushData();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Destroy();
                }

                if (_mipmaps != null)
                    Array.ForEach(_mipmaps, x => x.Dispose());

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposedValue = true;
            }
        }
    }
}

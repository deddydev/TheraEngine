using System;
using System.ComponentModel;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Rendering.Models.Materials
{
    [FileExt("trefv2d")]
    [FileDef("2D Texture Reference View")]
    public class TexRefView2D : TexRef2D
    {
        private TexRef2D _viewedTexture;
        private int _minLevel;
        private int _numLevels;
        private int _minLayer;
        private int _numLayers;

        public TexRefView2D(TexRef2D viewedTexture, int minLevel, int numLevels, int minLayer, int numLayers, EPixelType type, EPixelFormat format, EPixelInternalFormat internalFormat)
        {
            _viewedTexture = viewedTexture;
            _minLevel = minLevel;
            _numLevels = numLevels;
            _minLayer = minLayer;
            _numLayers = numLayers;
            PixelType = type;
            PixelFormat = format;
            InternalFormat = internalFormat;
            _width = viewedTexture.Width;
            _height = viewedTexture.Height;
            MinFilter = viewedTexture.MinFilter;
            MagFilter = viewedTexture.MagFilter;
        }
        
        protected override void CreateRenderTexture()
        {
            base.CreateRenderTexture();
            _texture.Generated += _texture_Generated;
            _texture.PrePushData += _texture_PrePushData1;
            _texture.PostPushData += _texture_PostPushData;
        }

        private void _texture_PostPushData()
        {

        }

        private void _texture_PrePushData1(PrePushDataCallback callback)
        {
            callback.ShouldPush = false;
        }

        protected override void SetParameters()
        {

        }

        private void _texture_Generated()
        {
            BaseRenderTexture vtex = _viewedTexture.GetRenderTextureGeneric(true);
            Engine.Renderer.TextureView(
                _texture.BindingId, ETexTarget.Texture2D, vtex.BindingId, InternalFormat,
                _minLevel, _numLevels, _minLayer, _numLayers);

            _texture.Bind();
            int dsmode = DepthStencilFormat == EDepthStencilFmt.Stencil ? 6401 : 6402;
            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.DepthStencilTextureMode, dsmode);
            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.TextureLodBias, LodBias);
            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.TextureMagFilter, (int)MagFilter);
            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.TextureMinFilter, (int)MinFilter);
            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.TextureWrapS, (int)UWrap);
            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.TextureWrapT, (int)VWrap);
        }
    }
}

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
        }

        private void _texture_Generated()
        {
            Engine.Renderer.CheckErrors();
            //OpenTK.Graphics.OpenGL.GL.TexStorage2D(OpenTK.Graphics.OpenGL.TextureTarget2d.Texture2D, 0, 
            //    OpenTK.Graphics.OpenGL.SizedInternalFormat.r, _viewedTexture.Width, _viewedTexture.Height);
            Engine.Renderer.CheckErrors();
            BaseRenderTexture vtex = _viewedTexture.GetTextureGeneric(true);
            Engine.Renderer.CheckErrors();
            Engine.Renderer.TextureView(
                _texture.BindingId, ETexTarget.Texture2D, vtex.BindingId, InternalFormat,
                _minLevel, _numLevels, _minLayer, _numLayers);
            Engine.Renderer.CheckErrors();
        }
    }
}

using TheraEngine.Files;
using TheraEngine.Rendering.Textures;
using System.Drawing.Imaging;
using System.Drawing;
using System.ComponentModel;
using System.Threading.Tasks;
using System;
using TheraEngine.Rendering.Models.Materials.Textures;
using System.Drawing.Drawing2D;

namespace TheraEngine.Rendering.Models.Materials
{
    public class CubeMipmap
    {
        public bool IsCrossMap => Sides.Length == 1;

        public RefCubeSide[] Sides { get; private set; }
        
        public CubeMipmap(GlobalFileRef<TextureFile2D> crossMap)
            => Sides = new RefCubeSide[1] { new RefCubeSideTextured(crossMap) };

        public CubeMipmap(
            RefCubeSide posX, RefCubeSide negX,
            RefCubeSide posY, RefCubeSide negY,
            RefCubeSide posZ, RefCubeSide negZ)
            => Sides = new RefCubeSide[6] { posX, negX, posY, negY, posZ, negZ };

        public CubeMipmap(int width, int height, EPixelInternalFormat internalFormat, EPixelFormat format, EPixelType type)
        {
            Sides = new RefCubeSide[6];
            Sides.FillWith(i => new RefCubeSideBlank(width, height, internalFormat, format, type));
        }

        public CubeMipmap(int width, int height, PixelFormat bitmapFormat)
        {
            Sides = new RefCubeSide[6];
            Sides.FillWith(i => new RefCubeSideTextured(width, height, bitmapFormat));
        }

        public void FillRenderMap(RenderCubeMipmap mip, int mipIndex)
        {
            if (mip == null)
                return;
            if (IsCrossMap)
            {
                RefCubeSideTextured crossTex = ((RefCubeSideTextured)Sides[0]);
                TextureFile2D tex = crossTex.Map.GetInstance();
                //Task.Run(() => crossTex.Map.GetInstance()).ContinueWith(x =>
                {
                    Bitmap[] maps = tex.Bitmaps;//x.Result.Bitmaps;
                    if (maps != null)
                    {
                        if (mipIndex >= maps.Length)
                        {
                            Bitmap bmp = maps[maps.Length - 1];
                            int w = bmp.Width;
                            int h = bmp.Height;
                            for (int i = maps.Length; i <= mipIndex; ++i)
                            {
                                w /= 2;
                                h /= 2;
                            }
                            bmp = bmp.Resized(w, h, InterpolationMode.HighQualityBicubic); //TODO: use NearestNeighbor instead?
                        }
                        if (!mip.SetCrossCubeMap(maps[mipIndex]))
                            Engine.LogWarning("Cubemap cross dimensions are invalid; width/height be a 4:3 or 3:4 ratio.");
                    }
                }//);
            }
            else
            {
                mip.SetSides(
                    Sides[0].AsRenderSide(mipIndex),
                    Sides[1].AsRenderSide(mipIndex),
                    Sides[2].AsRenderSide(mipIndex),
                    Sides[3].AsRenderSide(mipIndex),
                    Sides[4].AsRenderSide(mipIndex),
                    Sides[5].AsRenderSide(mipIndex));
            }
        }
        public RenderCubeMipmap AsRenderMipmap(int mipIndex)
        {
            RenderCubeMipmap mip = new RenderCubeMipmap();
            FillRenderMap(mip, mipIndex);
            return mip;
        }
    }

    [FileExt("trefcube")]
    [FileDef("Cubemap Texture Reference")]
    public class TexRefCube : BaseTexRef
    {
        #region Constructors
        public TexRefCube() : this(null, 1) { }
        public TexRefCube(string name, int dim)
        {
            Mipmaps = null;
            _name = name;
            _cubeExtent = dim;
        }
        public TexRefCube(
            string name,
            int dim,
            PixelFormat bitmapFormat = PixelFormat.Format32bppArgb,
            int mipCount = 1)
            : this(name, dim)
        {
            int sDim = dim;
            Mipmaps = new CubeMipmap[mipCount];
            for (int i = 0, scale = 1; i < mipCount; scale = 1 << ++i, sDim = dim / scale)
                Mipmaps[i] = new CubeMipmap(sDim, sDim, bitmapFormat);
        }
        public TexRefCube(
            string name,
            int dim,
            EPixelInternalFormat internalFormat,
            EPixelFormat pixelFormat,
            EPixelType pixelType,
            int mipCount = 1)
            : this(name, dim)
        {
            int sDim = dim;
            Mipmaps = new CubeMipmap[mipCount];
            for (int i = 0, scale = 1; i < mipCount; scale = 1 << ++i, sDim = dim / scale)
                Mipmaps[i] = new CubeMipmap(sDim, sDim, internalFormat, pixelFormat, pixelType);
        }
        public TexRefCube(string name, int dim, params CubeMipmap[] mipmaps)
        {
            _name = name;
            Mipmaps = mipmaps;
        }
        #endregion
        
        [TSerialize]
        public CubeMipmap[] Mipmaps { get; set; }
        
        private RenderTexCube _texture;

        [TSerialize("CubeExtent")]
        private int _cubeExtent;
        
        private ETexWrapMode _uWrapMode = ETexWrapMode.ClampToEdge;
        private ETexWrapMode _vWrapMode = ETexWrapMode.ClampToEdge;
        private ETexWrapMode _wWrapMode = ETexWrapMode.ClampToEdge;
        private ETexMinFilter _minFilter = ETexMinFilter.Nearest;
        private ETexMagFilter _magFilter = ETexMagFilter.Nearest;
        private float _lodBias = 0.0f;

        [TSerialize]
        public ETexMagFilter MagFilter
        {
            get => _magFilter;
            set => _magFilter = value;
        }
        [TSerialize]
        public ETexMinFilter MinFilter
        {
            get => _minFilter;
            set => _minFilter = value;
        }
        [TSerialize]
        public ETexWrapMode UWrap
        {
            get => _uWrapMode;
            set => _uWrapMode = value;
        }
        [TSerialize]
        public ETexWrapMode VWrap
        {
            get => _vWrapMode;
            set => _vWrapMode = value;
        }
        [TSerialize]
        public ETexWrapMode WWrap
        {
            get => _wWrapMode;
            set => _wWrapMode = value;
        }
        [TSerialize]
        public float LodBias
        {
            get => _lodBias;
            set => _lodBias = value;
        }
        public int CubeExtent => _cubeExtent;

        private void SetParameters()
        {
            if (_texture == null)
                return;

            _texture.Bind();

            Engine.Renderer.TexParameter(ETexTarget.TextureCubeMap, ETexParamName.TextureLodBias, _lodBias);
            Engine.Renderer.TexParameter(ETexTarget.TextureCubeMap, ETexParamName.TextureMagFilter, (int)_magFilter);
            Engine.Renderer.TexParameter(ETexTarget.TextureCubeMap, ETexParamName.TextureMinFilter, (int)_minFilter);
            Engine.Renderer.TexParameter(ETexTarget.TextureCubeMap, ETexParamName.TextureWrapS, (int)_uWrapMode);
            Engine.Renderer.TexParameter(ETexTarget.TextureCubeMap, ETexParamName.TextureWrapT, (int)_vWrapMode);
            Engine.Renderer.TexParameter(ETexTarget.TextureCubeMap, ETexParamName.TextureWrapR, (int)_wWrapMode);
            //AttachToFBO();
            //if (FrameBufferAttachment.HasValue && Material != null && Material.HasAttachment(FrameBufferAttachment.Value))
            //    OpenTK.Graphics.OpenGL.GL.FramebufferTexture(
            //        OpenTK.Graphics.OpenGL.FramebufferTarget.Framebuffer,
            //        OpenTK.Graphics.OpenGL.FramebufferAttachment.DepthAttachment,
            //        _texture.BindingId, 0);
            //for (int i = 0; i < Mipmaps.Length; ++i)
            //    for (int x = 0; x < 6; ++x)
            //        Engine.Renderer.AttachTextureToFrameBuffer(EFramebufferTarget.Framebuffer,
            //            FrameBufferAttachment.Value, ETexTarget.TextureCubeMapPositiveX + x, _texture.BindingId, i);
        }

        internal override void AttachToFBO()
        {
            if (FrameBufferAttachment.HasValue && Material != null && Material.HasAttachment(FrameBufferAttachment.Value))
                OpenTK.Graphics.OpenGL.GL.FramebufferTexture(
                    OpenTK.Graphics.OpenGL.FramebufferTarget.Framebuffer,
                    OpenTK.Graphics.OpenGL.FramebufferAttachment.DepthAttachment,
                    _texture.BindingId, 0);
        }

        private bool _isLoading = false;
        public async Task<RenderTexCube> GetTextureAsync()
        {
            if (_texture != null)
                return _texture;

            if (!_isLoading)
            {
                CreateRenderTexture();
                await Task.Run((Action)LoadMipmaps);
            }

            return _texture;
        }
        public RenderTexCube GetTexture(bool loadSynchronously = false)
        {
            if (_texture != null)
                return _texture;

            if (!_isLoading)
            {
                CreateRenderTexture();
                if (loadSynchronously)
                    LoadMipmaps();
                else
                    Task.Run(() => LoadMipmaps());
            }

            return _texture;
        }
        
        public override BaseRenderTexture GetTextureGeneric(bool loadSynchronously = false) => GetTexture(loadSynchronously);
        public override async Task<BaseRenderTexture> GetTextureGenericAsync() => await GetTextureAsync();

        //public bool ResizingDisabled { get; internal set; }

        ///// <summary>
        ///// Resizes the textures stored in memory.
        ///// </summary>
        //public void Resize(int cubeExtent)
        //{
        //    if (ResizingDisabled)
        //        return;

        //    _cubeExtent = cubeExtent;

        //    if (_isLoading)
        //        return;
            
        //    GetTexture(true)?.Resize(_cubeExtent);
        //}

        public bool IsLoaded => _texture != null;

        /// <summary>
        /// Call if you want to load all mipmap texture files, in a background thread for example.
        /// </summary>
        public void LoadMipmaps()
        {
            _isLoading = true;
            if (Mipmaps != null && Mipmaps.Length > 0)
            {
                _texture.Mipmaps = new RenderCubeMipmap[Mipmaps.Length];
                //Task.Run(() => Parallel.For(0, Mipmaps.Length, i =>
                for (int i = 0; i < Mipmaps.Length; ++i)
                    _texture.Mipmaps[i] = Mipmaps[i].AsRenderMipmap(i);
            }
            
            _isLoading = false;
        }
        private void CreateRenderTexture()
        {
            if (_texture != null)
                _texture.PostPushData -= SetParameters;
            _texture = new RenderTexCube();
            _texture.PostPushData += SetParameters;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Files;
using TheraEngine.Rendering.Textures;

namespace TheraEngine.Rendering.Models.Materials.Textures
{
    public abstract class RefCubeSide
    {
        public abstract RenderCubeSide AsRenderSide(int mipIndex);
    }
    public class RefCubeSideBlank : RefCubeSide
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public EPixelFormat PixelFormat { get; private set; }
        public EPixelType PixelType { get; private set; }
        public EPixelInternalFormat InternalFormat { get; private set; }
        
        public RefCubeSideBlank(int width, int height, EPixelInternalFormat internalFormat, EPixelFormat format, EPixelType type)
        {
            InternalFormat = internalFormat;
            PixelFormat = format;
            PixelType = type;
            Width = width;
            Height = height;
        }

        public override RenderCubeSide AsRenderSide(int mipIndex)
        {
            return new RenderCubeSide(Width, Height, InternalFormat, PixelFormat, PixelType);
        }
    }
    public class RefCubeSideTextured : RefCubeSide
    {
        public GlobalFileRef<TextureFile2D> Map { get; private set; }

        public RefCubeSideTextured(GlobalFileRef<TextureFile2D> map)
        {
            Map = map;
        }

        public RefCubeSideTextured(int width, int height, PixelFormat format)
        {
            Map = new TextureFile2D(width, height, format);
        }

        public override RenderCubeSide AsRenderSide(int mipIndex)
        {
            return new RenderCubeSide(Map.File.Bitmaps[mipIndex.Clamp(0, Map.File.Bitmaps.Length - 1)]);
        }

        public static implicit operator RefCubeSideTextured(GlobalFileRef<TextureFile2D> map)
            => new RefCubeSideTextured(map);
    }
}

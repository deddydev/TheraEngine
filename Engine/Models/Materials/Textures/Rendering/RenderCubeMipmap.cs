using Extensions;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace TheraEngine.Rendering.Models.Materials.Textures
{
    public enum ECubemapFace
    {
        PosX,
        NegX,
        PosY,
        NegY,
        PosZ,
        NegZ,
    }
    public class RenderCubeMipmap : IDisposable
    {
        public RenderCubeSide[] Sides { get; private set; } = new RenderCubeSide[6];

        public RenderCubeMipmap()
        {

        }
        public RenderCubeMipmap(Bitmap cubeCrossBmp, bool isFillerBitmap = false)
        {
            if (isFillerBitmap)
                SetSides(cubeCrossBmp);
            else if (!SetCrossCubeMap(cubeCrossBmp))
                throw new InvalidOperationException("Cubemap cross dimensions are invalid; width/height be a 4:3 or 3:4 ratio.");
        }

        public RenderCubeMipmap(
            RenderCubeSide posX, RenderCubeSide negX,
            RenderCubeSide posY, RenderCubeSide negY,
            RenderCubeSide posZ, RenderCubeSide negZ)
            => Sides = new RenderCubeSide[6] { posX, negX, posY, negY, posZ, negZ };
        
        public RenderCubeMipmap(int dim, PixelFormat bitmapFormat)
            => SetSides(dim, bitmapFormat);
        public RenderCubeMipmap(int dim, EPixelInternalFormat internalFormat, EPixelFormat format, EPixelType type)
        {
            Sides.FillWith(i => new RenderCubeSide(dim, dim, internalFormat, format, type));
        }

        public bool SetCrossCubeMap(Bitmap cubeCrossBmp)
        {
            int w = cubeCrossBmp.Width;
            int h = cubeCrossBmp.Height;
            if (w % 4 == 0 && w / 4 * 3 == h)
            {
                //Cross is on its side.
                //     __
                //  __|__|__ __        +Y
                // |__|__|__|__|   -X, -Z, +X, +Z
                //    |__|             -Y

                int dim = w / 4;
                Sides = new RenderCubeSide[]
                {
                    cubeCrossBmp.Clone(new Rectangle(dim * 2, dim, dim, dim), cubeCrossBmp.PixelFormat),
                    cubeCrossBmp.Clone(new Rectangle(0, dim, dim, dim), cubeCrossBmp.PixelFormat),
                    cubeCrossBmp.Clone(new Rectangle(dim, 0, dim, dim), cubeCrossBmp.PixelFormat),
                    cubeCrossBmp.Clone(new Rectangle(dim, dim * 2, dim, dim), cubeCrossBmp.PixelFormat),
                    cubeCrossBmp.Clone(new Rectangle(dim * 3, dim, dim, dim), cubeCrossBmp.PixelFormat),
                    cubeCrossBmp.Clone(new Rectangle(dim, dim, dim, dim), cubeCrossBmp.PixelFormat),
                };
                //Sides[0].Map.Save("posx.png");
                //Sides[1].Map.Save("negx.png");
                //Sides[2].Map.Save("posy.png");
                //Sides[3].Map.Save("negy.png");
                //Sides[4].Map.Save("posz.png");
                //Sides[5].Map.Save("negz.png");
                return true;
            }
            else if (h % 4 == 0 && h / 4 * 3 == w)
            {
                //Cross is standing up.
                //     __
                //  __|__|__        +Y
                // |__|__|__|   -X, -Z, +X
                //    |__|          -Y
                //    |__|          +Z

                int dim = h / 4;
                Sides = new RenderCubeSide[]
                {
                    cubeCrossBmp.Clone(new Rectangle(dim * 2, dim, dim, dim), cubeCrossBmp.PixelFormat),
                    cubeCrossBmp.Clone(new Rectangle(0, dim, dim, dim), cubeCrossBmp.PixelFormat),
                    cubeCrossBmp.Clone(new Rectangle(dim, 0, dim, dim), cubeCrossBmp.PixelFormat),
                    cubeCrossBmp.Clone(new Rectangle(dim, dim * 2, dim, dim), cubeCrossBmp.PixelFormat),
                    cubeCrossBmp.Clone(new Rectangle(dim, dim * 3, dim, dim), cubeCrossBmp.PixelFormat),
                    cubeCrossBmp.Clone(new Rectangle(dim, dim, dim, dim), cubeCrossBmp.PixelFormat),
                };
                //Sides[0].Map.Save("posx.png");
                //Sides[1].Map.Save("negx.png");
                //Sides[2].Map.Save("posy.png");
                //Sides[3].Map.Save("negy.png");
                //Sides[4].Map.Save("posz.png");
                //Sides[5].Map.Save("negz.png");
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetSides(
            RenderCubeSide posX, RenderCubeSide negX,
            RenderCubeSide posY, RenderCubeSide negY,
            RenderCubeSide posZ, RenderCubeSide negZ)
            => Sides = new RenderCubeSide[6] { posX, negX, posY, negY, posZ, negZ };

        public void SetSides(Bitmap bmp)
        {
            //TODO: Determine if clones of the bitmap are necessary or not
            Sides.FillWith(bmp);
            //Sides.FillWith(i => cubeCrossBmp.Clone());
        }
        public void SetSides(int dim, PixelFormat bitmapFormat)
        {
            Sides.FillWith(i => new Bitmap(dim, dim, bitmapFormat));
        }
        public void SetSides(int dim, EPixelInternalFormat internalFormat, EPixelFormat format, EPixelType type)
        {
            Sides.FillWith(i => new RenderCubeSide(dim, dim, internalFormat, format, type));
        }
        //public void PushData(int mipIndex)
        //{
        //    for (int i = 0; i < Sides.Length; ++i)
        //        Sides[i].PushData(i, mipIndex);
        //}

        public void Dispose()
        {
            foreach (RenderCubeSide b in Sides)
                b.Dispose();
        }
    }
}

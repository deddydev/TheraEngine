using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Extensions
{
    public static class BitmapExtension
    {
        public static Bitmap Resize(this Bitmap i, int width, int height, InterpolationMode mode = InterpolationMode.HighQualityBicubic)
        {
            Bitmap r = new Bitmap(width, height);
            r.SetResolution(i.HorizontalResolution, i.VerticalResolution);
            using (Graphics graphics = Graphics.FromImage(r))
            {
                graphics.InterpolationMode = mode;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                using (ImageAttributes wrapMode = new ImageAttributes())
                {
                    //This fixes the antialiasing on the edges of the image
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(i, new Rectangle(0, 0, r.Width, r.Height), 0, 0, i.Width, i.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return r;
        }

        public static Bitmap Copy(this Bitmap sourceImage)
        {
            if (sourceImage.Palette.Entries.Length > 0)
            {
                //Indexed
                Bitmap targetImage = new Bitmap(sourceImage.Width, sourceImage.Height,
                  sourceImage.PixelFormat);
                BitmapData sourceData = sourceImage.LockBits(
                  new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                  ImageLockMode.ReadOnly, sourceImage.PixelFormat);
                BitmapData targetData = targetImage.LockBits(
                  new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                  ImageLockMode.WriteOnly, targetImage.PixelFormat);
                Memory.Move(targetData.Scan0, sourceData.Scan0, (uint)sourceData.Stride * (uint)sourceData.Height);
                sourceImage.UnlockBits(sourceData);
                targetImage.UnlockBits(targetData);
                targetImage.Palette = sourceImage.Palette;
                return targetImage;
            }
            else
            {
                //Non-indexed
                Bitmap targetImage = new Bitmap(sourceImage.Width, sourceImage.Height, sourceImage.PixelFormat);
                targetImage.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
                using (Graphics g = Graphics.FromImage(targetImage))
                    g.DrawImageUnscaled(sourceImage, 0, 0);
                return targetImage;
            }
        }
        public static unsafe Bitmap GenerateMip(this Bitmap bmp, int level)
        {
            if (level <= 1)
                return (Bitmap)bmp.Clone();

            int scale = 1 << (level - 1);
            int w = bmp.Width / scale, h = bmp.Height / scale;

            Bitmap dst = new Bitmap(w, h, bmp.PixelFormat);

            //Step-scale indexed elements
            if (bmp.IsIndexed())
            {
                BitmapData srcData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
                BitmapData dstData = dst.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, bmp.PixelFormat);

                float xStep = (float)bmp.Width / w;
                float yStep = (float)bmp.Height / h;
                int x, y;
                float fx, fy;

                byte* sPtr, dPtr = (byte*)dstData.Scan0;
                if (bmp.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    for (y = 0, fy = 0.5f; y < h; y++, fy += yStep, dPtr += dstData.Stride)
                    {
                        sPtr = (byte*)srcData.Scan0 + ((int)fy * srcData.Stride);
                        for (x = 0, fx = 0.5f; x < w; x++, fx += xStep)
                            dPtr[x] = sPtr[(int)fx];
                    }
                }
                else
                {
                    for (y = 0, fy = 0.5f; y < h; y++, fy += yStep, dPtr += dstData.Stride)
                    {
                        sPtr = (byte*)srcData.Scan0 + ((int)fy * srcData.Stride);
                        int b = 0, ind;
                        for (x = 0, fx = 0.5f; x < w; x++, fx += xStep)
                        {
                            ind = (int)fx;
                            if ((x & 1) == 0)
                            {
                                if ((ind & 1) == 0)
                                    b = sPtr[ind >> 1] & 0xF0;
                                else
                                    b = sPtr[ind >> 1] << 4;
                            }
                            else
                            {
                                if ((ind & 1) == 0)
                                    b |= sPtr[ind >> 1] >> 4;
                                else
                                    b |= sPtr[ind >> 1] & 0xF;
                                dPtr[x >> 1] = (byte)b;
                            }
                        }
                        if ((x & 1) != 0)
                            dPtr[x >> 1] = (byte)b;
                    }

                }

                bmp.UnlockBits(srcData);
                dst.UnlockBits(dstData);
            }
            else
            {
                using (Graphics g = Graphics.FromImage(dst))
                {
                    g.CompositingMode = CompositingMode.SourceCopy;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    g.DrawImage(bmp, new Rectangle(0, 0, w, h));
                }
            }
            return dst;
        }
        public static bool IsIndexed(this Bitmap bmp) { return (bmp.PixelFormat & PixelFormat.Indexed) != 0; }
    }
}

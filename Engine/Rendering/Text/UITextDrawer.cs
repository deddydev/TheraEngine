using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Text
{
    public delegate void DelTextRedraw(bool forceFullRedraw);
    public class TextDrawer
    {
        public class TextSort : IComparer<UIString2D>
        {
            int IComparer<UIString2D>.Compare(UIString2D x, UIString2D y)
            {
                if (x.Order < y.Order)
                    return -1;

                if (x.Order > y.Order)
                    return 1;

                return -1;
            }
        }

        public event DelTextRedraw NeedsRedraw;

        private TextSort _sorter;
        private HashSet<UIString2D> _text;
        private LinkedList<UIString2D> _modified;

        protected void OnDoRedraw(bool forceFullRedraw) 
            => NeedsRedraw?.Invoke(forceFullRedraw);

        public bool Modified => _modified.Count > 0;

        public TextDrawer()
        {
            _sorter = new TextSort();
            _text = new HashSet<UIString2D>();
            _modified = new LinkedList<UIString2D>();
        }
        
        public void Clear(bool redraw = true)
        {
            _text.Clear();
            _modified.Clear();

            if (redraw)
                OnDoRedraw(true);
        }
        public void Intersect<T>(IEnumerable<T> text, bool redraw = true) where T : UIString2D
        {
            _text.IntersectWith(text);
        }
        public void Add(UIString2D text, bool redraw = true)
        {
            if (text == null)
                return;

            text.Parent = this;
            _text.Add(text);
            _modified.AddLast(text);

            if (redraw)
                OnDoRedraw(false);
        }
        public void Add(bool redraw, params UIString2D[] text) => Add(text, redraw);
        public void Add(IEnumerable<UIString2D> text, bool redraw = true)
        {
            foreach (UIString2D str in text)
                Add(str, false);

            if (redraw)
                OnDoRedraw(false);
        }

        public unsafe void Draw(TexRef2D texture, Vec2 texRes, TextRenderingHint textQuality, bool forceFullRedraw)
        {
            forceFullRedraw = true;

            if (texture == null ||
                texture.Mipmaps == null ||
                texture.Mipmaps.Length == 0)
                return;

            if (!forceFullRedraw && _modified.Count == 0)
                return;

            Bitmap b = texture.Mipmaps[0].File.Bitmaps[0];

            //TODO: instead of redrawing the whole image, keep track of overlapping text
            //and only redraw the previous and new regions. Repeat for any other overlapping texts.
            //Then textsubimage2d using the min and max values of all updated texts.

            //Draw text information onto the bitmap
            using (Graphics g = Graphics.FromImage(b))
            {
                //Set quality modes
                g.TextRenderingHint = textQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                //Get drawing bounds
                RectangleF rect = new RectangleF(0, 0, b.Width, b.Height);

                if (forceFullRedraw)
                {
                    //Reset canvas
                    g.ResetClip();
                    g.Clear(Color.Transparent);

                    foreach (UIString2D text in _text.OrderBy(x => x.Order))
                    {
                        var bounds = text.Region;
                        if (!bounds.DisjointWith(b.Width, b.Height))
                        {
                            PointF pos = bounds.OriginTranslation;

                            g.ResetTransform();
                            g.TranslateTransform(bounds.Translation.X, bounds.Translation.Y);
                            g.RotateTransformAt(text.Rotation, pos);
                            g.ScaleTransformAt(texRes.X, texRes.Y, pos);
                            g.DrawString(text.Text, text.Font, text.Brush, rect, text.Format);
                        }
                    }
                }
                else
                {
                    foreach (UIString2D text in _modified)
                    {
                        var bounds = text.Region;
                        if (!bounds.DisjointWith(b.Width, b.Height))
                        {
                            PointF pos = bounds.OriginTranslation;

                            g.ResetClip();
                            g.SetClip(bounds.AsRectangleF(b.Height));
                            g.Clear(Color.Transparent);

                            g.ResetTransform();
                            g.TranslateTransform(bounds.Translation.X, bounds.Translation.Y);
                            g.RotateTransformAt(text.Rotation, pos);
                            g.ScaleTransformAt(texRes.X, texRes.Y, pos);
                            g.DrawString(text.Text, text.Font, text.Brush, rect, text.Format);
                        }
                    }
                }

                g.Flush();
            }
            
            _modified.Clear();
        }
        internal void TextChanged(UIString2D textData)
        {
            _modified.AddLast(textData);
            OnDoRedraw(false);
        }
    }
}

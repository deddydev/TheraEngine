using Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Text
{
    public delegate void DelTextRedraw(bool forceFullRedraw);
    public class TextRasterizer : TObjectSlim
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

        public event DelTextRedraw Invalidated;

        private TextSort _sorter;
        private ConcurrentDictionary<Guid, UIString2D> _modified;

        protected void OnInvalidated(bool forceFullRedraw) 
            => Invalidated?.Invoke(forceFullRedraw);

        [Browsable(false)]
        public bool Modified => _modified.Count > 0;
        public EventList<UIString2D> Text { get; }

        public TextRasterizer()
        {
            _modified = new ConcurrentDictionary<Guid, UIString2D>();
            _sorter = new TextSort();
            Text = new EventList<UIString2D>();
            Text.PostAdded += Text_PostAdded;
            Text.PostRemoved += Text_PostRemoved;
            Text.PostAddedRange += Text_PostAddedRange;
            Text.PostRemovedRange += Text_PostRemovedRange;
            Text.PostInserted += Text_PostInserted;
            Text.PostInsertedRange += Text_PostInsertedRange;
        }

        private void Text_PostInsertedRange(IEnumerable<UIString2D> items, int index)
        {
            Text_PostAddedRange(items);
        }
        private void Text_PostInserted(UIString2D item, int index)
        {
            Text_PostAdded(item);
        }
        private void Text_PostAddedRange(IEnumerable<UIString2D> items)
        {
            foreach (var item in items)
                if (item != null)
                    item.Parent = this;
            OnInvalidated(true);
        }
        private void Text_PostRemovedRange(IEnumerable<UIString2D> items)
        {
            foreach (var item in items)
                if (item != null && item.Parent == this)
                    item.Parent = null;
            OnInvalidated(true);
        }
        private void Text_PostRemoved(UIString2D item)
        {
            if (item != null && item.Parent == this)
                item.Parent = null;
            OnInvalidated(true);
        }
        private void Text_PostAdded(UIString2D item)
        {
            if (item != null)
                item.Parent = this;
            OnInvalidated(true);
        }

        public void Clear(bool redraw = true)
        {
            Text.Clear();
            _modified.Clear();
            if (redraw)
                OnInvalidated(true);
        }
        //public void Intersect<T>(IEnumerable<T> text, bool redraw = true) where T : UIString2D
        //{
        //    _text.IntersectWith(text);
        //}
        //public void Add(UIString2D text, bool redraw = true)
        //{
        //    if (text is null)
        //        return;

        //    text.Parent = this;
        //    _text.Add(text);
        //    _modified.AddLast(text);

        //    if (redraw)
        //        OnDoRedraw(false);
        //}
        //public void Add(bool redraw, params UIString2D[] text) => Add(text, redraw);
        //public void Add(IEnumerable<UIString2D> text, bool redraw = true)
        //{
        //    foreach (UIString2D str in text)
        //        Add(str, false);

        //    if (redraw)
        //        OnDoRedraw(false);
        //}

        public unsafe void Draw(TexRef2D texture, Vec2 texScale, TextRenderingHint textQuality, bool forceFullRedraw)
        {
            if (texture is null ||
                texture.Mipmaps is null ||
                texture.Mipmaps.Length == 0)
                return;

            if (!forceFullRedraw && _modified.Count == 0)
                return;

            var tex = texture.GetTexture(true);
            Bitmap b = tex.Mipmaps[0];

            try
            {
                //Draw text information onto the bitmap
                using Graphics g = Graphics.FromImage(b);

                //Set quality modes
                g.TextRenderingHint = textQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                //Get drawing bounds
                Rectangle rect = new Rectangle(0, 0, b.Width, b.Height);

                if (forceFullRedraw)
                    DrawFull(texScale, b, g, rect);
                else
                    DrawPartial(texScale, b, g, rect);
                
                g.Flush();
            }
            catch (Exception ex)
            {
                Engine.LogException(ex);
            }

            _modified.Clear();
            tex.Invalidate();
        }

        private unsafe void DrawPartial(Vec2 texScale, Bitmap b, Graphics g, Rectangle rect)
        {
            float scaledWidth = b.Width / texScale.X;
            float scaledHeight = b.Height / texScale.Y;

            //Only redraw modified sections of the full texture
            //TODO: determine regions intersecting with this one.
            //Redraw their unions with the current region.

            //Then textsubimage2d using the min and max values of all updated texts.

            foreach (UIString2D text in _modified.Values)
            {
                //Don't draw strings that aren't in the bounds of the texture
                var bounds = text.Region;
                if (bounds.DisjointWith(scaledWidth, scaledHeight))
                    continue;

                PointF originPos = bounds.OriginTranslation;
                originPos.X *= texScale.X;
                originPos.Y *= texScale.Y;

                RectangleF clipRect = bounds.AsRectangleF(b.Height / texScale.Y);
                clipRect.X *= texScale.X;
                clipRect.Y *= texScale.Y;
                clipRect.Width *= texScale.X;
                clipRect.Height *= texScale.Y;

                g.ResetClip();
                g.SetClip(clipRect);
                g.Clear(Color.Transparent);

                g.ResetTransform();
                g.TranslateTransform(clipRect.X, clipRect.Y);
                g.RotateTransformAt(text.Rotation, originPos);
                g.ScaleTransformAt(texScale.X, texScale.Y, originPos);

                DrawText(g, rect, text);
            }
        }

        private unsafe void DrawFull(Vec2 texScale, Bitmap b, Graphics g, Rectangle rect)
        {
            float scaledWidth = b.Width / texScale.X;
            float scaledHeight = b.Height / texScale.Y;

            //Reset whole canvas
            g.ResetClip();
            g.Clear(Color.Transparent);

            foreach (UIString2D text in Text.OrderBy(x => x.Order))
            {
                //Don't draw strings that aren't in the bounds of the texture
                var bounds = text.Region;
                if (bounds.DisjointWith(scaledWidth, scaledHeight))
                    continue;

                PointF originPos = bounds.OriginTranslation;

                g.ResetTransform();
                g.TranslateTransform(bounds.Translation.X, bounds.Translation.Y);
                g.RotateTransformAt(text.Rotation, originPos);
                g.ScaleTransformAt(texScale.X, texScale.Y, originPos);

                DrawText(g, rect, text);
            }
        }

        //TODO: option for compatible text rendering
        private static unsafe void DrawText(Graphics g, Rectangle rect, UIString2D text)
            => TextRenderer.DrawText(g, text.Text, text.Font, rect, text.TextColor.Color, text.Format);

        internal void TextChanged(UIString2D textData)
        {
            _modified.TryAdd(textData.Guid, textData);
            OnInvalidated(false);
        }
    }
}

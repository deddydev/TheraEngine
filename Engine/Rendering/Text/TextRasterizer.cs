﻿using Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
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

        public event DelTextRedraw NeedsRedraw;

        private TextSort _sorter;
        private LinkedList<UIString2D> _modified;

        protected void OnDoRedraw(bool forceFullRedraw) 
            => NeedsRedraw?.Invoke(forceFullRedraw);

        [Browsable(false)]
        public bool Modified => _modified.Count > 0;
        public EventList<UIString2D> Text { get; }

        public TextRasterizer()
        {
            _sorter = new TextSort();
            Text = new EventList<UIString2D>();
            Text.PostAdded += _text_PostAdded;
            Text.PostRemoved += _text_PostRemoved;
            Text.PostAddedRange += Text_PostAddedRange;
            Text.PostRemovedRange += Text_PostRemovedRange;
            Text.PostInserted += Text_PostInserted;
            Text.PostInsertedRange += Text_PostInsertedRange;
            _modified = new LinkedList<UIString2D>();
        }

        private void Text_PostInsertedRange(IEnumerable<UIString2D> items, int index)
        {
            Text_PostAddedRange(items);
        }
        private void Text_PostInserted(UIString2D item, int index)
        {
            _text_PostAdded(item);
        }
        private void Text_PostAddedRange(IEnumerable<UIString2D> items)
        {
            foreach (var item in items)
                if (item != null)
                    item.Parent = this;
            OnDoRedraw(true);
        }
        private void Text_PostRemovedRange(IEnumerable<UIString2D> items)
        {
            foreach (var item in items)
                if (item != null && item.Parent == this)
                    item.Parent = null;
            OnDoRedraw(true);
        }
        private void _text_PostRemoved(UIString2D item)
        {
            if (item != null && item.Parent == this)
                item.Parent = null;
            OnDoRedraw(true);
        }
        private void _text_PostAdded(UIString2D item)
        {
            if (item != null)
                item.Parent = this;
            OnDoRedraw(true);
        }

        public void Clear(bool redraw = true)
        {
            Text.Clear();
            _modified.Clear();

            if (redraw)
                OnDoRedraw(true);
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
                        //Reset whole canvas
                        g.ResetClip();
                        g.Clear(Color.Transparent);

                        foreach (UIString2D text in Text.OrderBy(x => x.Order))
                        {
                            //Don't draw strings that aren't in the bounds of the texture
                            var bounds = text.Region;
                            if (bounds.DisjointWith(b.Width / texScale.X, b.Height / texScale.Y))
                                continue;

                            PointF originPos = bounds.OriginTranslation;

                            g.ResetTransform();
                            g.TranslateTransform(bounds.Translation.X, bounds.Translation.Y);
                            g.RotateTransformAt(text.Rotation, originPos);
                            g.ScaleTransformAt(texScale.X, texScale.Y, originPos);

                            g.DrawString(text.Text, text.Font, text.Brush, rect, text.Format);
                        }
                    }
                    else
                    {
                        //Only redraw modified sections of the full texture
                        //TODO: determine regions intersecting with this one.
                        //Redraw their unions with the current region.

                        //Then textsubimage2d using the min and max values of all updated texts.

                        foreach (UIString2D text in _modified)
                        {
                            //Don't draw strings that aren't in the bounds of the texture
                            var bounds = text.Region;
                            if (bounds.DisjointWith(b.Width / texScale.X, b.Height / texScale.Y))
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

                            g.DrawString(text.Text, text.Font, text.Brush, rect, text.Format);
                        }
                    }

                    g.Flush();
                }
            }
            catch (Exception ex)
            {
                Engine.LogException(ex);
            }

            _modified.Clear();
            tex.Invalidate();
        }
        internal void TextChanged(UIString2D textData)
        {
            _modified.AddLast(textData);
            OnDoRedraw(false);
        }
    }
}

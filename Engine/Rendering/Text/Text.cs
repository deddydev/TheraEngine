using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Text
{
    public class UIString : TObject
    {
        public UIString() { }

        private string _text = string.Empty;
        private Font _font = new Font("Segoe UI", 9.0f, FontStyle.Regular);
        private Vec2 _position = Vec2.Zero;
        private Vec2 _originPercentages = Vec2.Zero;
        private Vec2 _scale = Vec2.One;
        private int _order = 0;
        private float _rotation = 0.0f;
        private ColorF4 _color = new ColorF4(1.0f);
        private StringFormat _format = new StringFormat();
        private BoundingRectangle _bounds = new BoundingRectangle();

        public BoundingRectangle Bounds => _bounds;
        internal SolidBrush Brush { get; private set; } = new SolidBrush(Color.White);
        internal TextDrawer Parent { get; set; }
        internal List<UIString> Overlapping { get; set; } //Set after being drawn

        /// <summary>
        /// The text string to render.
        /// </summary>
        [Category("UI String")]
        public string Text
        {
            get => _text;
            set
            {
                _text = value ?? string.Empty;
                _bounds.Bounds = TextRenderer.MeasureText(_text, _font);
                Parent?.TextChanged(this);
            }
        }
        /// <summary>
        /// The font to render the text with.
        /// </summary>
        [Category("UI String")]
        public Font Font
        {
            get => _font;
            set
            {
                if (value == null)
                    return;

                _font = value;
                _bounds.Bounds = TextRenderer.MeasureText(_text, _font);
                Parent?.TextChanged(this);
            }
        }
        /// <summary>
        /// The color of the text.
        /// </summary>
        [Category("UI String")]
        public ColorF4 TextColor
        {
            get => _color;
            set
            {
                _color = value;
                Brush = new SolidBrush((Color)_color);
                Parent?.TextChanged(this);
            }
        }
        /// <summary>
        /// The position of the text string relative to the origin set with OriginPercentages.
        /// </summary>
        [Category("UI String")]
        public Vec2 Position
        {
            get => _bounds.OriginTranslation;
            set
            {
                _bounds.OriginTranslation = value;
                Parent?.TextChanged(this);
            }
        }
        /// <summary>
        /// Where the origin of the text string is. 0,0 is bottom left, 1,1 is top right. Default is 0,0.
        /// </summary>
        [Category("UI String")]
        public Vec2 OriginPercentages
        {
            get => _bounds.LocalOriginPercentage;
            set
            {
                _bounds.LocalOriginPercentage = value;
                Parent?.TextChanged(this);
            }
        }
        /// <summary>
        /// How big the text is. Default is 1,1.
        /// </summary>
        [Category("UI String")]
        public Vec2 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                Parent?.TextChanged(this);
            }
        }
        /// <summary>
        /// The priority this text should render with. 
        /// A value of 0 is drawn first and higher values are drawn after.
        /// </summary>
        [Category("UI String")]
        public int Order
        {
            get => _order;
            set
            {
                _order = value;
                Parent?.TextChanged(this);
            }
        }
        /// <summary>
        /// The rotation in degrees of the text, where positive means counter-clockwise.
        /// </summary>
        [Category("UI String")]
        public float Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                Parent?.TextChanged(this);
            }
        }
        /// <summary>
        /// Quality and alignment settings.
        /// </summary>
        [Category("UI String")]
        public StringFormat Format
        {
            get => _format;
            set
            {
                _format = value;
                Parent?.TextChanged(this);
            }
        }
    }
    public delegate void DelTextRedraw(bool forceFullRedraw);
    public class TextDrawer
    {
        private SortedDictionary<int, UIString> _text = new SortedDictionary<int, UIString>();
        private LinkedList<UIString> _modified = new LinkedList<UIString>();
        public event DelTextRedraw NeedsRedraw;

        protected void OnDoRedraw(bool forceFullRedraw) 
            => NeedsRedraw?.Invoke(forceFullRedraw);

        public bool Modified => _modified.Count > 0;

        public TextDrawer() { }
        
        public void Clear(bool redraw = true)
        {
            _text.Clear();
            _modified.Clear();

            if (redraw)
                OnDoRedraw(true);
        }

        public void Add(UIString text, bool redraw = true)
        {
            if (text == null)
                return;

            text.Parent = this;
            _text.Add(text.Order, text);
            _modified.AddLast(text);

            if (redraw)
                OnDoRedraw(false);
        }
        public void Add(bool redraw, params UIString[] text) => Add(text, redraw);
        public void Add(IEnumerable<UIString> text, bool redraw = true)
        {
            foreach (UIString str in text)
                Add(str, false);

            if (redraw)
                OnDoRedraw(false);
        }

        public unsafe void Draw(TexRef2D texture, Vec2 texRes, TextRenderingHint textQuality, bool forceFullRedraw)
        {
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

                    foreach (UIString text in _text.Values)
                    {
                        BoundingRectangle bounds = text.Bounds;
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
                    foreach (UIString text in _modified)
                    {
                        BoundingRectangle bounds = text.Bounds;
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
        internal void TextChanged(UIString textData)
        {
            _modified.AddLast(textData);
            OnDoRedraw(false);
        }
    }
}

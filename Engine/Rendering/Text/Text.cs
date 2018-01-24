using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials.Textures;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Text
{
    public class TextData
    {
        /// <summary>
        /// Constructs a new text data class.
        /// </summary>
        /// <param name="text">The text string to render.</param>
        /// <param name="font">The font to render it with.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="position">The 2D location of the text relative to the bottom left (0, 0).</param>
        /// <param name="originPercentages">The relative position of the origin of the text from the bottom left, as a UV-style coordinate (0 is left/bottom, 1 is right/top).</param>
        /// <param name="depth">The angle to render this text at.</param>
        /// <param name="scale">The scale of the text.</param>
        /// <param name="depth">The order to render this text in. 0.0 is first, 1.0 is last.</param>
        public TextData(string text, Font font, ColorF4 color, Vec2 position, Vec2 originPercentages, float rotation, Vec2 scale, float depth)
        {
            _text = text;
            _font = font;
            _brush = new SolidBrush((Color)color);
            _position = position;
            _originPercentages = originPercentages;
            _scale = scale;
            _depth = depth;
            _rotation = rotation;
        }

        private string _text;
        private Font _font;
        private Vec2 _position, _originPercentages;
        private Vec2 _scale;
        private float _depth;
        private float _rotation;
        private ColorF4 _color;

        internal TextDrawer _parent;
        internal BoundingRectangle _bounds; //Set after being drawn
        internal List<TextData> _overlapping; //Set after being drawn
        internal SolidBrush _brush;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                _parent?.TextChanged(this);
            }
        }
        public Font Font
        {
            get => _font;
            set
            {
                _font = value;
                _parent?.TextChanged(this);
            }
        }
        public ColorF4 Color
        {
            get => _color;
            set
            {
                _color = value;
                _brush = new SolidBrush((Color)_color);
                _parent?.TextChanged(this);
            }
        }
        public Vec2 Position
        {
            get => _position;
            set
            {
                _position = value;
                _parent?.TextChanged(this);
            }
        }
        public Vec2 OriginPercentages
        {
            get => _originPercentages;
            set
            {
                _originPercentages = value;
                _parent?.TextChanged(this);
            }
        }
        public Vec2 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                _parent?.TextChanged(this);
            }
        }
        public float Depth
        {
            get => _depth;
            set
            {
                _depth = value;
                _parent?.TextChanged(this);
            }
        }

        public float Rotation { get => _rotation; set => _rotation = value; }
    }

    public class TextDrawer
    {
        internal SortedDictionary<float, TextData> _text;
        private LinkedList<TextData> _modified;
        public event Action NeedsRedraw;

        public bool Modified => _modified.Count > 0;

        public TextDrawer()
        {
            _text = new SortedDictionary<float, TextData>();
            _modified = new LinkedList<TextData>();
        }

        public void Clear()
        {
            _text.Clear();
            _modified.Clear();
        }

        public void Add(TextData text)
        {
            text._parent = this;
            _text.Add(text.Depth, text);
            _modified.AddLast(text);
        }

        public unsafe void Draw(RenderTex2D texture)
        {
            Bitmap b = texture.Mipmaps[0];
            b.MakeTransparent();

            //TODO: instead of redrawing the whole image, keep track of overlapping text
            //and only redraw the previous and new regions. Repeat for any other overlapping texts.
            //Then textsubimage2d using the min and max values of all updated texts.

            //Draw text information onto the bitmap
            using (Graphics g = Graphics.FromImage(b))
            {
                g.Clear(Color.Transparent);
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.SmoothingMode = SmoothingMode.HighQuality;
                foreach (TextData text in _text.Values)
                {
                    Vec2 size = g.MeasureString(text.Text, text.Font);
                    Vec2 localOrigin = size * text.OriginPercentages;
                    text._bounds = new BoundingRectangle(text.Position, size, text.OriginPercentages);
                    if (!text._bounds.DisjointWith(b.Width, b.Height))
                    {
                        g.ResetTransform();
                        g.TranslateTransform(text.Position.X - localOrigin.X, text.Position.Y - localOrigin.Y);
                        //g.RotateTransformAt(text.Rotation);
                        //g.ScaleTransform(text.Scale.X, text.Scale.Y);
                        g.DrawString(text.Text, text.Font, text._brush, 0.0f, 0.0f);
                    }
                }
            }
            texture.PushData();
            _modified.Clear();
        }

        internal void TextChanged(TextData textData)
        {
            _modified.AddLast(textData);
            NeedsRedraw?.Invoke();
        }
    }
}

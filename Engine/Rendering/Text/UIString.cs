﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Text
{
    public class UIString3D : UIString2D
    {
        private float _worldZ = 0.0f;
        private float _prevWorldZ = 0.0f;
        private Vec3 _prevWorldPosition = Vec3.Zero;
        private Vec3 _worldPosition = Vec3.Zero;
        private Vec3 _screenVelocity = Vec3.Zero;

        public Vec3 WorldPosition
        {
            get => _worldPosition;
            set
            {
                _prevWorldPosition = _worldPosition;
                _worldPosition = value;
            }
        }
        public float WorldZ => _worldZ;
        public float PrevWorldZ => _prevWorldZ;
        public Vec3 ScreenVelocity => _screenVelocity;
        public Vec3 PrevWorldPosition => _prevWorldPosition;

        public void UpdateScreenPosition(Viewport v)
        {
            Vec3 screenPrev = v.WorldToScreen(_prevWorldPosition);
            Vec3 screen = v.WorldToScreen(_worldPosition);

            _prevWorldZ = TMath.DepthToDistance(screenPrev.Z, v.Camera.NearZ, v.Camera.FarZ);
            _worldZ = TMath.DepthToDistance(screen.Z, v.Camera.NearZ, v.Camera.FarZ);

            _screenVelocity.X = screen.X - Position.X;
            _screenVelocity.Y = screen.Y - Position.Y;
            _screenVelocity.Z = _worldZ - _prevWorldZ;

            Position = screen.Xy;
        }
    }
    public class UIString2D : TObject
    {
        public UIString2D() { }

        private string _text = string.Empty;
        private Font _font = new Font("Segoe UI", 9.0f, FontStyle.Regular);
        private Vec2 _scale = Vec2.One;
        private int _order = 0;
        private float _rotation = 0.0f;
        private ColorF4 _color = new ColorF4(1.0f);
        private StringFormat _format = new StringFormat();
        private BoundingRectangle _bounds = new BoundingRectangle();

        public BoundingRectangle Bounds => _bounds;
        internal SolidBrush Brush { get; private set; } = new SolidBrush(Color.White);
        internal TextDrawer Parent { get; set; }
        internal List<UIString2D> Overlapping { get; set; } //Set after being drawn

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
                _bounds.Extents = TextRenderer.MeasureText(_text, _font);
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
                _bounds.Extents = TextRenderer.MeasureText(_text, _font);
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

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

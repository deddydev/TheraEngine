using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;

namespace CustomEngine.Rendering.HUD
{
    public class HudComponent : ObjectBase, IPanel, IRenderable, IEnumerable<HudComponent>
    {
        public HudComponent(HudComponent owner) { _owner = owner; }

        public HudComponent _owner;
        public List<HudComponent> _children = new List<HudComponent>();

        public HudDockStyle _dockStyle;
        public AnchorFlags _anchorFlags;

        Matrix4 _transform = Matrix4.Identity;
        private RectangleF _region = new RectangleF();
        private float _rotationAngle = 0.0f;
        private Vec2 _rotationLocalOrigin = new Vec2(0.5f);
        public Vec2 _scale = Vec2.One;
        
        [Category("Transform"), Default, Animatable, PostCall("OnResized")]
        public RectangleF Region
        {
            get { return _region; }
            set { _region = value; }
        }
        [Category("Transform"), State, Animatable, PostCall("OnResized")]
        public SizeF Size
        {
            get { return _region.Size; }
            set { _region.Size = value; }
        }
        [Category("Transform"), State, Animatable, PostCall("OnResized")]
        public float Height
        {
            get { return _region.Height; }
            set { _region.Height = value; }
        }
        [Category("Transform"), State, Animatable, PostCall("OnResized")]
        public float Width
        {
            get { return _region.Width; }
            set { _region.Width = value; }
        }
        [Category("Transform"), State, Animatable, PostCall("OnTransformed")]
        public PointF Location
        {
            get { return _region.Location; }
            set { _region.Location = value; }
        }
        [Category("Transform"), State, Animatable, PostCall("OnTransformed")]
        public float TranslationX
        {
            get { return _region.X; }
            set { _region.X = value; }
        }
        [Category("Transform"), State, Animatable, PostCall("OnTransformed")]
        public float TranslationY
        {
            get { return _region.Y; }
            set { _region.Y = value; }
        }
        /// <summary>
        /// The rotation angle of the component in degrees.
        /// </summary>
        [Category("Transform"), Default, State, Animatable, PostCall("OnTransformed")]
        public float RotationAngle
        {
            get { return _rotationAngle; }
            set { _rotationAngle = value.RemapToRange(0.0f, 360.0f); }
        }
        /// <summary>
        /// The origin of the component's rotation angle, as a percentage.
        /// 0,0 is bottom left, 0.5,0.5 is center, 1.0,1.0 is top right.
        /// </summary>
        [Category("Transform"), Default, State, Animatable, PostCall("OnTransformed")]
        public Vec2 RotationLocalOrigin
        {
            get { return _rotationLocalOrigin; }
            set { _rotationLocalOrigin = value; }
        }
        [Category("Transform"), State, Animatable, PostCall("OnTransformed")]
        public Vec2 Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }
        [Category("Transform"), State, Animatable, PostCall("OnTransformed")]
        public float ScaleX
        {
            get { return _scale.X; }
            set { _scale.X = value; }
        }
        [Category("Transform"), State, Animatable, PostCall("OnTransformed")]
        public float ScaleY
        {
            get { return _scale.Y; }
            set { _scale.Y = value; }
        }
        public void OnTransformed()
        {
            //step 1: set identity matrix
            //step 2: translate into position (bottom left corner)
            //step 3: rotate in position
            //step 4: translate backward, relative to the rotation, by the local rotation origin to center on the rotation point
            //step 5: scale the component

            Matrix4 rotation = Matrix4.Identity;

            //Ignore rotations if docked or anchored
            if (_dockStyle == HudDockStyle.None && _anchorFlags == AnchorFlags.None)
                rotation =
                    Matrix4.CreateTranslation(-_rotationLocalOrigin.X * Width, -_rotationLocalOrigin.Y * Height, 0.0f) *
                    Matrix4.CreateRotationZ(RotationAngle);

            _transform =
                Matrix4.CreateScale(ScaleX, ScaleY, 1.0f) *
                rotation *
                Matrix4.CreateTranslation(TranslationX, TranslationY, 0.0f);
        }
        /// <summary>
        /// Returns the available real estate for the next components to use.
        /// </summary>
        public virtual RectangleF OnResized(RectangleF parentRegion)
        {
            RectangleF leftOver = parentRegion;
            if (_dockStyle != HudDockStyle.None || _anchorFlags != AnchorFlags.None)
            {
                bool allowLeft = true, allowRight = true, allowTop = true, allowBottom = true;
                if (_dockStyle != HudDockStyle.None)
                {
                    allowLeft = false;
                    allowRight = false;
                    allowTop = false;
                    allowBottom = false;
                    switch (_dockStyle)
                    {
                        case HudDockStyle.Fill:
                            _region.Size = parentRegion.Size;
                            _region.Location = parentRegion.Location;
                            break;
                        case HudDockStyle.Bottom:
                            _region.Location = parentRegion.Location;
                            _region.Width = parentRegion.Width;
                            allowTop = true;
                            break;
                        case HudDockStyle.Top:
                            _region.Location = parentRegion.Location;
                            _region.Y += parentRegion.Height - _region.Height;
                            _region.Width = parentRegion.Width;
                            allowBottom = true;
                            break;
                        case HudDockStyle.Left:
                            _region.Location = parentRegion.Location;
                            _region.Height = parentRegion.Height;
                            allowRight = true;
                            break;
                        case HudDockStyle.Right:
                            _region.Location = parentRegion.Location;
                            _region.X += parentRegion.Width - _region.Width;
                            _region.Height = parentRegion.Height;
                            allowLeft = true;
                            break;
                    }
                }
                if (_anchorFlags != AnchorFlags.None)
                {
                    if ((_anchorFlags & AnchorFlags.Bottom) != 0 && allowBottom)
                    {

                    }
                    if ((_anchorFlags & AnchorFlags.Top) != 0 && allowTop)
                    {

                    }
                    if ((_anchorFlags & AnchorFlags.Left) != 0 && allowLeft)
                    {

                    }
                    if ((_anchorFlags & AnchorFlags.Right) != 0 && allowRight)
                    {

                    }
                }
                leftOver = RegionComplement(parentRegion, Region);
            }

            RectangleF region = Region;
            foreach (HudComponent c in _children)
                region = c.OnResized(region);

            return leftOver;
        }

        private RectangleF RegionComplement(RectangleF parentRegion, RectangleF region)
        {
            RectangleF leftOver = new RectangleF();



            return leftOver;
        }

        public virtual void Render(float delta)
        {
            Renderer.PushMatrix();
            Renderer.MultMatrix(_transform);
            OnRender(delta);
            foreach (HudComponent comp in _children)
                comp.Render(delta);
            Renderer.PopMatrix();
        }
        protected virtual void OnRender(float delta) { }

        public IEnumerator<HudComponent> GetEnumerator() { return ((IEnumerable<HudComponent>)_children).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<HudComponent>)_children).GetEnumerator(); }
    }
    [Flags]
    public enum AnchorFlags
    {
        None = 0,
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right = 8,
    }
    public enum HudDockStyle
    {
        None,
        Fill,
        Left,
        Right,
        Top,
        Bottom,
    }
}

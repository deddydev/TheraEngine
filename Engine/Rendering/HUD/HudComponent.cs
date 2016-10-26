using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;

namespace CustomEngine.Rendering.HUD
{
    public abstract class HudComponent : ObjectBase, IPanel, IRenderable, IEnumerable<HudComponent>
    {
        private RectangleF _region = new RectangleF();
        private float _rotationAngle;
        private PointF _rotationLocalOrigin;
        public HudComponent _owner;
        public List<HudComponent> _children = new List<HudComponent>();

        public HudComponent(HudComponent owner) { _owner = owner; }

        [Category("Transform"), Default, Animatable]
        public RectangleF Region { get { return _region; } set { _region = value; OnResized(); } }
        [Category("Transform"), State, Animatable]
        public SizeF Size
        {
            get { return _region.Size; }
            set { _region.Size = value; }
        }
        [Category("Transform"), State, Animatable]
        public PointF Location
        {
            get { return _region.Location; }
            set { _region.Location = value; }
        }
        [Category("Transform"), State, Animatable]
        public float Height
        {
            get { return _region.Height; }
            set
            {
                _region.Height = value;
                OnResized();
            }
        }
        [Category("Transform"), State, Animatable]
        public float Width
        {
            get { return _region.Width; }
            set
            {
                _region.Width = value;
                OnResized();
            }
        }
        [Category("Transform"), State, Animatable]
        public float X
        {
            get { return _region.X; }
            set { _region.X = value; }
        }
        [Category("Transform"), State, Animatable]
        public float Y
        {
            get { return _region.Y; }
            set { _region.Y = value; }
        }
        /// <summary>
        /// The rotation angle of the component in degrees.
        /// </summary>
        [Category("Transform"), Default, State, Animatable]
        public float RotationAngle
        {
            get { return _rotationAngle; }
            set { _rotationAngle = value.RemapToRange(0.0f, 360.0f); }
        }
        /// <summary>
        /// The origin of the component's rotation angle, as a percentage.
        /// 0,0 is bottom left, 0.5,0.5 is center, 1.0,1.0 is top right.
        /// </summary>
        [Category("Transform"), Default, State, Animatable]
        public PointF RotationLocalOrigin
        {
            get { return _rotationLocalOrigin; }
            set { _rotationLocalOrigin = value; }
        }
        public virtual void OnResized()
        {
            foreach (HudComponent c in _children)
                c.OnResized();
        }
        public virtual void Render(float delta)
        {
            Renderer.PushMatrix();
            Renderer.MultMatrix(Matrix4.CreateTranslation(X, Y, 0));
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
        None,
        Top,
        Bottom,
        Left,
        Right,
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

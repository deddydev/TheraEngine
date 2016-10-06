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
        private float _rotationAngle, _rotationLocalOrigin;
        public HudComponent _owner;
        public List<HudComponent> _children = new List<HudComponent>();

        public HudComponent(HudComponent owner) { _owner = owner; }

        [Category("Transform"), Transient, Animatable]
        public RectangleF Region { get { return _region; } set { _region = value; OnResized(); } }
        [Category("Transform"), Transient, Animatable]
        public float Height
        {
            get { return _region.Height; }
            set
            {
                _region.Height = value;
                OnResized();
            }
        }
        [Category("Transform"), Transient, Animatable]
        public float Width
        {
            get { return _region.Width; }
            set
            {
                _region.Width = value;
                OnResized();
            }
        }
        [Category("Transform"), Transient, Animatable]
        public float X
        {
            get { return _region.X; }
            set { _region.X = value; }
        }
        [Category("Transform"), Transient, Animatable]
        public float Y
        {
            get { return _region.Y; }
            set { _region.Y = value; }
        }
        /// <summary>
        /// The rotation angle of the component in degrees.
        /// </summary>
        [Category("Transform"), Transient, Animatable]
        public float RotationAngle
        {
            get { return _rotationAngle; }
            set { _rotationAngle = value; }
        }
        /// <summary>
        /// The origin of the component's rotation angle.
        /// </summary>
        [Category("Transform"), Transient, Animatable]
        public float RotationLocalOrigin
        {
            get { return _rotationLocalOrigin; }
            set { _rotationLocalOrigin = value; }
        }
        public virtual void OnResized()
        {
            foreach (HudComponent c in _children)
                c.OnResized();
        }
        public virtual void Render()
        {
            Renderer.PushMatrix();
            Renderer.Translate(X, Y, 0);
            OnRender();
            foreach (HudComponent comp in _children)
                comp.Render();
            Renderer.PopMatrix();
        }
        protected virtual void OnRender() { }

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

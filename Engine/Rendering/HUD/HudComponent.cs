using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;

namespace CustomEngine.Rendering.HUD
{
    public abstract class HudComponent : ObjectBase, IPanel, IRenderable, IEnumerable<HudComponent>
    {
        public HudComponent(HudComponent owner) { _owner = owner; }

        private RectangleF _region = new RectangleF();
        private float _rotationAngle;
        private PointF _rotationLocalOrigin;
        public HudComponent _owner;
        public List<HudComponent> _children = new List<HudComponent>();
        Matrix4 _transform = Matrix4.Identity;
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
        public PointF RotationLocalOrigin
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
            _transform =
                Matrix4.CreateScale(ScaleX, ScaleY, 1.0f) * 
                Matrix4.TransformMatrix(
                Vec3.One, 
                Quaternion.FromAxisAngle(Vec3.UnitZ, RotationAngle), 
                new Vec3(TranslationX, TranslationY, 0.0f), 
                Matrix4.MultiplyOrder.STR);
        }
        public virtual void OnResized()
        {
            foreach (HudComponent c in _children)
                c.OnResized();
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

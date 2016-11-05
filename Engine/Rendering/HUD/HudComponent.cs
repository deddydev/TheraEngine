using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Rendering.Models;

namespace CustomEngine.Rendering.HUD
{
    public class HudComponent : ObjectBase, IPanel, IEnumerable<HudComponent>
    {
        public HudComponent(HudComponent owner) { _owner = owner; }

        protected HudComponent _owner;
        protected List<HudComponent> _children = new List<HudComponent>();

        protected ushort _zIndex;
        protected AnchorFlags _positionAnchorFlags;
        protected Matrix4 _transform = Matrix4.Identity;
        protected RectangleF _region = new RectangleF();
        protected Vec2 _scale = Vec2.One;
        
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

        public virtual void OnTransformed()
        {
            //step 1: set identity matrix
            //step 2: translate into position (bottom left corner)
            //step 5: scale the component

            _transform = Matrix4.TransformMatrix(
                new Vec3(ScaleX, ScaleY, 0.0f),
                Quaternion.Identity, 
                new Vec3(TranslationX, TranslationY, 0.0f), 
                Matrix4.MultiplyOrder.TRS);
        }
        public void Add(HudComponent child)
        {
            if (child == null)
                return;
            if (!_children.Contains(child))
                _children.Add(child);
            child._owner = this;
        }
        public void Remove(HudComponent child)
        {
            if (child == null)
                return;
            if (_children.Contains(child))
                _children.Remove(child);
            child._owner = null;
        }

        /// <summary>
        /// Returns the available real estate for the next components to use.
        /// </summary>
        public virtual RectangleF ParentResized(RectangleF parentRegion)
        {
            RectangleF region = Region;
            foreach (HudComponent c in _children)
                region = c.ParentResized(region);
            return parentRegion;
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
}

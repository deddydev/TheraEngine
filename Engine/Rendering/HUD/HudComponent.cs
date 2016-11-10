﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using CustomEngine.Worlds.Actors;

namespace CustomEngine.Rendering.HUD
{
    public class HudComponent : Pawn, IPanel, IEnumerable<HudComponent>
    {
        public HudComponent(HudComponent owner) { _parent = owner; }

        protected HudComponent _parent;
        protected List<HudComponent> _children = new List<HudComponent>();

        protected ushort _zIndex;
        protected AnchorFlags _positionAnchorFlags;
        protected Matrix4 _localTransform = Matrix4.Identity;
        protected Matrix4 _globalTransform = Matrix4.Identity;
        protected RectangleF _region = new RectangleF();
        protected Vec2 _scale = Vec2.One;
        protected Vec2 _translationLocalOrigin = Vec2.Zero;

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
        /// <summary>
        /// The origin of the component's rotation angle, as a percentage.
        /// 0,0 is bottom left, 0.5,0.5 is center, 1.0,1.0 is top right.
        /// </summary>
        [Category("Transform"), Default, State, Animatable, PostCall("OnTransformed")]
        public Vec2 TranslationLocalOrigin
        {
            get { return _translationLocalOrigin; }
            set { _translationLocalOrigin = value; }
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

            _localTransform = Matrix4.TransformMatrix(
                new Vec3(ScaleX, ScaleY, 0.0f),
                Quaternion.Identity, 
                new Vec3(TranslationX, TranslationY, 0.0f), 
                Matrix4.MultiplyOrder.TRS);

            RecalcGlobalTransform();
        }
        private void RecalcGlobalTransform()
        {
            Matrix4 parentTransform = _parent == null ? Matrix4.Identity : _parent._globalTransform;
            _globalTransform = _localTransform * parentTransform;
            foreach (HudComponent c in _children)
                c.RecalcGlobalTransform();
        }
        public void Add(HudComponent child)
        {
            if (child == null)
                return;
            if (!_children.Contains(child))
                _children.Add(child);
            child._parent = this;
            child._zIndex = (ushort)(_zIndex + 1);
        }
        public void Remove(HudComponent child)
        {
            if (child == null)
                return;
            if (_children.Contains(child))
                _children.Remove(child);
            child._parent = null;
            child._zIndex = 0;
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

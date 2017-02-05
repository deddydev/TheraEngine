using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using CustomEngine.Worlds.Actors;

namespace CustomEngine.Rendering.HUD
{
    public class HudComponent : Pawn, IPanel, IEnumerable<HudComponent>
    {
        public HudComponent(HudComponent owner)
        {
            if (owner != null)
                owner.Add(this);
        }

        protected HudComponent _parent, _left, _right, _down, _up;
        protected List<HudComponent> _children = new List<HudComponent>();

        protected bool _highlightable, _selectable;
        protected ushort _zIndex;
        protected AnchorFlags _positionAnchorFlags;
        protected Matrix4 _localTransform = Matrix4.Identity, _invLocalTransform = Matrix4.Identity;
        protected Matrix4 _globalTransform = Matrix4.Identity, _invGlobalTransform = Matrix4.Identity;
        protected RectangleF _region = new RectangleF();
        protected Vec2 _scale = Vec2.One;
        protected Vec2 _translationLocalOrigin = Vec2.Zero;

        [Category("Events")]
        public event Action Highlighted;
        [Category("Events")]
        public event Action Selected;

        public virtual void OnHighlighted()
        {
            if (_highlightable)
                Highlighted?.Invoke();
        }
        public virtual void OnSelected()
        {
            if (_selectable)
                Selected?.Invoke();
        }
        [Category("Interaction")]
        public bool Selectable
        {
            get { return _selectable; }
            set { _selectable = value; }
        }
        [Category("Interaction")]
        public bool Highlightable
        {
            get { return _highlightable; }
            set { _highlightable = value; }
        }
        [Category("Transform")]
        public RectangleF Region
        {
            get { return _region; }
            set { _region = value; OnResized(); }
        }
        [Category("Transform")]
        public SizeF Size
        {
            get { return _region.Size; }
            set { _region.Size = value; OnResized(); }
        }
        [Category("Transform")]
        public float Height
        {
            get { return _region.Height; }
            set { _region.Height = value; OnResized(); }
        }
        [Category("Transform")]
        public float Width
        {
            get { return _region.Width; }
            set { _region.Width = value; OnResized(); }
        }
        [Category("Transform")]
        public PointF Location
        {
            get { return _region.Location; }
            set { _region.Location = value; OnTransformed(); }
        }
        /// <summary>
        /// The origin of the component's rotation angle, as a percentage.
        /// 0,0 is bottom left, 0.5,0.5 is center, 1.0,1.0 is top right.
        /// </summary>
        [Category("Transform")]
        public Vec2 TranslationLocalOrigin
        {
            get { return _translationLocalOrigin; }
            set
            {
                Vec2 diff = value - _translationLocalOrigin;
                _region.X += diff.X;
                _region.Y += diff.Y;
                _translationLocalOrigin = value;
                OnTransformed();
            }
        }
        [Category("Transform")]
        public Vec2 BottomLeftTranslation
        {
            get
            {
                return new Vec2(
                    TranslationX - _translationLocalOrigin.X * Width,
                    TranslationY - _translationLocalOrigin.Y * Height);
            }
            set
            {
                TranslationX = value.X + _translationLocalOrigin.X * Width;
                TranslationY = value.Y + _translationLocalOrigin.Y * Height;
                OnTransformed();
            }
        }
        [Category("Transform")]
        public float TranslationX
        {
            get { return _region.X; }
            set { _region.X = value; OnTransformed(); }
        }
        [Category("Transform")]
        public float TranslationY
        {
            get { return _region.Y; }
            set { _region.Y = value; OnTransformed(); }
        }
        [Category("Transform")]
        public Vec2 Scale
        {
            get { return _scale; }
            set { _scale = value; OnTransformed(); }
        }
        [Category("Transform")]
        public float ScaleX
        {
            get { return _scale.X; }
            set { _scale.X = value; OnTransformed(); }
        }
        [Category("Transform")]
        public float ScaleY
        {
            get { return _scale.Y; }
            set { _scale.Y = value; OnTransformed(); }
        }

        public virtual void OnTransformed()
        {
            //step 1: set identity matrix
            //step 2: translate into position (bottom left corner)
            //step 5: scale the component

            _localTransform = Matrix4.TransformMatrix(
                new Vec3(ScaleX, ScaleY, 0.0f),
                Quaternion.Identity, 
                new Vec3(BottomLeftTranslation),
                Matrix4.MultiplyOrder.TRS);

            _invLocalTransform = Matrix4.TransformMatrix(
                new Vec3(1.0f / ScaleX, 1.0f / ScaleY, 0.0f),
                Quaternion.Identity,
                new Vec3(-BottomLeftTranslation),
                Matrix4.MultiplyOrder.SRT);

            RecalcGlobalTransform();
        }
        private void RecalcGlobalTransform()
        {
            Matrix4 parentTransform = _parent == null ? Matrix4.Identity : _parent._globalTransform;
            Matrix4 invParentTransform = _parent == null ? Matrix4.Identity : _parent._invGlobalTransform;

            _globalTransform = _localTransform * parentTransform;
            _invGlobalTransform = invParentTransform * _invLocalTransform;

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
        protected virtual void OnResized()
        {
            
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
        public HudComponent FindComponent(Vec2 viewportPoint)
        {
            if (!ContainsPoint(viewportPoint))
                return null;

            //TODO: create 2D quadtree for hud component searching
            foreach (HudComponent c in _children)
            {
                HudComponent comp = c.FindComponent(viewportPoint);
                if (comp != null)
                    return comp.FindComponent(viewportPoint);
            }

            return this;
        }
        public bool ContainsPoint(Vec2 viewportPoint)
        {
            Vec3 localPoint = _invGlobalTransform * new Vec3(viewportPoint, 0.0f);
            
            return false;
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

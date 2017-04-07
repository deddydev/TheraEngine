using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using CustomEngine.Worlds.Actors;
using CustomEngine.Worlds.Actors;

namespace CustomEngine.Rendering.HUD
{
    public delegate void DelScrolling(bool up);
    public class HudComponent : Pawn<SceneComponent>, IPanel, I2DBoundable, IEnumerable<HudComponent>
    {
        public HudComponent() : base()
        {

        }

        private HudManager _manager;
        protected HudComponent _parent, _left, _right, _down, _up;
        protected List<HudComponent> _children = new List<HudComponent>();

        protected Quadtree.Node _renderNode;
        protected bool _highlightable, _selectable, _scrollable;
        protected ushort _zIndex;
        protected AnchorFlags _positionAnchorFlags;
        protected Matrix4 _localTransform = Matrix4.Identity, _invLocalTransform = Matrix4.Identity;
        protected Matrix4 _globalTransform = Matrix4.Identity, _invGlobalTransform = Matrix4.Identity;
        protected BoundingRectangle _region = new BoundingRectangle();
        protected Vec2 _scale = Vec2.One;
        protected Vec2 _translationLocalOrigin = Vec2.Zero;
        protected BoundingRectangle _axisAlignedBounds;
        protected bool _isRendering;
        
        [Category("Events")]
        public event Action Highlighted;
        [Category("Events")]
        public event Action Selected;
        [Category("Events")]
        public event DelScrolling Scrolled;

        public virtual void OnHighlighted()
        {
            if (_highlightable)
                Highlighted?.Invoke();
        }
        public virtual void OnSelect()
        {
            if (_selectable)
                Selected?.Invoke();
        }
        public virtual void OnScrolled(bool up)
        {
            if (_scrollable)
                Scrolled?.Invoke(up);
        }
        public virtual void OnBack()
        {
            
        }

        [Category("Interaction")]
        public bool Selectable
        {
            get => _selectable;
            set => _selectable = value;
        }
        [Category("Interaction")]
        public bool Highlightable
        {
            get => _highlightable;
            set => _highlightable = value;
        }
        [Category("Interaction")]
        public bool Scrollable
        {
            get => _scrollable;
            set => _scrollable = value;
        }
        [Category("Transform")]
        public BoundingRectangle Region
        {
            get => _region;
            set
            {
                _region = value;
                OnResized();
            }
        }
        [Category("Transform")]
        public Vec2 Size
        {
            get => _region.Bounds;
            set
            {
                _region.Bounds = value;
                OnResized();
            }
        }
        [Category("Transform")]
        public float Height
        {
            get => _region.Height;
            set
            {
                _region.Height = value;
                OnResized();
            }
        }
        [Category("Transform")]
        public float Width
        {
            get => _region.Width;
            set
            {
                _region.Width = value;
                OnResized();
            }
        }
        [Category("Transform")]
        public Vec2 Translation
        {
            get => _region.Translation;
            set
            {
                _region.Translation = value;
                OnTransformed();
            }
        }
        /// <summary>
        /// The origin of the component's rotation angle, as a percentage.
        /// 0,0 is bottom left, 0.5,0.5 is center, 1.0,1.0 is top right.
        /// </summary>
        [Category("Transform")]
        public Vec2 TranslationLocalOrigin
        {
            get => _translationLocalOrigin;
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
            get => new Vec2(
                TranslationX - _translationLocalOrigin.X * Width,
                TranslationY - _translationLocalOrigin.Y * Height);
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
            get => _region.X;
            set
            {
                _region.X = value;
                OnTransformed();
            }
        }
        [Category("Transform")]
        public float TranslationY
        {
            get => _region.Y;
            set
            {
                _region.Y = value;
                OnTransformed();
            }
        }
        [Category("Transform")]
        public Vec2 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                OnTransformed();
            }
        }
        [Category("Transform")]
        public float ScaleX
        {
            get => _scale.X;
            set
            {
                _scale.X = value;
                OnTransformed();
            }
        }
        [Category("Transform")]
        public float ScaleY
        {
            get => _scale.Y;
            set
            {
                _scale.Y = value;
                OnTransformed();
            }
        }

        public BoundingRectangle AxisAlignedBounds
            => _axisAlignedBounds;

        public Quadtree.Node RenderNode
        {
            get => _renderNode;
            set => _renderNode = value;
        }
        public HudManager Manager
        {
            get => _manager;
            set
            {
                _manager?.UncacheComponent(this);
                _manager = value;
                _manager?.CacheComponent(this);
            }
        }
        public bool IsRendering
        {
            get => _isRendering;
            set => _isRendering = value;
        }
        public virtual void OnTransformed()
        {
            //step 1: set identity matrix
            //step 2: translate into position (bottom left corner)
            //step 5: scale the component

            _localTransform = Matrix4.TransformMatrix(
                new Vec3(ScaleX, ScaleY, 0.0f),
                Quat.Identity, 
                new Vec3(BottomLeftTranslation),
                TransformOrder.TRS);

            _invLocalTransform = Matrix4.TransformMatrix(
                new Vec3(1.0f / ScaleX, 1.0f / ScaleY, 0.0f),
                Quat.Identity,
                new Vec3(-BottomLeftTranslation),
                TransformOrder.SRT);

            RecalcGlobalTransform();
        }
        protected void RecalcGlobalTransform()
        {
            Matrix4 parentTransform = _parent == null ? Matrix4.Identity : _parent._globalTransform;
            Matrix4 invParentTransform = _parent == null ? Matrix4.Identity : _parent._invGlobalTransform;

            _globalTransform = _localTransform * parentTransform;
            _invGlobalTransform = invParentTransform * _invLocalTransform;

            _renderNode?.OnMoved(this);

            foreach (HudComponent c in _children)
                c.RecalcGlobalTransform();
        }
        protected virtual void OnChildAdded(HudComponent child)
        {
            child.Manager = Manager;
        }
        public void Add(HudComponent child)
        {
            if (child == null)
                return;
            if (!_children.Contains(child))
                _children.Add(child);
            child._parent = this;
            child._zIndex = (ushort)(_zIndex + 1);
            OnChildAdded(child);
        }
        protected virtual void OnChildRemoved(HudComponent child)
        {

        }
        public void Remove(HudComponent child)
        {
            if (child == null)
                return;
            if (_children.Contains(child))
                _children.Remove(child);
            child.Manager = null;
            child._parent = null;
            child._zIndex = 0;
        }
        protected virtual void OnResized()
        {
            
        }
        /// <summary>
        /// Returns the available real estate for the next components to use.
        /// </summary>
        public virtual BoundingRectangle Resize(BoundingRectangle parentRegion)
        {
            BoundingRectangle region = Region;
            foreach (HudComponent c in _children)
                region = c.Resize(region);
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

        public virtual void Render() { }

        public IEnumerator<HudComponent> GetEnumerator() => ((IEnumerable<HudComponent>)_children).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<HudComponent>)_children).GetEnumerator();
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

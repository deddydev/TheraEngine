using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using TheraEngine.Worlds.Actors;
using TheraEngine.Worlds;

namespace TheraEngine.Rendering.HUD
{
    public delegate void DelScrolling(bool up);
    public class HudComponent : SceneComponent, IPanel, I2DBoundable, IEnumerable<HudComponent>
    {
        public HudComponent() : base()
        {

        }
        
        //Used to select a new component when the user moves the gamepad stick.
        protected HudComponent _left, _right, _down, _up;

        protected Quadtree.Node _renderNode;
        protected bool _highlightable, _selectable, _scrollable;
        protected ushort _zIndex;
        protected AnchorFlags _positionAnchorFlags;
        protected BoundingRectangle _region = new BoundingRectangle();
        protected Vec2 _translationLocalOrigin = Vec2.Zero;
        protected Vec2 _scale = Vec2.One;
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
                RecalcLocalTransform();
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
                RecalcLocalTransform();
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
                RecalcLocalTransform();
            }
        }
        [Category("Transform")]
        public float TranslationX
        {
            get => _region.X;
            set
            {
                _region.X = value;
                RecalcLocalTransform();
            }
        }
        [Category("Transform")]
        public float TranslationY
        {
            get => _region.Y;
            set
            {
                _region.Y = value;
                RecalcLocalTransform();
            }
        }
        [Category("Transform")]
        public Vec2 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                RecalcLocalTransform();
            }
        }
        [Category("Transform")]
        public float ScaleX
        {
            get => _scale.X;
            set
            {
                _scale.X = value;
                RecalcLocalTransform();
            }
        }
        [Category("Transform")]
        public float ScaleY
        {
            get => _scale.Y;
            set
            {
                _scale.Y = value;
                RecalcLocalTransform();
            }
        }

        public BoundingRectangle AxisAlignedBounds
            => _axisAlignedBounds;

        public Quadtree.Node RenderNode
        {
            get => _renderNode;
            set => _renderNode = value;
        }
        public HudManager Owner
        {
            get => (HudManager)base.OwningActor;
            set
            {
                HudManager manager = Owner;
                manager?.UncacheComponent(this);
                manager = value;
                manager?.CacheComponent(this);
                base.OwningActor = value;
            }
        }
        public bool IsRendering
        {
            get => _isRendering;
            set => _isRendering = value;
        }
        public ushort ZIndex => _zIndex;
        
        public bool Contains(Vec2 point)
            => _region.Contains(point);
        
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = Matrix4.TransformMatrix(
                new Vec3(ScaleX, ScaleY, 1.0f),
                Quat.Identity,
                new Vec3(BottomLeftTranslation),
                TransformOrder.TRS);

            inverseLocalTransform = Matrix4.TransformMatrix(
                new Vec3(1.0f / ScaleX, 1.0f / ScaleY, 1.0f),
                Quat.Identity,
                new Vec3(-BottomLeftTranslation),
                TransformOrder.SRT);
        }
        internal override void RecalcGlobalTransform()
        {
            Matrix4 parentTransform = GetParentMatrix();
            Matrix4 invParentTransform = GetInverseParentMatrix();

            _worldTransform = parentTransform * _localTransform;
            _inverseWorldTransform = _inverseLocalTransform * invParentTransform;

            _renderNode?.ItemMoved(this);

            foreach (HudComponent c in _children)
                c.RecalcGlobalTransform();
        }
        protected virtual void OnChildAdded(HudComponent child)
        {
            child.Owner = Owner;
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
            child.Owner = null;
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
            Vec2 localPoint = (Vec2)(_inverseWorldTransform * new Vec3(viewportPoint, 0.0f));
            return Region.Bounds.Contains(viewportPoint);
        }

        public virtual void Render() { }

        protected internal override void OriginRebased(Vec3 newOrigin)
        {
            throw new NotImplementedException();
        }

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

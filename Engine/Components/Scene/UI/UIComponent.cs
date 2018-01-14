using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using TheraEngine.Worlds.Actors;
using TheraEngine.Core.Shapes;
using TheraEngine.Worlds.Actors.Components;
using TheraEngine.Worlds.Actors.Types.Pawns;

namespace TheraEngine.Rendering.UI
{
    public class UIComponent : SceneComponent, IPanel, I2DBoundable, IEnumerable<UIComponent>
    {
        public UIComponent() : base()
        {

        }

        public IQuadtreeNode QuadtreeNode { get; set; }

        //Used to select a new component when the user moves the gamepad stick.
        protected UIComponent _left, _right, _down, _up;

        protected IQuadtreeNode _renderNode;
        protected bool _highlightable, _selectable, _scrollable;
        protected ushort _layerIndex, _sameLayerIndex;
        protected AnchorFlags _positionAnchorFlags;
        protected BoundingRectangle _region = new BoundingRectangle();
        protected Vec2 _translationLocalOrigin = Vec2.Zero;
        protected Vec2 _scale = Vec2.One;
        protected BoundingRectangle _axisAlignedBounds;
        protected bool _isRendering;

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
        public virtual Vec2 TranslationLocalOrigin
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

        public BoundingRectangle AxisAlignedBounds => _axisAlignedBounds;
        public new UIManager OwningActor
        {
            get => (UIManager)base.OwningActor;
            set
            {
                if (IsSpawned && this is I2DRenderable r)
                {
                    OwningActor?.RemoveRenderableComponent(r);
                    value?.AddRenderableComponent(r);
                }
                base.OwningActor = value;
            }
        }
        public bool IsRendering
        {
            get => _isRendering;
            set => _isRendering = value;
        }
        public ushort LayerIndex => _layerIndex;
        public ushort SameLayerIndex => _sameLayerIndex;

        /// <summary>
        /// Returns true if the given point, relative to the owning HudManager pawn's transform, is contained within this component.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Vec2 point)
        {
            Vec3 transformedPoint = new Vec3(point, 0.0f) * OwningActor.RootComponent.InverseWorldMatrix;
            return Contains(transformedPoint);
        }
        /// <summary>
        /// Returns true if the given world point projected perpendicularly to the HUD as a 2D point is contained within this component and the Z value is within the given depth margin.
        /// </summary>
        /// <param name="worldPoint"></param>
        /// <param name="depthMargin">How far away the point can be on either side of the HUD for it to be considered close enough.</param>
        /// <returns></returns>
        public bool Contains(Vec3 worldPoint, float depthMargin = 0.5f)
        {
            Vec3 localPoint = worldPoint * _inverseWorldTransform;
            return Math.Abs(localPoint.Z) < depthMargin && Region.Contains(localPoint.Xy);
        }

        public override void OnSpawned()
        {
            if (this is I2DRenderable r)
                OwningActor.AddRenderableComponent(r);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            if (this is I2DRenderable r)
                OwningActor.RemoveRenderableComponent(r);
            base.OnDespawned();
        }
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

            foreach (UIComponent c in _children)
                c.RecalcGlobalTransform();
        }
        protected virtual void OnChildAdded(UIComponent child)
        {
            child.OwningActor = OwningActor;
        }
        public void Add(UIComponent child)
        {
            if (child == null)
                return;
            if (!_children.Contains(child))
                _children.Add(child);
            child._parent = this;
            child._layerIndex = (ushort)(_layerIndex + 1);
            OnChildAdded(child);
        }
        protected virtual void OnChildRemoved(UIComponent child)
        {

        }
        public void Remove(UIComponent child)
        {
            if (child == null)
                return;
            if (_children.Contains(child))
                _children.Remove(child);
            child.OwningActor = null;
            child._parent = null;
            child._layerIndex = 0;
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
            foreach (UIComponent c in _children)
                region = c.Resize(region);
            return parentRegion;
        }
        public UIComponent FindComponent(Vec2 viewportPoint)
        {
            if (!Contains(viewportPoint))
                return null;

            //TODO: create 2D quadtree for hud component searching
            foreach (UIComponent c in _children)
            {
                UIComponent comp = c.FindComponent(viewportPoint);
                if (comp != null)
                    return comp;
            }

            return this;
        }

        protected internal override void OriginRebased(Vec3 newOrigin) { }

        public IEnumerator<UIComponent> GetEnumerator() => ((IEnumerable<UIComponent>)_children).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<UIComponent>)_children).GetEnumerator();

        //public override void HandleLocalTranslation(Vec3 delta)
        //{

        //}

        //public override void HandleLocalScale(Vec3 delta)
        //{

        //}

        //public override void HandleLocalRotation(Quat delta)
        //{

        //}
    }
}

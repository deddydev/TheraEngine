using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Components;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.UI
{
    public interface IUIComponent : ISceneComponent
    {
        Vec2 ScreenTranslation { get; }
    }
    public abstract class UIComponent : OriginRebasableComponent, IPanel, I2DBoundable, IEnumerable<UIComponent>
    {
        public UIComponent() : base() { }

        public IQuadtreeNode QuadtreeNode { get; set; }

        //Used to select a new component when the user moves the gamepad stick.
        protected UIComponent _left, _right, _down, _up;

        protected IQuadtreeNode _renderNode;
        protected bool _highlightable, _selectable, _scrollable;
        protected ushort _layerIndex, _indexWithinLayer;
        protected Vec2 _localOriginPercentage = Vec2.Zero;
        protected Vec2 _size = Vec2.Zero, _scale = Vec2.One;
        protected BoundingRectangle _axisAlignedBounds = new BoundingRectangle();
        protected bool _isRendering;

        #region Bounds
        [Category("Transform")]
        public virtual Vec2 Size
        {
            get => _size;
            set
            {
                _size = value;
                OnResized();
            }
        }
        [Category("Transform")]
        public virtual float Height
        {
            get => _size.Y;
            set
            {
                _size.Y = value;
                OnResized();
            }
        }
        [Category("Transform")]
        public virtual float Width
        {
            get => _size.X;
            set
            {
                _size.X = value;
                OnResized();
            }
        }
        #endregion

        #region Translation

        [Browsable(false)]
        [Category("Transform")]
        public Vec2 ScreenTranslation => Vec3.TransformPosition(WorldPoint, GetInvActorTransform()).Xy;
        
        [Category("Transform")]
        public virtual Vec2 LocalTranslation
        {
            get => _localTransform.Translation.Xy;
            set
            {
                _localTransform.TranslationXy = value;
                RecalcLocalTransform();
            }
        }
        [Category("Transform")]
        public virtual float LocalTranslationX
        {
            get => _localTransform.TranslationX;
            set
            {
                _localTransform.TranslationX = value;
                RecalcLocalTransform();
            }
        }
        [Category("Transform")]
        public virtual float LocalTranslationY
        {
            get => _localTransform.TranslationY;
            set
            {
                _localTransform.TranslationY = value;
                RecalcLocalTransform();
            }
        }
        /// <summary>
        /// The origin of the component's rotation and scale, as a percentage.
        /// 0,0 is bottom left, 0.5,0.5 is center, 1.0,1.0 is top right.
        /// </summary>
        [Category("Transform")]
        public virtual Vec2 LocalOriginPercentage
        {
            get => _localOriginPercentage;
            set
            {
                _localTransform.TranslationXy += (value - _localOriginPercentage) * Size;
                _localOriginPercentage = value;
                RecalcLocalTransform();
            }
        }
        [Category("Transform")]
        public virtual Vec2 LocalOriginTranslation
        {
            get => LocalOriginPercentage * Size;
            set => LocalOriginPercentage = value / Size;
        }
        [Category("Transform")]
        public virtual Vec2 BottomLeftTranslation
        {
            get => LocalTranslation - LocalOriginTranslation;
            set => LocalTranslation = value + LocalOriginTranslation;
        }
        #endregion

        #region Scale
        [Category("Transform")]
        public virtual Vec2 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                RecalcLocalTransform();
            }
        }
        [Category("Transform")]
        public virtual float ScaleX
        {
            get => _scale.X;
            set
            {
                _scale.X = value;
                RecalcLocalTransform();
            }
        }
        [Category("Transform")]
        public virtual float ScaleY
        {
            get => _scale.Y;
            set
            {
                _scale.Y = value;
                RecalcLocalTransform();
            }
        }
        #endregion

        public virtual BoundingRectangle AxisAlignedRegion => _axisAlignedBounds;
        public new IUIManager OwningActor
        {
            get => (IUIManager)base.OwningActor;
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
        public ushort IndexWithinLayer => _indexWithinLayer;
        
        public bool Contains(Vec2 screenPoint)
            => Contains(Vec3.TransformPosition(screenPoint, GetActorTransform()));
        /// <summary>
        /// Returns true if the given world point projected perpendicularly to the HUD as a 2D point is contained within this component and the Z value is within the given depth margin.
        /// </summary>
        /// <param name="worldPoint"></param>
        /// <param name="zMargin">How far away the point can be on either side of the HUD for it to be considered close enough.</param>
        /// <returns></returns>
        public bool Contains(Vec3 worldPoint, float zMargin = 0.5f)
        {
            Vec3 localPoint = Vec3.TransformPosition(worldPoint, InverseWorldMatrix);
            return Math.Abs(localPoint.Z) < zMargin && Size.Contains(localPoint.Xy);
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
            localTransform = Matrix4.TransformMatrix(new Vec3(Scale, 1.0f), Quat.Identity, LocalTranslation - LocalOriginTranslation, TransformOrder.TRS);
            inverseLocalTransform = Matrix4.TransformMatrix(new Vec3(1.0f / Scale, 1.0f), Quat.Identity, -LocalTranslation + LocalOriginTranslation, TransformOrder.SRT);
        }
        //protected virtual void OnChildAdded(UIComponent child)
        //{
        //    child.OwningActor = OwningActor;
        //}
        //public void Add(UIComponent child)
        //{
        //    if (child == null)
        //        return;
        //    if (!_children.Contains(child))
        //        _children.Add(child);
        //    child._parent = this;
        //    child._layerIndex = (ushort)(_layerIndex + 1);
        //    OnChildAdded(child);
        //}
        //protected virtual void OnChildRemoved(UIComponent child)
        //{

        //}
        //public void Remove(UIComponent child)
        //{
        //    if (child == null)
        //        return;
        //    if (_children.Contains(child))
        //        _children.Remove(child);
        //    child.OwningActor = null;
        //    child._parent = null;
        //    child._layerIndex = 0;
        //}
        protected virtual void OnResized()
        {
            _axisAlignedBounds.Translation = Vec3.TransformPosition(WorldPoint, GetInvActorTransform()).Xy;
            _axisAlignedBounds.Bounds = Size;
            RecalcLocalTransform();
        }
        public virtual BoundingRectangle Resize(BoundingRectangle parentRegion)
        {
            foreach (UIComponent c in _children)
                c.Resize(parentRegion);
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

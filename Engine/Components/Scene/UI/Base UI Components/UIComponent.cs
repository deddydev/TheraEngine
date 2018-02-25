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
        protected bool _highlightable, _selectable, _scrollable, _isRendering;

        public virtual int LayerIndex { get; set; }
        public virtual int IndexWithinLayer { get; set; }

        protected Vec2 _size = Vec2.Zero;
        protected Vec2 _scale = Vec2.One;
        protected Vec2 _translation = Vec2.Zero;
        protected Vec2 _localOriginPercentage = Vec2.Zero;
        protected BoundingRectangle _axisAlignedBounds = new BoundingRectangle();

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
            get => _translation;
            set
            {
                _translation = value;
                RecalcLocalTransform();
            }
        }
        [Category("Transform")]
        public virtual float LocalTranslationX
        {
            get => _translation.X;
            set
            {
                _translation.X = value;
                RecalcLocalTransform();
            }
        }
        [Category("Transform")]
        public virtual float LocalTranslationY
        {
            get => _translation.Y;
            set
            {
                _translation.Y = value;
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
                _translation += (value - _localOriginPercentage) * Size;
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

        public bool Contains(Vec2 screenPoint)
        {
            Vec3 localPoint = Vec3.TransformPosition(screenPoint, InverseWorldMatrix);
            return Size.Contains(localPoint.Xy);
        }
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
            localTransform = Matrix4.TransformMatrix(new Vec3(Scale, 1.0f), Matrix4.Identity, LocalTranslation - LocalOriginTranslation, TransformOrder.TRS);
            inverseLocalTransform = Matrix4.TransformMatrix(new Vec3(1.0f / Scale, 1.0f), Matrix4.Identity, -LocalTranslation + LocalOriginTranslation, TransformOrder.SRT);
        }
        internal override void RecalcGlobalTransform()
        {
            base.RecalcGlobalTransform();
        }
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
        
        protected override void HandleSingleChildAdded(SceneComponent item)
        {
            base.HandleSingleChildAdded(item);
            if (item is UIComponent c)
                c.LayerIndex = LayerIndex;
        }

        protected internal override void OriginRebased(Vec3 newOrigin) { }

        public IEnumerator<UIComponent> GetEnumerator() => ((IEnumerable<UIComponent>)_children).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<UIComponent>)_children).GetEnumerator();
    }
}

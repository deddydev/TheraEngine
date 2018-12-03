using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Components;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.UI
{
    public interface IUIBoundableComponent : IUIComponent
    {
        Vec2 Size { get; set; }
        float Height { get; set; }
        float Width { get; set; }
        Vec2 LocalOriginPercentage { get; set; }
        Vec2 LocalOriginTranslation { get; set; }
        Vec2 BottomLeftTranslation { get; set; }
    }
    public abstract class UIBoundableComponent : UIComponent, IUIBoundableComponent, I2DBoundable, IEnumerable<UIComponent>
    {
        public UIBoundableComponent() : base() { }
        
        [Browsable(false)]
        public IQuadtreeNode QuadtreeNode { get; set; }

        protected IQuadtreeNode _renderNode;

        protected Vec2 _localOriginPercentage = Vec2.Zero;
        protected Vec2 _size = Vec2.Zero;
        protected BoundingRectangleF _axisAlignedRegion = new BoundingRectangleF();
        
        #region Bounds
        [Category("Transform")]
        public virtual Vec2 Size
        {
            get => _size;
            set
            {
                _size = value;
                PerformResize();
            }
        }
        [Browsable(false)]
        public virtual float Height
        {
            get => _size.Y;
            set
            {
                _size.Y = value;
                PerformResize();
            }
        }
        [Browsable(false)]
        public virtual float Width
        {
            get => _size.X;
            set
            {
                _size.X = value;
                PerformResize();
            }
        }
        #endregion
        
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
                PerformResize();
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
        
        [Browsable(false)]
        public BoundingRectangleF AxisAlignedRegion => _axisAlignedRegion;
        
        public bool Contains(Vec2 viewportPoint)
        {
            Vec3 localPoint = viewportPoint * GetInvComponentTransform();
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
        
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = Matrix4.TransformMatrix(
                new Vec3(Scale, 1.0f),
                Matrix4.Identity,
                BottomLeftTranslation,
                TransformOrder.TRS);

            inverseLocalTransform = Matrix4.TransformMatrix(
                new Vec3(1.0f / Scale, 1.0f),
                Matrix4.Identity,
                -BottomLeftTranslation,
                TransformOrder.SRT);
        }
        public override void RecalcWorldTransform()
        {
            base.RecalcWorldTransform();
            RemakeAxisAlignedRegion();
        }
        public override Vec2 Resize(Vec2 parentBounds)
        {
            Vec2 bounds = base.Resize(parentBounds);
            RemakeAxisAlignedRegion();
            return bounds;
        }
        protected virtual void RemakeAxisAlignedRegion()
        {
            _axisAlignedRegion.Translation = WorldPoint.Xy;
            _axisAlignedRegion.Extents = Size * WorldScale.Xy;
            //Engine.PrintLine($"Axis-aligned region remade: {_axisAlignedRegion.Translation} {_axisAlignedRegion.Extents}");
        }
        public override UIBoundableComponent FindDeepestComponent(Vec2 viewportPoint)
        {
            if (Size.X > 0.0f && Size.Y > 0.0f && !Contains(viewportPoint))
                return null;
            
            return base.FindDeepestComponent(viewportPoint) ?? this;
        }
        
        protected override void HandleSingleChildAdded(SceneComponent item)
        {
            base.HandleSingleChildAdded(item);
            if (item is UIComponent c)
                c.LayerIndex = LayerIndex;
        }
    }
}

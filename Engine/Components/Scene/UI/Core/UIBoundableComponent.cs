﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Components;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.UI
{
    public interface IUIBoundableComponent : IUIComponent
    {
        EventVec2 Size { get; set; }
        EventVec2 LocalOriginPercentage { get; set; }
        Vec2 LocalOriginTranslation { get; set; }
        Vec2 BottomLeftTranslation { get; set; }
    }
    /// <summary>
    /// Only applies if the parent is a UIBoundableComponent.
    /// </summary>
    public enum EVerticalSizingMode
    {
        Top,
        Center,
        Bottom,
        Stretch,
    }
    /// <summary>
    /// Only applies if the parent is a UIBoundableComponent.
    /// </summary>
    public enum EHorizontalSizingMode
    {
        Left,
        Center,
        Right,
        Stretch,
    }
    public abstract class UIBoundableComponent : UITransformComponent, IUIBoundableComponent, IEnumerable<IUIComponent>
    {
        public UIBoundableComponent() : base() { }

        private bool _isMouseOver = false;

        [TSerialize(nameof(LocalOriginPercentage))]
        private EventVec2 _localOriginPercentage = EventVec2.Zero;

        [TSerialize(nameof(Size))]
        private EventVec2 _size = EventVec2.Zero;

        [TSerialize(nameof(HorizontalSizingMode))]
        private EHorizontalSizingMode _horizontalSizingMode;

        [TSerialize(nameof(VerticalSizingMode))]
        private EVerticalSizingMode _verticalSizingMode;

        [Category("State")]
        public bool IsMouseOver
        {
            get => _isMouseOver;
            set => SetBackingField(ref _isMouseOver, value);
        }
        [Category("Transform")]
        public EVerticalSizingMode VerticalSizingMode
        {
            get => _verticalSizingMode;
            set => SetBackingField(ref _verticalSizingMode, value);
        }
        [Category("Transform")]
        public EHorizontalSizingMode HorizontalSizingMode
        {
            get => _horizontalSizingMode;
            set => SetBackingField(ref _horizontalSizingMode, value);
        }
        [Category("Transform")]
        public virtual EventVec2 Size
        {
            get => _size;
            set
            {
                SetBackingField(ref _size, value,
                    () => _size.Changed -= Resize,
                    () => _size.Changed += Resize,
                    false);

                Resize();
            }
        }
        [Browsable(false)]
        public float Width 
        {
            get => _size.X; 
            set => _size.X = value;
        }
        [Browsable(false)]
        public float Height
        {
            get => _size.Y; 
            set => _size.Y = value;
        }

        /// <summary>
        /// The origin of the component's rotation and scale, as a percentage.
        /// 0,0 is bottom left, 0.5,0.5 is center, 1.0,1.0 is top right.
        /// </summary>
        [Category("Transform")]
        public virtual EventVec2 LocalOriginPercentage
        {
            get => _localOriginPercentage;
            set
            {
                LocalTranslation.Xy += (value - _localOriginPercentage) * Size;
                _localOriginPercentage = value;
                Resize();
            }
        }
        [Category("Transform")]
        public virtual Vec2 LocalOriginTranslation
        {
            get => LocalOriginPercentage.Raw * Size.Raw;
            set => LocalOriginPercentage.Raw = value / Size.Raw;
        }
        [Category("Transform")]
        public virtual Vec2 BottomLeftTranslation
        {
            get => LocalTranslation.Raw.Xy - LocalOriginTranslation;
            set => LocalTranslation.Xy = value + LocalOriginTranslation;
        }

        public bool Contains(Vec2 worldPoint)
        {
            Vec3 localPoint = worldPoint * GetInvComponentTransform();
            return Size.Raw.Contains(localPoint.Xy);
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
            return Math.Abs(localPoint.Z) < zMargin && Size.Raw.Contains(localPoint.Xy);
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = Matrix4.TransformMatrix(
                Scale.Raw,
                Matrix4.Identity,
                BottomLeftTranslation,
                ETransformOrder.TRS);

            inverseLocalTransform = Matrix4.TransformMatrix(
                1.0f / Scale.Raw,
                Matrix4.Identity,
                -BottomLeftTranslation,
                ETransformOrder.SRT);
        }
        public override void RecalcWorldTransform()
        {
            base.RecalcWorldTransform();
            RemakeAxisAlignedRegion();
        }
        public override Vec2 OnResize(Vec2 parentBounds)
        {
            Vec2 bounds = base.OnResize(parentBounds);
            RemakeAxisAlignedRegion();
            return bounds;
        }
        protected virtual void RemakeAxisAlignedRegion()
        {
            Matrix4 mtx = WorldMatrix * Matrix4.CreateScale(Size.X, Size.Y, 0.0f);

            Vec3 minPos = Vec3.TransformPosition(Vec3.Zero, mtx);
            Vec3 maxPos = Vec3.TransformPosition(Vec2.One, mtx); //This is Vec2.One on purpose, we only want Z to be 0

            Vec2 min = new Vec2(Math.Min(minPos.X, maxPos.X), Math.Min(minPos.Y, maxPos.Y));
            Vec2 max = new Vec2(Math.Max(minPos.X, maxPos.X), Math.Max(minPos.Y, maxPos.Y));

            RenderInfo.AxisAlignedRegion = BoundingRectangleFStruct.FromMinMaxSides(min.X, max.X, min.Y, max.Y, 0.0f, 0.0f);
            //Engine.PrintLine($"Axis-aligned region remade: {_axisAlignedRegion.Translation} {_axisAlignedRegion.Extents}");
        }
        public override IUIComponent FindDeepestComponent(Vec2 worldPoint, bool includeThis)
        {
            if (includeThis && IsVisible && Contains(worldPoint))
                return this;

            foreach (ISceneComponent c in _children)
            {
                if (c is IUIComponent uiComp && uiComp.IsVisible)
                {
                    IUIComponent comp = uiComp.FindDeepestComponent(worldPoint, true);
                    if (comp != null)
                        return comp;
                }
            }

            return null;
        }

        protected override void OnChildAdded(ISceneComponent item)
        {
            base.OnChildAdded(item);
            if (item is IUIComponent c)
                c.RenderInfo.LayerIndex = RenderInfo.LayerIndex;
        }
    }
}

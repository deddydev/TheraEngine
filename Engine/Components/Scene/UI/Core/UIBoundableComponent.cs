﻿using Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Components;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.UI
{
    public interface IUIBoundableComponent : IUITransformComponent
    {
        EVerticalAlign VerticalAlignment { get; set; }
        EHorizontalAlign HorizontalAlignment { get; set; }

        float Width { get; set; }
        float Height { get; set; }
        EventVec2 Size { get; set; }

        float MinWidth { get; set; }
        float MinHeight { get; set; }
        EventVec2 MinSize { get; set; }
        
        float MaxWidth { get; set; }
        float MaxHeight { get; set; }
        EventVec2 MaxSize { get; set; }
        
        float OriginPercentX { get; set; }
        float OriginPercentY { get; set; }
        EventVec2 OriginPercent { get; set; }
        
        float OriginTranslationX { get; set; }
        float OriginTranslationY { get; set; }
        Vec2 OriginTranslation { get; set; }
        
        float ActualWidth { get; }
        float ActualHeight { get; }
        EventVec2 ActualSize { get; }

        IUIComponent FindDeepestComponent(Vec2 worldPoint, bool includeThis);
        List<IUIBoundableComponent> FindAllIntersecting(Vec2 worldPoint, bool includeThis);
        void FindAllIntersecting(Vec2 worldPoint, bool includeThis, List<IUIBoundableComponent> results);

        float GetHeight();
        float GetWidth();
    }
    /// <summary>
    /// Only applies if the parent is a UIBoundableComponent.
    /// </summary>
    public enum EVerticalAlign
    {
        Top,
        Center,
        Bottom,
        Stretch,
        Positional,
    }
    /// <summary>
    /// Only applies if the parent is a UIBoundableComponent.
    /// </summary>
    public enum EHorizontalAlign
    {
        Left,
        Center,
        Right,
        Stretch,
        Positional,
    }
    public abstract class UIBoundableComponent : UITransformComponent, IUIBoundableComponent, IEnumerable<IUIComponent>
    {
        public UIBoundableComponent() : base() 
        {
            OriginPercent = new EventVec2();
            Size = new EventVec2();
            MinSize = new EventVec2();
            MaxSize = new EventVec2();
            Margins = new EventVec4();
            Padding = new EventVec4();
        }

        private bool _isMouseOver = false;

        private EventVec2 _originPercent;
        private EventVec2 _size;
        private EventVec2 _minSize;
        private EventVec2 _maxSize;
        private EventVec4 _margins;
        private EventVec4 _padding;
        
        [TSerialize(nameof(HorizontalAlignment))]
        private EHorizontalAlign _horizontalAlign = EHorizontalAlign.Positional;
        [TSerialize(nameof(VerticalAlignment))]
        private EVerticalAlign _verticalAlign = EVerticalAlign.Positional;

        [Category("State")]
        public bool IsMouseOver
        {
            get => _isMouseOver;
            set => Set(ref _isMouseOver, value);
        }
        [Category("Transform")]
        public EVerticalAlign VerticalAlignment
        {
            get => _verticalAlign;
            set
            {
                if (Set(ref _verticalAlign, value))
                    InvalidateLayout();
            }
        }
        [Category("Transform")]
        public EHorizontalAlign HorizontalAlignment
        {
            get => _horizontalAlign;
            set
            {
                if (Set(ref _horizontalAlign, value))
                    InvalidateLayout();
            }
        }
        [Category("Transform")]
        public virtual float Width
        {
            get => _size.X;
            set => _size.X = value;
        }
        [Category("Transform")]
        public virtual float Height
        {
            get => _size.Y;
            set => _size.Y = value;
        }
        [TSerialize]
        [Browsable(false)]
        public EventVec2 Size
        {
            get => _size;
            set
            {
                if (Set(ref _size, value ?? new EventVec2(),
                    () =>
                    {
                        _size.Changed -= SizeChanged;

                        //OnPropertyChanging(nameof(Width));
                        //OnPropertyChanging(nameof(Height));
                    },
                    () =>
                    {
                        _size.Changed += SizeChanged;
                    }))
                {
                    SizeChanged();
                }
            }
        }

        private void SizeChanged()
        {
            OnPropertiesChanged(nameof(Width), nameof(Height));
            InvalidateLayout();
        }

        protected EventVec2 _actualSize = new EventVec2();
        [Browsable(false)]
        public EventVec2 ActualSize
        {
            get => _actualSize;
            set => Set(ref _actualSize, value ?? new EventVec2());
        }
        [Browsable(false)]
        public float ActualWidth => ActualSize.X;
        [Browsable(false)]
        public float ActualHeight => ActualSize.Y;

        [Category("Transform")]
        public virtual float MinWidth
        {
            get => _minSize.X;
            set => _minSize.X = value;
        }
        [Category("Transform")]
        public virtual float MinHeight
        {
            get => _minSize.Y;
            set => _minSize.Y = value;
        }
        [TSerialize]
        [Browsable(false)]
        public EventVec2 MinSize
        {
            get => _minSize;
            set
            {
                if (Set(ref _minSize, value,
                    () => _minSize.Changed -= MinSizeChanged,
                    () => _minSize.Changed += MinSizeChanged))
                    MinSizeChanged();
            }
        }

        private void MinSizeChanged()
        {
            OnPropertyChanged(nameof(MinWidth));
            OnPropertyChanged(nameof(MaxWidth));
            InvalidateLayout();
        }

        [Category("Transform")]
        public virtual float MaxWidth
        {
            get => _maxSize.X;
            set => _maxSize.X = value;
        }
        [Category("Transform")]
        public virtual float MaxHeight
        {
            get => _maxSize.Y;
            set => _maxSize.Y = value;
        }
        [TSerialize]
        [Browsable(false)]
        public EventVec2 MaxSize
        {
            get => _maxSize;
            set
            {
                if (Set(ref _maxSize, value,
                    () => _maxSize.Changed -= MaxSizeChanged,
                    () => _maxSize.Changed += MaxSizeChanged))
                    MaxSizeChanged();
            }
        }

        private void MaxSizeChanged()
        {
            OnPropertyChanged(nameof(MaxWidth));
            OnPropertyChanged(nameof(MaxHeight));
            InvalidateLayout();
        }

        [Category("Transform")]
        public virtual float OriginPercentX
        {
            get => _originPercent.X;
            set => _originPercent.X = value;
        }
        [Category("Transform")]
        public virtual float OriginPercentY
        {
            get => _originPercent.Y;
            set => _originPercent.Y = value;
        }
        [TSerialize]
        [Browsable(false)]
        public EventVec2 OriginPercent
        {
            get => _originPercent;
            set
            {
                if (Set(ref _originPercent, value,
                    () => _originPercent.Changed -= OriginPercentChanged,
                    () => _originPercent.Changed += OriginPercentChanged))
                    OriginPercentChanged();
            }
        }

        private void OriginPercentChanged()
        {
            OnPropertyChanged(nameof(OriginPercentX));
            OnPropertyChanged(nameof(OriginPercentY));
            OnPropertyChanged(nameof(OriginTranslationX));
            OnPropertyChanged(nameof(OriginTranslationY));
            InvalidateLayout();
        }

        [TSerialize]
        [Category("Transform")]
        public virtual EventVec4 Margins
        {
            get => _margins;
            set
            {
                if (Set(ref _margins, value,
                    () => _margins.Changed -= InvalidateLayout,
                    () => _margins.Changed += InvalidateLayout))
                    InvalidateLayout();
            }
        }
        [TSerialize]
        [Category("Transform")]
        public virtual EventVec4 Padding
        {
            get => _padding;
            set
            {
                if (Set(ref _padding, value,
                    () => _padding.Changed -= InvalidateLayout,
                    () => _padding.Changed += InvalidateLayout))
                    InvalidateLayout();
            }
        }
        [Category("Transform")]
        public virtual float OriginTranslationX
        {
            get => OriginPercentX * ActualWidth;
            set => OriginPercentX = value / ActualWidth;
        }
        [Category("Transform")]
        public virtual float OriginTranslationY
        {
            get => OriginPercentY * ActualHeight;
            set => OriginPercentY = value / ActualHeight;
        }
        [Browsable(false)]
        public Vec2 OriginTranslation
        {
            get => OriginPercent.Raw * ActualSize.Raw;
            set => OriginPercent.Raw = value / ActualSize.Raw;
        }

        public bool Contains(Vec2 worldPoint)
        {
            Vec3 localPoint = worldPoint * InverseActorRelativeMatrix;
            return ActualSize.Raw.Contains(localPoint.Xy);
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
            return Math.Abs(localPoint.Z) < zMargin && ActualSize.Raw.Contains(localPoint.Xy);
        }

        protected override void OnRecalcLocalTransform(
            out Matrix4 localTransform,
            out Matrix4 inverseLocalTransform)
        {
            base.OnRecalcLocalTransform(out localTransform, out inverseLocalTransform);

            localTransform *= Matrix4.CreateTranslation(-OriginTranslationX, -OriginTranslationY, 0.0f);
            inverseLocalTransform = Matrix4.CreateTranslation(OriginTranslationX, OriginTranslationY, 0.0f) * inverseLocalTransform;
        }
        protected override void OnResizeActual(BoundingRectangleF parentBounds)
        {
            switch (HorizontalAlignment)
            {
                case EHorizontalAlign.Stretch:
                    _actualSize.X = parentBounds.Width;
                    _actualTranslation.X = 0.0f;
                    break;
                case EHorizontalAlign.Left:
                    _actualSize.X = Width;
                    _actualTranslation.X = 0.0f;
                    break;
                case EHorizontalAlign.Right:
                    _actualSize.X = Width;
                    _actualTranslation.X = parentBounds.Width - Width;
                    break;
                case EHorizontalAlign.Center:
                    _actualSize.X = Width;
                    float extra = parentBounds.Width - Width;
                    _actualTranslation.X = extra * 0.5f;
                    break;
                case EHorizontalAlign.Positional:
                    _actualSize.X = Width;
                    _actualTranslation.X = _translation.X;
                    break;
            }

            switch (VerticalAlignment)
            {
                case EVerticalAlign.Stretch:
                    _actualSize.Y = parentBounds.Height;
                    _actualTranslation.Y = 0.0f;
                    break;
                case EVerticalAlign.Bottom:
                    _actualSize.Y = Height;
                    _actualTranslation.Y = 0.0f;
                    break;
                case EVerticalAlign.Top:
                    _actualSize.Y = Height;
                    _actualTranslation.Y = parentBounds.Height - Height;
                    break;
                case EVerticalAlign.Center:
                    _actualSize.Y = Height;
                    float extra = parentBounds.Height - Height;
                    _actualTranslation.Y = extra * 0.5f;
                    break;
                case EVerticalAlign.Positional:
                    _actualSize.Y = Height;
                    _actualTranslation.Y = _translation.Y;
                    break;
            }
        }
        protected override void OnResizeLayout(BoundingRectangleF parentBounds)
        {
            OnResizeActual(parentBounds);
            RecalcLocalTransform(true, false);

            Vec4 pad = Padding.Raw;
            Vec2 size = ActualSize.Raw;
            Vec2 pos = ActualTranslation.Raw;
            pos += new Vec2(pad.X, pad.Y);
            size -= new Vec2(pad.X + pad.Z, pad.Y + pad.W);
            var bounds = new BoundingRectangleF(pos, size);

            OnResizeChildComponents(bounds);
            RemakeAxisAlignedRegion();
        }
        protected virtual void RemakeAxisAlignedRegion()
        {
            Matrix4 mtx = WorldMatrix * Matrix4.CreateScale(ActualSize.X, ActualSize.Y, 1.0f);

            Vec3 minPos = Vec3.TransformPosition(Vec3.Zero, mtx);
            Vec3 maxPos = Vec3.TransformPosition(Vec2.One, mtx); //This is Vec2.One on purpose, we only want Z to be 0

            Vec2 min = new Vec2(Math.Min(minPos.X, maxPos.X), Math.Min(minPos.Y, maxPos.Y));
            Vec2 max = new Vec2(Math.Max(minPos.X, maxPos.X), Math.Max(minPos.Y, maxPos.Y));

            RenderInfo.AxisAlignedRegion = BoundingRectangleF.FromMinMaxSides(min.X, max.X, min.Y, max.Y, 0.0f, 0.0f);
            //Engine.PrintLine($"Axis-aligned region remade: {_axisAlignedRegion.Translation} {_axisAlignedRegion.Extents}");
        }
        public IUIComponent FindDeepestComponent(Vec2 worldPoint, bool includeThis)
        {
            foreach (ISceneComponent c in _children)
            {
                if (c is IUIBoundableComponent uiComp)
                {
                    IUIComponent comp = uiComp.FindDeepestComponent(worldPoint, true);
                    if (comp != null)
                        return comp;
                }
            }

            if (includeThis && Contains(worldPoint))
                return this;

            return null;
        }
        public List<IUIBoundableComponent> FindAllIntersecting(Vec2 worldPoint, bool includeThis)
        {
            List<IUIBoundableComponent> list = new List<IUIBoundableComponent>();
            FindAllIntersecting(worldPoint, includeThis, list);
            return list;
        }
        public void FindAllIntersecting(Vec2 worldPoint, bool includeThis, List<IUIBoundableComponent> results)
        {
            foreach (ISceneComponent c in _children)
                if (c is IUIBoundableComponent uiComp)
                    uiComp.FindAllIntersecting(worldPoint, true, results);

            if (includeThis && Contains(worldPoint))
                results.Add(this);
        }

        protected override void OnChildAdded(ISceneComponent item)
        {
            base.OnChildAdded(item);
            if (item is IUIComponent c)
                c.RenderInfo.LayerIndex = RenderInfo.LayerIndex;
        }

        public virtual float GetHeight()
            => Height.Clamp(MinHeight, MaxHeight);

        public virtual float GetWidth()
            => Width.Clamp(MinWidth, MaxWidth);
    }
}

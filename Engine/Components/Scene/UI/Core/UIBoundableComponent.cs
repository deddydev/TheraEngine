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
        
        float XOriginPercent { get; set; }
        float YOriginPercent { get; set; }
        EventVec2 OriginPercent { get; set; }
        
        float OriginTranslationX { get; set; }
        float OriginTranslationY { get; set; }
        Vec2 OriginTranslation { get; set; }
        
        Vec2 ActualSize { get; }

        IUIComponent FindDeepestComponent(Vec2 worldPoint, bool includeThis);
        List<IUIBoundableComponent> FindAllIntersecting(Vec2 worldPoint, bool includeThis);
        void FindAllIntersecting(Vec2 worldPoint, bool includeThis, List<IUIBoundableComponent> results);
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
        public UIBoundableComponent() : base() { }

        private bool _isMouseOver = false;

        [TSerialize(nameof(OriginPercent))]
        private EventVec2 _originPercent = new EventVec2();

        [TSerialize(nameof(Size))]
        private EventVec2 _size = new EventVec2();

        [TSerialize(nameof(MinSize))]
        private EventVec2 _minSize = new EventVec2();

        [TSerialize(nameof(MaxSize))]
        private EventVec2 _maxSize = new EventVec2();

        [TSerialize(nameof(Margins))]
        private EventVec4 _margins = new EventVec4();

        [TSerialize(nameof(Padding))]
        private EventVec4 _padding = new EventVec4();
        
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
        [Browsable(false)]
        public EventVec2 Size
        {
            get => _size;
            set
            {
                if (Set(ref _size, value,
                    () => _size.PropertyChanged -= SizingPropertyChanged,
                    () => _size.PropertyChanged += SizingPropertyChanged))
                    InvalidateLayout();
            }
        }

        protected EventVec2 _actualSize = new EventVec2();
        [Browsable(false)]
        public Vec2 ActualSize
        {
            get => _actualSize.Raw;
            set => _actualSize.Raw = value;
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
        [Browsable(false)]
        public EventVec2 MinSize
        {
            get => _minSize;
            set
            {
                if (Set(ref _minSize, value,
                    () => _minSize.PropertyChanged -= SizingPropertyChanged,
                    () => _minSize.PropertyChanged += SizingPropertyChanged))
                    InvalidateLayout();
            }
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
        [Browsable(false)]
        public EventVec2 MaxSize
        {
            get => _maxSize;
            set
            {
                if (Set(ref _maxSize, value,
                    () => _maxSize.PropertyChanged -= SizingPropertyChanged,
                    () => _maxSize.PropertyChanged += SizingPropertyChanged))
                    InvalidateLayout();
            }
        }
        [Category("Transform")]
        public virtual float XOriginPercent
        {
            get => _originPercent.X;
            set => _originPercent.X = value;
        }
        [Category("Transform")]
        public virtual float YOriginPercent
        {
            get => _originPercent.Y;
            set => _originPercent.Y = value;
        }
        [Browsable(false)]
        public EventVec2 OriginPercent
        {
            get => _originPercent;
            set
            {
                if (Set(ref _originPercent, value,
                    () => _originPercent.PropertyChanged -= SizingPropertyChanged,
                    () => _originPercent.PropertyChanged += SizingPropertyChanged))
                    InvalidateLayout();
            }
        }
        [Category("Transform")]
        public virtual EventVec4 Margins
        {
            get => _margins;
            set
            {
                if (Set(ref _margins, value,
                    () => _margins.PropertyChanged -= SizingPropertyChanged,
                    () => _margins.PropertyChanged += SizingPropertyChanged))
                    InvalidateLayout();
            }
        }
        [Category("Transform")]
        public virtual EventVec4 Padding
        {
            get => _padding;
            set
            {
                if (Set(ref _padding, value,
                    () => _padding.PropertyChanged -= SizingPropertyChanged,
                    () => _padding.PropertyChanged += SizingPropertyChanged))
                    InvalidateLayout();
            }
        }
        [Category("Transform")]
        public virtual float OriginTranslationX
        {
            get => XOriginPercent * ActualWidth;
            set => XOriginPercent = value / ActualWidth;
        }
        [Category("Transform")]
        public virtual float OriginTranslationY
        {
            get => YOriginPercent * ActualHeight;
            set => YOriginPercent = value / ActualHeight;
        }
        [Browsable(false)]
        public Vec2 OriginTranslation
        {
            get => OriginPercent.Raw * ActualSize;
            set => OriginPercent.Raw = value / ActualSize;
        }

        public bool Contains(Vec2 worldPoint)
        {
            Vec3 localPoint = worldPoint * InverseActorRelativeMatrix;
            return ActualSize.Contains(localPoint.Xy);
        }

        private void SizingPropertyChanged(object sender, PropertyChangedEventArgs e) => InvalidateLayout();
        
        /// <summary>
        /// Returns true if the given world point projected perpendicularly to the HUD as a 2D point is contained within this component and the Z value is within the given depth margin.
        /// </summary>
        /// <param name="worldPoint"></param>
        /// <param name="zMargin">How far away the point can be on either side of the HUD for it to be considered close enough.</param>
        /// <returns></returns>
        public bool Contains(Vec3 worldPoint, float zMargin = 0.5f)
        {
            Vec3 localPoint = Vec3.TransformPosition(worldPoint, InverseWorldMatrix);
            return Math.Abs(localPoint.Z) < zMargin && ActualSize.Contains(localPoint.Xy);
        }

        protected override void OnRecalcLocalTransform(
            out Matrix4 localTransform,
            out Matrix4 inverseLocalTransform)
        {
            base.OnRecalcLocalTransform(out localTransform, out inverseLocalTransform);

            localTransform *= Matrix4.CreateTranslation(-OriginTranslationX, -OriginTranslationY, 0.0f);
            inverseLocalTransform = Matrix4.CreateTranslation(OriginTranslationX, OriginTranslationY, 0.0f) * inverseLocalTransform;
        }
        public override void RecalcWorldTransform()
        {
            base.RecalcWorldTransform();
            RemakeAxisAlignedRegion();
        }
        protected override void OnResizeLayout(BoundingRectangleF parentBounds)
        {
            switch (HorizontalAlignment)
            {
                case EHorizontalAlign.Stretch:
                    _actualSize.X = parentBounds.Width;
                    _actualTranslation.X = parentBounds.Translation.X;
                    break;
                case EHorizontalAlign.Left:
                    _actualSize.X = Width;
                    _actualTranslation.X = parentBounds.Translation.X;
                    break;
                case EHorizontalAlign.Right:
                    _actualSize.X = Width;
                    _actualTranslation.X = parentBounds.Translation.X + parentBounds.X - Width;
                    break;
                case EHorizontalAlign.Center:
                    _actualSize.X = Width;
                    float extra = parentBounds.X - Width;
                    _actualTranslation.X = parentBounds.Translation.X + extra * 0.5f;
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
                    _actualTranslation.Y = parentBounds.Translation.Y;
                    break;
                case EVerticalAlign.Bottom:
                    _actualSize.Y = Height;
                    _actualTranslation.X = parentBounds.Translation.Y;
                    break;
                case EVerticalAlign.Top:
                    _actualSize.Y = Height;
                    _actualTranslation.Y = parentBounds.Translation.Y + parentBounds.Y - Height;
                    break;
                case EVerticalAlign.Center:
                    _actualSize.Y = Height;
                    float extra = parentBounds.Y - Height;
                    _actualTranslation.Y = parentBounds.Translation.Y + extra * 0.5f;
                    break;
                case EVerticalAlign.Positional:
                    _actualSize.Y = Height;
                    _actualTranslation.Y = _translation.Y;
                    break;
            }

            base.OnResizeLayout(new BoundingRectangleF(ActualTranslation, ActualSize));

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
    }
}

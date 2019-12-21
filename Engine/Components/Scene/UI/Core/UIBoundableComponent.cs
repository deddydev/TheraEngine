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
        float Width { get; set; }
        float Height { get; set; }
        Vec2 Size { get; set; }

        float MinWidth { get; set; }
        float MinHeight { get; set; }

        float MaxWidth { get; set; }
        float MaxHeight { get; set; }

        float XOriginPercent { get; set; }
        float YOriginPercent { get; set; }

        EVerticalAlign VerticalAlignment { get; set; }
        EHorizontalAlign HorizontalAlignment { get; set; }

        Vec2 OriginTranslation { get; set; }

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

        [TSerialize(nameof(XOriginPercent))]
        private float _xOriginPercent = 0.0f;
        [TSerialize(nameof(YOriginPercent))]
        private float _yOriginPercent = 0.0f;

        [TSerialize(nameof(Width))]
        private float _width = 0.0f;
        [TSerialize(nameof(Height))]
        private float _height = 0.0f;

        [TSerialize(nameof(MinWidth))]
        private float _minWidth = 0.0f;
        [TSerialize(nameof(MinHeight))]
        private float _minHeight = 0.0f;

        [TSerialize(nameof(MaxWidth))]
        private float _maxWidth = 0.0f;
        [TSerialize(nameof(MaxHeight))]
        private float _maxHeight = 0.0f;

        [TSerialize(nameof(Margins))]
        private EventVec4 _margins = new EventVec4();
        [TSerialize(nameof(Padding))]
        private EventVec4 _padding = new EventVec4();
        
        [TSerialize(nameof(HorizontalAlignment))]
        private EHorizontalAlign _horizontalAlign = EHorizontalAlign.Stretch;
        [TSerialize(nameof(VerticalAlignment))]
        private EVerticalAlign _verticalAlign = EVerticalAlign.Stretch;

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
            get => _width;
            set
            {
                if (Set(ref _width, value))
                    InvalidateLayout();
            }
        }
        [Category("Transform")]
        public virtual float Height
        {
            get => _height;
            set
            {
                if (Set(ref _height, value))
                    InvalidateLayout();
            }
        }
        [Category("Transform")]
        public virtual float MinWidth
        {
            get => _minWidth;
            set
            {
                if (Set(ref _minWidth, value))
                    InvalidateLayout();
            }
        }
        [Category("Transform")]
        public virtual float MinHeight
        {
            get => _minHeight;
            set
            {
                if (Set(ref _minHeight, value))
                    InvalidateLayout();
            }
        }
        [Category("Transform")]
        public virtual float MaxWidth
        {
            get => _maxWidth;
            set
            {
                if (Set(ref _maxWidth, value))
                    InvalidateLayout();
            }
        }
        [Category("Transform")]
        public virtual float MaxHeight
        {
            get => _maxHeight;
            set
            {
                if (Set(ref _maxHeight, value))
                    InvalidateLayout();
            }
        }
        [Browsable(false)]
        public Vec2 Size
        {
            get => new Vec2(Width, Height);
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }
        [Category("Transform")]
        public virtual float XOriginPercent
        {
            get => _xOriginPercent;
            set
            {
                if (Set(ref _xOriginPercent, value))
                    InvalidateLayout();
            }
        }
        [Category("Transform")]
        public virtual float YOriginPercent
        {
            get => _yOriginPercent;
            set
            {
                if (Set(ref _yOriginPercent, value))
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
        [Browsable(false)]
        public Vec2 OriginPercent
        {
            get => new Vec2(XOriginPercent, YOriginPercent);
            set
            {
                XOriginPercent = value.X;
                YOriginPercent = value.Y;
            }
        }
        [Category("Transform")]
        public virtual float OriginX
        {
            get => XOriginPercent * Width;
            set => XOriginPercent = value / Width;
        }
        [Category("Transform")]
        public virtual float OriginY
        {
            get => YOriginPercent * Height;
            set => YOriginPercent = value / Height;
        }
        [Browsable(false)]
        public Vec2 OriginTranslation
        {
            get => new Vec2(XOriginPercent * Width, YOriginPercent * Height);
            set
            {
                XOriginPercent = value.X / Width;
                YOriginPercent = value.Y / Height;
            }
        }

        public bool Contains(Vec2 worldPoint)
        {
            Vec3 localPoint = worldPoint * InverseActorRelativeTransform;
            return Size.Contains(localPoint.Xy);
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
            return Math.Abs(localPoint.Z) < zMargin && Size.Contains(localPoint.Xy);
        }

        protected override void OnRecalcLocalTransform(
            out Matrix4 localTransform,
            out Matrix4 inverseLocalTransform)
        {
            base.OnRecalcLocalTransform(out localTransform, out inverseLocalTransform);

            localTransform *= Matrix4.CreateTranslation(-OriginX, -OriginY, 0.0f);
            inverseLocalTransform = Matrix4.CreateTranslation(OriginX, OriginY, 0.0f) * inverseLocalTransform;
        }
        public override void RecalcWorldTransform()
        {
            base.RecalcWorldTransform();
            RemakeAxisAlignedRegion();
        }
        public override void ArrangeChildren(Vec2 translation, Vec2 parentBounds)
        {
            base.ArrangeChildren(translation, parentBounds);
            RemakeAxisAlignedRegion();
        }
        protected virtual void RemakeAxisAlignedRegion()
        {
            Matrix4 mtx = WorldMatrix * Matrix4.CreateScale(Size.X, Size.Y, 1.0f);

            Vec3 minPos = Vec3.TransformPosition(Vec3.Zero, mtx);
            Vec3 maxPos = Vec3.TransformPosition(Vec2.One, mtx); //This is Vec2.One on purpose, we only want Z to be 0

            Vec2 min = new Vec2(Math.Min(minPos.X, maxPos.X), Math.Min(minPos.Y, maxPos.Y));
            Vec2 max = new Vec2(Math.Max(minPos.X, maxPos.X), Math.Max(minPos.Y, maxPos.Y));

            RenderInfo.AxisAlignedRegion = BoundingRectangleFStruct.FromMinMaxSides(min.X, max.X, min.Y, max.Y, 0.0f, 0.0f);
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

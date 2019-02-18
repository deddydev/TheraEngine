using System;
using System.ComponentModel;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.UI
{
    public enum EUIDockStyle
    {
        None,
        Fill,
        Left,
        Right,
        Top,
        Bottom,
    }
    //[Flags]
    //public enum EAnchorFlags
    //{
    //    None,
    //    Left,
    //    Right,
    //    Top,
    //    Bottom,
    //}
    /// <summary>
    ///Component that uses a variety of positioning parameters
    ///to calculate the region's position and dimensions upon resize.
    /// </summary>
    public class UIDockableComponent : UIBoundableComponent
    {
        /// <summary>
        /// The X value of the right boundary line.
        /// Only moves the right edge by resizing width.
        /// </summary>
        [Category("Transform")]
        public float MaxX
        {
            get => LocalTranslationX + (Width < 0 ? 0 : Width);
            //set
            //{
            //    CheckProperDimensions();
            //    _size.X = value - LocalTranslationX;
            //    PerformResize();
            //}
        }
        /// <summary>
        /// The Y value of the top boundary line.
        /// Only moves the top edge by resizing height.
        /// </summary>
        [Category("Transform")]
        public float MaxY
        {
            get => LocalTranslationY + (Height < 0 ? 0 : Height);
            //set
            //{
            //    CheckProperDimensions();
            //    _size.Y = value - LocalTranslationY;
            //    PerformResize();
            //}
        }
        /// <summary>
        /// The X value of the left boundary line.
        /// Only moves the left edge by resizing width.
        /// </summary>
        [Category("Transform")]
        public float MinX
        {
            get => LocalTranslationX + (Width < 0 ? Width : 0);
            //set
            //{
            //    CheckProperDimensions();
            //    float origX = _size.X;
            //    _translation.X = value;
            //    _size.X = origX - LocalTranslationX;
            //    PerformResize();
            //}
        }
        /// <summary>
        /// The Y value of the bottom boundary line.
        /// Only moves the bottom edge by resizing height.
        /// </summary>
        [Category("Transform")]
        public float MinY
        {
            get => LocalTranslationY + (Height < 0 ? Height : 0);
            //set
            //{
            //    CheckProperDimensions();
            //    float origY = _size.Y;
            //    _translation.Y = value;
            //    _size.Y = origY - LocalTranslationY;
            //    PerformResize();
            //}
        }
        /// <summary>
        /// Checks that the width and height are positive values. Will move the location of the rectangle to fix this.
        /// </summary>
        //public void CheckProperDimensions()
        //{
        //    if (Width < 0)
        //    {
        //        LocalTranslationX += Width;
        //        Width = -Width;
        //    }
        //    if (Height < 0)
        //    {
        //        LocalTranslationY += Height;
        //        Height = -Height;
        //    }
        //}
        //[Category("Transform")]
        //public override Vec2 Size
        //{
        //    get => base.Size;
        //    set
        //    {
        //        SizeableWidth.SetModificationValueNoUpdate(value.X);
        //        SizeableHeight.SetModificationValueNoUpdate(value.Y);
        //        PerformResize();
        //    }
        //}
        //[Browsable(false)]
        //public override float Width
        //{
        //    get => SizeableWidth.ModificationValue;
        //    set => SizeableWidth.ModificationValue = value;
        //}
        //[Browsable(false)]
        //public override float Height
        //{
        //    get => SizeableHeight.ModificationValue;
        //    set => SizeableHeight.ModificationValue = value;
        //}
        //[Category("Transform")]
        //public override Vec2 LocalTranslation
        //{
        //    get => new Vec2(SizeablePosX.ModificationValue, SizeablePosY.ModificationValue);
        //    set
        //    {
        //        SizeablePosX.SetModificationValueNoUpdate(value.X);
        //        SizeablePosY.SetModificationValueNoUpdate(value.Y);
        //        PerformResize();
        //    }
        //}
        //[Browsable(false)]
        //public override float LocalTranslationX
        //{
        //    get => SizeablePosX.ModificationValue;
        //    set => SizeablePosX.ModificationValue = value;
        //}
        //[Browsable(false)]
        //public override float LocalTranslationY
        //{
        //    get => SizeablePosY.ModificationValue;
        //    set => SizeablePosY.ModificationValue = value;
        //}
        public UIDockableComponent() : base()
        {
            SizeableHeight.ParameterChanged += PerformResize;
            SizeableWidth.ParameterChanged += PerformResize;
            SizeablePosX.ParameterChanged += PerformResize;
            SizeablePosY.ParameterChanged += PerformResize;
            _sizeableElements = new ISizeable[]
            {
                SizeableWidth,
                SizeableHeight,
                SizeablePosX,
                SizeablePosY,
                Padding,
                Anchor,
            };
        }
        
        private readonly ISizeable[] _sizeableElements;

        [Category("Transform")]
        public SizeableElement SizeableWidth { get; } = new SizeableElement() { ParentBoundsInherited = EParentBoundsInheritedValue.Width };
        [Category("Transform")]
        public SizeableElement SizeableHeight { get; } = new SizeableElement() { ParentBoundsInherited = EParentBoundsInheritedValue.Height };
        [Category("Transform")]
        public SizeableElement SizeablePosX { get; } = new SizeableElement() { ParentBoundsInherited = EParentBoundsInheritedValue.Width };
        [Category("Transform")]
        public SizeableElement SizeablePosY { get; } = new SizeableElement() { ParentBoundsInherited = EParentBoundsInheritedValue.Height };
        [Category("Transform")]
        protected SizeableElementQuad Padding { get; } = new SizeableElementQuad();
        [Category("Transform")]
        protected SizeableElementQuad Anchor { get; } = new SizeableElementQuad();

        private EUIDockStyle _dockStyle = EUIDockStyle.None;
        //private EAnchorFlags _anchorFlags = EAnchorFlags.None;

        [Category("Transform")]
        public EUIDockStyle DockStyle
        {
            get => _dockStyle;
            set
            {
                IgnoreResizes = true;
                switch (value)
                {
                    case EUIDockStyle.None:
                        SizeablePosX.SetSizingPixels(SizeablePosX.GetValue(ParentBounds));
                        SizeablePosY.SetSizingPixels(SizeablePosY.GetValue(ParentBounds));
                        SizeableWidth.SetSizingPixels(SizeableWidth.GetValue(ParentBounds));
                        SizeableHeight.SetSizingPixels(SizeableHeight.GetValue(ParentBounds));
                        break;
                    case EUIDockStyle.Fill:
                        SizeablePosX.SetSizingPixels(0.0f);
                        SizeablePosY.SetSizingPixels(0.0f);
                        SizeableWidth.SetSizingPercentageOfParent(1.0f);
                        SizeableHeight.SetSizingPercentageOfParent(1.0f);
                        break;
                    case EUIDockStyle.Left:
                        SizeablePosX.SetSizingPixels(0.0f);
                        SizeablePosY.SetSizingPixels(0.0f);
                        SizeableWidth.SetSizingPercentageOfParent(1.0f);
                        SizeableHeight.SetSizingPercentageOfParent(1.0f);
                        break;
                    case EUIDockStyle.Right:
                        SizeablePosX.SetSizingPixels(0.0f);
                        SizeablePosY.SetSizingPixels(0.0f);
                        SizeableWidth.SetSizingPercentageOfParent(1.0f);
                        SizeableHeight.SetSizingPercentageOfParent(1.0f);
                        break;
                    case EUIDockStyle.Top:
                        SizeablePosX.SetSizingPixels(0.0f);
                        SizeablePosY.SetSizingPixels(0.0f);
                        SizeableWidth.SetSizingPercentageOfParent(1.0f);
                        SizeableHeight.SetSizingPercentageOfParent(1.0f);
                        break;
                    case EUIDockStyle.Bottom:
                        SizeablePosX.SetSizingPixels(0.0f);
                        SizeablePosY.SetSizingPixels(0.0f);
                        SizeableWidth.SetSizingPercentageOfParent(1.0f);
                        SizeableHeight.SetSizingPercentageOfParent(1.0f);
                        break;
                }
                IgnoreResizes = false;
                _dockStyle = value;
                PerformResize();
            }
        }
        //[Category("Transform")]
        //public EAnchorFlags SideAnchorFlags
        //{
        //    get => _anchorFlags;
        //    set
        //    {
        //        _anchorFlags = value;
        //        PerformResize();
        //    }
        //}

        [Browsable(false)]
        public bool Docked => _dockStyle != EUIDockStyle.None;

        //[Browsable(false)]
        //public bool Anchored => _anchorFlags != EAnchorFlags.None;

        //[Browsable(false)]
        //public bool AnchoredBottom
        //{
        //    get => (_anchorFlags & EAnchorFlags.Bottom) != 0;
        //    set
        //    {
        //        if (value == AnchoredBottom)
        //            return;
        //        if (value)
        //            _anchorFlags |= EAnchorFlags.Bottom;
        //        else
        //            _anchorFlags &= ~EAnchorFlags.Bottom;
        //        PerformResize();
        //    }
        //}
        //[Browsable(false)]
        //public bool AnchoredTop
        //{
        //    get => (_anchorFlags & EAnchorFlags.Top) != 0;
        //    set
        //    {
        //        if (value == AnchoredTop)
        //            return;
        //        if (value)
        //            _anchorFlags |= EAnchorFlags.Top;
        //        else
        //            _anchorFlags &= ~EAnchorFlags.Top;
        //        PerformResize();
        //    }
        //}
        //[Browsable(false)]
        //public bool AnchoredLeft
        //{
        //    get => (_anchorFlags & EAnchorFlags.Left) != 0;
        //    set
        //    {
        //        if (value == AnchoredLeft)
        //            return;
        //        if (value)
        //            _anchorFlags |= EAnchorFlags.Left;
        //        else
        //            _anchorFlags &= ~EAnchorFlags.Left;
        //        PerformResize();
        //    }
        //}
        //[Browsable(false)]
        //public bool AnchoredRight
        //{
        //    get => (_anchorFlags & EAnchorFlags.Right) != 0;
        //    set
        //    {
        //        if (value == AnchoredRight)
        //            return;
        //        if (value)
        //            _anchorFlags |= EAnchorFlags.Right;
        //        else
        //            _anchorFlags &= ~EAnchorFlags.Right;
        //        PerformResize();
        //    }
        //}

        public override unsafe Vec2 Resize(Vec2 parentBounds)
        {
            if (IgnoreResizes)
                return parentBounds;
            IgnoreResizes = true;

            ParentBounds = parentBounds;
            Vec2 leftOver = parentBounds;
            Vec2 prevRegion = Size;
            
            _size.X = SizeableWidth.GetValue(parentBounds);
            _size.Y = SizeableHeight.GetValue(parentBounds);
            _translation.X = SizeablePosX.GetValue(parentBounds);
            _translation.Y = SizeablePosY.GetValue(parentBounds);
            
            RecalcLocalTransform();

            Vec2 bounds = Size;
            foreach (UIComponent c in _children)
                bounds = c.Resize(bounds);

            IgnoreResizes = false;

            return leftOver;
        }
        //private BoundingRectangleF RegionDockComplement(BoundingRectangleF parentRegion, BoundingRectangleF region)
        //{
        //    if (parentRegion.MaxX != region.MaxX)
        //        return BoundingRectangleF.FromMinMaxSides(
        //            region.MaxX, parentRegion.MaxX,
        //            parentRegion.MinY, parentRegion.MaxY,
        //            0.0f, 0.0f);
        //    if (parentRegion.MinX != region.MinX)
        //        return BoundingRectangleF.FromMinMaxSides(
        //            parentRegion.MinX, region.MinX,
        //            parentRegion.MinY, parentRegion.MaxY,
        //            0.0f, 0.0f);
        //    if (parentRegion.MaxY != region.MaxY)
        //        return BoundingRectangleF.FromMinMaxSides(
        //            parentRegion.MinX, parentRegion.MaxX,
        //            region.MaxY, parentRegion.MaxY,
        //            0.0f, 0.0f);
        //    if (parentRegion.MinY != region.MinY)
        //        return BoundingRectangleF.FromMinMaxSides(
        //            parentRegion.MinX, parentRegion.MaxX,
        //            parentRegion.MinY, region.MinY,
        //            0.0f, 0.0f);
        //    return BoundingRectangleF.Empty;
        //}
    }
}

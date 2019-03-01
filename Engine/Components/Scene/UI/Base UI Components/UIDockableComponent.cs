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
    public class EditInPlace : Attribute
    {
        public EditInPlace() { }
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
            get => BottomLeftTranslation.X + (Width < 0 ? 0 : Width);
            set
            {
                CheckProperDimensions();
                Width = value - BottomLeftTranslation.X;
            }
        }
        /// <summary>
        /// The Y value of the top boundary line.
        /// Only moves the top edge by resizing height.
        /// </summary>
        [Category("Transform")]
        public float MaxY
        {
            get => BottomLeftTranslation.Y + (Height < 0 ? 0 : Height);
            set
            {
                CheckProperDimensions();
                Height = value - BottomLeftTranslation.Y;
            }
        }
        /// <summary>
        /// The X value of the left boundary line.
        /// Only moves the left edge by resizing width.
        /// </summary>
        [Category("Transform")]
        public float MinX
        {
            get => BottomLeftTranslation.X + (Width < 0 ? Width : 0);
            set
            {
                CheckProperDimensions();
                float origX = MaxX;
                SizeablePosX.SetResultingValueNoUpdate(value, ParentBounds);
                Width = origX - BottomLeftTranslation.X;
            }
        }
        /// <summary>
        /// The Y value of the bottom boundary line.
        /// Only moves the bottom edge by resizing height.
        /// </summary>
        [Category("Transform")]
        public float MinY
        {
            get => BottomLeftTranslation.Y + (Height < 0 ? Height : 0);
            set
            {
                CheckProperDimensions();
                float origY = MaxY;
                SizeablePosY.SetResultingValueNoUpdate(value, ParentBounds);
                Height = origY - BottomLeftTranslation.Y;
            }
        }
        /// <summary>
        /// Checks that the width and height are positive values. Will move the location of the rectangle to fix this.
        /// </summary>
        public void CheckProperDimensions()
        {
            if (Width < 0)
            {
                LocalTranslationX += Width;
                Width = -Width;
            }
            if (Height < 0)
            {
                LocalTranslationY += Height;
                Height = -Height;
            }
        }
        [Category("Transform")]
        public override Vec2 Size
        {
            get => base.Size;
            set
            {
                SizeableWidth.SetResultingValueNoUpdate(value.X, ParentBounds);
                SizeableHeight.SetResultingValueNoUpdate(value.Y, ParentBounds);
                PerformResize();
            }
        }
        [Browsable(false)]
        public override float Width
        {
            get => SizeableWidth.GetResultingValue(ParentBounds);
            set => SizeableWidth.SetResultingValue(value, ParentBounds);
        }
        [Browsable(false)]
        public override float Height
        {
            get => SizeableHeight.GetResultingValue(ParentBounds);
            set => SizeableHeight.SetResultingValue(value, ParentBounds);
        }
        [Category("Transform")]
        public override Vec2 LocalTranslation
        {
            get => new Vec2(SizeablePosX.GetResultingValue(ParentBounds), SizeablePosY.GetResultingValue(ParentBounds));
            set
            {
                SizeablePosX.SetResultingValueNoUpdate(value.X, ParentBounds);
                SizeablePosY.SetResultingValueNoUpdate(value.Y, ParentBounds);
                PerformResize();
            }
        }
        [Browsable(false)]
        public override float LocalTranslationX
        {
            get => SizeablePosX.GetResultingValue(ParentBounds);
            set => SizeablePosX.SetResultingValue(value, ParentBounds);
        }
        [Browsable(false)]
        public override float LocalTranslationY
        {
            get => SizeablePosY.GetResultingValue(ParentBounds);
            set => SizeablePosY.SetResultingValue(value, ParentBounds);
        }
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
                //Padding,
                //Anchor,
            };
        }
        
        private readonly ISizeable[] _sizeableElements;

        //[EditInPlace]
        [Category("Transform")]
        public SizeableElement SizeableWidth { get; } = new SizeableElement() { ParentBoundsInherited = EParentBoundsInheritedValue.Width };
        //[EditInPlace]
        [Category("Transform")]
        public SizeableElement SizeableHeight { get; } = new SizeableElement() { ParentBoundsInherited = EParentBoundsInheritedValue.Height };
        //[EditInPlace]
        [Category("Transform")]
        public SizeableElement SizeablePosX { get; } = new SizeableElement() { ParentBoundsInherited = EParentBoundsInheritedValue.Width };
        //[EditInPlace]
        [Category("Transform")]
        public SizeableElement SizeablePosY { get; } = new SizeableElement() { ParentBoundsInherited = EParentBoundsInheritedValue.Height };
        //[EditInPlace]
        //[Category("Transform")]
        //public SizeableElementQuad Padding { get; } = new SizeableElementQuad();
        //[EditInPlace]
        //[Category("Transform")]
        //public SizeableElementQuad Anchor { get; } = new SizeableElementQuad();

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
                        SizeablePosX.SetSizingPixels(SizeablePosX.GetResultingValue(ParentBounds));
                        SizeablePosY.SetSizingPixels(SizeablePosY.GetResultingValue(ParentBounds));
                        SizeableWidth.SetSizingPixels(SizeableWidth.GetResultingValue(ParentBounds));
                        SizeableHeight.SetSizingPixels(SizeableHeight.GetResultingValue(ParentBounds));
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
        
        public override unsafe Vec2 Resize(Vec2 parentBounds)
        {
            if (IgnoreResizes)
                return parentBounds;
            IgnoreResizes = true;

            ParentBounds = parentBounds;
            Vec2 leftOver = parentBounds;
            Vec2 prevRegion = Size;
            
            _size.X = SizeableWidth.GetResultingValue(parentBounds);
            _size.Y = SizeableHeight.GetResultingValue(parentBounds);
            _translation.X = SizeablePosX.GetResultingValue(parentBounds);
            _translation.Y = SizeablePosY.GetResultingValue(parentBounds);
            
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

using System;
using System.Reflection;
using TheraEngine.Components;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.UI
{
    public enum UIDockStyle
    {
        None,
        Fill,
        Left,
        Right,
        Top,
        Bottom,
    }
    [Flags]
    public enum AnchorFlags
    {
        None,
        Left,
        Right,
        Top,
        Bottom,
    }
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
        public float MaxX
        {
            get => LocalTranslationX + (Width < 0 ? 0 : Width);
            set
            {
                CheckProperDimensions();
                _size.X = value - LocalTranslationX;
                PerformResize();
            }
        }
        /// <summary>
        /// The Y value of the top boundary line.
        /// Only moves the top edge by resizing height.
        /// </summary>
        public float MaxY
        {
            get => LocalTranslationY + (Height < 0 ? 0 : Height);
            set
            {
                CheckProperDimensions();
                _size.Y = value - LocalTranslationY;
                PerformResize();
            }
        }
        /// <summary>
        /// The X value of the left boundary line.
        /// Only moves the left edge by resizing width.
        /// </summary>
        public float MinX
        {
            get => LocalTranslationX + (Width < 0 ? Width : 0);
            set
            {
                CheckProperDimensions();
                float origX = _size.X;
                _translation.X = value;
                _size.X = origX - LocalTranslationX;
                PerformResize();
            }
        }
        /// <summary>
        /// The Y value of the bottom boundary line.
        /// Only moves the bottom edge by resizing height.
        /// </summary>
        public float MinY
        {
            get => LocalTranslationY + (Height < 0 ? Height : 0);
            set
            {
                CheckProperDimensions();
                float origY = _size.Y;
                _translation.Y = value;
                _size.Y = origY - LocalTranslationY;
                PerformResize();
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

        public override Vec2 Size
        {
            get => base.Size;
            set
            {
                SizeableWidth.CurrentValue = value.X;
                SizeableHeight.CurrentValue = value.Y;
                base.Size = value;
            }
        }
        public override float Width
        {
            get => base.Width;
            set
            {
                SizeableWidth.CurrentValue = value;
                base.Width = value;
            }
        }
        public override float Height
        {
            get => base.Height;
            set
            {
                SizeableHeight.CurrentValue = value;
                base.Height = value;
            }
        }
        public override Vec2 LocalTranslation
        {
            get => base.LocalTranslation;
            set
            {
                SizeablePosX.CurrentValue = value.X;
                SizeablePosY.CurrentValue = value.Y;
                base.LocalTranslation = value;
            }
        }
        public override float LocalTranslationX
        {
            get => base.LocalTranslationX;
            set
            {
                SizeablePosX.CurrentValue = value;
                base.LocalTranslationX = value;
            }
        }
        public override float LocalTranslationY
        {
            get => base.LocalTranslationY;
            set
            {
                SizeablePosY.CurrentValue = value;
                base.LocalTranslationY = value;
            }
        }
        public UIDockableComponent()
        {
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

        private ISizeable[] _sizeableElements;

        public SizeableElement SizeableWidth { get; } = new SizeableElement() { ParentBoundsInherited = ParentBoundsInheritedValue.Width };
        public SizeableElement SizeableHeight { get; } = new SizeableElement() { ParentBoundsInherited = ParentBoundsInheritedValue.Height };
        public SizeableElement SizeablePosX { get; } = new SizeableElement() { ParentBoundsInherited = ParentBoundsInheritedValue.Width };
        public SizeableElement SizeablePosY { get; } = new SizeableElement() { ParentBoundsInherited = ParentBoundsInheritedValue.Height };
        protected SizeableElementQuad Padding { get; } = new SizeableElementQuad();
        protected SizeableElementQuad Anchor { get; } = new SizeableElementQuad();

        private UIDockStyle _dockStyle = UIDockStyle.None;
        private AnchorFlags _anchorFlags = AnchorFlags.None;
        
        public UIDockStyle DockStyle
        {
            get => _dockStyle;
            set
            {
                _dockStyle = value;
                PerformResize();
            }
        }
        public AnchorFlags SideAnchorFlags
        {
            get => _anchorFlags;
            set
            {
                _anchorFlags = value;
                PerformResize();
            }
        }

        public bool Docked => _dockStyle != UIDockStyle.None;
        public bool Anchored => _anchorFlags != AnchorFlags.None;

        public bool AnchoredBottom
        {
            get => (_anchorFlags & AnchorFlags.Bottom) != 0;
            set
            {
                if (value == AnchoredBottom)
                    return;
                if (value)
                    _anchorFlags |= AnchorFlags.Bottom;
                else
                    _anchorFlags &= ~AnchorFlags.Bottom;
                PerformResize();
            }
        }
        public bool AnchoredTop
        {
            get => (_anchorFlags & AnchorFlags.Top) != 0;
            set
            {
                if (value == AnchoredTop)
                    return;
                if (value)
                    _anchorFlags |= AnchorFlags.Top;
                else
                    _anchorFlags &= ~AnchorFlags.Top;
                PerformResize();
            }
        }
        public bool AnchoredLeft
        {
            get => (_anchorFlags & AnchorFlags.Left) != 0;
            set
            {
                if (value == AnchoredLeft)
                    return;
                if (value)
                    _anchorFlags |= AnchorFlags.Left;
                else
                    _anchorFlags &= ~AnchorFlags.Left;
                PerformResize();
            }
        }
        public bool AnchoredRight
        {
            get => (_anchorFlags & AnchorFlags.Right) != 0;
            set
            {
                if (value == AnchoredRight)
                    return;
                if (value)
                    _anchorFlags |= AnchorFlags.Right;
                else
                    _anchorFlags &= ~AnchorFlags.Right;
                PerformResize();
            }
        }

        public override unsafe Vec2 Resize(Vec2 parentBounds)
        {
            Vec2 leftOver = parentBounds;
            Vec2 prevRegion = Size;

            //foreach (ISizeable s in _sizeableElements)
            //    s.Update(parentBounds);

            _size.X = SizeableWidth.GetValue(parentBounds);
            _size.Y = SizeableHeight.GetValue(parentBounds);
            _translation.X = SizeablePosX.GetValue(parentBounds);
            _translation.Y = SizeablePosY.GetValue(parentBounds);

            if (Docked || Anchored)
            {
                bool 
                    allowLeft = true, 
                    allowRight = true, 
                    allowTop = true, 
                    allowBottom = true;

                if (Docked)
                {
                    allowLeft = false;
                    allowRight = false;
                    allowTop = false;
                    allowBottom = false;
                    switch (_dockStyle)
                    {
                        case UIDockStyle.Fill:
                            _size = parentBounds;
                            _translation = Vec2.Zero;
                            break;
                        case UIDockStyle.Bottom:
                            _localOriginPercentage = new Vec2(0.0f, 0.0f);
                            _translation = Vec2.Zero;
                            _size.X = parentBounds.X;
                            allowTop = true;
                            break;
                        case UIDockStyle.Top:
                            _localOriginPercentage = new Vec2(0.0f, 1.0f);
                            _translation = new Vec2(0.0f, parentBounds.Y);
                            _size.X = parentBounds.X;
                            allowBottom = true;
                            break;
                        case UIDockStyle.Left:
                            _localOriginPercentage = new Vec2(0.0f, 0.0f);
                            _translation = Vec2.Zero;
                            _size.Y = parentBounds.Y;
                            allowRight = true;
                            break;
                        case UIDockStyle.Right:
                            _localOriginPercentage = new Vec2(1.0f, 0.0f);
                            _translation = new Vec2(parentBounds.X, 0.0f);
                            _size.Y = parentBounds.Y;
                            allowLeft = true;
                            break;
                    }
                }
                if (Anchored)
                {
                    if (allowBottom && AnchoredBottom)
                        MinY = Anchor.Bottom.GetValue(parentBounds);
                    if (allowTop && AnchoredTop)
                        MaxY = Anchor.Top.GetValue(parentBounds);
                    if (allowLeft && AnchoredLeft)
                        MinX = Anchor.Left.GetValue(parentBounds);
                    if (allowRight && AnchoredRight)
                        MaxX = Anchor.Right.GetValue(parentBounds);
                }

                //if (_dockStyle != HudDockStyle.None)
                //    leftOver = RegionDockComplement(parentBounds, AxisAlignedRegion);
            }

            RecalcLocalTransform();

            Vec2 bounds = Size;
            foreach (UIComponent c in _children)
                bounds = c.Resize(bounds);

            return leftOver;
        }
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            base.OnRecalcLocalTransform(out localTransform, out inverseLocalTransform);
        }
        private BoundingRectangleF RegionDockComplement(BoundingRectangleF parentRegion, BoundingRectangleF region)
        {
            if (parentRegion.MaxX != region.MaxX)
                return BoundingRectangleF.FromMinMaxSides(
                    region.MaxX, parentRegion.MaxX,
                    parentRegion.MinY, parentRegion.MaxY,
                    0.0f, 0.0f);
            if (parentRegion.MinX != region.MinX)
                return BoundingRectangleF.FromMinMaxSides(
                    parentRegion.MinX, region.MinX,
                    parentRegion.MinY, parentRegion.MaxY,
                    0.0f, 0.0f);
            if (parentRegion.MaxY != region.MaxY)
                return BoundingRectangleF.FromMinMaxSides(
                    parentRegion.MinX, parentRegion.MaxX,
                    region.MaxY, parentRegion.MaxY,
                    0.0f, 0.0f);
            if (parentRegion.MinY != region.MinY)
                return BoundingRectangleF.FromMinMaxSides(
                    parentRegion.MinX, parentRegion.MaxX,
                    parentRegion.MinY, region.MinY,
                    0.0f, 0.0f);
            return BoundingRectangleF.Empty;
        }
    }
}

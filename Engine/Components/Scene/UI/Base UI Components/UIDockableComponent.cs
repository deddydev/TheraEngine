using System;
using System.ComponentModel;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.UI
{
    public enum WidthHeightConstraint
    {
        /// <summary>
        /// WidthValue = ratio of Width/Height
        /// </summary>
        WidthAsRatioToHeight,
        /// <summary>
        /// HeightValue = ratio of Height/Width
        /// </summary>
        HeightAsRatioToWidth,
        /// <summary>
        /// Neither dimension depends on the other
        /// </summary>
        NoConstraint,
    }
    //TODO: move constraints into sizing mode
    public enum SizingMode
    {
        Pixels,
        ParentPercentage,
    }
    public enum HudDockStyle
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
    public enum BackgroundImageDisplay
    {
        Stretch,
        CenterFit,
        ResizeWithBars,
        Tile,
    }
    public class UIDockableComponent : UIComponent
    {
        //A variety of positioning parameters are used
        //to calculate the region's position and dimensions upon resize.

        public UIDockableComponent()
        {
            DockStyle = HudDockStyle.Fill;
        }

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
                OnResized();
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
                OnResized();
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
                _localTransform.TranslationX = value;
                _size.X = origX - LocalTranslationX;
                OnResized();
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
                _localTransform.TranslationY = value;
                _size.Y = origY - LocalTranslationY;
                OnResized();
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

        public class SizeableElement
        {
            private float _value;
            private SizingMode _sizingMode;

            public SizeableElement()
            {
                _value = 0;
                _sizingMode = SizingMode.Pixels;
            }
            public SizeableElement(float value, SizingMode sizingMode)
            {
                _value = value;
                _sizingMode = sizingMode;
            }

            public float Value { get => _value; set => _value = value; }
            public SizingMode SizingMode { get => _sizingMode; set => _sizingMode = value; }

            public float GetValue(float parentValue)
                => _sizingMode == SizingMode.ParentPercentage ? parentValue * _value : _value;
        }
        public class SizeableElementQuad
        {
            private SizeableElement _left, _right, _top, _bottom;

            public SizeableElement Left { get => _left; set => _left = value; }
            public SizeableElement Right { get => _right; set => _right = value; }
            public SizeableElement Top { get => _top; set => _top = value; }
            public SizeableElement Bottom { get => _bottom; set => _bottom = value; }
        }

        protected SizeableElement
            _width = new SizeableElement(1.0f, SizingMode.ParentPercentage),
            _height = new SizeableElement(1.0f, SizingMode.ParentPercentage),
            _posX = new SizeableElement(),
            _posY = new SizeableElement();
        protected SizeableElementQuad _margin, _padding, _anchor;

        protected WidthHeightConstraint _whConstraint = WidthHeightConstraint.NoConstraint;
        private HudDockStyle _dockStyle = HudDockStyle.None;
        private AnchorFlags _anchorFlags = AnchorFlags.None;
        
        public HudDockStyle DockStyle
        {
            get => _dockStyle;
            set
            {
                _dockStyle = value;
                if (ParentSocket is UIComponent h)
                    Resize(h.AxisAlignedRegion);
            }
        }
        public AnchorFlags SideAnchorFlags
        {
            get => _anchorFlags;
            set
            {
                _anchorFlags = value;
                if (ParentSocket is UIComponent h)
                    Resize(h.AxisAlignedRegion);
            }
        }

        public bool Docked => _dockStyle != HudDockStyle.None;
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
                if (ParentSocket is UIComponent h)
                    Resize(h.AxisAlignedRegion);
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
                if (ParentSocket is UIComponent h)
                    Resize(h.AxisAlignedRegion);
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
                if (ParentSocket is UIComponent h)
                    Resize(h.AxisAlignedRegion);
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
                if (ParentSocket is UIComponent h)
                    Resize(h.AxisAlignedRegion);
            }
        }
        
        public WidthHeightConstraint WidthHeightConstraint
        {
            get => _whConstraint;
            set
            {
                _whConstraint = value;
                if (ParentSocket is UIComponent h)
                    Resize(h.AxisAlignedRegion);
            }
        }
        
        public override unsafe BoundingRectangle Resize(BoundingRectangle parentRegion)
        {
            BoundingRectangle leftOver = parentRegion;
            BoundingRectangle prevRegion = AxisAlignedRegion;

            //float* points = stackalloc float[8];
            //float tAspect = (float)_bgImage.Width / (float)_bgImage.Height;
            //float wAspect = (float)Width / (float)Height;

            //switch (_bgType)
            //{
            //    case BGImageType.Stretch:

            //        points[0] = points[1] = points[3] = points[6] = 0.0f;
            //        points[2] = points[4] = Width;
            //        points[5] = points[7] = Height;

            //        break;

            //    case BGImageType.Center:

            //        if (tAspect > wAspect)
            //        {
            //            points[1] = points[3] = 0.0f;
            //            points[5] = points[7] = Height;

            //            points[0] = points[6] = Width * ((Width - ((float)Height / _bgImage.Height * _bgImage.Width)) / Width / 2.0f);
            //            points[2] = points[4] = Width - points[0];
            //        }
            //        else
            //        {
            //            points[0] = points[6] = 0.0f;
            //            points[2] = points[4] = Width;

            //            points[1] = points[3] = Height * (((Height - ((float)Width / _bgImage.Width * _bgImage.Height))) / Height / 2.0f);
            //            points[5] = points[7] = Height - points[1];
            //        }
            //        break;

            //    case BGImageType.ResizeWithBars:

            //        if (tAspect > wAspect)
            //        {
            //            points[0] = points[6] = 0.0f;
            //            points[2] = points[4] = Width;

            //            points[1] = points[3] = Height * (((Height - ((float)Width / _bgImage.Width * _bgImage.Height))) / Height / 2.0f);
            //            points[5] = points[7] = Height - points[1];
            //        }
            //        else
            //        {
            //            points[1] = points[3] = 0.0f;
            //            points[5] = points[7] = Height;

            //            points[0] = points[6] = Width * ((Width - ((float)Height / _bgImage.Height * _bgImage.Width)) / Width / 2.0f);
            //            points[2] = points[4] = Width - points[0];
            //        }

            //        break;
            //}

            float x = parentRegion.MinX + _posX.GetValue(parentRegion.Width);
            float y = parentRegion.MinY + _posY.GetValue(parentRegion.Height);
            float w = _width.GetValue(parentRegion.Width);
            float h = _height.GetValue(parentRegion.Height);
            if (_whConstraint != WidthHeightConstraint.NoConstraint)
            {
                switch (_whConstraint)
                {
                    case WidthHeightConstraint.HeightAsRatioToWidth:
                        h = w * _height.Value;
                        break;
                    case WidthHeightConstraint.WidthAsRatioToHeight:
                        w = h * _width.Value;
                        break;
                }
            }
            //_region = new BoundingRectangle(x, y, w, h, _localOriginPercentage.X, _localOriginPercentage.Y);
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
                        case HudDockStyle.Fill:
                            _size = parentRegion.Bounds;
                            _localTransform.TranslationXy = Vec2.Zero;
                            break;
                        case HudDockStyle.Bottom:
                            _localOriginPercentage = new Vec2(0.0f, 0.0f);
                            _localTransform.TranslationXy = Vec2.Zero;
                            _size.X = parentRegion.Width;
                            allowTop = true;
                            break;
                        case HudDockStyle.Top:
                            _localOriginPercentage = new Vec2(0.0f, 1.0f);
                            _localTransform.TranslationXy = new Vec2(0.0f, parentRegion.Height);
                            _size.X = parentRegion.Width;
                            allowBottom = true;
                            break;
                        case HudDockStyle.Left:
                            _localOriginPercentage = new Vec2(0.0f, 0.0f);
                            _localTransform.TranslationXy = Vec2.Zero;
                            _size.Y = parentRegion.Height;
                            allowRight = true;
                            break;
                        case HudDockStyle.Right:
                            _localOriginPercentage = new Vec2(1.0f, 0.0f);
                            _localTransform.TranslationXy = new Vec2(parentRegion.Width, 0.0f);
                            _size.Y = parentRegion.Height;
                            allowLeft = true;
                            break;
                    }
                }
                if (Anchored)
                {
                    if (allowBottom && AnchoredBottom)
                        MinY = _anchor.Bottom.GetValue(parentRegion.Height);
                    if (allowTop && AnchoredTop)
                        MaxY = _anchor.Top.GetValue(parentRegion.Height);
                    if (allowLeft && AnchoredLeft)
                        MinX = _anchor.Left.GetValue(parentRegion.Width);
                    if (allowRight && AnchoredRight)
                        MaxX = _anchor.Right.GetValue(parentRegion.Width);
                }

                if (_dockStyle != HudDockStyle.None)
                    leftOver = RegionDockComplement(parentRegion, AxisAlignedRegion);
            }

            RecalcLocalTransform();

            BoundingRectangle region = AxisAlignedRegion;
            foreach (UIComponent c in _children)
                region = c.Resize(region);

            return leftOver;
        }
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            base.OnRecalcLocalTransform(out localTransform, out inverseLocalTransform);
        }
        private BoundingRectangle RegionDockComplement(BoundingRectangle parentRegion, BoundingRectangle region)
        {
            if (parentRegion.MaxX != region.MaxX)
                return BoundingRectangle.FromMinMaxSides(
                    region.MaxX, parentRegion.MaxX, 
                    parentRegion.MinY, parentRegion.MaxY,
                    0.0f, 0.0f);
            if (parentRegion.MinX != region.MinX)
                return BoundingRectangle.FromMinMaxSides(
                    parentRegion.MinX, region.MinX,
                    parentRegion.MinY, parentRegion.MaxY,
                    0.0f, 0.0f);
            if (parentRegion.MaxY != region.MaxY)
                return BoundingRectangle.FromMinMaxSides(
                    parentRegion.MinX, parentRegion.MaxX,
                    region.MaxY, parentRegion.MaxY,
                    0.0f, 0.0f);
            if (parentRegion.MinY != region.MinY)
                return BoundingRectangle.FromMinMaxSides(
                    parentRegion.MinX, parentRegion.MaxX,
                    parentRegion.MinY, region.MinY,
                    0.0f, 0.0f);
            return BoundingRectangle.Empty;
        }
    }
}

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

        [Category("Transform")]
        public override BoundingRectangle Region
        {
            get => _region;
            set
            {
                _region = value;
                OnResized();
            }
        }
        [Category("Transform")]
        public override Vec2 Size
        {
            get => _region.Bounds;
            set
            {
                _region.Bounds = value;
                OnResized();
            }
        }
        [Category("Transform")]
        public override float Height
        {
            get => _region.Height;
            set
            {
                _region.Height = value;
                OnResized();
            }
        }
        [Category("Transform")]
        public override float Width
        {
            get => _region.Width;
            set
            {
                _region.Width = value;
                OnResized();
            }
        }
        [Category("Transform")]
        public override Vec2 Translation
        {
            get => _region.Translation;
            set
            {
                _region.Translation = value;
                RecalcLocalTransform();
            }
        }
        /// <summary>
        /// The origin of the component's rotation angle, as a percentage.
        /// 0,0 is bottom left, 0.5,0.5 is center, 1.0,1.0 is top right.
        /// </summary>
        [Category("Transform")]
        public override Vec2 TranslationLocalOrigin
        {
            get => _translationLocalOrigin;
            set
            {
                Vec2 diff = value - _translationLocalOrigin;
                _region.X += diff.X;
                _region.Y += diff.Y;
                _translationLocalOrigin = value;
                RecalcLocalTransform();
            }
        }
        [Category("Transform")]
        public override float TranslationX
        {
            get => _region.X;
            set
            {
                _region.X = value;
                RecalcLocalTransform();
            }
        }
        [Category("Transform")]
        public override float TranslationY
        {
            get => _region.Y;
            set
            {
                _region.Y = value;
                RecalcLocalTransform();
            }
        }
        [Category("Transform")]
        public override Vec2 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                RecalcLocalTransform();
            }
        }
        [Category("Transform")]
        public override float ScaleX
        {
            get => _scale.X;
            set
            {
                _scale.X = value;
                RecalcLocalTransform();
            }
        }
        [Category("Transform")]
        public override float ScaleY
        {
            get => _scale.Y;
            set
            {
                _scale.Y = value;
                RecalcLocalTransform();
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
                    Resize(h.Region);
            }
        }
        public AnchorFlags SideAnchorFlags
        {
            get => _anchorFlags;
            set
            {
                _anchorFlags = value;
                if (ParentSocket is UIComponent h)
                    Resize(h.Region);
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
                    Resize(h.Region);
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
                    Resize(h.Region);
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
                    Resize(h.Region);
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
                    Resize(h.Region);
            }
        }
        
        public WidthHeightConstraint WidthHeightConstraint
        {
            get => _whConstraint;
            set
            {
                _whConstraint = value;
                if (ParentSocket is UIComponent h)
                    Resize(h.Region);
            }
        }
        
        public override unsafe BoundingRectangle Resize(BoundingRectangle parentRegion)
        {
            BoundingRectangle leftOver = parentRegion;
            BoundingRectangle prevRegion = Region;

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
            _region = new BoundingRectangle(x, y, w, h, _translationLocalOrigin.X, _translationLocalOrigin.Y);
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
                            _region.Bounds = parentRegion.Bounds;
                            _region.Translation = parentRegion.Translation;
                            break;
                        case HudDockStyle.Bottom:
                            _region.Translation = parentRegion.Translation;
                            _region.Width = parentRegion.Width;
                            allowTop = true;
                            break;
                        case HudDockStyle.Top:
                            _region.Translation = parentRegion.Translation;
                            _region.Y += parentRegion.Height - _region.Height;
                            _region.Width = parentRegion.Width;
                            allowBottom = true;
                            break;
                        case HudDockStyle.Left:
                            _region.Translation = parentRegion.Translation;
                            _region.Height = parentRegion.Height;
                            allowRight = true;
                            break;
                        case HudDockStyle.Right:
                            _region.Translation = parentRegion.Translation;
                            _region.X += parentRegion.Width - _region.Width;
                            _region.Height = parentRegion.Height;
                            allowLeft = true;
                            break;
                    }
                }
                if (Anchored)
                {
                    if (allowBottom && AnchoredBottom)
                        _region.MinY = _anchor.Bottom.GetValue(parentRegion.Height);
                    if (allowTop && AnchoredTop)
                        _region.MaxY = _anchor.Top.GetValue(parentRegion.Height);
                    if (allowLeft && AnchoredLeft)
                        _region.MinX = _anchor.Left.GetValue(parentRegion.Width);
                    if (allowRight && AnchoredRight)
                        _region.MaxX = _anchor.Right.GetValue(parentRegion.Width);
                }
                if (_dockStyle != HudDockStyle.None)
                    leftOver = RegionDockComplement(parentRegion, Region);
            }

            RecalcLocalTransform();

            BoundingRectangle region = Region;

            //Engine.PrintLine("Resized {0} to {0}", prevRegion, region);

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

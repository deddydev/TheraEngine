using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.HUD
{
    public enum WidthHeightConstraint
    {
        WidthAsRatioToHeight,
        HeightAsRatioToWidth,
        NoConstraint,
    }
    public enum SizingMode
    {
        Percentage,
        Pixels,
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
    public class DockableHudComponent : HudComponent
    {
        //A variety of positioning parameters are used
        //to calculate the region's position and dimensions upon resize.

        private SizingMode
            _widthMode = SizingMode.Pixels,
            _heightMode = SizingMode.Pixels,
            _positionXMode = SizingMode.Pixels,
            _positionYMode = SizingMode.Pixels,
            _marginLeftMode = SizingMode.Pixels,
            _marginRightMode = SizingMode.Pixels,
            _marginTopMode = SizingMode.Pixels,
            _marginBottomMode = SizingMode.Pixels,
            _paddingLeftMode = SizingMode.Pixels,
            _paddingRightMode = SizingMode.Pixels,
            _paddingTopMode = SizingMode.Pixels,
            _paddingBottomMode = SizingMode.Pixels,
            _anchorLeftMode = SizingMode.Pixels,
            _anchorRightMode = SizingMode.Pixels,
            _anchorTopMode = SizingMode.Pixels,
            _anchorBottomMode = SizingMode.Pixels;

        private WidthHeightConstraint _whConstraint = WidthHeightConstraint.NoConstraint;
        private HudDockStyle _dockStyle = HudDockStyle.None;
        private AnchorFlags _anchorFlags = AnchorFlags.None;
        
        private float _widthValue, _heightValue, _posXValue, _posYValue, _originXValue, _originYValue;
        private float _anchorBottomValue, _anchorTopValue, _anchorLeftValue, _anchorRightValue;
        private float _marginBottomValue, _marginTopValue, _marginLeftValue, _marginRightValue;
        private float _paddingBottomValue, _paddingTopValue, _paddingLeftValue, _paddingRightValue;

        public HudDockStyle DockStyle
        {
            get => _dockStyle;
            set
            {
                _dockStyle = value;
            }
        }
        public AnchorFlags SideAnchorFlags
        {
            get => _anchorFlags;
            set
            {
                _anchorFlags = value;
            }
        }
        public SizingMode WidthMode { get => _widthMode; set => _widthMode = value; }
        public SizingMode HeightMode { get => _heightMode; set => _heightMode = value; }
        public SizingMode PositionXMode { get => _positionXMode; set => _positionXMode = value; }
        public SizingMode PositionYMode { get => _positionYMode; set => _positionYMode = value; }

        public SizingMode MarginLeftMode { get => _marginLeftMode; set => _marginLeftMode = value; }
        public SizingMode MarginRightMode { get => _marginRightMode; set => _marginRightMode = value; }
        public SizingMode MarginTopMode { get => _marginTopMode; set => _marginTopMode = value; }
        public SizingMode MarginBottomMode { get => _marginBottomMode; set => _marginBottomMode = value; }

        public SizingMode PaddingLeftMode { get => _paddingLeftMode; set => _paddingLeftMode = value; }
        public SizingMode PaddingRightMode { get => _paddingRightMode; set => _paddingRightMode = value; }
        public SizingMode PaddingTopMode { get => _paddingTopMode; set => _paddingTopMode = value; }
        public SizingMode PaddingBottomMode { get => _paddingBottomMode; set => _paddingBottomMode = value; }

        public SizingMode AnchorLeftMode { get => _anchorLeftMode; set => _anchorLeftMode = value; }
        public SizingMode AnchorRightMode { get => _anchorRightMode; set => _anchorRightMode = value; }
        public SizingMode AnchorTopMode { get => _anchorTopMode; set => _anchorTopMode = value; }
        public SizingMode AnchorBottomMode { get => _anchorBottomMode; set => _anchorBottomMode = value; }

        public bool Docked => _dockStyle != HudDockStyle.None;
        public bool Anchored => _anchorFlags != AnchorFlags.None;

        public bool AnchoredBottom
        {
            get => (_anchorFlags & AnchorFlags.Bottom) != 0;
            set
            {
                if (value)
                    _anchorFlags |= AnchorFlags.Bottom;
                else
                    _anchorFlags &= ~AnchorFlags.Bottom;
            }
        }
        public bool AnchoredTop
        {
            get => (_anchorFlags & AnchorFlags.Top) != 0;
            set
            {
                if (value)
                    _anchorFlags |= AnchorFlags.Top;
                else
                    _anchorFlags &= ~AnchorFlags.Top;
            }
        }
        public bool AnchoredLeft
        {
            get => (_anchorFlags & AnchorFlags.Left) != 0;
            set
            {
                if (value)
                    _anchorFlags |= AnchorFlags.Left;
                else
                    _anchorFlags &= ~AnchorFlags.Left;
            }
        }
        public bool AnchoredRight
        {
            get => (_anchorFlags & AnchorFlags.Right) != 0;
            set
            {
                if (value)
                    _anchorFlags |= AnchorFlags.Right;
                else
                    _anchorFlags &= ~AnchorFlags.Right;
            }
        }

        public float WidthValue { get => _widthValue; set => _widthValue = value; }
        public float HeightValue { get => _heightValue; set => _heightValue = value; }
        public float PosXValue { get => _posXValue; set => _posXValue = value; }
        public float PosYValue { get => _posYValue; set => _posYValue = value; }
        public float AnchorBottomValue { get => _anchorBottomValue; set => _anchorBottomValue = value; }
        public float AnchorTopValue { get => _anchorTopValue; set => _anchorTopValue = value; }
        public float AnchorLeftValue { get => _anchorLeftValue; set => _anchorLeftValue = value; }
        public float AnchorRightValue { get => _anchorRightValue; set => _anchorRightValue = value; }
        public float PaddingBottomValue { get => _paddingBottomValue; set => _paddingBottomValue = value; }
        public float PaddingTopValue { get => _paddingTopValue; set => _paddingTopValue = value; }
        public float PaddingLeftValue { get => _paddingLeftValue; set => _paddingLeftValue = value; }
        public float PaddingRightValue { get => _paddingRightValue; set => _paddingRightValue = value; }
        public float MarginBottomValue { get => _marginBottomValue; set => _marginBottomValue = value; }
        public float MarginTopValue { get => _marginTopValue; set => _marginTopValue = value; }
        public float MarginLeftValue { get => _marginLeftValue; set => _marginLeftValue = value; }
        public float MarginRightValue { get => _marginRightValue; set => _marginRightValue = value; }
        public float OriginXPercentage { get => _originXValue; set => _originXValue = value; }
        public float OriginYPercentage { get => _originYValue; set => _originYValue = value; }
        public WidthHeightConstraint WidthHeightConstraint { get => _whConstraint; set => _whConstraint = value; }
        
        public override BoundingRectangle Resize(BoundingRectangle parentRegion)
        {
            BoundingRectangle leftOver = parentRegion;
            float x = parentRegion.MinX + (_positionXMode == SizingMode.Percentage ? parentRegion.Width * _posXValue : _posXValue);
            float y = parentRegion.MinY + (_positionYMode == SizingMode.Percentage ? parentRegion.Height * _posYValue : _posYValue);
            float w = (_widthMode == SizingMode.Percentage ? parentRegion.Width * _widthValue : _widthValue);
            float h = (_heightMode == SizingMode.Percentage ? parentRegion.Height * _heightValue : _heightValue);
            if (_whConstraint != WidthHeightConstraint.NoConstraint)
            {

            }
            _region = new BoundingRectangle(x, y, w, h, _originXValue, _originYValue);
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
                    {
                        if (_anchorBottomMode == SizingMode.Percentage)
                            _region.MinY = parentRegion.Height * _anchorBottomValue;
                        else
                            _region.MinY = _anchorBottomValue;
                    }
                    if (allowTop && AnchoredTop)
                    {
                        if (_anchorTopMode == SizingMode.Percentage)
                            _region.MaxY = parentRegion.Height * _anchorTopValue;
                        else
                            _region.MaxY = _anchorTopValue;
                    }
                    if (allowLeft && AnchoredLeft)
                    {
                        if (_anchorLeftMode == SizingMode.Percentage)
                            _region.MinX = parentRegion.Width * _anchorLeftValue;
                        else
                            _region.MinX = _anchorLeftValue;
                    }
                    if (allowRight && AnchoredRight)
                    {
                        if (_anchorRightMode == SizingMode.Percentage)
                            _region.MaxX = parentRegion.Width * _anchorRightValue;
                        else
                            _region.MaxX = _anchorRightValue;
                    }
                }
                if (_dockStyle != HudDockStyle.None)
                    leftOver = RegionDockComplement(parentRegion, Region);
            }

            BoundingRectangle region = Region;
            foreach (HudComponent c in _children)
                region = c.Resize(region);

            //RecalcLocalTransform();

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

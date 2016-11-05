using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using CustomEngine.Rendering.Models.Materials;

namespace CustomEngine.Rendering.HUD
{
    public class DockableHudComponent : HudComponent
    {
        public DockableHudComponent(HudComponent owner) : base(owner) { }
        
        public HudDockStyle _dockStyle;
        public AnchorFlags _sideAnchorFlags;

        /// <summary>
        /// Returns the available real estate for the next components to use.
        /// </summary>
        public override RectangleF ParentResized(RectangleF parentRegion)
        {
            RectangleF leftOver = parentRegion;
            if (_dockStyle != HudDockStyle.None || _sideAnchorFlags != AnchorFlags.None)
            {
                bool allowLeft = true, allowRight = true, allowTop = true, allowBottom = true;
                if (_dockStyle != HudDockStyle.None)
                {
                    allowLeft = false;
                    allowRight = false;
                    allowTop = false;
                    allowBottom = false;
                    switch (_dockStyle)
                    {
                        case HudDockStyle.Fill:
                            _region.Size = parentRegion.Size;
                            _region.Location = parentRegion.Location;
                            break;
                        case HudDockStyle.Bottom:
                            _region.Location = parentRegion.Location;
                            _region.Width = parentRegion.Width;
                            allowTop = true;
                            break;
                        case HudDockStyle.Top:
                            _region.Location = parentRegion.Location;
                            _region.Y += parentRegion.Height - _region.Height;
                            _region.Width = parentRegion.Width;
                            allowBottom = true;
                            break;
                        case HudDockStyle.Left:
                            _region.Location = parentRegion.Location;
                            _region.Height = parentRegion.Height;
                            allowRight = true;
                            break;
                        case HudDockStyle.Right:
                            _region.Location = parentRegion.Location;
                            _region.X += parentRegion.Width - _region.Width;
                            _region.Height = parentRegion.Height;
                            allowLeft = true;
                            break;
                    }
                }
                if (_sideAnchorFlags != AnchorFlags.None)
                {
                    if ((_sideAnchorFlags & AnchorFlags.Bottom) != 0 && allowBottom)
                    {

                    }
                    if ((_sideAnchorFlags & AnchorFlags.Top) != 0 && allowTop)
                    {

                    }
                    if ((_sideAnchorFlags & AnchorFlags.Left) != 0 && allowLeft)
                    {

                    }
                    if ((_sideAnchorFlags & AnchorFlags.Right) != 0 && allowRight)
                    {

                    }
                }
                leftOver = RegionComplement(parentRegion, Region);
            }

            RectangleF region = Region;
            foreach (HudComponent c in _children)
                region = c.ParentResized(region);

            return leftOver;
        }

        private RectangleF RegionComplement(RectangleF parentRegion, RectangleF region)
        {
            RectangleF leftOver = new RectangleF();



            return leftOver;
        }
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
}

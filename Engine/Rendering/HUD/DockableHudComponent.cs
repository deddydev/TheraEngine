using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.HUD
{
    public class DockableHudComponent : HudComponent
    {
        public HudDockStyle _dockStyle;
        public AnchorFlags _sideAnchorFlags;

        /// <summary>
        /// Returns the available real estate for the next components to use.
        /// </summary>
        public override BoundingRectangle Resize(BoundingRectangle parentRegion)
        {
            BoundingRectangle leftOver = parentRegion;
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

            BoundingRectangle region = Region;
            foreach (HudComponent c in _children)
                region = c.Resize(region);

            return leftOver;
        }

        private BoundingRectangle RegionComplement(BoundingRectangle parentRegion, BoundingRectangle region)
        {
            BoundingRectangle leftOver = new BoundingRectangle();



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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Rendering.HUD
{
    public class TextureHudComponent : DockableHudComponent
    {
        PrimitiveManager _quad = new PrimitiveManager();
        public unsafe override BoundingRectangle Resize(BoundingRectangle parentRegion)
        {
            BoundingRectangle r = base.Resize(parentRegion);
            VertexBuffer buffer = _quad.Data[0];
            Vec3* data = (Vec3*)buffer.Address;
            data[0] = new Vec3(Region.BottomLeft, 0.0f);
            data[1] = new Vec3(Region.BottomRight, 0.0f);
            data[2] = new Vec3(Region.TopLeft, 0.0f);
            data[3] = new Vec3(Region.TopRight, 0.0f);
            return r;
        }
    }
}

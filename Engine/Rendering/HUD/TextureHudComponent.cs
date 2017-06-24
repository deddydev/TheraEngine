using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Textures;

namespace TheraEngine.Rendering.HUD
{
    public class TextureHudComponent : DockableHudComponent, I2DRenderable
    {
        public TextureHudComponent()
        {
            PrimitiveData quad = PrimitiveData.FromQuads(Culling.Back, PrimitiveBufferInfo.JustPositions(), VertexQuad.ZUpQuad(_region.Width, _region.Height, true));
            _quad = new PrimitiveManager(quad, Material.GetUnlitTextureMaterial());
        }
        public TextureHudComponent(TextureReference texture)
        {
            PrimitiveData quad = PrimitiveData.FromQuads(Culling.Back, PrimitiveBufferInfo.JustPositions(), VertexQuad.ZUpQuad(_region.Width, _region.Height, true));
            _quad = new PrimitiveManager(quad, Material.GetUnlitTextureMaterial(texture));
        }

        PrimitiveManager _quad;

        public bool HasTransparency => _quad.Material.HasTransparency;

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
        public void Render()
        {
            _quad.Render(WorldMatrix, Matrix3.Identity);
        }
    }
}

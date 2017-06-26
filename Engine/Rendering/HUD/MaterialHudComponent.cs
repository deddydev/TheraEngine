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
    public class MaterialHudComponent : DockableHudComponent, I2DRenderable
    {
        public MaterialHudComponent(Material m)
        {
            PrimitiveData quad = PrimitiveData.FromQuads(Culling.Back, PrimitiveBufferInfo.JustPositions(), VertexQuad.ZUpQuad(_region.Width, _region.Height, true));
            _quad = new PrimitiveManager(quad, m);
        }

        private PrimitiveManager _quad;

        public bool HasTransparency => _quad.Material.HasTransparency;

        public Material Material
        {
            get => _quad.Material;
            set => _quad.Material = value;
        }
        /// <summary>
        /// Retrieves the linked material's uniform parameter at the given index.
        /// Use this to set uniform values to be passed to the shader.
        /// </summary>
        public T2 Parameter<T2>(int index) where T2 : GLVar
            => _quad.Parameter<T2>(index);
        /// <summary>
        /// Retrieves the linked material's uniform parameter with the given name.
        /// Use this to set uniform values to be passed to the shader.
        /// </summary>
        public T2 Parameter<T2>(string name) where T2 : GLVar
            => _quad.Parameter<T2>(name);
        
        public unsafe override BoundingRectangle Resize(BoundingRectangle parentRegion)
        {
            BoundingRectangle r = base.Resize(parentRegion);
            VertexBuffer buffer = _quad.Data[0];
            Vec3* data = (Vec3*)buffer.Address;
            data[0] = new Vec3(Region.BottomLeft, -2.0f);
            data[1] = new Vec3(Region.BottomRight, -2.0f);
            data[2] = new Vec3(Region.TopLeft, -2.0f);
            data[3] = new Vec3(Region.TopRight, -2.0f);
            return r;
        }
        public void Render()
            => _quad.Render(WorldMatrix, Matrix3.Identity);
    }
}

using System;
using System.Collections.Generic;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Textures;
using TheraEngine.Core.Shapes;
using System.Threading.Tasks;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Rendering.UI
{
    public class UIMaterialComponent : UIDockableComponent, I2DRenderable
    {
        public RenderInfo2D RenderInfo { get; } = new RenderInfo2D(ERenderPass2D.OnTop, 0, 0);

        public UIMaterialComponent(TMaterial m)
        {
            VertexQuad quad = VertexQuad.PosZQuad(_region.Width, _region.Height, -2.0f, true);
            PrimitiveData quadData = PrimitiveData.FromQuads(Culling.Back, VertexShaderDesc.PosNormTex(), quad);
            _quad = new PrimitiveManager(quadData, m);
        }

        private PrimitiveManager _quad;
        
        public TMaterial Material
        {
            get => _quad.Material;
            set => _quad.Material = value;
        }
        
        public BaseRenderTexture Texture(int index)
        {
            if (_quad.Material.Textures.IndexInRange(index))
                return _quad.Material.Textures[index].GetTextureGeneric(true);
            return null;
        }
        /// <summary>
        /// Retrieves the linked material's uniform parameter at the given index.
        /// Use this to set uniform values to be passed to the shader.
        /// </summary>
        public T2 Parameter<T2>(int index) where T2 : ShaderVar
            => _quad.Parameter<T2>(index);
        /// <summary>
        /// Retrieves the linked material's uniform parameter with the given name.
        /// Use this to set uniform values to be passed to the shader.
        /// </summary>
        public T2 Parameter<T2>(string name) where T2 : ShaderVar
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
        public virtual void Render()
            => _quad.Render(WorldMatrix, Matrix3.Identity);
    }
}

using System;
using System.Collections.Generic;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials.Textures;
using System.Drawing;

namespace TheraEngine.Rendering.UI
{
    public class UIMaterialRectangleComponent : UIDockableComponent, I2DRenderable
    {
        public RenderInfo2D RenderInfo { get; } = new RenderInfo2D(ERenderPass2D.Opaque, 0, 0);

        public UIMaterialRectangleComponent() 
            : this(TMaterial.CreateUnlitColorMaterialForward(Color.Magenta)) { }
        public UIMaterialRectangleComponent(TMaterial material)
        {
            VertexQuad quad = VertexQuad.PosZQuad(Width, Height, 0.0f, true);
            PrimitiveData quadData = PrimitiveData.FromQuads(Culling.Back, VertexShaderDesc.PosNormTex(), quad);
            _quad = new PrimitiveManager(quadData, material);
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
        
        // 3--2
        // |\ |
        // | \|
        // 0--1
        public unsafe override BoundingRectangle Resize(BoundingRectangle parentRegion)
        {
            //013312
            BoundingRectangle r = base.Resize(parentRegion);
            VertexBuffer buffer = _quad.Data[0];
            Vec3* data = (Vec3*)buffer.Address;
            data[0] = new Vec3(0.0f);
            data[1] = data[4] = new Vec3(Width, 0.0f, 0.0f);
            data[2] = data[3] = new Vec3(0.0f, Height, 0.0f);
            data[5] = new Vec3(Width, Height, 0.0f);
            return r;
        }
        
        public virtual void Render() => _quad.Render(WorldMatrix, Matrix3.Identity);
    }
}

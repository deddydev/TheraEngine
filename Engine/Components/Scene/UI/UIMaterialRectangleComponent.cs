using System;
using System.Collections.Generic;
using System.Drawing;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.UI
{
    public class UIMaterialRectangleComponent : UIDockableComponent, I2DRenderable
    {
        public RenderInfo2D RenderInfo { get; } = new RenderInfo2D(ERenderPass.OpaqueForward, 0, 0);

        public UIMaterialRectangleComponent() 
            : this(TMaterial.CreateUnlitColorMaterialForward(Color.Magenta)) { }
        public UIMaterialRectangleComponent(TMaterial material, bool flipVerticalUVCoord = false)
        {
            VertexQuad quad = VertexQuad.PosZQuad(1.0f, 1.0f, 0.0f, true, flipVerticalUVCoord);
            PrimitiveData quadData = PrimitiveData.FromQuads(VertexShaderDesc.PosTex(), quad);
            _quad = new PrimitiveManager(quadData, material);
        }

        protected PrimitiveManager _quad;
        
        /// <summary>
        /// The material used to render on this UI component.
        /// </summary>
        public TMaterial InterfaceMaterial
        {
            get => _quad.Material;
            set => _quad.Material = value;
        }

        public BaseTexRef Texture(int index)
        {
            if (_quad.Material.Textures.IndexInRange(index))
                return _quad.Material.Textures[index];
            return null;
        }
        public T Texture<T>(int index) where T : BaseTexRef
        {
            if (_quad.Material.Textures.IndexInRange(index))
                return _quad.Material.Textures[index] as T;
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
        //public unsafe override Vec2 Resize(Vec2 parentBounds)
        //{
        //    //013312

        //    Vec2 r = base.Resize(parentBounds);

        //    DataBuffer buffer = _quad.Data[0];
        //    Vec3* data = (Vec3*)buffer.Address;
        //    data[0] = new Vec3(0.0f);
        //    data[1] = data[4] = new Vec3(Width, 0.0f, 0.0f);
        //    data[2] = data[3] = new Vec3(0.0f, Height, 0.0f);
        //    data[5] = new Vec3(Width, Height, 0.0f);
        //    buffer.PushData();

        //    return r;
        //}

        //protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        //{
        //    base.OnRecalcLocalTransform(out localTransform, out inverseLocalTransform);
        //    localTransform = localTransform * Matrix4.CreateScale(_size.X, _size.Y, 1.0f);
        //    inverseLocalTransform = Matrix4.CreateScale(1.0f / _size.X, 1.0f / _size.Y, 1.0f) * inverseLocalTransform;
        //}

        protected override void OnWorldTransformChanged()
        {
            _renderMatrix = WorldMatrix * Matrix4.CreateScale(Width, Height, 1.0f);
            base.OnWorldTransformChanged();
        }

        private Matrix4 _renderMatrix;
        private RenderCommandMesh2D _renderCommand = new RenderCommandMesh2D();
        public virtual void AddRenderables(RenderPasses passes)
        {
            _renderCommand.Primitives = _quad;
            _renderCommand.WorldMatrix = _renderMatrix;
            _renderCommand.NormalMatrix = Matrix3.Identity;
            _renderCommand.ZIndex = 0;
            passes.Add(_renderCommand, RenderInfo.RenderPass);
        }

        //public enum BackgroundImageDisplay
        //{
        //    Stretch,
        //    CenterFit,
        //    ResizeWithBars,
        //    Tile,
        //}
        //private BackgroundImageDisplay _backgroundUV = BackgroundImageDisplay.Stretch;
        //public BackgroundImageDisplay BackgroundUV
        //{
        //    get => _backgroundUV;
        //    set
        //    {
        //        _backgroundUV = value;
        //        OnResized();
        //    }
        //}

        //float* points = stackalloc float[8];
        //float tAspect = (float)_bgImage.Width / (float)_bgImage.Height;
        //float wAspect = (float)Width / (float)Height;

        //switch (_bgType)
        //{
        //    case BackgroundImageDisplay.Stretch:

        //        points[0] = points[1] = points[3] = points[6] = 0.0f;
        //        points[2] = points[4] = Width;
        //        points[5] = points[7] = Height;

        //        break;

        //    case BackgroundImageDisplay.Center:

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

        //    case BackgroundImageDisplay.ResizeWithBars:

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
    }
}

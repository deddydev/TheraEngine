using Extensions;
using System;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.UI
{
    public class UIMaterialRectangleComponent : UIBoundableComponent, I2DRenderable
    {
        public UIMaterialRectangleComponent() 
            : this(TMaterial.CreateUnlitColorMaterialForward(Color.Magenta)) { }
        public UIMaterialRectangleComponent(TMaterial material, bool flipVerticalUVCoord = false)
        {
            VertexQuad quad = VertexQuad.PosZQuad(1.0f, 1.0f, 0.0f, true, flipVerticalUVCoord);
            PrimitiveData quadData = PrimitiveData.FromQuads(VertexShaderDesc.PosTex(), quad);
            RenderCommand.Mesh = new PrimitiveManager(quadData, material);
            RenderCommand.ZIndex = 0;
        }

        //[Category("Rendering")]
        //public IRenderInfo2D RenderInfo { get; } = new RenderInfo2D(0, 0);

        //[Category("Rendering")]
        //public override int LayerIndex
        //{
        //    get => base.LayerIndex;
        //    set
        //    {
        //        base.LayerIndex = value;
        //        RenderInfo.LayerIndex = value;
        //    }
        //}
        //[Category("Rendering")]
        //public override int IndexWithinLayer
        //{
        //    get => base.IndexWithinLayer;
        //    set
        //    {
        //        base.IndexWithinLayer = value;
        //        RenderInfo.IndexWithinLayer = value;
        //    }
        //}

        /// <summary>
        /// The material used to render on this UI component.
        /// </summary>
        [Category("Rendering")]
        public TMaterial InterfaceMaterial
        {
            get => RenderCommand.Mesh.Material;
            set => RenderCommand.Mesh.Material = value;
        }

        public BaseTexRef Texture(int index)
        {
            if (RenderCommand.Mesh.Material.Textures.IndexInRange(index))
                return RenderCommand.Mesh.Material.Textures[index];

            return null;
        }
        public T Texture<T>(int index) where T : BaseTexRef
        {
            if (RenderCommand.Mesh.Material.Textures.IndexInRange(index))
                return RenderCommand.Mesh.Material.Textures[index] as T;

            return null;
        }

        /// <summary>
        /// Retrieves the linked material's uniform parameter at the given index.
        /// Use this to set uniform values to be passed to the shader.
        /// </summary>
        public T2 Parameter<T2>(int index) where T2 : ShaderVar
            => RenderCommand.Mesh.Parameter<T2>(index);
        /// <summary>
        /// Retrieves the linked material's uniform parameter with the given name.
        /// Use this to set uniform values to be passed to the shader.
        /// </summary>
        public T2 Parameter<T2>(string name) where T2 : ShaderVar
            => RenderCommand.Mesh.Parameter<T2>(name);

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
            RenderCommand.WorldMatrix = WorldMatrix * Matrix4.CreateScale(ActualWidth, ActualHeight, 1.0f);
            base.OnWorldTransformChanged();
        }

        [Category("Rendering")]
        public RenderCommandMesh2D RenderCommand { get; } = new RenderCommandMesh2D(ERenderPass.OpaqueForward);

        public override void AddRenderables(RenderPasses passes, ICamera camera)
        {
            base.AddRenderables(passes, camera);
            passes.Add(RenderCommand);
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

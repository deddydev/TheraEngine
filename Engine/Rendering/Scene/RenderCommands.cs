using System;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public abstract class RenderCommand : IComparable<RenderCommand>, IComparable
    {
        public abstract int CompareTo(RenderCommand other);
        public int CompareTo(object obj) => CompareTo(obj as RenderCommand);
        public abstract void Render();
    }
    public abstract class RenderCommand3D : RenderCommand
    {
        /// <summary>
        /// Used to determine what order to render in.
        /// Opaque objects closer to the camera are drawn first,
        /// whereas translucent objects farther from the camera are drawn first.
        /// </summary>
        public float RenderDistance { get; set; }

        public override int CompareTo(RenderCommand other)
            => RenderDistance < ((other as RenderCommand3D)?.RenderDistance ?? 0.0f) ? -1 : 1;
        
        public RenderCommand3D() { }
        public RenderCommand3D(float renderDistance)
        {
            RenderDistance = renderDistance;
        }

        public void SetDistance(Camera camera, Vec3 point, bool planar)
        {
            if (camera == null)
                return;
            RenderDistance = planar ?
                camera.DistanceFromScreenPlane(point) :
                camera.DistanceFromWorldPointFast(point);
        }
    }
    public abstract class RenderCommand2D : RenderCommand
    {
        public int ZIndex { get; set; }

        public override int CompareTo(RenderCommand other)
            => ZIndex < ((other as RenderCommand2D)?.ZIndex ?? 0) ? -1 : 1;

        public RenderCommand2D() { }
        public RenderCommand2D(int zIndex)
        {
            ZIndex = zIndex;
        }
    }
    public class RenderCommandMethod2D : RenderCommand2D
    {
        public RenderCommandMethod2D() { }
        public RenderCommandMethod2D(Action render) => Rendered = render;
        public Action Rendered { get; set; }
        public override void Render() => Rendered?.Invoke();
    }
    public class RenderCommandMethod3D : RenderCommand3D
    {
        public RenderCommandMethod3D() { }
        public RenderCommandMethod3D(Action render) => Rendered = render;
        public Action Rendered { get; set; }
        public override void Render() => Rendered?.Invoke();
    }
    public class RenderCommandMesh3D : RenderCommand3D
    {
        public PrimitiveManager Mesh { get; set; }
        public Matrix4 WorldMatrix { get; set; } = Matrix4.Identity;
        public Matrix3 NormalMatrix { get; set; } = Matrix3.Identity;
        public TMaterial MaterialOverride { get; set; }

        public RenderCommandMesh3D() { }
        public RenderCommandMesh3D(
            PrimitiveManager manager,
            Matrix4 worldMatrix,
            Matrix3 normalMatrix,
            float renderDistance,
            TMaterial materialOverride = null) : base(renderDistance)
        {
            Mesh = manager;
            WorldMatrix = worldMatrix;
            NormalMatrix = normalMatrix;
            MaterialOverride = materialOverride;
        }

        public override void Render()
        {
            Mesh?.Render(WorldMatrix, NormalMatrix, MaterialOverride);
        }
    }
    public class RenderCommandMesh2D : RenderCommand2D
    {
        public PrimitiveManager Mesh { get; set; }
        public Matrix4 WorldMatrix { get; set; } = Matrix4.Identity;
        public TMaterial MaterialOverride { get; set; }

        public RenderCommandMesh2D() { }
        public RenderCommandMesh2D(
            PrimitiveManager manager,
            Matrix4 worldMatrix,
            int zIndex,
            TMaterial materialOverride = null) : base(zIndex)
        {
            Mesh = manager;
            WorldMatrix = worldMatrix;
            MaterialOverride = materialOverride;
        }

        public override void Render()
        {
            Mesh?.Render(WorldMatrix, Matrix3.Identity, MaterialOverride);
        }
    }
    public class RenderCommandViewport : RenderCommandMesh2D
    {
        public Viewport Viewport { get; set; }
        public MaterialFrameBuffer Framebuffer { get; set; }
        
        public RenderCommandViewport() { }
        public RenderCommandViewport(
            Viewport viewport,
            PrimitiveManager quad, 
            MaterialFrameBuffer viewportFBO, 
            Matrix4 worldMatrix, 
            int zIndex)
            : base(quad, worldMatrix, zIndex, null)
        {
            Viewport = viewport;
            Framebuffer = viewportFBO;
        }

        public override void Render()
        {
            //TODO: viewport renders all viewed items to the framebuffer,
            //But this method is called within the parent's rendering to its framebuffer.
            var curr = FrameBuffer.CurrentlyBound;
            Viewport.Render(Viewport.Camera?.OwningComponent?.OwningScene, Viewport.Camera, Framebuffer);
            curr?.Bind(EFramebufferTarget.DrawFramebuffer);
            base.Render();
        }
    }
}

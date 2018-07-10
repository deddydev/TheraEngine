using System;
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
    public class RenderCommandDebug2D : RenderCommand2D
    {
        public RenderCommandDebug2D() { }
        public RenderCommandDebug2D(Action render) => Rendered = render;
        public Action Rendered { get; set; }
        public override void Render() => Rendered?.Invoke();
    }
    public class RenderCommandDebug3D : RenderCommand3D
    {
        public RenderCommandDebug3D() { }
        public RenderCommandDebug3D(Action render) => Rendered = render;
        public Action Rendered { get; set; }
        public override void Render() => Rendered?.Invoke();
    }
    public class RenderCommandMesh3D : RenderCommand3D
    {
        public PrimitiveManager Primitives { get; set; }
        public Matrix4 WorldMatrix { get; set; }
        public Matrix3 NormalMatrix { get; set; }
        public TMaterial Material { get; set; }

        public RenderCommandMesh3D() { }
        public RenderCommandMesh3D(
            PrimitiveManager manager,
            Matrix4 worldMatrix,
            Matrix3 normalMatrix,
            float renderDistance,
            TMaterial materialOverride = null) : base(renderDistance)
        {
            Primitives = manager;
            WorldMatrix = worldMatrix;
            NormalMatrix = normalMatrix;
            Material = materialOverride;
        }

        public override void Render()
        {
            Primitives?.Render(WorldMatrix, NormalMatrix, Material);
        }
    }
    public class RenderCommandMesh2D : RenderCommand2D
    {
        public PrimitiveManager Primitives { get; set; }
        public Matrix4 WorldMatrix { get; set; }
        public Matrix3 NormalMatrix { get; set; }
        public TMaterial Material { get; set; }

        public RenderCommandMesh2D() { }
        public RenderCommandMesh2D(
            PrimitiveManager manager,
            Matrix4 worldMatrix,
            Matrix3 normalMatrix,
            int zIndex,
            TMaterial materialOverride = null) : base(zIndex)
        {
            Primitives = manager;
            WorldMatrix = worldMatrix;
            NormalMatrix = normalMatrix;
            Material = materialOverride;
        }

        public override void Render()
        {
            Primitives?.Render(WorldMatrix, NormalMatrix, Material);
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
            Matrix3 normalMatrix,
            int zIndex)
            : base(quad, worldMatrix, normalMatrix, zIndex, null)
        {
            Viewport = viewport;
            Framebuffer = viewportFBO;
        }

        public override void Render()
        {
            Viewport.Render(Viewport.Camera?.OwningComponent?.OwningScene, Viewport.Camera, Framebuffer);
            base.Render();
        }
    }
}

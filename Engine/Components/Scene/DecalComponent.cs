using System;
using TheraEngine.Components.Scene.Shapes;
using TheraEngine.Rendering.Models.Materials;
using System.Drawing;
using TheraEngine.Rendering;

namespace TheraEngine.Components.Scene
{
    public class DecalComponent : BoxComponent
    {
        public TMaterial Material { get; set; }
        public Matrix4 DecalRenderMatrix { get; private set; }
        public Matrix4 InverseDecalRenderMatrix { get; private set; }
        public FrameBuffer RenderFBO { get; } = new FrameBuffer();

        public DecalComponent() : base() { }
        public DecalComponent(Vec3 extents) : base(extents, null) { }

        protected override void OnWorldTransformChanged()
        {
            Vec3 extents = Box.HalfExtents.Raw * 2.0f;
            DecalRenderMatrix = WorldMatrix * extents.AsScaleMatrix();
            InverseDecalRenderMatrix = (1.0f / extents).AsScaleMatrix() * InverseWorldMatrix;
            base.OnWorldTransformChanged();
        }
        public override void OnSpawned()
        {
            TMaterial mat = new TMaterial();
            //TODO
            RenderFBO.SetRenderTargets(mat);
            Material = TMaterial.CreateLitColorMaterial(Color.Orange, true);
            OwningScene.Decals.Add(this);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            OwningScene.Decals.Add(this);
            base.OnDespawned();
        }
    }
}

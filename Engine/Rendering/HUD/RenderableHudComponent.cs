using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Rendering.Models;
using CustomEngine.Worlds;

namespace CustomEngine.Rendering.HUD
{
    public class RenderableHudComponent : DockableHudComponent
    {
        public RenderableHudComponent(HudComponent owner) : base(owner) { }

        private static SkeletalSubMesh SquareMesh = new SkeletalSubMesh(PrimitiveData.FromQuads(
            Culling.Back, new PrimitiveBufferInfo(), VertexQuad.MakeQuad(
            Vec3.Zero, Vec3.UnitX, new Vec3(1.0f, 1.0f, 0.0f), Vec3.UnitY, Vec3.UnitZ)), "HudComponent");

        public MeshProgram _material;
    }
}

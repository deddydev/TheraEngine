using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Rendering.Models;

namespace CustomEngine.Rendering.HUD
{
    public class TextureHudComponent : DockableHudComponent
    {
        public TextureHudComponent(HudComponent owner) : base(owner) { }

        private static Mesh SquareMesh = new Mesh(PrimitiveData.FromQuads(VertexQuad.MakeQuad(
            Vec3.Zero, Vec3.UnitX, new Vec3(1.0f, 1.0f, 0.0f), Vec3.UnitY, Vec3.UnitZ)));

        public Material _material;
    }
}

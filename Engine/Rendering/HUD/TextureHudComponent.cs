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

        private static Mesh SquareMesh = new Mesh(PrimitiveData.FromQuads());
        public Material _material;
    }
}

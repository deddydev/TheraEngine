using System;
using System.Collections.Generic;
using BulletSharp;

namespace CustomEngine.Rendering.Models
{
    public class Mesh : ObjectBase, IRenderable
    {
        public Model _owner;
        public CollisionShape _collision;
        public PrimitiveManager _manager = new PrimitiveManager();
        public Materials.Material _material;

        public void SetPrimitiveData(PrimitiveData data)
        {
            _manager.SetPrimitiveData(data);
        }

        public void Render(float delta) { _manager.Render(); }

        public static implicit operator Mesh(Box b) { Mesh m = new Mesh(); m.SetPrimitiveData(b.GetPrimitives()); return m; }
    }
}

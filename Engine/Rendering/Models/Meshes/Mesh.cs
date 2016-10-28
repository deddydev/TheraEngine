using System;
using System.Collections.Generic;
using BulletSharp;
using CustomEngine.Rendering.Models.Materials;

namespace CustomEngine.Rendering.Models
{
    public class Mesh : ObjectBase, IRenderable
    {
        public Mesh(Model owner) { _owner = owner; }

        private Model _owner;
        private CollisionShape _collision;
        private PrimitiveManager _manager = new PrimitiveManager();
        private Material _material;

        public Material Material
        {
            get { return _material; }
            set { _material = value; }
        }
        public CollisionShape CollisionShape
        {
            get { return _collision; }
            set { _collision = value; }
        }

        public void SetPrimitiveData(PrimitiveData data)
        {
            _manager.SetPrimitiveData(data);
        }

        public void Render(float delta)
        {
            if (_material != null)
                _manager.Render(_material._programId);
        }

        public static implicit operator Mesh(Box b) { Mesh m = new Mesh(null); m.SetPrimitiveData(b.GetPrimitives()); return m; }
    }
}

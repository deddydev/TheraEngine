using System;
using System.Collections.Generic;
using BulletSharp;
using CustomEngine.Rendering.Models.Materials;

namespace CustomEngine.Rendering.Models
{
    public class Mesh : ObjectBase, IRenderable
    {
        public Mesh(string name, Model owner)
        {
            _owner = owner;
            _name = name;
        }
        public Mesh(string name)
        {
            _name = name;
        }

        private Model _owner;
        private CollisionShape _collision;
        internal PrimitiveManager _manager = new PrimitiveManager();
        private Material _material;

        public CollisionShape CollisionShape
        {
            get { return _collision; }
            set { _collision = value; }
        }
        public Model Model
        {
            get { return _owner; }
            set { _owner = value; }
        }
        public Material Material
        {
            get { return _material; }
            set { _material = value; }
        }

        public void SetPrimitiveData(PrimitiveData data)
        {
            _manager.SetPrimitiveData(data);
        }

        public void Render(float delta)
        {
            if (_material != null)
                _manager.Render(_material);
        }

        public static implicit operator Mesh(Box b)
        {
            Mesh m = new Mesh("Box");
            m.SetPrimitiveData(b.GetPrimitives()[0]);
            return m;
        }
    }
}

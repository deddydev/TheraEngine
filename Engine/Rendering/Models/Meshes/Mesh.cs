using System;
using System.Collections.Generic;
using BulletSharp;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Files;

namespace CustomEngine.Rendering.Models
{
    public class Mesh : ObjectBase, IRenderable
    {
        public Mesh(PrimitiveData data)
        {
            _manager.SetPrimitiveData(data);
            _owner = null;
            _name = null;
        }
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
        private FileRef<Material> _material;
        protected uint? _renderKey = null;
        protected bool _rendering = false, _visible = false, _visibleByDefault = true;

        /// <summary>
        /// True if this mesh is literally visible on screen at this moment in any viewport.
        /// AKA: short-term visibility
        /// </summary>
        public bool IsRendering
        {
            get { return _rendering; }
            set { _rendering = value; }
        }
        /// <summary>
        /// Visible means this mesh will never be rendered, or will be rendered if placed onscreen.
        /// AKA: long-term visibility
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible == value)
                    return;

                if (value)
                    Engine.Renderer.Scene.AddRenderable(this);
                else
                    Engine.Renderer.Scene.RemoveRenderable(this);

                _visible = value;
            }
        }
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

        public RenderKey RenderKey
        {
            get { return _renderKey; }
            set { _renderKey = value; }
        }

        public Box CullingVolume
        {
            get
            {
                return new Box(0.0f, 0.0f, 0.0f);
            }
        }

        public void SetPrimitiveData(PrimitiveData data) => _manager.SetPrimitiveData(data);

        public void OnSpawned()
        {
            _material.LoadFile();
            Visible = _visibleByDefault;
        }
        public void OnDespawned()
        {
            _material.UnloadReference();
            Visible = false;
        }
        public void Render()
        {
            if (!_visible || !_rendering)
                return;

            if (_material.File != null)
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

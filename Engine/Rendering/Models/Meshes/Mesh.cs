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
            _manager.Data = data;
            _name = null;
        }
        public Mesh(string name)
        {
            _name = name;
        }
        
        private CollisionShape _collision;
        internal PrimitiveManager _manager = new PrimitiveManager();
        private SingleFileRef<Material> _material;
        protected uint? _renderKey = null;
        protected bool _rendering = false, _visible = false, _visibleByDefault = true;
        private Box _cullingVolume;
        private Matrix4 _transform = Matrix4.Identity;
        int _instanceCount = 0;

        /// <summary>
        /// True if this mesh is literally visible on screen at this moment in any viewport.
        /// AKA: short-term visibility
        /// </summary>
        public bool IsRendering
        {
            get { return _rendering; }
            set { _rendering = value; }
        }
        public CollisionShape CollisionShape
        {
            get { return _collision; }
            set { _collision = value; }
        }
        public Material Material
        {
            get { return _material; }
            set { _material = value; }
        }
        public Box CullingVolume
        {
            get { return _cullingVolume; }
            set { _cullingVolume = value; }
        }
        public int InstanceCount
        {
            get { return _instanceCount; }
            set { _instanceCount = value; }
        }
        public Matrix4 Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }

        public void SetPrimitiveData(PrimitiveData data) => _manager.Data = data;

        public void OnSpawned()
        {
            //TODO: add material to list, get material id, add to cache with id, sort renderables by material id
            _material.GetInstance();
            //Visible = _visibleByDefault;
        }
        public void OnDespawned()
        {
            _material.UnloadReference();
            //Visible = false;
        }
        public void Render()
        {
            if (/*!_visible || */!_rendering)
                return;

            if (_material.File != null)
                _manager.Render(_material, _transform);
        }
        public static implicit operator Mesh(Box b)
        {
            Mesh m = new Mesh("Box");
            m.SetPrimitiveData(b.GetPrimitives()[0]);
            m.CullingVolume = b;
            return m;
        }
    }
}

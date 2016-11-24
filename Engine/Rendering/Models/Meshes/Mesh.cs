using System;
using System.Collections.Generic;
using BulletSharp;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Files;
using CustomEngine.Worlds.Actors.Components;

namespace CustomEngine.Rendering.Models
{
    public class Mesh : RenderableObject
    {
        public Mesh() { }
        public Mesh(PrimitiveData data) { _manager.Data = data; }
        public Mesh(Shape shape)
        {
            Name = shape.GetType().ToString();
            SetPrimitiveData(shape.GetPrimitiveData());
            SetCullingVolume(shape);
        }

        private Model _model;
        private CollisionShape _collision;
        internal PrimitiveManager _manager = new PrimitiveManager();
        private SingleFileRef<Material> _material;
        protected bool _visible = false, _visibleByDefault = true;
        private Shape _cullingVolume;
        public CollisionShape CollisionShape
        {
            get { return _collision; }
            set
            {
                if (_collision != null)
                    _collision.UserObject = null;
                _collision = value;
                if (_collision != null)
                    _collision.UserObject = this;
            }
        }
        public Material Material
        {
            get { return _material; }
            set { _material = value; }
        }
        public override Matrix4 GetWorldMatrix() { return _model != null ? _model.GetWorldMatrix() : Matrix4.Identity; }
        public override Matrix4 GetInverseWorldMatrix() { return _model != null ? _model.GetInverseWorldMatrix() : Matrix4.Identity; }
        public override Shape GetCullingVolume() { return _cullingVolume; }

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
        public override void Render()
        {
            //if (/*!_visible || */!_rendering)
            //    return;

            if (_material.File != null)
                _manager.Render(_material, GetWorldMatrix());
        }
        public static implicit operator Mesh(Shape shape)
        {
            Mesh m = new Mesh();
            m.Name = shape.GetType().ToString();
            m.SetPrimitiveData(shape.GetPrimitiveData());
            m.SetCullingVolume(shape);
            return m;
        }
        public override PrimitiveData GetPrimitiveData() { return _manager.Data; }
        public void SetPrimitiveData(PrimitiveData data) => _manager.Data = data;
        public void SetCullingVolume(Shape volume) { _cullingVolume = volume; }
    }
}

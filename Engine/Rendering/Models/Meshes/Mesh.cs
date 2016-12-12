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
            _name = shape.GetType().ToString();
            SetPrimitiveData(shape.GetPrimitiveData());
            SetCullingVolume(shape);
        }

        private Model _model;
        internal PrimitiveManager _manager = new PrimitiveManager();

        public override Material Material
        {
            get { return base.Material; }
            set
            {
                base.Material = value;
                _manager.SetMaterial(value);
            }
        }
        public Model Model
        {
            get { return _model; }
            set { _model = value; }
        }
        public override Matrix4 GetWorldMatrix() { return _model != null ? _model.GetWorldMatrix() : Matrix4.Identity; }
        public override Matrix4 GetInverseWorldMatrix() { return _model != null ? _model.GetInverseWorldMatrix() : Matrix4.Identity; }

        public override void OnSpawned()
        {
            //TODO: add material to list, get material id, add to cache with id, sort renderables by material id
            //_material.GetInstance();
        }
        public override void OnDespawned()
        {
            //_material.UnloadReference();
        }
        public override void Render()
        {
            //if (!Visible || !IsRendering)
            //    return;

            //if (_material.File == null)
            //    return;

            _manager.Render(GetWorldMatrix());
        }
        public static implicit operator Mesh(Shape shape) { return new Mesh(shape); }
        public override PrimitiveData GetPrimitiveData() { return _manager.Data; }
        public void SetPrimitiveData(PrimitiveData data) => _manager.Data = data;
        public void SetCullingVolume(Shape volume) { _cullingVolume = volume; }
    }
}

using System;
using System.Collections.Generic;
using BulletSharp;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Files;
using CustomEngine.Worlds.Actors.Components;

namespace CustomEngine.Rendering.Models
{
    public class Mesh : RenderableObject, ICollidable
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

        protected bool _visible = false, _visibleByDefault = true;
        private Shape _cullingVolume;
        private RigidBody _collision;
        public RigidBody CollisionObject
        {
            get { return _collision; }
            set
            {
                if (_collision != null)
                {
                    if (_visible)
                        Engine.World.PhysicsScene.AddRigidBody(_collision);
                    _collision.UserObject = null;
                }
                _collision = value;
                if (_collision != null)
                {
                    if (_visible)
                        Engine.World.PhysicsScene.AddRigidBody(_collision);
                    _collision.UserObject = this;
                }
            }
        }
        public Model Model
        {
            get { return _model; }
            set { _model = value; }
        }
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible == value)
                    return;
                
                if (_visible && _collision != null)
                    Engine.World.PhysicsScene.RemoveRigidBody(_collision);
                _visible = value;
                if (_visible && _collision != null)
                    Engine.World.PhysicsScene.AddRigidBody(_collision);
            }
        }

        public override Matrix4 GetWorldMatrix() { return _model != null ? _model.GetWorldMatrix() : Matrix4.Identity; }
        public override Matrix4 GetInverseWorldMatrix() { return _model != null ? _model.GetInverseWorldMatrix() : Matrix4.Identity; }
        public override Shape GetCullingVolume() { return _cullingVolume; }

        public override void OnSpawned()
        {
            //TODO: add material to list, get material id, add to cache with id, sort renderables by material id
            _material.GetInstance();
            //Visible = _visibleByDefault;
        }
        public override void OnDespawned()
        {
            _material.UnloadReference();
            //Visible = false;
        }
        public override void Render()
        {
            //if (!Visible || !IsRendering)
            //    return;

            if (_material.File == null)
                return;

            _manager.Render(_material, GetWorldMatrix());
        }
        public static implicit operator Mesh(Shape shape) { return new Mesh(shape); }
        public override PrimitiveData GetPrimitiveData() { return _manager.Data; }
        public void SetPrimitiveData(PrimitiveData data) => _manager.Data = data;
        public void SetCullingVolume(Shape volume) { _cullingVolume = volume; }
    }
}

using CustomEngine.Rendering.Models;
using System;
using System.Reflection;
using CustomEngine.Rendering.Animation;
using CustomEngine.Rendering;
using System.ComponentModel;
using BulletSharp;

namespace CustomEngine.Worlds.Actors.Components
{
    public class StaticMeshComponent : TRSComponent, IRenderable, IPhysicsDrivable
    {
        public StaticMeshComponent(StaticMesh m, PhysicsDriverInfo info, bool visibleByDefault)
        {
            Model = m;
            _physicsDriver = new PhysicsDriver(info, _physicsDriver_TransformChanged);
            _cullingVolume = Model.CullingVolume;
            _visibleByDefault = visibleByDefault;
        }

        private void _physicsDriver_TransformChanged(Matrix4 worldMatrix)
        {
            _worldTransform = worldMatrix;
            _inverseWorldTransform = worldMatrix.Inverted();
            foreach (SceneComponent c in _children)
                c.RecalcGlobalTransform();
        }

        public override void RecalcGlobalTransform()
        {
            if (!_physicsDriver.SimulatingPhysics)
                base.RecalcGlobalTransform();
        }

        protected bool _visible, _rendering, _visibleByDefault;
        internal StaticMesh _model;
        private PhysicsDriver _physicsDriver;
        protected IShape _cullingVolume;

        public StaticMesh Model
        {
            get { return _model; }
            set
            {
                if (_model == value)
                    return;
                if (_model != null)
                    _model.LinkedComponents.Remove(this);
                if (value != null)
                    value.LinkedComponents.Add(this);
            }
        }
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible == value)
                    return;

                _visible = value;
                if (_visible)
                    Engine.Renderer.Scene.AddRenderable(this);
                else
                    Engine.Renderer.Scene.RemoveRenderable(this);
            }
        }
        public bool VisibleByDefault { get { return _visibleByDefault; } }
        public bool IsRendering
        {
            get { return _rendering; }
            set { _rendering = value; }
        }
        public PhysicsDriver PhysicsDriver { get { return _physicsDriver; } }
        public IShape CullingVolume { get { return _cullingVolume; } }
        
        public override void OnSpawned()
        {
            base.OnSpawned();
            Visible = _visibleByDefault;
            Model?.OnSpawned();
        }
        public override void OnDespawned()
        {
            base.OnDespawned();
            Visible = false;
            Model?.OnDespawned();
        }
        internal override void OriginRebased(Vec3 newOrigin)
        {
            Translation -= newOrigin;
        }
        public void Render()
        {
            //if (!Visible || !IsRendering)
            //    return;

            _model?.Render(_worldTransform);
        }
    }
}

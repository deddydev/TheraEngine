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
            : this(m, Vec3.Zero, Rotator.GetZero(), Vec3.One, info, visibleByDefault) { }
        public StaticMeshComponent(
            StaticMesh m, 
            Vec3 translation, 
            Rotator rotation, 
            Vec3 scale,
            PhysicsDriverInfo info, 
            bool visibleByDefault)
        {
            Model = m;
            _cullingVolume = Model.CullingVolume.HardCopy();
            _visibleByDefault = visibleByDefault;

            _translation = translation;
            _rotation = rotation;
            _scale = scale;
            RecalcLocalTransform();

            if (info.BodyInfo != null)
            {
                info.BodyInfo.MotionState = new DefaultMotionState(WorldMatrix);
                //if (info.BodyInfo.MotionState != null)
                //{
                //    DefaultMotionState ms = (DefaultMotionState)info.BodyInfo.MotionState;
                //    ms.StartWorldTrans = WorldMatrix;
                //    ms.WorldTransform = WorldMatrix;
                //    ms.GraphicsWorldTrans = WorldMatrix;
                //}
                //else
                //    info.BodyInfo.StartWorldTransform = WorldMatrix;
            }

            _physicsDriver = new PhysicsDriver(info, _physicsDriver_TransformChanged);
        }

        private void _physicsDriver_TransformChanged(Matrix4 worldMatrix)
        {
            WorldMatrix = worldMatrix;
        }

        internal override void RecalcGlobalTransform()
        {
            if (_physicsDriver == null || !_physicsDriver.SimulatingPhysics)
                base.RecalcGlobalTransform();
            _cullingVolume?.SetTransform(WorldMatrix);
        }

        protected bool _visible, _rendering = true, _visibleByDefault;
        internal StaticMesh _model;
        private PhysicsDriver _physicsDriver;
        protected Shape _cullingVolume;

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
        public Shape CullingVolume { get { return _cullingVolume; } }

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

            _model?.Render(WorldMatrix);
        }
    }
}

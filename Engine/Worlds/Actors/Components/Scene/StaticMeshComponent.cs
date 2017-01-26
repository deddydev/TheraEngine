using CustomEngine.Rendering.Models;
using System;
using System.Reflection;
using CustomEngine.Rendering.Animation;
using CustomEngine.Rendering;
using System.ComponentModel;
using BulletSharp;

namespace CustomEngine.Worlds.Actors.Components
{
    public class StaticMeshComponent : TRSComponent, IPhysicsDrivable
    {
        public StaticMeshComponent(StaticMesh m, PhysicsDriverInfo info) 
            : this(m, Vec3.Zero, Rotator.GetZero(), Vec3.One, info) { }
        public StaticMeshComponent(
            StaticMesh m, 
            Vec3 translation, 
            Rotator rotation, 
            Vec3 scale,
            PhysicsDriverInfo info)
        {
            Model = m;
            //_cullingVolume = Model.CullingVolume.HardCopy();

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
            foreach (RenderableMesh m in _meshes)
                m.CullingVolume.SetTransform(WorldMatrix);
            //_cullingVolume?.SetTransform(WorldMatrix);
        }

        private StaticMesh _model;
        private PhysicsDriver _physicsDriver;
        //protected Shape _cullingVolume;
        internal RenderableMesh[] _meshes;

        public StaticMesh Model
        {
            get { return _model; }
            set
            {
                if (_model == value)
                    return;
                _model = value;
                if (_model != null)
                {
                    _meshes = new RenderableMesh[_model.RigidChildren.Count + _model.SoftChildren.Count];
                    for (int i = 0; i < _model.RigidChildren.Count; ++i)
                        _meshes[i] = new RenderableMesh(_model.RigidChildren[i], this);
                    for (int i = 0; i < _model.SoftChildren.Count; ++i)
                        _meshes[_model.RigidChildren.Count + i] = new RenderableMesh(_model.SoftChildren[i], this);
                }
            }
        }
        public PhysicsDriver PhysicsDriver { get { return _physicsDriver; } }

        public override void OnSpawned()
        {
            base.OnSpawned();
            foreach (RenderableMesh m in _meshes)
                m.Visible = m.Mesh.VisibleByDefault;
        }
        public override void OnDespawned()
        {
            base.OnDespawned();
            foreach (RenderableMesh m in _meshes)
                m.Visible = false;
        }
        internal override void OriginRebased(Vec3 newOrigin)
        {
            Translation -= newOrigin;
        }

        internal class RenderableMesh : IRenderable
        {
            public RenderableMesh(IStaticMesh mesh, SceneComponent component)
            {
                _mesh = mesh;
                _component = component;
                _cullingVolume = _mesh.CullingVolume.HardCopy();
                Visible = false;
                IsRendering = true;
            }

            private bool _isVisible, _isRendering;
            private SceneComponent _component;
            private IStaticMesh _mesh;
            private RenderOctree.Node _renderNode;
            private Shape _cullingVolume;

            //public Shape CullingVolume { get { return _mesh.CullingVolume.TransformedBy(_component.WorldMatrix); } }
            public Shape CullingVolume { get { return _cullingVolume; } }
            public bool Visible
            {
                get { return _isVisible; }
                set
                {
                    _isVisible = value;
                    if (_isVisible)
                        Engine.Renderer.Scene.AddRenderable(this);
                    else
                        Engine.Renderer.Scene.RemoveRenderable(this);
                }
            }
            public bool IsRendering
            {
                get { return _isRendering; }
                set { _isRendering = value; }
            }
            public IStaticMesh Mesh
            {
                get { return _mesh; }
                set { _mesh = value; }
            }
            public RenderOctree.Node RenderNode
            {
                get { return _renderNode; }
                set { _renderNode = value; }
            }
            public void Render()
            {
                _mesh.PrimitiveManager.Render(_component.WorldMatrix);
            }
        }
    }
}
